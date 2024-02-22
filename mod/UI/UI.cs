using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Colossal.Entities;
using Colossal.Json;
using Colossal.UI.Binding;
using ExtraLandscapingTools.Patches;
using ExtraLandscapingTools.UI;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using Game.UI;
using Game.UI.InGame;
using HarmonyLib;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ExtraLandscapingTools
{
	
	public class ELT_UI : UISystemBase
	{	

		private static readonly GameObject eLT_UI_Object = new();
		internal static ELT_UI_Mono eLT_UI_Mono;

		internal static List<SettingsUI> settings = [
			new("ELT", [
				new SettingsCheckBox("Enable Transfrom Section", "elt.enabletransformsection"),
				new SettingsButton("Clear Data", "elt.cleardata", "Use this before uninstalling."),
			]),
			new("Surfaces", [
				new SettingsCheckBox("[RESTART] Load Custom Surfaces", "elt.loadcustomsurfaces"),
				new SettingsCheckBox("[EXPERIMENTAL] [RESTART] Enable Snow On Surfaces.", "elt.enableSnowSurfaces"),
				// new SettingsButton("Clear Cache", "elt.clearsurfacescache"),
			]),
			new("Decals", [
				new SettingsCheckBox("[RESTART] Load Custom Decals", "elt.loadcustomdecals"),
			])
		];

		private static readonly List<AssetCat> assetsCats = [
			new("Custom Surfaces", $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Surfaces.svg"),
			new("Custom Decals", $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Decals.svg")
		];
		// private static string selectedAssetscat = assetsCats[0].catName; 
		// private static GetterValueBinding<string> selectedAssetCat;

		private static GetterValueBinding<bool> showMarker;
		private static GetterValueBinding<bool> selectSurfaceReplacerTool;
		private static GetterValueBinding<bool> loadcustomsurfaces;
		private static GetterValueBinding<bool> loadcustomdecals;
		private static GetterValueBinding<bool> enableTransformSection;
		private static GetterValueBinding<bool> enableSnowSurfaces;
		internal static bool isMarkerVisible = false;
		// private static bool isEnableCustomSurfaces = true;

		private static EntityQuery UIAssetCategoryQuery;
		internal static List<string> validMenuForELTSettings = ["Landscaping"];

		private static ToolBaseSystem previousTool;

		protected override void OnCreate() {

			base.OnCreate();
			ELT.m_RenderingSystem = base.World.GetOrCreateSystemManaged<RenderingSystem>();
			ELT.m_ToolSystem = base.World.GetOrCreateSystemManaged<ToolSystem>();
			ELT.m_ToolbarUISystem = base.World.GetOrCreateSystemManaged<ToolbarUISystem>();
			// ELT.m_ToolUISystem = base.World.GetOrCreateSystemManaged<ToolUISystem>();
			ELT.m_SurfaceReplacerTool = base.World.GetOrCreateSystemManaged<SurfaceReplacerTool>();
			ELT.m_EntityManager = EntityManager;
			
			eLT_UI_Mono = eLT_UI_Object.AddComponent<ELT_UI_Mono>();

			AddBinding(new GetterValueBinding<string>("elt", "settings", () => Encoder.Encode(settings, EncodeOptions.None)));
			if(Prefab.CanCreateCustomAssetMenu()) {
				AddBinding(new GetterValueBinding<string>("elt", "assetscat", () => Encoder.Encode(assetsCats, EncodeOptions.None)));
				// AddBinding(selectedAssetCat = new GetterValueBinding<string>("elt", "selectedassetcat", () => selectedAssetscat));
				AddBinding(new TriggerBinding<string>("elt", "assetscat", new Action<string>(OnAssetCatClick)));
			}
			AddBinding(selectSurfaceReplacerTool = new GetterValueBinding<bool>("elt", "selectsurfacereplacertool", () => ELT.m_ToolSystem.activeTool is SurfaceReplacerTool));
			AddBinding(new TriggerBinding<bool>("elt", "selectsurfacereplacertool", new Action<bool>(SelectSurfaceReplacerTool)));

			AddBinding(showMarker = new GetterValueBinding<bool>("elt", "showmarker", () => ELT.m_RenderingSystem.markersVisible));
			AddBinding(new TriggerBinding<bool>("elt", "showmarker", new Action<bool>(ShowMarker)));

			AddBinding(loadcustomsurfaces = new GetterValueBinding<bool>("elt", "loadcustomsurfaces", () => Settings.settings.LoadCustomSurfaces));
			AddBinding(new TriggerBinding<bool>("elt", "loadcustomsurfaces", new Action<bool>(LoadCustomSurfaces)));

			AddBinding(loadcustomdecals = new GetterValueBinding<bool>("elt", "loadcustomdecals", () => Settings.settings.LoadCustomDecals));
			AddBinding(new TriggerBinding<bool>("elt", "loadcustomdecals", new Action<bool>(LoadCustomDecals)));

			AddBinding(enableTransformSection = new GetterValueBinding<bool>("elt", "enabletransformsection", () => Settings.settings.EnableTransformSection));
			AddBinding(new TriggerBinding<bool>("elt", "enabletransformsection", new Action<bool>(EnableTransformSection)));

			AddBinding(enableSnowSurfaces = new GetterValueBinding<bool>("elt", "enableSnowSurfaces", () => Settings.settings.EnableSnowSurfaces));
			AddBinding(new TriggerBinding<bool>("elt", "enableSnowSurfaces", new Action<bool>(EnableSurfacesSnow)));


			// AddBinding(new TriggerBinding("elt", "clearsurfacescache", new Action(CustomSurfaces.ClearSurfacesCache)));
			AddBinding(new TriggerBinding("elt", "cleardata", new Action(ELT.ClearData)));

			UIAssetCategoryQuery = GetEntityQuery(new EntityQueryDesc
			{
				All =
			   [
					ComponentType.ReadOnly<UIAssetCategoryData>()
			   ],
			});

			// GameManager.instance.userInterface.view.View.ExecuteScript(GetStringFromEmbbededJSFile("Setup.js"));

		}

		public static void SelectSurfaceReplacerTool(bool b) {
			if(b) {
				previousTool = ELT.m_ToolSystem.activeTool;
				ELT.m_SurfaceReplacerTool.TrySetPrefab(ELT.m_ToolSystem.activePrefab);
				ELT.m_ToolUISystem.SelectTool(ELT.m_SurfaceReplacerTool);
				// ELT.m_ToolSystem.activeTool = ELT.m_SurfaceReplacerTool;
			}
			else ELT.m_ToolUISystem.SelectTool(previousTool);
			selectSurfaceReplacerTool.Update();
		}

		public static void ShowMarker(bool b) {
			ELT.m_RenderingSystem.markersVisible = b;
			showMarker.Update();
		}

		private void LoadCustomSurfaces(bool b) {
			Settings.settings.LoadCustomSurfaces = b;
			Settings.SaveSettings("ELT", Settings.settings);
			loadcustomsurfaces.Update();
		}

		private void LoadCustomDecals(bool b) {
			Settings.settings.LoadCustomDecals = b;
			Settings.SaveSettings("ELT", Settings.settings);
			loadcustomdecals.Update();
		}

		private void EnableTransformSection( bool b ) {
			Settings.settings.EnableTransformSection = b;
			Settings.SaveSettings("ELT", Settings.settings);
			enableTransformSection.Update();
		}

		private void EnableSurfacesSnow( bool b ) {
			Settings.settings.EnableSnowSurfaces = b;
			Settings.SaveSettings("ELT", Settings.settings);
			// if(!b) CustomSurfaces.ClearSurfacesCache();
			enableSnowSurfaces.Update();
		}

		private void OnAssetCatClick(string assetCat) {
			bool first = true;
			NativeArray<Entity> entities =  UIAssetCategoryQuery.ToEntityArray(AllocatorManager.Temp);
			foreach(Entity entity in entities) {
				UIAssetCategoryPrefab uIAssetCategoryPrefab = ELT.m_PrefabSystem.GetPrefab<UIAssetCategoryPrefab>(entity);
				if(uIAssetCategoryPrefab.m_Menu.name != "Custom Assets") continue;
				if(assetCat == "Custom Surfaces") {
					if(uIAssetCategoryPrefab.name.ToLower().Contains("decals")) RemoveEntityCategoryUiFromMenuUI(entity);
					else if(uIAssetCategoryPrefab.name.ToLower().Contains("surfaces")) {
						AddEntityCategoryUiToMenuUI(entity);
						if(first) {
							first = false;
							ToolbarUISystemPatch.SelectCatUI(entity);
						}
					}
				}
				else if(assetCat == "Custom Decals") {
					if(uIAssetCategoryPrefab.name.ToLower().Contains("decals")) {
						AddEntityCategoryUiToMenuUI(entity);
						if(first) {
							first = false;
							ToolbarUISystemPatch.SelectCatUI(entity);
						}
					}
					else if(uIAssetCategoryPrefab.name.ToLower().Contains("surfaces")) RemoveEntityCategoryUiFromMenuUI(entity);
				}
			}
			// selectedAssetscat = assetCat;
			// selectedAssetCat.Update();
			ToolbarUISystemPatch.UpdateMenuUI();
		}

		public static void RemoveEntityObjectFromCategoryUI(Entity entity) {
			UIObjectData uIObjectData = ELT.m_EntityManager.GetComponentData<UIObjectData>(entity);
			if(!ELT.m_EntityManager.TryGetBuffer(uIObjectData.m_Group, false, out DynamicBuffer<UIGroupElement> uiGroupBuffer)) {
				Plugin.Logger.LogError($"Could not get the buffer for RemoveEntityObjectFromCategoryUI");
				return;
			};

			for(int i = 0; i < uiGroupBuffer.Length; i++) {
				if(uiGroupBuffer.ElementAt(i).m_Prefab == entity) {
					uiGroupBuffer.RemoveAt(i);
					break;
				}
			}

			if(uiGroupBuffer.Length <= 0) {
				RemoveEntityCategoryUiFromMenuUI(uIObjectData.m_Group);
			}
		}

		public static void AddEntityObjectToCategoryUI(Entity entity) {

			UIObjectData uIObjectData = ELT.m_EntityManager.GetComponentData<UIObjectData>(entity);
			if(!ELT.m_EntityManager.TryGetBuffer(uIObjectData.m_Group, false, out DynamicBuffer<UIGroupElement> uiGroupBuffer)) {
				Plugin.Logger.LogError($"Could not get the buffer for AddEntityObjectToCategoryUI");
				return;
			};

			bool isNotInTheBuffer = true;
			for (int i = 0; i < uiGroupBuffer.Length; i++)
			{
				if(uiGroupBuffer.ElementAt(i).m_Prefab == entity) {
					isNotInTheBuffer = false;
					break;
				}
			}

			if(isNotInTheBuffer) uiGroupBuffer.Add(new UIGroupElement(entity));

			AddEntityCategoryUiToMenuUI(uIObjectData.m_Group);

		}

		public static void RemoveEntityCategoryUiFromMenuUI(Entity entity) {
			UIAssetCategoryData uIAssetCategoryData = ELT.m_EntityManager.GetComponentData<UIAssetCategoryData>(entity);
			if(!ELT.m_EntityManager.TryGetBuffer(uIAssetCategoryData.m_Menu, false, out DynamicBuffer<UIGroupElement> uiGroupBuffer)) {
				Plugin.Logger.LogError($"Could not get the buffer for RemoveEntityCategoryUiFromMenuUI");
				return;
			};

			for(int i = 0; i < uiGroupBuffer.Length; i++) {
				if(uiGroupBuffer.ElementAt(i).m_Prefab == entity) {
					uiGroupBuffer.RemoveAt(i);
					break;
				}
			}
			
			// ELT.m_EntityManager.AddComponentData(uIAssetCategoryData.m_Menu, new Unlock(uIAssetCategoryData.m_Menu));
		}

		public static void AddEntityCategoryUiToMenuUI(Entity entity) {

			UIAssetCategoryData uIAssetCategoryData = ELT.m_EntityManager.GetComponentData<UIAssetCategoryData>(entity);
			if(!ELT.m_EntityManager.TryGetBuffer(uIAssetCategoryData.m_Menu, false, out DynamicBuffer<UIGroupElement> uiGroupBuffer)) {
				Plugin.Logger.LogError($"Could not get the buffer for AddEntityCategoryUiToMenuUI");
				return;
			};
			
			bool isNotInTheBuffer = true;
			for (int i = 0; i < uiGroupBuffer.Length; i++)
			{
				if(uiGroupBuffer.ElementAt(i).m_Prefab == entity) {
					isNotInTheBuffer = false;
					break;
				}
			}

			if(isNotInTheBuffer) uiGroupBuffer.Add(new UIGroupElement(entity));
		}

		internal static string GetStringFromEmbbededJSFile(string path) {
			return new StreamReader(ELT.GetEmbedded("UI."+path)).ReadToEnd();
		}

		internal static void SetUpMarker(PrefabBase prefabBase) {

			if (prefabBase != null && !ELT.m_RenderingSystem.markersVisible)
			{
				Entity entity = ELT.m_PrefabSystem.GetEntity(prefabBase);
				
				if (ELT.m_EntityManager.HasComponent<MarkerNetData>(entity) ||  prefabBase is MarkerObjectPrefab) {
					ShowMarker(true);
					isMarkerVisible = true;
				}
				else if (isMarkerVisible)
				{	
					ShowMarker(false);
					isMarkerVisible = false;
				}
			}
		}

		public static void ShowELTSettingsButton(bool show) {
			if(show) {
				eLT_UI_Mono.ChangeUiNextFrame(GetStringFromEmbbededJSFile("SetupSettings.js"));
			} else {
				eLT_UI_Mono.ChangeUiNextFrame(GetStringFromEmbbededJSFile("REMOVE_Settings.js"));
			}
		}
	}

	internal class ELT_UI_Mono : MonoBehaviour
	{
		public void ChangeUiNextFrame(string js) {
			StartCoroutine(ChangeUI(js));
		}

		private IEnumerator ChangeUI(string js) {
			yield return new WaitForEndOfFrame();
			GameManager.instance.userInterface.view.View.ExecuteScript(js);
			yield return null;
		}
	}
}