using HarmonyLib;
using Game.UI.InGame;
using Game.Tools;
using Game.Prefabs;
using Unity.Entities;
using Game.Common;
using UnityEngine;
using Game.SceneFlow;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Game.UI;
using System.Linq;
using Game;
using System;
using Colossal.Localization;
using Colossal.IO.AssetDatabase;
using System.IO.Compression;
using Unity.Collections;
using Colossal.PSI.Environment;

namespace ExtraLandscapingTools.Patches
{

	[HarmonyPatch(typeof(GameManager), "Awake")]
	internal class GameManager_Awake
	{	
		static internal readonly string ELTGameDataPath = $"{EnvPath.kStreamingDataPath}\\Mods\\ELT"; //: $"{EnvPath.kUserDataPath}\\Mods\\ELT"; Settings.settings.UseGameFolderForCache ? 
		static internal readonly string ELTUserDataPath = $"{EnvPath.kUserDataPath}\\Mods\\ELT"; //: $"{EnvPath.kUserDataPath}\\Mods\\ELT"; Settings.settings.UseGameFolderForCache ? 
		static private readonly string PathToParent = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
		public static readonly string PathToMods = Path.Combine(PathToParent,"ExtraLandscapingTools_mods");
		public static readonly string PathToCustomBrushes = Path.Combine(PathToMods,"CustomBrushes");
		// public static readonly string PathToCustomSurface = Path.Combine(PathToMods,"CustomSurfaces");
		public static readonly string PathToCustomDecal = Path.Combine(PathToMods,"CustomDecals");

		static readonly string pathToZip = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\resources.zip";

		static internal readonly string resources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources");
		static internal readonly string resourcesIcons = Path.Combine(resources, "Icons");
		static internal readonly string resourcesBrushes = Path.Combine(resources, "Brushes");
		static internal readonly string resourcesCache = Path.Combine(resources, "Cache");

		static void Postfix(GameManager __instance)
		{
			if(File.Exists(pathToZip)) {
				if(Directory.Exists(resources)) Directory.Delete(resources, true);
				ZipFile.ExtractToDirectory(pathToZip, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				File.Delete(pathToZip);
			}

			Settings.settings = Settings.LoadSettings("ELT", new ELTSettings());
			ELT.ClearData();

			CustomSurfaces.SearchForCustomSurfacesFolder(PathToParent);
			CustomDecals.SearchForCustomDecalsFolder(PathToParent);

			if(!PrefabSystem_OnCreate.FolderToLoadBrush.Contains(resourcesBrushes) && Directory.Exists(resourcesBrushes)) PrefabSystem_OnCreate.FolderToLoadBrush.Add(resourcesBrushes);
			if(!PrefabSystem_OnCreate.FolderToLoadBrush.Contains(PathToCustomBrushes) && Directory.Exists(PathToCustomBrushes)) PrefabSystem_OnCreate.FolderToLoadBrush.Add(PathToCustomBrushes);
		}
	}

	[HarmonyPatch(typeof(LocalizationManager), "AddLocale", typeof(LocaleAsset))]
	internal class LocalizationManager_AddLocale
	{	
		static void Prefix(LocaleAsset asset)
		{	
			Localization.AddCustomLocal(asset);
		}
	}

	[HarmonyPatch(typeof(GameManager), "InitializeThumbnails")]
	public class GameManager_InitializeThumbnails
	{	
		private static readonly string IconsResourceKey = $"{MyPluginInfo.PLUGIN_NAME.ToLower()}";

		public static readonly string COUIBaseLocation = $"coui://{IconsResourceKey}";

		internal readonly static List<string> pathToIconToLoad = [Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)];


		static void Prefix(GameManager __instance)
		{

			// if(Directory.Exists(GameManager_Awake.PathToMods)) pathToIconToLoad.Add(GameManager_Awake.PathToMods);

			var gameUIResourceHandler = (GameUIResourceHandler)GameManager.instance.userInterface.view.uiSystem.resourceHandler;
			
			if (gameUIResourceHandler == null)
			{
				UnityEngine.Debug.LogError("Failed retrieving GameManager's GameUIResourceHandler instance, exiting.");
				return;
			}
			
			gameUIResourceHandler.HostLocationsMap.Add(
				IconsResourceKey, pathToIconToLoad
			);
		}

		internal static void AddNewIconsFolder(string pathToFolder) {
			if(!pathToIconToLoad.Contains(pathToFolder)) pathToIconToLoad.Add(pathToFolder);
		}

	}

	[HarmonyPatch(typeof(SystemOrder), "Initialize")]
	public static class SystemOrderPatch {
		public static void Postfix(UpdateSystem updateSystem) {
			updateSystem.UpdateAt<ELT_UI>(SystemUpdatePhase.UIUpdate);
			updateSystem.UpdateAt<TransformSection>(SystemUpdatePhase.UIUpdate);
			// updateSystem.UpdateAfter<MainSysteme>(SystemUpdatePhase.PrefabUpdate);
			// updateSystem.UpdateAt<SurfaceReplacerTool>(SystemUpdatePhase.ToolUpdate);
		}
	}

