using System;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;
using Unity.Mathematics;

namespace ExtraLandscapingTools
{
	
	public class TransformSection : UISystemBase
	{	
		private static GetterValueBinding<float3> transformSectionGetPos;

		public static Entity selectedEntity;
		public static Entity selectedPrefab;


		protected override void OnCreate() {

			base.OnCreate();
			SelectedInfoUISystem selectedInfoUISystem = base.World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
			selectedInfoUISystem.eventSelectionChanged += OnSelectionChanged;

			AddBinding(transformSectionGetPos = new GetterValueBinding<float3>("elt", "transformsection_getpos", GetPosition));
			AddBinding(new TriggerBinding<float3>("elt", "transformsection_getpos", new Action<float3>(SetPosition)));
		}

		private static void SetPosition(float3 position) {
			Plugin.Logger.LogMessage(position);
			ELT.m_EntityManager.TryGetComponent(selectedEntity, out Game.Objects.Transform transform);
			transform.m_Position = position;
			ELT.m_EntityManager.SetComponentData(selectedEntity, transform);
			transformSectionGetPos.Update();
		}


		private static float3 GetPosition() {

			Entity selected = selectedEntity;
			Entity pref = selectedPrefab;
// Game.Objects.Object
// Game.Objects.Static
// Game.Objects.ObjectGeometry
// Game.Tools.Hidden
			ELT.m_EntityManager.TryGetComponent(selectedEntity, out Game.Objects.Transform transform);
			return transform.m_Position;
		}

		private static void OnSelectionChanged(Entity entity, Entity prefab, float3 SelectedPosition) {

			if(entity == Entity.Null) return;

			selectedEntity = entity;
			selectedPrefab = prefab;

			transformSectionGetPos.Update();

			ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("TransformSection.js"));
		}
	}
}