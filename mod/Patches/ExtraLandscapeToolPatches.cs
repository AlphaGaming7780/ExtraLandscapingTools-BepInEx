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
		static readonly string IconsResourceKey = $"{MyPluginInfo.PLUGIN_NAME.ToLower()}";

		public static readonly string COUIBaseLocation = $"coui://{IconsResourceKey}";

		static void Prefix(GameManager __instance)
		{
			List<string> pathToIconToLoad = [Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)];

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

		private static PrefabSystem prefabSystem;

        public static void Postfix( PrefabSystem __instance)
		{

			prefabSystem = __instance;

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

			StaticObjectPrefab staticObjectPrefab = (StaticObjectPrefab)ScriptableObject.CreateInstance("StaticObjectPrefab");
			staticObjectPrefab.name = "TestDecals";
			SubObjectDefaultProbability subObjectDefaultProbability = staticObjectPrefab.AddComponent<SubObjectDefaultProbability>();
			subObjectDefaultProbability.active =  true;

			__instance.AddPrefab(staticObjectPrefab);
		}

		internal static void CreateCustomSurfaces(Material material, Material materialPlaceHolder) {
			foreach(string folder in FolderToLoadSurface) {;
				foreach(string filePath in Directory.GetDirectories( folder )) 
				{	
					SurfacePrefab surfacePrefab = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
					surfacePrefab.name = new DirectoryInfo(filePath).Name;//Path.GetFileNameWithoutExtension("test");
					// surfacePrefab.active = true;

					SurfacePrefab surfacePrefabPlaceHolder = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
					surfacePrefabPlaceHolder.name = surfacePrefab.name  + "_Placeholder";//Path.GetFileNameWithoutExtension("test");
					// surfacePrefabPlaceHolder.active = true;

					Material newMaterial = new(material);
					// newMaterial.SetVector("_BaseColor", new Vector4(1,1,1,1));
					// newMaterial.SetVector("_BaseColorMap_TexelSize", new Vector4(0,0,1024,1024));
					// newMaterial.SetFloat("_Metallic", 0.1f);
					// newMaterial.SetFloat("_Smoothness", 0.1f);

					byte[] fileData;

					try {
						fileData = File.ReadAllBytes(filePath+"\\_BaseColorMap.png");
						Texture2D texture2D_BaseColorMap = new(1024, 1024);
						if(!texture2D_BaseColorMap.LoadImage(fileData)) UnityEngine.Debug.LogError("Failed to Load Image");
						newMaterial.SetTexture("_BaseColorMap", texture2D_BaseColorMap);
					} catch {}

					try {
						fileData = File.ReadAllBytes(filePath+"\\_NormalMap.png");
						Texture2D texture2D_NormalMap = new(1024, 1024);
						if(!texture2D_NormalMap.LoadImage(fileData)) UnityEngine.Debug.LogError("Failed to Load Image");
						newMaterial.SetTexture("_NormalMap", texture2D_NormalMap);
					} catch {}

					try {
						fileData = File.ReadAllBytes(filePath+"\\_MaskMap.png");
						Texture2D texture2D_MaskMap = new(1024, 1024);
						if(!texture2D_MaskMap.LoadImage(fileData)) UnityEngine.Debug.LogError("Failed to Load Image");
						newMaterial.SetTexture("_MaskMap", texture2D_MaskMap);
						newMaterial.SetFloat("_Metallic", 1f);
						newMaterial.SetFloat("_Smoothness", 1f);
					} catch {}

					RenderedArea renderedArea = surfacePrefabPlaceHolder.AddComponent<RenderedArea>();
					renderedArea.m_RendererPriority = -100;
					renderedArea.m_LodBias = 0;
					renderedArea.m_Roundness = 1;
					renderedArea.m_Material = newMaterial;

					// renderedArea.m_Material = new(materialPlaceHolder);
					// renderedArea.m_Material.SetTexture("_BaseColorMap", texture2D_BaseColorMap);
					// renderedArea.m_Material.SetTexture("_NormalMap", texture2D_NormalMap);
					// renderedArea.m_Material.SetTexture("_MaskMap", texture2D_MaskMap);
					// renderedArea.m_Material.SetVector("_BaseColor", new Vector4(1,1,1,1));
					// renderedArea.m_Material.SetVector("_BaseColorMap_TexelSize", new Vector4(0,0,1024,1024));
					// renderedArea.m_Material.SetFloat("_Metallic", 1);
					// renderedArea.m_Material.SetFloat("_Smoothness", 1);
					// renderedArea.m_Material.SetFloat("_DecalColorMask0", 15);
					// renderedArea.m_Material.SetFloat("_DecalColorMask1", 0);
					// renderedArea.m_Material.SetFloat("_DecalColorMask2", 0);
					// renderedArea.m_Material.SetFloat("_DecalColorMask3", 0);

					// renderedArea.active=true;

					PlaceholderArea placeholderArea = surfacePrefabPlaceHolder.AddComponent<PlaceholderArea>();
					// placeholderArea.active = true;
					// surfacePrefab.prefab = surfacePrefab;

					SpawnableArea spawnableArea = surfacePrefab.AddComponent<SpawnableArea>();
					spawnableArea.m_Placeholders = new AreaPrefab[1];
					spawnableArea.m_Placeholders[0] = surfacePrefabPlaceHolder;
					// spawnableArea.m_Placeholders[1] = surfacePrefabPlaceHolder;
					// spawnableArea.active = true;

					RenderedArea renderedArea1 = surfacePrefab.AddComponent<RenderedArea>();
					renderedArea1.m_RendererPriority = -100;
					renderedArea1.m_LodBias = 0;
					renderedArea1.m_Roundness = 1;
					renderedArea1.m_Material = newMaterial;

					UIObject surfacePrefabUI = surfacePrefab.AddComponent<UIObject>();
					surfacePrefabUI.active = true;
					surfacePrefabUI.m_IsDebugObject = false;
					surfacePrefabUI.m_Icon = File.Exists(filePath+"\\icon.svg") ? $"{GameManager_InitializeThumbnails.COUIBaseLocation}/CustomSurfaces/{new DirectoryInfo(filePath).Name}/icon.svg" : ExtraLandscapingTools.GetIcon(surfacePrefab);
					surfacePrefabUI.m_Priority = 0;


					// foreach(PrefabBase prefabBase in spawnableArea.m_Placeholders) Plugin.Logger.LogMessage(prefabBase.name);

					prefabSystem.AddPrefab(surfacePrefab);
					prefabSystem.AddPrefab(surfacePrefabPlaceHolder);
				}
			}

			// __instance.AddPrefab(staticObjectPrefab);

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
		private static string UIAssetCategoryPrefabName = "";
		private static readonly Dictionary<string, List<PrefabBase>> failedSurfacePrefabs = [];

		private static bool setupCustomSurfaces = true;

		public static bool Prefix( PrefabSystem __instance, PrefabBase prefab)
		{

			if (Traverse.Create(__instance).Field("m_Entities").GetValue<Dictionary<PrefabBase, Entity>>().ContainsKey(prefab)) {
				
				// if(prefab is SurfacePrefab) Plugin.Logger.LogWarning(prefab.name + " is already added");
				return false;
			}
			try {

				if(failedSurfacePrefabs.ContainsKey(UIAssetCategoryPrefabName)) {
					string cat = UIAssetCategoryPrefabName;
					UIAssetCategoryPrefabName = "";
					PrefabBase[] temp = new PrefabBase[failedSurfacePrefabs[cat].Count];
					failedSurfacePrefabs[cat].CopyTo(temp);
					foreach(PrefabBase prefabBase in temp) {
						// Plugin.Logger.LogMessage("Trying to add prefab " + prefab.name + " to the game.");
						failedSurfacePrefabs[cat].Remove(prefabBase);
						__instance.AddPrefab(prefabBase);
					}
				}

				if (ExtraLandscapingTools.removeTools.Contains(prefab.name) || (prefab is not TerraformingPrefab && prefab is not SpacePrefab && prefab is not SurfacePrefab && prefab is not PathwayPrefab && prefab is not ObjectPrefab/* && prefab is not TaxiwayPrefab && prefab is not TrackPrefab && prefab is not NetLanePrefab*/))
				{	

					// if(prefab is UIAssetCategoryPrefab uIAssetCategoryPrefab2) Plugin.Logger.LogMessage(uIAssetCategoryPrefab2.name);

					if(prefab is UIAssetMenuPrefab uIAssetMenuPrefab && failedSurfacePrefabs.ContainsKey(uIAssetMenuPrefab.name)) {
						UIAssetCategoryPrefabName = uIAssetMenuPrefab.name;
					} else if(prefab is UIAssetCategoryPrefab uIAssetCategoryPrefab && failedSurfacePrefabs.ContainsKey(uIAssetCategoryPrefab.name)) {
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
						PrefabSystem_OnCreate.CreateCustomSurfaces(prefab.GetComponent<RenderedArea>().m_Material, spawnableArea.m_Placeholders[0].GetComponent<RenderedArea>().m_Material);
					} catch (Exception e) {Plugin.Logger.LogWarning(e);}

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

				if(prefab is TerraformingPrefab) TerraformingUI.m_Group = GetExistingToolCategory(__instance, prefab, "Terraforming") ?? TerraformingUI.m_Group;
				else if(prefab is SurfacePrefab) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, prefab, "Surfaces", "Terraforming"); //?? TerraformingUI.m_Group;
				else if(prefab is SpacePrefab) TerraformingUI.m_Group ??= GetOrCreateNewToolCategory(__instance, prefab, "Spaces", "Decals");
				else if(prefab is PathwayPrefab) TerraformingUI.m_Group = GetExistingToolCategory(__instance, prefab, "Pathways") ?? TerraformingUI.m_Group;
				// else if(prefab is AreaPrefab) TerraformingUI.m_Group ??= GetOrCreateNewToolCategory(__instance, prefab, "AreaPrefab", "Decals");
				// else if(prefab is NetLanePrefab) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, prefab, "NetLanePrefab", "Pathways") ?? TerraformingUI.m_Group;
				// else if(prefab is NetLaneGeometryPrefab) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, prefab, "NetLaneGeometryPrefab", "Pathways") ?? TerraformingUI.m_Group;
				// else if(prefab is TaxiwayPrefab) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, prefab, "TaxiwayPrefab", "Pathways") ?? TerraformingUI.m_Group;
				// else if(prefab is TrackPrefab) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, prefab, "TrackPrefab", "Pathways") ?? TerraformingUI.m_Group;
				// else if(prefab is PathfindPrefab) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, "PathfindPrefab", 1) ?? TerraformingUI.m_Group;
				// else if(prefab is RenderPrefab) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, "RenderPrefab") ?? TerraformingUI.m_Group;
				else if(prefab.name.ToLower().Contains("decal") && prefab.GetComponent<ObjectSubLanes>() != null) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, prefab, "Parkings Decals", "Pathways") ?? TerraformingUI.m_Group;
				else if(prefab.name.ToLower().Contains("decal")) TerraformingUI.m_Group = GetOrCreateNewToolCategory(__instance, prefab, "Decals", "Pathways") ?? TerraformingUI.m_Group;
				// else if(prefab.GetComponent<WaterSource>() != null) TerraformingUI.m_Group ??= GetOrCreateNewToolCategory(__instance, prefab, "Water", "Decals");
				// else if(prefab.name.ToLower().Contains("spawner")) TerraformingUI.m_Group ??= GetOrCreateNewToolCategory(__instance, "Spawner");
				
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

		private static UIAssetCategoryPrefab GetExistingToolCategory(PrefabSystem prefabSystem, PrefabBase prefabBase ,string cat)
		{

			if (!prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), cat), out var p1)
				|| p1 is not UIAssetCategoryPrefab terraformingCategory)
			{	
				AddPrefabToFailedPrefabList(prefabBase, cat);
				return null;
			}

			return terraformingCategory;

		}

		private static UIAssetCategoryPrefab GetOrCreateNewToolCategory(PrefabSystem prefabSystem, PrefabBase prefabBase, string cat, string behindcat)
		{

			if (prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), cat), out var p1)
				&& p1 is UIAssetCategoryPrefab surfaceCategory)
			{
				return surfaceCategory;
			}

			if (!prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetMenuPrefab), "Landscaping"), out var p2)
				|| p2 is not UIAssetMenuPrefab landscapingMenu)
			{	
				AddPrefabToFailedPrefabList(prefabBase, "Landscaping");
				// it can happen that the "landscaping" menu isn't added yet, as is the case on the first run through
				return null;
			}

			if (!prefabSystem.TryGetPrefab(new PrefabID(nameof(UIAssetCategoryPrefab), behindcat), out var p3)
				|| p3 is not UIAssetCategoryPrefab behindCategory)
			{
				AddPrefabToFailedPrefabList(prefabBase, behindcat);
				return null;
			}

			surfaceCategory = ScriptableObject.CreateInstance<UIAssetCategoryPrefab>();
			surfaceCategory.name = cat;
			surfaceCategory.m_Menu = landscapingMenu;
			var surfaceCategoryUI = surfaceCategory.AddComponent<UIObject>();
			surfaceCategoryUI.m_Icon = "Media/Game/Icons/LotTool.svg";
			surfaceCategoryUI.m_Priority = behindCategory.GetComponent<UIObject>().m_Priority+1;
			surfaceCategoryUI.active = true;
			surfaceCategoryUI.m_IsDebugObject = false;

			prefabSystem.AddPrefab(surfaceCategory);

			// UnityEngine.Debug.Log($"Registered Surfaces category");


			return surfaceCategory;
		}

		static void AddPrefabToFailedPrefabList(PrefabBase prefabBase, string cat) {
			if(failedSurfacePrefabs.ContainsKey(cat)) {
				failedSurfacePrefabs[cat].Add(prefabBase);
			} else {
				failedSurfacePrefabs.Add(cat, []);
				failedSurfacePrefabs[cat].Add(prefabBase);
			}

			// Plugin.Logger.LogMessage("Prefab " + prefabBase.name + " has been added to the failedPrefab "+ failedSurfacePrefabs[cat].Contains(prefabBase));

		}
	}
}
