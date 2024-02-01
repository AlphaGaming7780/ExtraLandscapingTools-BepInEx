using System.Collections.Generic;
using Game.Prefabs;
using UnityEngine;

namespace ExtraLandscapingTools;

public class Prefab
{

    internal delegate void OnAddPrefab(PrefabBase prefabBase);
    internal static OnAddPrefab onAddPrefab;

    internal static Dictionary<string, List<string>> newUiMenu = [];

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

    public static void RegisterNewToolMenu(string name, string insertAfter = null, bool elt = true) {

        insertAfter ??= "none";

        if(newUiMenu.ContainsKey(insertAfter)) {
            newUiMenu[insertAfter].Add(name);
        } else {
            newUiMenu.Add(insertAfter, new([name]));
        }

        if(elt) ELT_UI.validMenuForELTSettings.Add(name);

    }

    internal static void CreateNewUiToolMenu(PrefabBase prefab, string menu) {
        if (!ELT.m_PrefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetMenuPrefab), menu), out var p2) //Landscaping
            || p2 is not UIAssetMenuPrefab SurfaceMenu)
            {
                SurfaceMenu = ScriptableObject.CreateInstance<UIAssetMenuPrefab>();
                SurfaceMenu.name = menu;
                var SurfaceMenuUI = SurfaceMenu.AddComponent<UIObject>();
                SurfaceMenuUI.m_Icon = ELT.GetIcon(SurfaceMenu);
                SurfaceMenuUI.m_Priority = prefab.GetComponent<UIObject>().m_Priority;
                SurfaceMenuUI.active = true;
                SurfaceMenuUI.m_IsDebugObject = false;
                SurfaceMenuUI.m_Group = prefab.GetComponent<UIObject>().m_Group;

                __instance.AddPrefab(SurfaceMenu);
            }
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
}