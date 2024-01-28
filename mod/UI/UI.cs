using System;
using System.Collections;
using System.IO;
using Colossal.UI.Binding;
using Game.Rendering;
using Game.SceneFlow;
using Game.UI;
using UnityEngine;

namespace ExtraLandscapingTools
{
	
	internal class ELT_UI : UISystemBase
	{	
        private static readonly GameObject eLT_UI_Object = new();
        internal static ELT_UI_Mono eLT_UI_Mono;

		private static GetterValueBinding<bool> showMarker;
		// CameraUpdateSystem m_CameraUpdateSystem;
		// private static RenderingSystem m_RenderingSystem;

		// static bool angleBrushWithCamera = false;

        protected override void OnCreate() {

			base.OnCreate();
			ELT.m_RenderingSystem = base.World.GetOrCreateSystemManaged<RenderingSystem>();
			ELT.m_EntityManager = EntityManager;

			
			// foreach(Shader shader in m_RenderingSystem.enabledShaders.Keys) {
			// 	Plugin.Logger.LogMessage(shader.name);
			// }

            eLT_UI_Mono = eLT_UI_Object.AddComponent<ELT_UI_Mono>();
			AddBinding(showMarker = new GetterValueBinding<bool>("elt", "showmarker", () => ELT.m_RenderingSystem.markersVisible));
			AddBinding(new TriggerBinding<bool>("elt", "showmarker", new Action<bool>(ShowMarker)));

        }

		// protected override void OnUpdate()
		// {
		// 	if(angleBrushWithCamera) CameraRotation.Update();
		// }

		// private static void EnableCamRot(bool statu) {
		// 	angleBrushWithCamera = statu;
		// }

		internal static void ShowMarker(bool b) {
			ELT.m_RenderingSystem.markersVisible = b;
			showMarker.Update();
		}

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