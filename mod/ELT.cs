using System.Reflection;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;

namespace ExtraLandscapingTools 
{
	public class ExtraLandscapingTools
	{
		internal static System.IO.Stream GetEmbedded(string embeddedPath) {
			return Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraLandscapingTools.embedded."+embeddedPath);
		}

		internal static string GetIcon(PrefabBase prefab) {
			// UnityEngine.Debug.Log(prefab.name);
			return prefab.name switch
			{   
				"Agriculture Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Concrete Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Concrete Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Grass Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Grass Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Pavement Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Pavement Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Sand Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Sand Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Tiles Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Tiles Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Tiles Surface 03" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Forestry Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Landfill Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Oil Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				"Ore Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				// "Material 1" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				// "Material 2" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/{prefab.name}.svg",
				_ => "Media/Game/Icons/LotTool.svg",
			};

		}

	}
}