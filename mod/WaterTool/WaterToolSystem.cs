using Game.Prefabs;
using Game.Tools;
using Unity.Jobs;

namespace ExtraLandscapingTools 
{
    public class WaterToolSystem : ToolBaseSystem
    {
        private WaterPrefab prefab;

        public override string toolID => "WaterTool";
        public override PrefabBase GetPrefab()
        {
            return prefab;
        }

        public override bool TrySetPrefab(PrefabBase prefab)
        {
            if (prefab is WaterPrefab waterPrefab)
            {
                this.prefab = waterPrefab;
                return true;
            }

            return false;
        }

        public override void InitializeRaycast()
        {
            base.InitializeRaycast();
            Plugin.Logger.LogMessage("Water Tool InitializeRaycast");
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Plugin.Logger.LogMessage("Water Tool OnUpdate");
            return base.OnUpdate(inputDeps);
        }
    }
}