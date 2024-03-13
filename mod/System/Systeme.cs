// using Colossal.Serialization.Entities;
// using Game;
// using Game.Common;
// using Game.Prefabs;
// using Unity.Collections;
// using Unity.Entities;

// namespace ExtraLandscapingTools;

// class MainSysteme : GameSystemBase
// {
//     public EntityQuery UIAssetCategoryQuery { get; private set; }

//     protected override void OnCreate() {

// 			base.OnCreate();

//             ELT.m_EntityManager = EntityManager;
//             ELT.m_PrefabSystem = base.World.GetOrCreateSystemManaged<PrefabSystem>();

//             Plugin.Logger.LogMessage("CREATED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

// 			UIAssetCategoryQuery = GetEntityQuery(new EntityQueryDesc
// 			{
// 				All =
// 			   [
// 					ComponentType.ReadOnly<ObjectGeometryData>(),

// 			   ],
// 			});
//             RequireForUpdate(UIAssetCategoryQuery);
// 		}

//     protected override void OnGameLoadingComplete(Colossal.Serialization.Entities.Purpose purpose, GameMode mode)
//     {
        
//         foreach(Entity entity in UIAssetCategoryQuery.ToEntityArray(AllocatorManager.Temp)) {
//             if(entity == null) Plugin.Logger.LogMessage("Entity null");
//             if(ELT.m_PrefabSystem.TryGetPrefab(entity, out PrefabBase staticObjectPrefab)) {

//                 // if(staticObjectPrefab is not StaticObjectPrefab) continue;

//                 if(staticObjectPrefab.GetComponent<DecalProperties>() == null) continue;

//                 Plugin.Logger.LogMessage(staticObjectPrefab.name);

//                 ELT.m_EntityManager.GetComponentData<Deleted>(entity);

//                 ObjectGeometryData objectGeometryData = ELT.m_EntityManager.GetComponentData<ObjectGeometryData>(entity);
//                 objectGeometryData.m_Flags &= Game.Objects.GeometryFlags.Overridable;
//                 ELT.m_EntityManager.SetComponentData(entity, objectGeometryData);
//             }
//         }

//         base.OnGameLoadingComplete(purpose, mode);       
//     }

//     protected override void OnUpdate()
//     {
//     }
// }