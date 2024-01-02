using System.Collections;
using System.IO;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace ExtraLandscapingTools
{
	
	internal class ELT_UI : UISystemBase
	{	
        private static readonly GameObject eLT_UI_Object = new();
        internal static ELT_UI_Mono eLT_UI_Mono;

        protected override void OnCreate() {

			base.OnCreate();

            eLT_UI_Mono = eLT_UI_Object.AddComponent<ELT_UI_Mono>();
			
        }

		internal static string GetStringFromEmbbededJSFile(string path) {
			return new StreamReader(ExtraLandscapingTools.GetEmbedded("UI."+path)).ReadToEnd();
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