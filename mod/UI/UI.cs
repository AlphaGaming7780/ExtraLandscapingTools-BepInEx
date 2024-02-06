using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Prefabs;
using Game.Rendering;
using Game.SceneFlow;
using Game.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace ExtraLandscapingTools
{
	
	public class ELT_UI : UISystemBase
	{	
		private static readonly GameObject eLT_UI_Object = new();
		internal static ELT_UI_Mono eLT_UI_Mono;

		private static GetterValueBinding<bool> showMarker;
		// private static GetterValueBinding<bool> enableCustomSurfaces;
		internal static bool isMarkerVisible = false;

		// private static bool isEnableCustomSurfaces = true;
		// private static EntityQuery surfaceQuery;

		protected override void OnCreate() {

			base.OnCreate();
			// SelectedInfoUISystem selectedInfoUISystem = base.World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
			ELT.m_RenderingSystem = base.World.GetOrCreateSystemManaged<RenderingSystem>();
			ELT.m_EntityManager = EntityManager;

			// selectedInfoUISystem.AddMiddleSection(base.World.GetOrCreateSystemManaged<TransformSection>());

			eLT_UI_Mono = eLT_UI_Object.AddComponent<ELT_UI_Mono>();
			AddBinding(showMarker = new GetterValueBinding<bool>("elt", "showmarker", () => ELT.m_RenderingSystem.markersVisible));
			AddBinding(new TriggerBinding<bool, bool>("elt", "showmarker", new Action<bool, bool>(ShowMarker)));

			// AddBinding(enableCustomSurfaces = new GetterValueBinding<bool>("elt", "enablecustomsurfaces", () => isEnableCustomSurfaces));
			// AddBinding(new TriggerBinding<bool>("elt", "enablecustomsurfaces", new Action<bool>(EnableCustomSurfaces)));

			// surfaceQuery = GetEntityQuery(new EntityQueryDesc
			// {
			//     All =
            //    [
            //         ComponentType.ReadOnly<SurfaceData>(),
			//         ComponentType.ReadOnly<UIObjectData>(),
			//    ],
			// });
		}

		public static void ShowMarker(bool b, bool forceUpdate = false) {
			ELT.m_RenderingSystem.markersVisible = b;
			showMarker.Update();
		}

		// private void EnableCustomSurfaces(bool b) {
		// 	NativeArray<Entity> entities =  surfaceQuery.ToEntityArray(AllocatorManager.Temp);

		// 	foreach(Entity entity in entities) {
		// 		if(b) AddEntityObjectToCategoryUI(entity);
		// 		else RemoveEntityObjectFromCategoryUI(entity);
		// 	}

		// 	isEnableCustomSurfaces = b;
		// 	enableCustomSurfaces.Update();
		// }


		// public static void RemoveEntityObjectFromCategoryUI(Entity entity) {
		// 	UIObjectData uIObjectData = ELT.m_EntityManager.GetComponentData<UIObjectData>(entity);
		// 	if(!ELT.m_EntityManager.TryGetBuffer(uIObjectData.m_Group, false, out DynamicBuffer<UIGroupElement> uiGroupBuffer)) {
		// 		Plugin.Logger.LogError($"Could not get the buffer for RemoveEntityObjectFromCategoryUI");
		// 		return;
		// 	};

		// 	for(int i = 0; i < uiGroupBuffer.Length; i++) {
		// 		if(uiGroupBuffer.ElementAt(i).m_Prefab == entity) {
		// 			uiGroupBuffer.RemoveAt(i);
		// 			break;
		// 		}
		// 	}

		// 	if(uiGroupBuffer.Length <= 0) {
		// 		RemoveEntityCategoryUiFromMenuUI(uIObjectData.m_Group);
		// 	}
		// }

		// public static void AddEntityObjectToCategoryUI(Entity entity) {

		// 	UIObjectData uIObjectData = ELT.m_EntityManager.GetComponentData<UIObjectData>(entity);
		// 	if(!ELT.m_EntityManager.TryGetBuffer(uIObjectData.m_Group, false, out DynamicBuffer<UIGroupElement> uiGroupBuffer)) {
		// 		Plugin.Logger.LogError($"Could not get the buffer for AddEntityObjectToCategoryUI");
		// 		return;
		// 	};

		// 	bool isNotInTheBuffer = true;
		// 	for (int i = 0; i < uiGroupBuffer.Length; i++)
		// 	{
		// 		if(uiGroupBuffer.ElementAt(i).m_Prefab == entity) {
		// 			isNotInTheBuffer = false;
		// 			break;
		// 		}
		// 	}

		// 	if(isNotInTheBuffer) uiGroupBuffer.Add(new UIGroupElement(entity));

		// 	AddEntityCategoryUiToMenuUI(uIObjectData.m_Group);

		// }

		// public static void RemoveEntityCategoryUiFromMenuUI(Entity entity) {
		// 	UIAssetCategoryData uIAssetCategoryData = ELT.m_EntityManager.GetComponentData<UIAssetCategoryData>(entity);
		// 	if(!ELT.m_EntityManager.TryGetBuffer(uIAssetCategoryData.m_Menu, false, out DynamicBuffer<UIGroupElement> uiGroupBuffer)) {
		// 		Plugin.Logger.LogError($"Could not get the buffer for RemoveEntityCategoryUiFromMenuUI");
		// 		return;
		// 	};

		// 	for(int i = 0; i < uiGroupBuffer.Length; i++) {
		// 		if(uiGroupBuffer.ElementAt(i).m_Prefab == entity) {
		// 			uiGroupBuffer.RemoveAt(i);
		// 			break;
		// 		}
		// 	}

		// }

		// public static void AddEntityCategoryUiToMenuUI(Entity entity) {

		// 	UIAssetCategoryData uIAssetCategoryData = ELT.m_EntityManager.GetComponentData<UIAssetCategoryData>(entity);
		// 	if(!ELT.m_EntityManager.TryGetBuffer(uIAssetCategoryData.m_Menu, false, out DynamicBuffer<UIGroupElement> uiGroupBuffer)) {
		// 		Plugin.Logger.LogError($"Could not get the buffer for AddEntityCategoryUiToMenuUI");
		// 		return;
		// 	};

		// 	bool isNotInTheBuffer = true;
		// 	for (int i = 0; i < uiGroupBuffer.Length; i++)
		// 	{
		// 		if(uiGroupBuffer.ElementAt(i).m_Prefab == entity) {
		// 			isNotInTheBuffer = false;
		// 			break;
		// 		}
		// 	}

		// 	if(isNotInTheBuffer) uiGroupBuffer.Add(new UIGroupElement(entity));

		// }


		internal static string GetStringFromEmbbededJSFile(string path) {
			return new StreamReader(ELT.GetEmbedded("UI."+path)).ReadToEnd();
		}

		// internal static void SetUpMarker(PrefabBase prefabBase) {

		// 	if(prefabBase is null) return;

		// 	Entity entity = ELT.m_PrefabSystem.GetEntity(prefabBase);
			
		// 	if (ELT.m_EntityManager.HasComponent<MarkerNetData>(entity) ||  prefabBase is MarkerObjectPrefab) {
		// 		ShowMarker(true);
		// 		isMarkerVisible = true;
		// 	}
		// 	else if (isMarkerVisible)
		// 	{	
		// 		ShowMarker(false);
		// 		isMarkerVisible = false;
		// 	}
		// }

		// public static void ShowELTSettingsButton(bool show) {
		// 	if(show) {
		// 		eLT_UI_Mono.ChangeUiNextFrame(GetStringFromEmbbededJSFile("SetupSettings.js"));
		// 	} else {
		// 		eLT_UI_Mono.ChangeUiNextFrame(GetStringFromEmbbededJSFile("REMOVE_Settings.js"));
		// 	}
		// }
	}

	internal class ELT_UI_Mono : MonoBehaviour
	{
		internal void ChangeUiNextFrame(string js) {
			StartCoroutine(ChangeUI(js));
		}

		private IEnumerator ChangeUI(string js) {
			yield return new WaitForEndOfFrame();
			GameManager.instance.userInterface.view.View.ExecuteScript(js);
			yield return null;
		}
	}
}