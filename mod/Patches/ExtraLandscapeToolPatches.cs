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

namespace ExtraLandscapingTools.Patches
{

	[HarmonyPatch(typeof(GameSystemBase), "OnCreate")]
	internal class GameSystemBase_OnCreate
	{	

		static private readonly string PathToParent = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
		public static readonly string PathToMods = Path.Combine(PathToParent,"ExtraLandscapingTools_mods");
		public static readonly string PathToCustomBrushes = Path.Combine(PathToMods,"CustomBrushes");
		public static readonly string PathToCustomSurface = Path.Combine(PathToMods,"CustomSurfaces");

		static internal readonly string resources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources");
		static internal readonly string resourcesIcons = Path.Combine(resources, "Icons");
		static internal readonly string resourcesBrushes = Path.Combine(resources, "Brushes");

		static void Prefix(GameSystemBase __instance)
		{		
			if(!PrefabSystem_OnCreate.FolderToLoadBrush.Contains(resourcesBrushes) && Directory.Exists(resourcesBrushes)) PrefabSystem_OnCreate.FolderToLoadBrush.Add(resourcesBrushes);
			if(!PrefabSystem_OnCreate.FolderToLoadBrush.Contains(PathToCustomBrushes) && Directory.Exists(PathToCustomBrushes)) PrefabSystem_OnCreate.FolderToLoadBrush.Add(PathToCustomBrushes);
			if(!PrefabSystem_OnCreate.FolderToLoadSurface.Contains(PathToCustomSurface) && Directory.Exists(PathToCustomSurface)) PrefabSystem_OnCreate.FolderToLoadSurface.Add(PathToCustomSurface);

			if(!Directory.Exists(resources)) {
				Directory.CreateDirectory(resources);
				Directory.CreateDirectory(resourcesBrushes);
				Directory.CreateDirectory(resourcesIcons);
				foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.svg")) {
					File.Move(file, Path.Combine(resourcesIcons,Path.GetFileName(file)));
				}
				foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.png")) {
					if(Path.GetFileNameWithoutExtension(file) != "icon") File.Move(file, Path.Combine(resourcesBrushes,Path.GetFileName(file)));
				}
			}
		}
	}

	[HarmonyPatch(typeof(GameManager), "InitializeThumbnails")]
	internal class GameManager_InitializeThumbnails
	{	
		internal static  GameObject extendedRadioGameObject = new();
		static readonly string IconsResourceKey = $"{MyPluginInfo.PLUGIN_NAME.ToLower()}";

		public static readonly string COUIBaseLocation = $"coui://{IconsResourceKey}";

		static void Prefix(GameManager __instance)
		{		
			List<string> pathToIconToLoad = [Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)];

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

			if(tool.toolID == "Terrain Tool") {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("UI.js"));
			} else {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("REMOVE_UI.js"));
			}
		}
	}

	[HarmonyPatch(typeof(PrefabSystem), "OnCreate")]
	public class PrefabSystem_OnCreate
	{
        internal static List<string> FolderToLoadBrush = [];
        internal static List<string> FolderToLoadSurface = [];
        public static void Postfix( PrefabSystem __instance)
		{
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

			// foreach(string folder in FolderToLoadSurface) {;
			// 	foreach(string filePath in Directory.GetFiles(folder)) 
			// 	{	
			// 		SurfacePrefab surfacePrefab = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
			// 		surfacePrefab.name = "test";//Path.GetFileNameWithoutExtension("test");
			// 		surfacePrefab.active = true;

			// 		// surfacePrefab.prefab = surfacePrefab;

			// 		SpawnableArea spawnableArea2 = surfacePrefab.AddComponent<SpawnableArea>();
			// 		spawnableArea2.m_Placeholders = new AreaPrefab[1];
			// 		spawnableArea2.m_Placeholders[0] = surfacePrefab;

			// 		__instance.AddPrefab(surfacePrefab);
			// 	}
			// }
        }
	}


	[HarmonyPatch(typeof(PrefabSystem), nameof(PrefabSystem.AddPrefab))]
	public class PrefabSystem_AddPrefab
	{
		public static bool Prefix( PrefabSystem __instance, PrefabBase prefab)
		{
			try {
				if (ExtraLandscapingTools.removeTools.Contains(prefab.name) || (prefab is not TerraformingPrefab && prefab is not SurfacePrefab))
				{
					return true;
				}

				var spawnableArea = prefab.GetComponent<SpawnableArea>();
				if (prefab is SurfacePrefab && spawnableArea == null)
				{
					return true;
				}

				var TerraformingUI = prefab.GetComponent<UIObject>();
				if (TerraformingUI == null)
				{
					TerraformingUI = prefab.AddComponent<UIObject>();
					TerraformingUI.active = true;
					TerraformingUI.m_IsDebugObject = false;
					TerraformingUI.m_Icon = ExtraLandscapingTools.GetIcon(prefab);
					TerraformingUI.m_Priority = 1;
				}

				if(prefab is TerraformingPrefab) TerraformingUI.m_Group = GetTerraformingToolCategory(__instance) ?? TerraformingUI.m_Group;
				if(prefab is SurfacePrefab) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, "Surfaces") ?? TerraformingUI.m_Group;
			} catch (Exception e) {UnityEngine.Debug.LogError(e);}

			return true;
		}

		private static UIAssetCategoryPrefab GetTerraformingToolCategory(PrefabSystem prefabSystem)
		{

			prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), "Terraforming"), out var p1);

			return (UIAssetCategoryPrefab)p1;

		}

		private static UIAssetCategoryPrefab GetOrCreateNewToolCategory(PrefabSystem prefabSystem, string cat)
		{
			if (prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), cat), out var p1)
				&& p1 is UIAssetCategoryPrefab surfaceCategory)
			{
				return surfaceCategory;
			}

			if (!prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetMenuPrefab), "Landscaping"), out var p2)
				|| p2 is not UIAssetMenuPrefab landscapingMenu)
			{
				// it can happen that the "landscaping" menu isn't added yet, as is the case on the first run through
				return null;
			}

			surfaceCategory = ScriptableObject.CreateInstance<UIAssetCategoryPrefab>();
			surfaceCategory.name = cat;
			surfaceCategory.m_Menu = landscapingMenu;
			var surfaceCategoryUI = surfaceCategory.AddComponent<UIObject>();
			surfaceCategoryUI.m_Icon = "Media/Game/Icons/LotTool.svg";
			surfaceCategoryUI.m_Priority = 10;
			surfaceCategoryUI.active = true;
			surfaceCategoryUI.m_IsDebugObject = false;

			prefabSystem.AddPrefab(surfaceCategory);

			// UnityEngine.Debug.Log($"Registered Surfaces category");

			return surfaceCategory;
		}
	}
}
