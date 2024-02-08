using System;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.UI.Binding;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.UI;
using Game.UI.InGame;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Game.Buildings;
using Game.Net;

namespace ExtraLandscapingTools
{
	
	public class TransformSection : UISystemBase
	{	
		private static GetterValueBinding<float3> transformSectionGetPos;
		private static GetterValueBinding<float3> transformSectionGetRot;

		public static Entity selectedEntity;
		public static Entity selectedPrefab;


		protected override void OnCreate() {

			base.OnCreate();
			SelectedInfoUISystem selectedInfoUISystem = base.World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
			selectedInfoUISystem.eventSelectionChanged += OnSelectionChanged;

			AddBinding(transformSectionGetPos = new GetterValueBinding<float3>("elt", "transformsection_getpos", GetPosition));
			AddBinding(new TriggerBinding<float3>("elt", "transformsection_getpos", new Action<float3>(SetPosition)));

			AddBinding(transformSectionGetRot = new GetterValueBinding<float3>("elt", "transformsection_getrot", GetRotation));
			AddBinding(new TriggerBinding<float3>("elt", "transformsection_getrot", new Action<float3>(SetRotattion)));
		}

		private static void SetPosition(float3 position) {
			UpdateObjectTransform(position, null);
		}

		private static void SetRotattion(float3 rotation) {
			UpdateObjectTransform(null, rotation);
		}

		private static void UpdateObjectTransform( float3? position, float3? rotation) {
			if(!ELT.m_EntityManager.TryGetComponent(selectedEntity, out Game.Objects.Transform transform)) {
				Plugin.Logger.LogWarning("Can't get the transform object.");
				transformSectionGetPos.Update();
				transformSectionGetRot.Update();
				return;
			}

			float3 positionOffset = float3.zero;

			if(position is float3 positionFloat3) {
				positionOffset = positionFloat3 - transform.m_Position;
				transform.m_Position = positionFloat3;
			}
			if(rotation is float3 rotationFloat3) {
				transform.m_Rotation = Quaternion.Euler(rotationFloat3);
			}

			if (!ELT.m_EntityManager.TryGetComponent(selectedEntity, out PrefabRef prefabRef) || !ELT.m_EntityManager.TryGetComponent(prefabRef.m_Prefab, out ObjectGeometryData geometryData) || !ELT.m_EntityManager.TryGetComponent(selectedEntity, out CullingInfo cullingInfo))
			{
				Plugin.Logger.LogWarning("Failed to get the CullingInfo on the Entity");
				transformSectionGetPos.Update();
				transformSectionGetRot.Update();
				return;
			}

			Bounds3 bounds3 = new(transform.m_Position, transform.m_Position);
			bounds3 = ObjectUtils.CalculateBounds(transform.m_Position, transform.m_Rotation, geometryData);
			cullingInfo.m_Bounds = bounds3;

			if (ELT.m_EntityManager.TryGetBuffer(selectedEntity, false, out DynamicBuffer<Game.Buildings.InstalledUpgrade> installedUpgrades))
			{	
				foreach (Game.Buildings.InstalledUpgrade installedUpgrade in installedUpgrades) {
					if(!ELT.m_EntityManager.TryGetComponent(installedUpgrade, out Game.Objects.Transform transform1)) {
						continue;
					}
					if(!ELT.m_EntityManager.TryGetComponent(installedUpgrade, out PrefabRef prefabRef1) || !ELT.m_EntityManager.TryGetComponent(prefabRef1.m_Prefab, out ObjectGeometryData geometryData1) || !ELT.m_EntityManager.TryGetComponent(installedUpgrade, out CullingInfo cullingInfo1)) {
						continue;
					}
					// UnityEngine.Quaternion quaternion = transform.m_Rotation;
					// float3 newAngle = quaternion.eulerAngles;
					transform1.m_Position += positionOffset;
					// float3 distance = transform1.m_Position-transform.m_Position;
					// float lenght = (float)Math.Sqrt(Math.Pow(distance.x, 2)+Math.Pow(distance.z, 2));
					// transform1.m_Position -= distance; // * (float)Math.Cos(newAngle.y) //  * (float)Math.Sin(newAngle.y)
					// transform1.m_Position = transform.m_Position + new float3(lenght * (float)Math.Cos(newAngle.y), distance.y, lenght * (float)Math.Sin(newAngle.y));
					transform1.m_Rotation = transform.m_Rotation;
					cullingInfo1.m_Bounds = ObjectUtils.CalculateBounds(transform1.m_Position, transform1.m_Rotation, geometryData1);
					ELT.m_EntityManager.SetComponentData(installedUpgrade, transform1);
					ELT.m_EntityManager.SetComponentData(installedUpgrade, cullingInfo1);
					ELT.m_EntityManager.AddComponentData(installedUpgrade, new Game.Common.Updated());
				}
			}
			
			ELT.m_EntityManager.SetComponentData(selectedEntity, transform);
			ELT.m_EntityManager.SetComponentData(selectedEntity, cullingInfo);
			ELT.m_EntityManager.AddComponentData(selectedEntity, new Game.Common.Updated());
			if(!position.Equals(float3.zero)) transformSectionGetPos.Update();
			if(!rotation.Equals(float3.zero)) transformSectionGetRot.Update();
		}


		private static float3 GetPosition() {

			Entity selected = selectedEntity;
			Entity pref = selectedPrefab;
			ELT.m_EntityManager.TryGetComponent(selectedEntity, out Game.Objects.Transform transform);
			return transform.m_Position;
		}

		private static float3 GetRotation() {
			ELT.m_EntityManager.TryGetComponent(selectedEntity, out Game.Objects.Transform transform);
			UnityEngine.Quaternion quaternion = transform.m_Rotation;
			return quaternion.eulerAngles;
		}

		private static void OnSelectionChanged(Entity entity, Entity prefab, float3 SelectedPosition) {
			if( !Settings.settings.EnableTransformSection || entity == Entity.Null || ELT.m_EntityManager.HasComponent<Building>(entity) || ELT.m_EntityManager.HasComponent<LabelExtents>(entity)) return;

			selectedEntity = entity;
			selectedPrefab = prefab;

			transformSectionGetPos.Update();
			transformSectionGetRot.Update();

			ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("TransformSection.js"));
		}
	}
}