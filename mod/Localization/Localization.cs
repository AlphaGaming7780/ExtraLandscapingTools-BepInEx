using System;
using System.Collections.Generic;
using System.IO;
using Colossal.IO.AssetDatabase;
using Colossal.Json;

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
				if(localeAsset.data.entries.ContainsKey(key)) {localeAsset.data.entries[key] = localization[loc][key]; Plugin.Logger.LogWarning($"The key {key} already existe in {localeAsset.localeId} localization.");}
				else localeAsset.data.entries.Add(key, localization[loc][key]);

				if(localeAsset.data.indexCounts.ContainsKey(key)) localeAsset.data.indexCounts[key] = localeAsset.data.indexCounts.Count;
				else localeAsset.data.indexCounts.Add(key, localeAsset.data.indexCounts.Count);
				
			}

		}

		private static void LoadLocalization() {
			localization = Decoder.Decode(new StreamReader(ELT.GetEmbedded("Localization.Localization.jsonc")).ReadToEnd()).Make<LocalizationJS>().Localization;
			if(CustomSurfaces.FolderToLoadSurface.Count > 0) CustomSurfaces.LoadLocalization();
		}

	}

	[Serializable]
	internal class LocalizationJS
	{	
		public Dictionary<string, Dictionary<string, string>> Localization = [];

	}
}