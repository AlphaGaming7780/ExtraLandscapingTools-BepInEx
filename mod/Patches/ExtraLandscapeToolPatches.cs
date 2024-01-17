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

namespace ExtraLandscapingTools.Patches
{

	[HarmonyPatch(typeof(GameSystemBase), "OnCreate")]
	internal class GameSystemBase_OnCreate
	{	
		static private readonly string PathToParent = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
		public static readonly string PathToMods = Path.Combine(PathToParent,"ExtraLandscapingTools_mods");
		public static readonly string PathToCustomBrushes = Path.Combine(PathToMods,"CustomBrushes");
		public static readonly string PathToCustomSurface = Path.Combine(PathToMods,"CustomSurfaces");

		static readonly string pathToZip = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\resources.zip";

		static internal readonly string resources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources");
		static internal readonly string resourcesIcons = Path.Combine(resources, "Icons");
		static internal readonly string resourcesBrushes = Path.Combine(resources, "Brushes");

		static void Prefix(GameSystemBase __instance)
		{

			if(File.Exists(pathToZip)) {
				if(Directory.Exists(resources)) Directory.Delete(resources, true);
				ZipFile.ExtractToDirectory(pathToZip, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				File.Delete(pathToZip);
			}

			if(!PrefabSystem_OnCreate.FolderToLoadBrush.Contains(resourcesBrushes) && Directory.Exists(resourcesBrushes)) PrefabSystem_OnCreate.FolderToLoadBrush.Add(resourcesBrushes);
			if(!PrefabSystem_OnCreate.FolderToLoadBrush.Contains(PathToCustomBrushes) && Directory.Exists(PathToCustomBrushes)) PrefabSystem_OnCreate.FolderToLoadBrush.Add(PathToCustomBrushes);
			if(!CustomSurfaces.FolderToLoadSurface.Contains(PathToCustomSurface) && Directory.Exists(PathToCustomSurface)) CustomSurfaces.FolderToLoadSurface.Add(PathToCustomSurface);

			// if(!Directory.Exists(resources)) {
			// 	Directory.CreateDirectory(resources);
			// 	Directory.CreateDirectory(resourcesBrushes);
			// 	Directory.CreateDirectory(resourcesIcons);
			// 	foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.svg")) {
			// 		File.Move(file, Path.Combine(resourcesIcons,Path.GetFileName(file)));
			// 	}
			// 	foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.png")) {
			// 		if(Path.GetFileNameWithoutExtension(file) != "icon") File.Move(file, Path.Combine(resourcesBrushes,Path.GetFileName(file)));
			// 	}
			// }
		}
	}

	[HarmonyPatch(typeof(LocalizationManager), "AddLocale", typeof(LocaleAsset))]
	internal class LocalizationManager_AddLocale
	{	
		static void Prefix(LocaleAsset asset)
		{	
			try {
				Localization.AddCustomLocal(asset);
			} catch (Exception e) {UnityEngine.Debug.Log(e);}
		}
	}

	[HarmonyPatch(typeof(GameManager), "InitializeThumbnails")]
	internal class GameManager_InitializeThumbnails
	{	
		private static readonly string IconsResourceKey = $"{MyPluginInfo.PLUGIN_NAME.ToLower()}";

		public static readonly string COUIBaseLocation = $"coui://{IconsResourceKey}";

        internal readonly static List<string> pathToIconToLoad = [Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)];


		static void Prefix(GameManager __instance)
		{

			if(Directory.Exists(GameSystemBase_OnCreate.PathToMods)) pathToIconToLoad.Add(GameSystemBase_OnCreate.PathToMods);

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
	}

	[HarmonyPatch(typeof(SystemOrder), "Initialize")]
	public static class SystemOrderPatch {
		public static void Postfix(UpdateSystem updateSystem) {
			updateSystem.UpdateAt<ELT_UI>(SystemUpdatePhase.UIUpdate);
		}
	}

	[HarmonyPatch(typeof(ToolUISystem), "OnCreate")]

