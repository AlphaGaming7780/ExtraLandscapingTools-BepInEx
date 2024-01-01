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

namespace ExtraLandscapingTools.Patches
{

	[HarmonyPatch(typeof(GameManager), "InitializeThumbnails")]
	internal class GameManager_InitializeThumbnails
	{	
		internal static  GameObject extendedRadioGameObject = new();
		static readonly string IconsResourceKey = $"{MyPluginInfo.PLUGIN_NAME.ToLower()}";

		public static readonly string COUIBaseLocation = $"coui://{IconsResourceKey}";

		static internal readonly string resources = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "resources");

		static void Prefix(GameManager __instance)
		{		

			List<string> pathToIconToLoad = [Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)];

			if(!Directory.Exists(resources)) {
				Directory.CreateDirectory(resources);
				foreach(string file in Directory.GetFiles(Assembly.GetExecutingAssembly().Location, ".svg")) {
					File.Move(file, resources);
				}
				// File.Move(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DefaultIcon.svg"), Path.Combine(resources , "DefaultIcon.svg"));
			}

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
		public static void Postfix(Game.UpdateSystem updateSystem) {
			updateSystem.UpdateAt<ELT_UI>(Game.SystemUpdatePhase.UIUpdate);
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

	[HarmonyPatch(typeof(PrefabSystem), nameof(PrefabSystem.AddPrefab))]
	public class PrefabSystem_AddPrefab
	{
		public static bool Prefix( PrefabSystem __instance, PrefabBase prefab)
		{
			if (prefab is not TerraformingPrefab && prefab is not SurfacePrefab)
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
				// UnityEngine.Debug.Log($"Not Found TerraformingPrefab UIObject: '{prefab.GetPrefabID()}'");
				TerraformingUI = prefab.AddComponent<UIObject>();
				TerraformingUI.active = true;
				TerraformingUI.m_IsDebugObject = false;
				TerraformingUI.m_Icon = ExtraLandscapingTools.GetIcon(prefab); // TODO actual icon?
				TerraformingUI.m_Priority = 1;
			}

			if(prefab is TerraformingPrefab) TerraformingUI.m_Group = GetOrCreateTerraformingToolCategory(__instance) ?? TerraformingUI.m_Group;
			if(prefab is SurfacePrefab) TerraformingUI.m_Group = GetOrCreateSurfacesToolCategory(__instance) ?? TerraformingUI.m_Group;

			return true;
		}

		private static UIAssetCategoryPrefab? GetOrCreateTerraformingToolCategory(PrefabSystem prefabSystem)
		{

			prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), "Terraforming"), out var p1);

			return (UIAssetCategoryPrefab)p1;

		}

		private static UIAssetCategoryPrefab? GetOrCreateSurfacesToolCategory(PrefabSystem prefabSystem)
        {
            if (prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), "Surfaces"), out var p1)
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
            surfaceCategory.name = "Surfaces";
            surfaceCategory.m_Menu = landscapingMenu;
            var surfaceCategoryUI = surfaceCategory.AddComponent<UIObject>();
            surfaceCategoryUI.m_Icon = "Media/Game/Icons/LotTool.svg";
            surfaceCategoryUI.m_Priority = 4;
            surfaceCategoryUI.active = true;
            surfaceCategoryUI.m_IsDebugObject = false;

            prefabSystem.AddPrefab(surfaceCategory);

            // UnityEngine.Debug.Log($"Registered Surfaces category");

            return surfaceCategory;
        }

	}


}
