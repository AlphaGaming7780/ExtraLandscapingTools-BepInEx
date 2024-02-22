using System.Collections.Generic;
using Game.Prefabs;
using UnityEngine;

namespace ExtraLandscapingTools;

public class Prefab
{

	internal delegate bool OnAddPrefab(PrefabBase prefabBase);
	internal static OnAddPrefab onAddPrefab;

	internal static readonly Dictionary<string, List<PrefabBase>> failedPrefabs = [];

	public static UIAssetCategoryPrefab GetExistingToolCategory(PrefabBase prefabBase ,string cat)
	{

		if (!ELT.m_PrefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), cat), out var p1)
			|| p1 is not UIAssetCategoryPrefab terraformingCategory)
		{	
			AddPrefabToFailedPrefabList(prefabBase, cat);
			return null;
		}

		return terraformingCategory;

	}

	public static UIAssetCategoryPrefab GetOrCreateNewToolCategory(PrefabBase prefabBase, string menu, string cat, string behindcat = null, string iconPath = null)
	{

		if (ELT.m_PrefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), cat), out var p1)
			&& p1 is UIAssetCategoryPrefab surfaceCategory)
		{
			return surfaceCategory;
		}

		UIAssetCategoryPrefab behindCategory = null;

		if(behindcat != null) {
			if (!ELT.m_PrefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), behindcat), out var p3)
				|| p3 is not UIAssetCategoryPrefab behindCategory2)
			{
				AddPrefabToFailedPrefabList(prefabBase, behindcat);
				return null;
			} else {
				behindCategory = behindCategory2;
			}
		}

		if (!ELT.m_PrefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetMenuPrefab), menu), out var p2)
			|| p2 is not UIAssetMenuPrefab landscapingMenu)
		{	
			AddPrefabToFailedPrefabList(prefabBase, menu);
			// it can happen that the "landscaping" menu isn't added yet, as is the case on the first run through
			return null;
		}

		surfaceCategory = ScriptableObject.CreateInstance<UIAssetCategoryPrefab>();
		surfaceCategory.name = cat;
		surfaceCategory.m_Menu = landscapingMenu;
		var surfaceCategoryUI = surfaceCategory.AddComponent<UIObject>();
		surfaceCategoryUI.m_Icon = iconPath ?? ELT.GetIcon(surfaceCategory);
		if(behindCategory != null) surfaceCategoryUI.m_Priority = behindCategory.GetComponent<UIObject>().m_Priority+1;
		surfaceCategoryUI.active = true;
		surfaceCategoryUI.m_IsDebugObject = false;

		ELT.m_PrefabSystem.AddPrefab(surfaceCategory);

		return surfaceCategory;
	}

	public static void CreateNewUiToolMenu(PrefabBase prefab, string menu, int offset = 1) {
	if (!ELT.m_PrefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetMenuPrefab), menu), out var p2) //Landscaping
		|| p2 is not UIAssetMenuPrefab SurfaceMenu)
		{
			SurfaceMenu = ScriptableObject.CreateInstance<UIAssetMenuPrefab>();
			SurfaceMenu.name = menu;
			var SurfaceMenuUI = SurfaceMenu.AddComponent<UIObject>();
			SurfaceMenuUI.m_Icon = ELT.GetIcon(SurfaceMenu);
			SurfaceMenuUI.m_Priority = prefab.GetComponent<UIObject>().m_Priority + offset;
			SurfaceMenuUI.active = true;
			SurfaceMenuUI.m_IsDebugObject = false;
			SurfaceMenuUI.m_Group = prefab.GetComponent<UIObject>().m_Group;

			ELT_UI.validMenuForELTSettings.Add(menu);

			ELT.m_PrefabSystem.AddPrefab(SurfaceMenu);
		}
	}

	private static void AddPrefabToFailedPrefabList(PrefabBase prefabBase, string cat) {
		if(failedPrefabs.ContainsKey(cat)) {
			failedPrefabs[cat].Add(prefabBase);
		} else {
			failedPrefabs.Add(cat, []);
			failedPrefabs[cat].Add(prefabBase);
		}
	}
	internal static string GetCustomAssetMenuName() {
		if(CanCreateCustomAssetMenu()) return "Custom Assets";
		else if(CustomSurfaces.CanCreateCustomSurfaces()) return "Custom Surfaces";
		else if(CustomDecals.CanCreateCustomDecals()) return "Custom Decals";
		else return "Custom Assets";
	}

	internal static bool CanCreateCustomAssetMenu() {
		return CustomSurfaces.CanCreateCustomSurfaces() && CustomDecals.CanCreateCustomDecals();
	}

}