	public class ToolUISystem_OnCreate 
	{
		public static ToolUISystem toolUISystem;
		public static Traverse toolUISystemTraverse;
		internal static PrefabSystem m_PrefabSystem;
		internal static EntityQuery m_BrushQuery;
		internal static ToolSystem m_ToolSystem;

		internal static ToolUISystem.Brush[] brushs;

		static void Postfix( ToolUISystem __instance) {

			toolUISystem = __instance;
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
		private static void Postfix( ToolBaseSystem tool ) {

			if(tool is TerrainToolSystem) {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("UI.js"));
			} else {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("REMOVE_UI.js"));
			}

			if(tool is AreaToolSystem) {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("ShowMarker.js"));
			} else {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("REMOVE_ShowMarker.js"));
			}

		}
	}

	[HarmonyPatch(typeof(PrefabSystem), "OnCreate")]
	public class PrefabSystem_OnCreate
	{
		internal static List<string> FolderToLoadBrush = [];

		private static PrefabSystem prefabSystem;

		public static void Postfix( PrefabSystem __instance)
		{

			prefabSystem = __instance;

			// CustomSurfaces.CallOnCustomSurfaces += LoadCustomSurfaces;

			foreach(string folder in FolderToLoadBrush) {;
				foreach(string filePath in Directory.GetFiles(folder)) 
				{
					byte[] fileData = File.ReadAllBytes(filePath);
					Texture2D texture2D = new(512, 512);
					if(!texture2D.LoadImage(fileData)) UnityEngine.Debug.LogError("Failed to Load Image");
								
					BrushPrefab brushPrefab = (BrushPrefab)ScriptableObject.CreateInstance("BrushPrefab");
					brushPrefab.name = Path.GetFileNameWithoutExtension(filePath);
					brushPrefab.m_Texture = texture2D;
					__instance.AddPrefab(brushPrefab);
				}
			}

			// StaticObjectPrefab staticObjectPrefab = (StaticObjectPrefab)ScriptableObject.CreateInstance("StaticObjectPrefab");
			// staticObjectPrefab.name = "TestDecals";
			// SubObjectDefaultProbability subObjectDefaultProbability = staticObjectPrefab.AddComponent<SubObjectDefaultProbability>();
			// subObjectDefaultProbability.active =  true;

			// __instance.AddPrefab(staticObjectPrefab);
		}

		// private static void LoadCustomSurfaces(Material material) {
		// 	CustomSurfaces.LoadCustomSurfaces(material, GameSystemBase_OnCreate.PathToCustomSurface, GameManager_InitializeThumbnails.COUIBaseLocation);
		// }

		internal static void CreateCustomSurfaces(Material material) {
			foreach(string folder in CustomSurfaces.FolderToLoadSurface) {
				foreach(string surfacesCat in Directory.GetDirectories( folder )) {
					foreach(string filePath in Directory.GetDirectories( surfacesCat )) 
					{	
						CustomSurfaces.CreateCustomSurface(prefabSystem, filePath, material, new DirectoryInfo(surfacesCat).Name);
					}
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

		private static bool setupCustomSurfaces = true;

		public static bool Prefix( PrefabSystem __instance, PrefabBase prefab)
		{

			if (Traverse.Create(__instance).Field("m_Entities").GetValue<Dictionary<PrefabBase, Entity>>().ContainsKey(prefab)) {
				
				// if(prefab is SurfacePrefab) Plugin.Logger.LogWarning(prefab.name + " is already added");
				return false;
			}


			if(prefab is UIAssetMenuPrefab) {
				if(prefab.name == "Landscaping" && CustomSurfaces.FolderToLoadSurface.Count > 0) {
					if (!__instance.TryGetPrefab(new PrefabID(nameof(UIAssetMenuPrefab), "Custom Surfaces"), out var p2) //Landscaping
					|| p2 is not UIAssetMenuPrefab SurfaceMenu)
					{
						SurfaceMenu = ScriptableObject.CreateInstance<UIAssetMenuPrefab>();
						SurfaceMenu.name = "Custom Surfaces";
						var SurfaceMenuUI = SurfaceMenu.AddComponent<UIObject>();
						SurfaceMenuUI.m_Icon = ELT.GetIcon(SurfaceMenu);
						SurfaceMenuUI.m_Priority = prefab.GetComponent<UIObject>().m_Priority+1;
						SurfaceMenuUI.active = true;
						SurfaceMenuUI.m_IsDebugObject = false;
						SurfaceMenuUI.m_Group = prefab.GetComponent<UIObject>().m_Group;

						__instance.AddPrefab(SurfaceMenu);
					}
				}
			}

			try {

				if(Prefab.failedSurfacePrefabs.ContainsKey(UIAssetCategoryPrefabName)) {
					// Plugin.Logger.LogMessage(UIAssetCategoryPrefabName);
					string cat = UIAssetCategoryPrefabName;
					UIAssetCategoryPrefabName = "";
					PrefabBase[] temp = new PrefabBase[Prefab.failedSurfacePrefabs[cat].Count];
					Prefab.failedSurfacePrefabs[cat].CopyTo(temp);
					foreach(PrefabBase prefabBase in temp) {
						// Plugin.Logger.LogMessage("Trying to add prefab " + prefabBase.name + " to the game.");
						Prefab.failedSurfacePrefabs[cat].Remove(prefabBase);
						__instance.AddPrefab(prefabBase);
					}
				}

				if (removeTools.Contains(prefab.name) || (prefab is not TerraformingPrefab && prefab is not SpacePrefab && prefab is not SurfacePrefab && prefab is not ObjectPrefab /* && prefab is not TaxiwayPrefab && prefab is not NetLanePrefab*/))
				{	

					// if(prefab is UIAssetCategoryPrefab uIAssetCategoryPrefab2) Plugin.Logger.LogMessage(uIAssetCategoryPrefab2.name);

					if(prefab is UIAssetMenuPrefab uIAssetMenuPrefab && Prefab.failedSurfacePrefabs.ContainsKey(uIAssetMenuPrefab.name)) {
						UIAssetCategoryPrefabName = uIAssetMenuPrefab.name;
					} else if(prefab is UIAssetCategoryPrefab uIAssetCategoryPrefab && Prefab.failedSurfacePrefabs.ContainsKey(uIAssetCategoryPrefab.name)) {
						// Plugin.Logger.LogMessage("Now adding asset for " + uIAssetCategoryPrefab.name + " cat");
						UIAssetCategoryPrefabName = uIAssetCategoryPrefab.name;
					}
					return true;
				}

				try {
					// WaterSource waterSource = prefab.GetComponent<WaterSource>(); && waterSource == null
					if (prefab is ObjectPrefab && !prefab.name.ToLower().Contains("decal")) 
					{
						return true;
					}
				} catch { return true;}

				if(prefab is StaticObjectPrefab staticObjectPrefab && prefab.name.ToLower().Contains("decal")) {
					if(prefab.name.ToLower().Contains("invisible")) {
						return true;
					}

					// Plugin.Logger.LogMessage(prefab.name);

					// foreach (ObjectMeshInfo objectMeshInfo in staticObjectPrefab.m_Meshes) {
					// 	foreach(ComponentBase componentBase in objectMeshInfo.m_Mesh.components) {
					// 		if(componentBase is DecalProperties decalProperties) {
					// 			Plugin.Logger.LogMessage(decalProperties.prefab == staticObjectPrefab);
					// 			Plugin.Logger.LogMessage(decalProperties.m_TextureArea);
					// 		}
					// 		Plugin.Logger.LogMessage(componentBase);
					// 	}
					// }

					// foreach(ComponentBase componentBase in prefab.components) {
					// 	Plugin.Logger.LogMessage(componentBase);
					// 	if (componentBase is ObjectSubObjects objectSubObjects) {
					// 		foreach(ObjectSubObjectInfo objectSubObjectInfo in objectSubObjects.m_SubObjects) {
					// 			foreach(ComponentBase componentBase1 in objectSubObjectInfo.m_Object.components) {
					// 				Plugin.Logger.LogMessage(componentBase1);

					// 				Plugin.Logger.LogWarning(componentBase1 + "is not on the prefab PlaceHolder");
					// 			}
					// 		}
					// 	} else if(componentBase is SubObjectDefaultProbability subObjectDefaultProbability) {
					// 		Plugin.Logger.LogMessage(subObjectDefaultProbability.name);
					// 		Plugin.Logger.LogMessage(subObjectDefaultProbability);
					// 	} else {
					// 		Plugin.Logger.LogWarning(componentBase + "is not on the prefab");
					// 	}
					// }
				
					// Plugin.Logger.LogMessage(prefab.GetComponent<ObjectSubObjects>().m_SubObjects);
					// Plugin.Logger.LogMessage(prefab);

					// foreach(ObjectSubObjectInfo objectSubObjectInfo in prefab.GetComponent<ObjectSubObjects>().m_SubObjects) {
					// 	// Plugin.Logger.LogMessage(objectSubObjectInfo.m_Object);
					// 	foreach(ComponentBase componentBase in objectSubObjectInfo.m_Object.components) {
					// 		Plugin.Logger.LogMessage(componentBase);
					// 	}
					// 	// Plugin.Logger.LogMessage(objectSubObjectInfo.m_ParentMesh);
					// 	// Plugin.Logger.LogMessage(objectSubObjectInfo.m_GroupIndex);
					// }

					
					// foreach(ComponentBase componentBase in prefab.components) {
					// 	Plugin.Logger.LogMessage(componentBase);
					// }

				}

				// if(prefab is SurfacePrefab) Plugin.Logger.LogMessage(prefab.name);

				var spawnableArea = prefab.GetComponent<SpawnableArea>();
				if (prefab is SurfacePrefab && spawnableArea == null)
				{
					return true;
				} else if(prefab is SurfacePrefab && spawnableArea != null && setupCustomSurfaces) {
					setupCustomSurfaces = false;
					try {
						PrefabSystem_OnCreate.CreateCustomSurfaces(prefab.GetComponent<RenderedArea>().m_Material);
					} catch (Exception e) {Plugin.Logger.LogWarning(e);}
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

				// if(prefab is TerraformingPrefab) TerraformingUI.m_Group = GetExistingToolCategory(__instance, prefab, "Terraforming") ?? TerraformingUI.m_Group;
				if(prefab is SurfacePrefab) TerraformingUI.m_Group ??= CustomSurfaces.SetupUIGroupe(__instance, prefab);
				else if(prefab is SpacePrefab) TerraformingUI.m_Group ??= Prefab.GetOrCreateNewToolCategory(__instance, prefab, "Landscaping", "Spaces", "Decals");
				// else if(prefab.name.ToLower().Contains("decal") && prefab.GetComponent<ObjectSubLanes>() != null) TerraformingUI.m_Group = Prefab.GetOrCreateNewToolCategory(__instance, prefab, "Landscaping", "Parkings Decals", "Pathways") ?? TerraformingUI.m_Group;
				else if(prefab.name.ToLower().Contains("decal")) TerraformingUI.m_Group = Prefab.GetOrCreateNewToolCategory(__instance, prefab, "Landscaping", "Decals", "Pathways") ?? TerraformingUI.m_Group;

				if(TerraformingUI.m_Group == null) {
					// Plugin.Logger.LogWarning($"Failed to add {prefab.GetType()} | {prefab.name} to the game.");
					return false;
				} else {
					// Plugin.Logger.LogMessage($"Success to add {prefab.GetType()} | {prefab.name} to the game.");
				}

			} catch (Exception e) {Plugin.Logger.LogError(e);}

			// if(prefab is SurfacePrefab) Plugin.Logger.LogMessage(prefab.name + " Have been added in the game.");

			return true;
		}
	}
}
