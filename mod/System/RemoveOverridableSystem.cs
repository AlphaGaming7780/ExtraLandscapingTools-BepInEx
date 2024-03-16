namespace RemoveOverridable.Systems
{
    using Colossal.Entities;
    using Colossal.Logging;
    using Colossal.PSI.Common;
    using ExtraLandscapingTools;
    using Game;
    using Game.Objects;
    using Game.Prefabs;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// A system that prevents objects from being overriden when placed on each other.
    /// </summary>
    public partial class RemoveOverridableSystem : GameSystemBase
    {
        private PrefabSystem m_PrefabSystem;
        private EntityQuery m_ObjectGeometryQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnarchyPlopSystem"/> class.
        /// </summary>
        public RemoveOverridableSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            Plugin.Logger.LogInfo($"{nameof(RemoveOverridableSystem)} Created.");
            m_PrefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
           
            m_ObjectGeometryQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<ObjectGeometryData>(),
                },
            });

            RequireForUpdate(m_ObjectGeometryQuery);
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnGameLoadingComplete(Colossal.Serialization.Entities.Purpose purpose, GameMode mode)
        {
            NativeArray<Entity> entitiesWithComponent = m_ObjectGeometryQuery.ToEntityArray(Allocator.Temp);
            Plugin.Logger.LogInfo($"{nameof(RemoveOverridable)}.{nameof(OnGameLoadingComplete)} ");
            // Cycle through all entities with Prevent Override component and look for any that shouldn't have been added. Remove component if it is not Overridable Static Object.
            foreach (Entity entity in entitiesWithComponent)
            {
                PrefabBase prefabBase = null;
                if (m_PrefabSystem.TryGetPrefab(entity, out prefabBase) )
                {
                    if (prefabBase is StaticObjectPrefab && EntityManager.TryGetComponent(entity, out ObjectGeometryData objectGeometryData) && (prefabBase.name.ToLower().Contains("decal") || prefabBase.name.ToLower().Contains("roadarrow") || prefabBase.name.ToLower().Contains("lanemarkings") || prefabBase.GetComponent<CustomDecal>() != null))
                    {
                        objectGeometryData.m_Flags &= ~GeometryFlags.Overridable;
                        objectGeometryData.m_Flags |= GeometryFlags.IgnoreBottomCollision;
                        objectGeometryData.m_Flags |= GeometryFlags.IgnoreSecondaryCollision;
                        EntityManager.SetComponentData(entity, objectGeometryData);
                        Plugin.Logger.LogInfo($"{nameof(RemoveOverridable)}.{nameof(OnGameLoadingComplete)} Removed Overridable from {prefabBase.name}");
                    } else
                    {
                        Plugin.Logger.LogInfo($"{nameof(RemoveOverridable)}.{nameof(OnGameLoadingComplete)} Did not remove Overridable from {prefabBase.name}. PrefabBase is StaticObjectPrefab: {prefabBase is StaticObjectPrefab} HasComponent objectGeometryData: {EntityManager.HasComponent<ObjectGeometryData>(entity)} prefabBase.GetComponent<DecalProperties>() != null : {prefabBase.GetComponent<DecalProperties>() != null}");
                    }
                } else
                {
                    Plugin.Logger.LogInfo($"{nameof(RemoveOverridable)}.{nameof(OnGameLoadingComplete)} Couldn't find prefabBase for entity: {entity.Index}:{entity.Version}");
                }

            }

            entitiesWithComponent.Dispose();

            base.OnGameLoadingComplete(purpose, mode);
        }

        protected override void OnUpdate()
        {
            Enabled = false;
        }

    }
}
