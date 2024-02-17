using System.Collections.Generic;
using Game;
using Game.Areas;
using Game.Common;
using Game.Input;
using Game.Prefabs;
using Game.Tools;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.InputSystem.EnhancedTouch;

namespace ExtraLandscapingTools;

public class SurfaceReplacerTool : ToolBaseSystem
{
	public override string toolID => "surfacereplacertool";

	private ProxyAction m_ApplyAction;
	private ProxyAction m_SecondaryApplyAction;
	private AreaPrefab prefab;

	public override void GetUIModes(List<ToolMode> modes)
	{
		modes.Add(new ToolMode("Replace", 0));
	}

	public override PrefabBase GetPrefab()
	{	

		// if(prefab is null && m_ToolSystem.activePrefab is AreaPrefab areaPrefab) {
		// 	prefab = areaPrefab;
		// }

		return prefab;
	}

	public override bool TrySetPrefab(PrefabBase prefab)
	{
		if(prefab is AreaPrefab surfacePrefab) {
			this.prefab = surfacePrefab;
			return true;
		}
		return false;
	}

	public void RequestDisable()
	{
		m_ToolSystem.activeTool = m_DefaultToolSystem; 
	}
	protected override void OnCreate()
	{
		// Unless you are overriding the default tool set your tool Enabled to false.
		Enabled = false;

		// Input Action for left click/press. May work differently for game pads.
		m_ApplyAction = InputManager.instance.FindAction("Tool", "Apply");

		// Input Action for right click/press. May work differently for game pads.
		m_SecondaryApplyAction = InputManager.instance.FindAction("Tool", "Secondary Apply");

		// How to setup a hot key. We don’t have standards for them yet. It’s the Wild West out there. 
		// hotKey.AddCompositeBinding("ButtonWithOneModifier").With("Modifier", "<Keyboard>/ctrl").With("Button", "<Keyboard>/t");
		// hotKey.performed += this.OnKeyPressed;
		// hotKey.Enable();

	base.OnCreate();
	}
	// sets up actions or whatever else you want to happen when your tool becomes active.
	protected override void OnStartRunning()
	{
		m_ApplyAction.shouldBeEnabled = true;
		m_SecondaryApplyAction.shouldBeEnabled = true;
	}

	// cleans up actions or whatever else you want to happen when your tool becomes inactive.
	protected override void OnStopRunning()
	{
		m_ApplyAction.shouldBeEnabled = false;
		m_SecondaryApplyAction.shouldBeEnabled = false;
	}

	public override void InitializeRaycast()
	{
		// base.InitializeRaycast();
		m_ToolRaycastSystem.typeMask = TypeMask.Areas | TypeMask.RouteSegments;
		m_ToolRaycastSystem.areaTypeMask = AreaTypeMask.Surfaces;
		m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubElements;
		m_ToolRaycastSystem.collisionMask = CollisionMask.OnGround | CollisionMask.Overground;
	}
	protected override JobHandle OnUpdate(JobHandle inputDeps) {
		bool raycastFlag = GetRaycastResult(out Entity e, out RaycastHit hit);
		if(e == Entity.Null) return inputDeps;

		PrefabRef prefabRef = ELT.m_EntityManager.GetComponentData<PrefabRef>(e);

		ELT.m_EntityManager.AddComponentData(e, new Temp());
		ELT.m_EntityManager.AddComponentData(prefabRef.m_Prefab, new Temp());
		ELT.m_EntityManager.AddComponentData(e, new BatchesUpdated());
		ELT.m_EntityManager.AddComponentData(prefabRef.m_Prefab, new BatchesUpdated());

		Plugin.Logger.LogMessage(e);
		Plugin.Logger.LogMessage(hit.m_HitPosition);

		if (m_ApplyAction.WasPressedThisFrame() && raycastFlag)
		{
			Entity newEntity = ELT.m_PrefabSystem.GetEntity(prefab);
			prefabRef.m_Prefab = newEntity;
			ELT.m_EntityManager.SetComponentData(e, prefabRef);
			ELT.m_EntityManager.AddComponentData(e, new Updated());
		}

		return inputDeps;
	}

}