	[HarmonyPatch(typeof(AreaToolSystem), nameof(AreaToolSystem.TrySetPrefab))]
	public class AreaToolSystem_TrySetPrefab 
	{
		private static bool Prefix(PrefabBase prefab) {
			if(ELT.m_ToolSystem.activeTool is SurfaceReplacerTool) return false;
			return true;
		}
	}

	[HarmonyPatch(typeof(ToolUISystem), "OnCreate")]
	public class ToolUISystem_OnCreate 
	{
		// public static ToolUISystem toolUISystem;
		public static Traverse toolUISystemTraverse;
		internal static PrefabSystem m_PrefabSystem;
		internal static EntityQuery m_BrushQuery;
		internal static ToolSystem m_ToolSystem;

		internal static ToolUISystem.Brush[] brushs;

		static void Postfix( ToolUISystem __instance) {

			ELT.m_ToolUISystem = __instance;
			toolUISystemTraverse = Traverse.Create(__instance);

			m_PrefabSystem = toolUISystemTraverse.Field("m_PrefabSystem").GetValue<PrefabSystem>();
			m_BrushQuery = toolUISystemTraverse.Field("m_BrushQuery").GetValue<EntityQuery>();
			m_ToolSystem = toolUISystemTraverse.Field("m_ToolSystem").GetValue<ToolSystem>();

			brushs = toolUISystemTraverse.Method("BindBrushTypes").GetValue<ToolUISystem.Brush[]>();
		}
	}

	[HarmonyPatch( typeof( ToolUISystem ), "OnToolChanged", typeof(ToolBaseSystem) )]
	class ToolUISystem_OnToolChanged
	{
		internal static bool showMarker = false;
		private static void Postfix( ToolBaseSystem tool ) {

			if(tool is TerrainToolSystem) {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("UI.js"));
			} else {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("REMOVE_UI.js"));
			}

