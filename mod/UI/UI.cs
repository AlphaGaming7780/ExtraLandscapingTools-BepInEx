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
	
	public class ELT_UI : UISystemBase
	{	
		private static readonly GameObject eLT_UI_Object = new();
		internal static ELT_UI_Mono eLT_UI_Mono;

		private static GetterValueBinding<bool> showMarker;
		internal static bool isMarkerVisible = false;

		protected override void OnCreate() {

			base.OnCreate();
			ELT.m_RenderingSystem = base.World.GetOrCreateSystemManaged<RenderingSystem>();
			ELT.m_EntityManager = EntityManager;

			eLT_UI_Mono = eLT_UI_Object.AddComponent<ELT_UI_Mono>();
			AddBinding(showMarker = new GetterValueBinding<bool>("elt", "showmarker", () => ELT.m_RenderingSystem.markersVisible));
			AddBinding(new TriggerBinding<bool, bool>("elt", "showmarker", new Action<bool, bool>(ShowMarker)));
		}

		public static void ShowMarker(bool b, bool forceUpdate = false) {
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