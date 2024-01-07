using System;
using System.Collections.Generic;
using System.IO;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using MonoMod.Utils;
using UnityEngine;

namespace ExtraLandscapingTools
{
	class Localization
	{

		internal static Dictionary<string, Dictionary<string, string>> localization;

		internal static void AddCustomLocal(LocaleAsset localeAsset) { //Dictionary<string, string>

			if(localization == null) {
				LoadLocalization();
			}

			string loc = localeAsset.localeId;

			if(!localization.ContainsKey(loc)) loc = "en-US";

			foreach(string key in localization[loc].Keys) {
				if(localeAsset.data.entries.ContainsKey(key)) localeAsset.data.entries[key] = localization[loc][key];
				else localeAsset.data.entries.Add(key, localization[loc][key]);

				if(localeAsset.data.indexCounts.ContainsKey(key)) localeAsset.data.indexCounts[key] = localeAsset.data.indexCounts.Count;
				else localeAsset.data.indexCounts.Add(key, localeAsset.data.indexCounts.Count);
				
			}

		}

		internal static void LoadLocalization() {
			localization = Decoder.Decode(new StreamReader(ExtraLandscapingTools.GetEmbedded("Localization.Localization.json")).ReadToEnd()).Make<LocalizationJS>().Localization;
		}

	}

	[Serializable]
	internal class LocalizationJS
	{	
		public Dictionary<string, Dictionary<string, string>> Localization = [];

	}
}