			if(tool is AreaToolSystem || tool is SurfaceReplacerTool) { //|| tool is ObjectToolSystem
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("ShowMarker.js"));
				showMarker = true;
			} else if(showMarker){
				ELT_UI.ShowMarker(false);
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("REMOVE_ShowMarker.js"));
				showMarker = false;
			}

			ELT_UI.SetUpMarker(tool.GetPrefab());

		}
	}

	[HarmonyPatch( typeof( ToolUISystem ), "OnPrefabChanged", typeof(PrefabBase) )]
	class ToolUISystem_OnPrefabChanged
	{
		private static void Postfix(PrefabBase prefab) {
			ELT_UI.SetUpMarker(prefab);
		}
	}

	[HarmonyPatch(typeof(PrefabSystem), "OnCreate")]
	public class PrefabSystem_OnCreate
	{
		internal static List<string> FolderToLoadBrush = [];

		public static void Postfix( PrefabSystem __instance)
		{

			ELT.m_PrefabSystem = __instance;

			foreach(string folder in FolderToLoadBrush) {;
				foreach(string filePath in Directory.GetFiles(folder)) 
				{
					byte[] fileData = File.ReadAllBytes(filePath);
					Texture2D texture2D = new(1, 1);
					if(!texture2D.LoadImage(fileData)) UnityEngine.Debug.LogError("Failed to Load Image");
								
					BrushPrefab brushPrefab = (BrushPrefab)ScriptableObject.CreateInstance("BrushPrefab");
					brushPrefab.name = Path.GetFileNameWithoutExtension(filePath);
					brushPrefab.m_Texture = texture2D;
					__instance.AddPrefab(brushPrefab);
				}
			}
		}
	}

	// [HarmonyPatch(typeof(GameModeExtensions), "IsEditor")]
	// public class GameModeExtensions_IsEditor
	// {
	// 	public static void Postfix(ref bool __result) {
	// 		__result = true;
	// 	}
	// }


	[HarmonyPatch(typeof(PrefabSystem), nameof(PrefabSystem.AddPrefab))]
	public class PrefabSystem_AddPrefab
	{
		private static readonly string[] removeTools = ["Material 1", "Material 2"];

		private static string UIAssetCategoryPrefabName = "";

		private static bool setupCustomSurfaces = CustomSurfaces.CanCreateCustomSurfaces();
		private static bool setupCustomDecals = CustomDecals.CanCreateCustomDecals();

		public static bool Prefix( PrefabSystem __instance, PrefabBase prefab)
		{

			if (Traverse.Create(__instance).Field("m_Entities").GetValue<Dictionary<PrefabBase, Entity>>().ContainsKey(prefab)) {
				return false;
			}


			if(prefab is UIAssetMenuPrefab) {

				if(prefab.name == "Landscaping") {
					Prefab.CreateNewUiToolMenu(prefab, Prefab.GetCustomAssetMenuName());

					// Prefab.CreateNewUiToolMenu(prefab, "Custom Assets");
					// Prefab.CreateNewUiToolMenu(prefab, "Custom Surfaces");
				}
			}

			try {

				if(Prefab.failedPrefabs.ContainsKey(UIAssetCategoryPrefabName)) {
					string cat = UIAssetCategoryPrefabName;
					UIAssetCategoryPrefabName = "";
					PrefabBase[] temp = new PrefabBase[Prefab.failedPrefabs[cat].Count];
					Prefab.failedPrefabs[cat].CopyTo(temp);
					foreach(PrefabBase prefabBase in temp) {
						Prefab.failedPrefabs[cat].Remove(prefabBase);
						__instance.AddPrefab(prefabBase);
					}
				}

				if(Prefab.onAddPrefab != null) foreach(Delegate @delegate in Prefab.onAddPrefab.GetInvocationList()) {

					object result = @delegate.DynamicInvoke(prefab);

					if(result is bool b && !b) return false;
				}

				if (removeTools.Contains(prefab.name) || 
					(	
						prefab is not TerraformingPrefab && 
						prefab is not SurfacePrefab && 
						prefab is not StaticObjectPrefab
					) || 
					prefab is BuildingPrefab || 
					prefab is BuildingExtensionPrefab)
				{	
					if(prefab is UIAssetMenuPrefab uIAssetMenuPrefab && Prefab.failedPrefabs.ContainsKey(uIAssetMenuPrefab.name)) {
						UIAssetCategoryPrefabName = uIAssetMenuPrefab.name;
					} else if(prefab is UIAssetCategoryPrefab uIAssetCategoryPrefab && Prefab.failedPrefabs.ContainsKey(uIAssetCategoryPrefab.name)) {
						UIAssetCategoryPrefabName = uIAssetCategoryPrefab.name;
					}
					return true;
				}

				if(prefab.GetComponent<PlaceholderObject>() != null) return true;

				if (prefab is StaticObjectPrefab && 
					!prefab.name.ToLower().Contains("decal") && 
					!prefab.name.ToLower().Contains("roadarrow") && 
					!prefab.name.ToLower().Contains("lanemarkings") &&
					prefab.GetComponent<CustomDecal>() == null) 
				{
					return true;
				}

				if(prefab is StaticObjectPrefab staticObjectPrefab && (prefab.name.ToLower().Contains("decal") || prefab.name.ToLower().Contains("roadarrow") || prefab.name.ToLower().Contains("lanemarkings") )) {
					if(prefab.name.ToLower().Contains("invisible")) {
						return true;
					}else if(setupCustomDecals) {
						setupCustomDecals = false;
						CustomDecals.CreateCustomDecals();
					}
				}

				var spawnableArea = prefab.GetComponent<SpawnableArea>();
				if (prefab is SurfacePrefab && spawnableArea == null)
				{
					return true;
				} else if(prefab is SurfacePrefab surfacePrefab && spawnableArea != null) {

					Material material = surfacePrefab.GetComponent<RenderedArea>().m_Material;

					if(setupCustomSurfaces) {
						setupCustomSurfaces = false;
						try {
							CustomSurfaces.CreateCustomSurfaces(material);

						} catch (Exception e) {Plugin.Logger.LogWarning(e);}
					}

					if(Settings.settings.EnableSnowSurfaces && !surfacePrefab.Has<CustomSurface>()) {
						Vector4 baseColor = material.GetVector("_BaseColor");
						baseColor.w -= 0.1f;
						material.SetVector("_BaseColor", baseColor);
					}
				}	

				var TerraformingUI = prefab.GetComponent<UIObject>();
				if (TerraformingUI == null)
				{
					TerraformingUI = prefab.AddComponent<UIObject>();
					TerraformingUI.active = true;
					TerraformingUI.m_IsDebugObject = false;
					TerraformingUI.m_Icon = ELT.GetIcon(prefab);
					TerraformingUI.m_Priority = 1;
				}

				if(prefab is TerraformingPrefab) TerraformingUI.m_Group = Prefab.GetExistingToolCategory(prefab, "Terraforming") ?? TerraformingUI.m_Group;
				else if(prefab is SurfacePrefab) TerraformingUI.m_Group ??= CustomSurfaces.SetupUIGroupe(prefab);
				else if(prefab.name.ToLower().Contains("decal") || prefab.name.ToLower().Contains("roadarrow") || prefab.name.ToLower().Contains("lanemarkings") || prefab.GetComponent<CustomDecal>() != null) TerraformingUI.m_Group = CustomDecals.SetupUIGroupe(prefab);
				else TerraformingUI.m_Group ??= Prefab.GetOrCreateNewToolCategory(prefab, "Landscaping", "[ELT] Failed Prefab, IF you see this tab, repport it, it's a bug.");
				
				if(TerraformingUI.m_Group == null) {
					return false;
				} else {
				}

			} catch (Exception e) {Plugin.Logger.LogError(e);}

			return true;
		}
	}
}


