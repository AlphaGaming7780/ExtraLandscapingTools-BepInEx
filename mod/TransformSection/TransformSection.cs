using cohtml.Net;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.UI.InGame;
using UnityEngine;

namespace ExtraLandscapingTools;

class TransformSection : InfoSectionBase
{

	private Vector3 position;
	private Quaternion rotation;


	protected override string group 		
	{
		get
		{
			return "TransformSection";
		}
	}

    protected override bool displayForDestroyedObjects
	{
		get
		{
			return true;
		}
	}


	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("position");
		writer.ArrayBegin(3);
		writer.Write(position.x);
		writer.Write(position.y);
		writer.Write(position.z);
		writer.ArrayEnd();

		writer.PropertyName("rotation");
		writer.ArrayBegin(4);
		writer.Write(rotation.x);
		writer.Write(rotation.y);
		writer.Write(rotation.z);
		writer.Write(rotation.w);
		writer.ArrayEnd();
	}

	protected override void OnProcess()
	{
		base.EntityManager.TryGetComponent(base.selectedEntity, out Game.Objects.Transform transform);
		position = transform.m_Position;
		rotation = transform.m_Rotation;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = true;
	}

	protected override void Reset()
	{
        position = new(0, 0, 0);
		rotation = new(0,0,0,0);
	}
}

