using System;
using System.IO;
using Colossal.Json;
using ExtraLandscapingTools.Patches;

namespace ExtraLandscapingTools
{	
	public class Settings
	{
		internal static ELTSettings settings;

		internal static T LoadSettings<T>(string id, T ExtensionSettings) {
			if(Directory.Exists($"{GameManager_Awake.PathToMods}\\Settings")) {
				if(File.Exists($"{GameManager_Awake.PathToMods}\\Settings\\{id}.json")) {
					ExtensionSettings = Decoder.Decode(File.ReadAllText($"{GameManager_Awake.PathToMods}\\Settings\\{id}.json")).Make<T>();
				}
			}
			return ExtensionSettings;
		}

		internal static void SaveSettings<T>( string id, T ExtensionSettings) {
			if(!Directory.Exists($"{GameManager_Awake.PathToMods}\\Settings")) Directory.CreateDirectory($"{GameManager_Awake.PathToMods}\\Settings");
			File.WriteAllText($"{GameManager_Awake.PathToMods}\\Settings\\{id}.json", Encoder.Encode(ExtensionSettings, EncodeOptions.None));
		}
	}

	[Serializable]
	internal class ELTSettings
	{
		public bool LoadCustomSurfaces = true;
		public bool EnableTransformSection = true;
		public bool EnableSnowSurfaces = false;
	}

}