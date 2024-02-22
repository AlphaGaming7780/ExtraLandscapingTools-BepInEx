using cohtml.Net;
using Colossal.UI.Binding;
using Game.Common;
using Game.Prefabs;
using Game.UI;
using Game.UI.InGame;
using HarmonyLib;
using Unity.Entities;
using UnityEngine.InputSystem.EnhancedTouch;

namespace ExtraLandscapingTools.Patches;

public class ToolbarUISystemPatch
{
	[HarmonyPatch( typeof( ToolbarUISystem ), "SelectAssetMenu" )]
	class SelectAssetMenu 
	{
		static void Postfix( Entity assetMenu ) {

			if (assetMenu != Entity.Null && ELT.m_EntityManager.HasComponent<UIAssetMenuData>(assetMenu)) {
				ELT.m_PrefabSystem.TryGetPrefab(assetMenu, out PrefabBase prefabBase);

				if(prefabBase is UIAssetMenuPrefab && ELT_UI.validMenuForELTSettings.Contains(prefabBase.name)) {
					ELT_UI.ShowELTSettingsButton(true);
				} else {
					ELT_UI.ShowELTSettingsButton(false);
				}

				if(prefabBase is UIAssetMenuPrefab && prefabBase.name == "Custom Assets") {
					ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("SetupCustomAssetTopBar.js"));
				} else {
					ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("REMOVE_CustomAssetTopBar.js"));
				}

			}
		}
	}

	internal static void UpdateCatUI() {
		Traverse.Create(ELT.m_ToolbarUISystem).Field("RawValueBinding").GetValue< RawMapBinding<Entity>>().UpdateAll();
	}

	internal static void UpdateMenuUI() {
		Traverse.Create(ELT.m_ToolbarUISystem).Field("m_AssetMenuCategoriesBinding").GetValue< RawMapBinding<Entity>>().UpdateAll();
	}

	internal static void SelectCatUI(Entity entity) {
		Traverse.Create(ELT.m_ToolbarUISystem).Method("SelectAssetCategory", [typeof(Entity)]).GetValue(entity);
	}

	internal static void SelectMenuUI(Entity entity) {
		Traverse.Create(ELT.m_ToolbarUISystem).Method("SelectAssetMenu", [typeof(Entity)]).GetValue(entity);
	}

}