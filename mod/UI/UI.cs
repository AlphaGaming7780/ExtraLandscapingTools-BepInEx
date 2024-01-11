using System.Collections;
using System.IO;
using Game.SceneFlow;
using Game.UI;
using UnityEngine;

namespace ExtraLandscapingTools
{
	
	internal class ELT_UI : UISystemBase
	{	
        private static readonly GameObject eLT_UI_Object = new();
        internal static ELT_UI_Mono eLT_UI_Mono;

		// GetterValueBinding<float3> CameraRotation;
		// CameraUpdateSystem m_CameraUpdateSystem;
		// RenderingSystem m_RenderingSystem;

		// static bool angleBrushWithCamera = false;

        protected override void OnCreate() {

			base.OnCreate();
			// m_RenderingSystem = base.World.GetOrCreateSystemManaged<RenderingSystem>();
			
			// foreach(Shader shader in m_RenderingSystem.enabledShaders.Keys) {
			// 	Plugin.Logger.LogMessage(shader.name);
			// }

            eLT_UI_Mono = eLT_UI_Object.AddComponent<ELT_UI_Mono>();
			// AddBinding(CameraRotation = new GetterValueBinding<float3>("extralandscapingtools", "camerarotation", () => m_CameraUpdateSystem.direction));
			// AddBinding(new TriggerBinding<bool>("extralandscapingtools", "enablecamrot", new Action<bool>(EnableCamRot)));

        }

		// protected override void OnUpdate()
		// {
		// 	if(angleBrushWithCamera) CameraRotation.Update();
		// }

		// private static void EnableCamRot(bool statu) {
		// 	angleBrushWithCamera = statu;
		// }

		internal static string GetStringFromEmbbededJSFile(string path) {
			return new StreamReader(ELT.GetEmbedded("UI."+path)).ReadToEnd();
		}

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