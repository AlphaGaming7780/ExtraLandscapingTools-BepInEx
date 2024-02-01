using System.Collections.Generic;
using Game.Prefabs;
using UnityEngine;

namespace ExtraLandscapingTools;

public class Prefab
{

    internal delegate void OnAddPrefab(PrefabBase prefabBase);
    internal static OnAddPrefab onAddPrefab;

    internal static readonly Dictionary<string, List<PrefabBase>> failedPrefabs = [];

    internal static UIAssetCategoryPrefab GetExistingToolCategory(PrefabSystem prefabSystem, PrefabBase prefabBase ,string cat)
    {

        if (!prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), cat), out var p1)
            || p1 is not UIAssetCategoryPrefab terraformingCategory)
        {	
            AddPrefabToFailedPrefabList(prefabBase, cat);
            return null;
        }

        return terraformingCategory;

    }

    internal static UIAssetCategoryPrefab GetOrCreateNewToolCategory(PrefabSystem prefabSystem, PrefabBase prefabBase, string menu, string cat, string behindcat = null, string iconPath = null)
    {

        if (prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), cat), out var p1)
            && p1 is UIAssetCategoryPrefab surfaceCategory)
        {
            return surfaceCategory;
        }

        UIAssetCategoryPrefab behindCategory = null;

        if(behindcat != null) {
            if (!prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), behindcat), out var p3)
                || p3 is not UIAssetCategoryPrefab behindCategory2)
            {
                AddPrefabToFailedPrefabList(prefabBase, behindcat);
                return null;
            } else {
                behindCategory = behindCategory2;
            }
        }

        if (!prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetMenuPrefab), menu), out var p2)
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

        prefabSystem.AddPrefab(surfaceCategory);

        return surfaceCategory;
    }

    public static void RegisterNewToolMenu(string name, string insertAfter) {
        
    }

    private static void AddPrefabToFailedPrefabList(PrefabBase prefabBase, string cat) {
        if(failedPrefabs.ContainsKey(cat)) {
            failedPrefabs[cat].Add(prefabBase);
        } else {
            failedPrefabs.Add(cat, []);
            failedPrefabs[cat].Add(prefabBase);
        }
    }
}