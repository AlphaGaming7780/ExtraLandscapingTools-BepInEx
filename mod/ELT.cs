using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using UnityEngine;

namespace ExtraLandscapingTools
{
	public class ELT
	{
		public static List<string> ELT_Extensions {private set; get;} = [];

		internal static Stream GetEmbedded(string embeddedPath) {
			return Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraLandscapingTools.embedded."+embeddedPath);
		}

		internal static void CreateIcon(string imagePath, Texture2D texture2D, int newSize) {

			Plugin.Logger.LogMessage($"Creating Surfaces Icon for {new DirectoryInfo(imagePath).Name}.");

			RenderTexture scaledRT = RenderTexture.GetTemporary( newSize, newSize );
			Graphics.Blit(texture2D, scaledRT);

			Texture2D outputTexture = new( newSize, newSize, texture2D.format, false);

			RenderTexture.active = scaledRT;
			outputTexture.ReadPixels( new Rect( 0, 0, newSize, newSize ), 0, 0 );

			outputTexture.Apply( );

			// Clean up
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary( scaledRT );

			string filename = $"{imagePath}\\icon.png";

			File.WriteAllBytes( filename, outputTexture.EncodeToPNG( ) );
		}

		internal static string GetIcon(PrefabBase prefab) {

			
			if(prefab is SurfacePrefab) {
				return prefab.name switch
				{   
					"Agriculture Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Concrete Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Concrete Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Grass Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Grass Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Pavement Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Pavement Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Sand Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Sand Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Tiles Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Tiles Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Tiles Surface 03" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Forestry Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Landfill Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Oil Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					"Ore Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Surfaces/{prefab.name}.svg",
					// "Material 1" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
					// "Material 2" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
					_ => "Media/Game/Icons/LotTool.svg",
				};
			} else if (prefab is UIAssetCategoryPrefab) {
				return prefab.name switch
				{   
					"Surfaces" => "Media/Game/Icons/LotTool.svg",
					"Decals" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Decals.svg",
					_ => "Media/Game/Icons/LotTool.svg",
				};
			} else if(prefab.name.ToLower().Contains("decal")) {
				return prefab.name switch
				{   

					_ => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Decals/Decal_Placeholder.svg",
				};
			}

			return "Media/Game/Icons/LotTool.svg";


			// UnityEngine.Debug.Log(prefab.name);
			// return prefab.name switch
			// {   
			// 	"Agriculture Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Concrete Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Concrete Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Grass Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Grass Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Pavement Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Pavement Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Sand Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Sand Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Tiles Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Tiles Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Tiles Surface 03" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Forestry Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Landfill Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Oil Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Ore Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	"Decals" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	// "Material 1" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	// "Material 2" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.name}.svg",
			// 	_ => "Media/Game/Icons/LotTool.svg",
			// };
		}

		public static void RegisterELTExtension( string extensionID ) {
			ELT_Extensions.Add(extensionID);
		}

	}
}