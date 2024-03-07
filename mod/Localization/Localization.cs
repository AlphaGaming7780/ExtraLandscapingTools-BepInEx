using System;
using System.Collections.Generic;
using System.IO;
using Colossal.IO.AssetDatabase;
using Colossal.Json;

namespace ExtraLandscapingTools
{
	public class Localization
	{
		internal delegate Dictionary<string, Dictionary<string, string>> OnLoadLocalization();
		internal static OnLoadLocalization onLoadLocalization = LoadLocalization;
		internal static Dictionary<string, Dictionary<string, string>> localization;

		internal static void AddCustomLocal(LocaleAsset localeAsset) { //Dictionary<string, string>

			if(onLoadLocalization != null) foreach(Delegate @delegate in onLoadLocalization.GetInvocationList()) {

				object result = @delegate.DynamicInvoke();

				if(result is not Dictionary<string, Dictionary<string, string>> localization) return;

				string loc = localeAsset.localeId;

				if(!localization.ContainsKey(loc)) loc = "en-US";

				foreach(string key in localization[loc].Keys) {
					if(localeAsset.data.entries.ContainsKey(key)) localeAsset.data.entries[key] = localization[loc][key];
					else localeAsset.data.entries.Add(key, localization[loc][key]);

					if(!key.Contains(":")) continue;

					string[] splited = key.Split(":");
					if(int.TryParse(splited[1], out int n)) {
						n++;
						if(localeAsset.data.indexCounts.ContainsKey(splited[0])) {
							if(localeAsset.data.indexCounts[splited[0]] < n) localeAsset.data.indexCounts[splited[0]] = n;
						} 
						else localeAsset.data.indexCounts.Add(splited[0], n);
					}		
				}
			}
		}

		private static Dictionary<string, Dictionary<string, string>> LoadLocalization() {
			localization = Decoder.Decode(new StreamReader(ELT.GetEmbedded("Localization.Localization.jsonc")).ReadToEnd()).Make<LocalizationJS>().Localization;
			if(CustomSurfaces.CanCreateCustomSurfaces()) CustomSurfaces.LoadLocalization();
			if(CustomDecals.CanCreateCustomDecals()) CustomDecals.LoadLocalization();
			return localization;
		}

	}

	[Serializable]
	public class LocalizationJS
	{	
		public Dictionary<string, Dictionary<string, string>> Localization = [];

	}
}