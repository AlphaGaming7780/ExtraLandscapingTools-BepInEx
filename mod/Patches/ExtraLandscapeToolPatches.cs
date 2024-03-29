﻿using HarmonyLib;
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

	[HarmonyPatch(typeof(GameManager), "Awake")]
	internal class GameManager_Awake
	{	
		static private readonly string PathToParent = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
		public static readonly string PathToMods = Path.Combine(PathToParent,"ExtraLandscapingTools_mods");
		public static readonly string PathToCustomBrushes = Path.Combine(PathToMods,"CustomBrushes");
		public static readonly string PathToCustomSurface = Path.Combine(PathToMods,"CustomSurfaces");

		static readonly string pathToZip = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\resources.zip";

		static internal readonly string resources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources");
		static internal readonly string resourcesIcons = Path.Combine(resources, "Icons");
		static internal readonly string resourcesBrushes = Path.Combine(resources, "Brushes");

		static void Postfix(GameSystemBase __instance)
		{
			if(File.Exists(pathToZip)) {
				if(Directory.Exists(resources)) Directory.Delete(resources, true);
				ZipFile.ExtractToDirectory(pathToZip, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				File.Delete(pathToZip);
			}

			if(!PrefabSystem_OnCreate.FolderToLoadBrush.Contains(resourcesBrushes) && Directory.Exists(resourcesBrushes)) PrefabSystem_OnCreate.FolderToLoadBrush.Add(resourcesBrushes);
			if(!PrefabSystem_OnCreate.FolderToLoadBrush.Contains(PathToCustomBrushes) && Directory.Exists(PathToCustomBrushes)) PrefabSystem_OnCreate.FolderToLoadBrush.Add(PathToCustomBrushes);
			if(!CustomSurfaces.FolderToLoadSurface.Contains(PathToCustomSurface) && Directory.Exists(PathToCustomSurface)) CustomSurfaces.FolderToLoadSurface.Add(PathToCustomSurface);

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
	internal class GameManager_InitializeThumbnails
	{	
		private static readonly string IconsResourceKey = $"{MyPluginInfo.PLUGIN_NAME.ToLower()}";

		public static readonly string COUIBaseLocation = $"coui://{IconsResourceKey}";

        internal readonly static List<string> pathToIconToLoad = [Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)];


		static void Prefix(GameManager __instance)
		{

			if(Directory.Exists(GameManager_Awake.PathToMods)) pathToIconToLoad.Add(GameManager_Awake.PathToMods);

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
		internal static bool showMarker = false;
		private static void Postfix( ToolBaseSystem tool ) {

			if(tool is TerrainToolSystem) {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("UI.js"));
			} else {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("REMOVE_UI.js"));
			}

			if(tool is AreaToolSystem) { //|| tool is ObjectToolSystem
				// if(!showMarker) 
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("ShowMarker.js"));
				showMarker = true;
			} else if(showMarker){
				ELT_UI.ShowMarker(false);
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("REMOVE_ShowMarker.js"));
				showMarker = false;
			}

			// ELT_UI.SetUpMarker(tool.GetPrefab());

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

		private static bool setupCustomSurfaces = true;
		private static bool setupCustomDecals = true;

		public static bool Prefix( PrefabSystem __instance, PrefabBase prefab)
		{

			if (Traverse.Create(__instance).Field("m_Entities").GetValue<Dictionary<PrefabBase, Entity>>().ContainsKey(prefab)) {
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

				if (prefab is StaticObjectPrefab && !prefab.name.ToLower().Contains("decal") && !prefab.name.ToLower().Contains("roadarrow")) 
				{
					return true;
				}

				if(prefab is StaticObjectPrefab staticObjectPrefab && (prefab.name.ToLower().Contains("decal") || prefab.name.ToLower().Contains("roadarrow"))) {
					if(prefab.name.ToLower().Contains("invisible")) {
						return true;
					}else if(setupCustomDecals) {
						if(staticObjectPrefab.m_Meshes[0].m_Mesh is RenderPrefab renderPrefab) {
							setupCustomDecals = false;
							// CustomDecals.CreateCustomDecals(renderPrefab);
						}
					}
				}

				var spawnableArea = prefab.GetComponent<SpawnableArea>();
				if (prefab is SurfacePrefab && spawnableArea == null)
				{
					return true;
				} else if(prefab is SurfacePrefab && spawnableArea != null && setupCustomSurfaces) {
					setupCustomSurfaces = false;
					try {
						CustomSurfaces.CreateCustomSurfaces(prefab.GetComponent<RenderedArea>().m_Material);

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

				if(prefab is TerraformingPrefab) TerraformingUI.m_Group = Prefab.GetExistingToolCategory(__instance, prefab, "Terraforming") ?? TerraformingUI.m_Group;
				else if(prefab is SurfacePrefab) TerraformingUI.m_Group ??= CustomSurfaces.SetupUIGroupe(__instance, prefab);
				else if(prefab.name.ToLower().Contains("decal")) TerraformingUI.m_Group = Prefab.GetOrCreateNewToolCategory(__instance, prefab, "Landscaping", "Decals", "Pathways") ?? TerraformingUI.m_Group;
				else if(prefab.name.ToLower().Contains("roadarrow")) TerraformingUI.m_Group = Prefab.GetOrCreateNewToolCategory(__instance, prefab, "Landscaping", "Decals", "Pathways") ?? TerraformingUI.m_Group;
				else TerraformingUI.m_Group ??= Prefab.GetOrCreateNewToolCategory(__instance, prefab, "Landscaping", "[ELT] Failed Prefab, IF you see this tab, repport it, it's a bug.");

				if(TerraformingUI.m_Group == null) {
					return false;
				} else {
				}

			} catch (Exception e) {Plugin.Logger.LogError(e);}

			return true;
		}
	}
}


