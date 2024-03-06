using System;
using Game.Prefabs;
using Game.Tools;
using HarmonyLib;

namespace ExtraLandscapingTools.Patches;

public class ToolsSnapMask
{
	[HarmonyPatch(typeof(ObjectToolSystem), nameof(ObjectToolSystem.GetAvailableSnapMask),
		new Type[] { typeof(PlaceableObjectData), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(Snap), typeof(Snap) },
		new ArgumentType[] {ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
	class ObjectToolSystem_GetAvailableSnapMask
	{
		private static bool Prefix(PlaceableObjectData prefabPlaceableData, bool editorMode, bool isBuilding, bool isAssetStamp, bool brushing, bool stamping, out Snap onMask, out Snap offMask) {
			onMask = Snap.Upright;
			offMask = Snap.None;
			if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.OwnerSide) != Game.Objects.PlacementFlags.None)
			{
				onMask |= Snap.OwnerSide;
			}
			else if ((prefabPlaceableData.m_Flags & (Game.Objects.PlacementFlags.RoadSide | Game.Objects.PlacementFlags.Shoreline | Game.Objects.PlacementFlags.Floating | Game.Objects.PlacementFlags.Hovering)) != Game.Objects.PlacementFlags.None)
			{
				if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.RoadSide) != Game.Objects.PlacementFlags.None)
				{
					onMask |= Snap.NetSide;
					offMask |= Snap.NetSide;
				}
				if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.Shoreline) != Game.Objects.PlacementFlags.None)
				{
					onMask |= Snap.Shoreline;
					offMask |= Snap.Shoreline;
				}
				if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.Hovering) != Game.Objects.PlacementFlags.None)
				{
					onMask |= Snap.ObjectSurface;
					offMask |= Snap.ObjectSurface;
				}
			}
			else if ((prefabPlaceableData.m_Flags & (Game.Objects.PlacementFlags.RoadNode | Game.Objects.PlacementFlags.RoadEdge)) != Game.Objects.PlacementFlags.None)
			{
				if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.RoadNode) != Game.Objects.PlacementFlags.None)
				{
					onMask |= Snap.NetNode;
				}
				if ((prefabPlaceableData.m_Flags & Game.Objects.PlacementFlags.RoadEdge) != Game.Objects.PlacementFlags.None)
				{
					onMask |= Snap.NetArea;
				}
			}
			else if (!isBuilding)
			{
				onMask |= Snap.ObjectSurface;
				offMask |= Snap.ObjectSurface;
				offMask |= Snap.Upright;
			}
			if ( editorMode && (!isAssetStamp || stamping))
			{
				onMask |= Snap.AutoParent;
				offMask |= Snap.AutoParent;
			}
			if (brushing)
			{
				onMask &= Snap.Upright;
				offMask &= Snap.Upright;
				onMask |= Snap.PrefabType;
				offMask |= Snap.PrefabType;
			}
			if (isBuilding || isAssetStamp)
			{
				onMask |= Snap.ContourLines;
				offMask |= Snap.ContourLines;
			}

			return false;
		}
	}

	[HarmonyPatch(typeof(NetToolSystem), nameof(NetToolSystem.GetAvailableSnapMask),
		new Type[] { typeof(NetGeometryData), typeof(PlaceableNetData), typeof(NetToolSystem.Mode), typeof(bool), typeof(bool), typeof(bool), typeof(Snap), typeof(Snap) },
		new ArgumentType[] {ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
	class NetToolSystem_GetAvailableSnapMask
	{
		private static bool Prefix( NetGeometryData prefabGeometryData, PlaceableNetData placeableNetData, NetToolSystem.Mode mode, bool editorMode, bool laneContainer, bool underground, out Snap onMask, out Snap offMask) {

			if (mode == NetToolSystem.Mode.Replace)
			{
				onMask = Snap.ExistingGeometry;
				offMask = onMask;
				if ((placeableNetData.m_PlacementFlags & Game.Net.PlacementFlags.UpgradeOnly) == Game.Net.PlacementFlags.None)
				{
					onMask |= Snap.ContourLines;
					offMask |= Snap.ContourLines;
				}
				if (laneContainer)
				{
					onMask &= ~Snap.ExistingGeometry;
					offMask &= ~Snap.ExistingGeometry;
					onMask |= Snap.NearbyGeometry;
					return false;
				}
				if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != (Game.Net.GeometryFlags)0)
				{
					offMask &= ~Snap.ExistingGeometry;
				}
				if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.SnapCellSize) != (Game.Net.GeometryFlags)0)
				{
					onMask |= Snap.CellLength;
					offMask |= Snap.CellLength;
					return false;
				}
			}
			else
			{
				onMask = (Snap.ExistingGeometry | Snap.CellLength | Snap.StraightDirection | Snap.ObjectSide | Snap.GuideLines | Snap.ZoneGrid | Snap.ContourLines);
				offMask = onMask;
				if (underground)
				{
					onMask &= ~(Snap.ObjectSide | Snap.ZoneGrid);
				}
				if (laneContainer)
				{
					onMask &= ~(Snap.CellLength | Snap.ObjectSide);
					offMask &= ~(Snap.CellLength | Snap.ObjectSide);
				}
				else if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.Marker) != (Game.Net.GeometryFlags)0)
				{
					onMask &= ~Snap.ObjectSide;
					offMask &= ~Snap.ObjectSide;
				}
				if (laneContainer)
				{
					onMask &= ~Snap.ExistingGeometry;
					offMask &= ~Snap.ExistingGeometry;
					onMask |= Snap.NearbyGeometry;
					offMask |= Snap.NearbyGeometry;
				}
				else if ((prefabGeometryData.m_Flags & Game.Net.GeometryFlags.StrictNodes) != (Game.Net.GeometryFlags)0)
				{
					offMask &= ~Snap.ExistingGeometry;
					onMask |= Snap.NearbyGeometry;
					offMask |= Snap.NearbyGeometry;
				}

				onMask |= Snap.ObjectSurface | Snap.LotGrid;
				offMask |= Snap.ObjectSurface | Snap.LotGrid;

				if (editorMode)
				{
					onMask |= Snap.AutoParent;
					offMask |= Snap.AutoParent;
				}
			}

			return false;
		}
	}

	[HarmonyPatch(typeof(AreaToolSystem), nameof(AreaToolSystem.GetAvailableSnapMask),
		new Type[] { typeof(AreaGeometryData), typeof(bool), typeof(Snap), typeof(Snap) },
		new ArgumentType[] {ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
	class AreaToolSystem_GetAvailableSnapMask
	{
		private static bool Prefix(AreaGeometryData prefabAreaData, bool editorMode, out Snap onMask, out Snap offMask) {

			onMask = (Snap.ExistingGeometry | Snap.StraightDirection);
			offMask = onMask;
			switch (prefabAreaData.m_Type)
			{
			case Game.Areas.AreaType.Lot:
				onMask |= Snap.NetSide | Snap.ObjectSide | Snap.LotGrid;
				offMask |= Snap.NetSide | Snap.ObjectSide | Snap.LotGrid;
				if (editorMode)
				{
					onMask |= Snap.AutoParent;
					offMask |= Snap.AutoParent;
					return false;
				}
				break;
			case Game.Areas.AreaType.District:
				onMask |= Snap.NetMiddle;
				offMask |= Snap.NetMiddle;
				return false;
			case Game.Areas.AreaType.MapTile:
				break;
			case Game.Areas.AreaType.Space:
				onMask |= Snap.NetSide | Snap.ObjectSide | Snap.ObjectSurface | Snap.LotGrid;
				offMask |= Snap.NetSide | Snap.ObjectSide | Snap.ObjectSurface | Snap.LotGrid;
				if (editorMode)
				{
					onMask |= Snap.AutoParent;
					offMask |= Snap.AutoParent;
					return false;
				}
				break;
			case Game.Areas.AreaType.Surface:
				onMask |= Snap.NetSide | Snap.ObjectSide | Snap.LotGrid;
				offMask |= Snap.NetSide | Snap.ObjectSide | Snap.LotGrid;
				if (editorMode)
				{
					onMask |= Snap.AutoParent;
					offMask |= Snap.AutoParent;
				}
				break;
			default:
				return false;
			}
			return false;
		}
	}


}