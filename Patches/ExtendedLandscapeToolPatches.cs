using HarmonyLib;
using Game.UI.InGame;
using Game.Tools;
using Game.Prefabs;
using Unity.Entities;
using Game.Common;

namespace ExtraLandscapingTools.Patches
{

    [HarmonyPatch(typeof(SystemOrder), "Initialize")]
    public static class SystemOrderPatch {
        public static void Postfix(Game.UpdateSystem updateSystem) {
            updateSystem.UpdateAt<ELT_UI>(Game.SystemUpdatePhase.UIUpdate);
        }
    }

	[HarmonyPatch(typeof(ToolUISystem), "OnCreate")]

	public class ToolUISystem_OnCreate 
	{
		public static ToolUISystem toolUISystem;
		public static Traverse toolUISystemTraverse;
		internal static PrefabSystem m_PrefabSystem;
		internal static EntityQuery m_BrushQuery;
		internal static ToolSystem m_ToolSystem;

		internal static ToolUISystem.Brush[] brushs;

		static void Postfix( ToolUISystem __instance) {

			toolUISystem = __instance;
			toolUISystemTraverse = Traverse.Create(__instance);

			m_PrefabSystem = toolUISystemTraverse.Field("m_PrefabSystem").GetValue<PrefabSystem>();
			m_BrushQuery = toolUISystemTraverse.Field("m_BrushQuery").GetValue<EntityQuery>();
			m_ToolSystem = toolUISystemTraverse.Field("m_ToolSystem").GetValue<ToolSystem>();

			brushs = toolUISystemTraverse.Method("BindBrushTypes").GetValue<ToolUISystem.Brush[]>();
		}
	}

	[HarmonyPatch( typeof( ToolUISystem ), "OnToolChanged", typeof(ToolBaseSystem) )]
	class ToolUISystem_OnToolChanged
	{
		private static void Postfix( ToolBaseSystem tool ) {

			if(tool.toolID == "Terrain Tool") {
				ELT_UI.eLT_UI_Mono.ChangeUiNextFrame(ELT_UI.GetStringFromEmbbededJSFile("UI.js"));
			}
		}
	}

	[HarmonyPatch(typeof(TerrainToolSystem), "SetPrefab")]
	internal class TerrainToolSystem_SetPrefab
	{
		// Token: 0x06000007 RID: 7 RVA: 0x0000212E File Offset: 0x0000032E
		private static void Postfix(TerraformingPrefab value)
		{
			// UnityEngine.Debug.Log(value.GetType().ToString());
			if( value.GetType().ToString() == "Game.Prefabs.TerraformingPrefab") {
				// UnityEngine.Debug.Log(ToolUISystem_OnCreate.brushs[1].m_Entity);
				// ToolUISystem_OnCreate.toolUISystemTraverse.Method("SelectBrush", ToolUISystem_OnCreate.brushes[1].m_Entity);
				
				// BrushPrefab brushPrefab = ToolUISystem_OnCreate.toolUISystemTraverse.Field("m_PrefabSystem").GetValue<PrefabSystem>().GetPrefab<BrushPrefab>(ToolUISystem_OnCreate.brushs[1].m_Entity);
				
				// ToolUISystem_OnCreate.m_ToolSystem.activeTool.brushType = brushPrefab;
			}
			
			// UnityEngine.Debug.Log(value.m_BrushMaterial.name);
		}
	}

}
