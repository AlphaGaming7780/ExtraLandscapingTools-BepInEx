using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using Game.Rendering;
using Unity.Entities;
using UnityEngine;

namespace ExtraLandscapingTools
{
	public class ELT
	{

		public static PrefabSystem m_PrefabSystem;
		public static RenderingSystem m_RenderingSystem;
		public static EntityManager m_EntityManager;

		public enum ELT_ExtensionType : int
		{
			Other = 0,
			Surfaces = 1,
			Decals = 2,
			Assets = 3
		}

		// public static List<string> ELT_Extensions {private set; get;} = [];
		public static Dictionary<string, ELT_ExtensionType> ELT_Extensions {private set; get;} = [];
		// public static Dictionary<ELT_ExtensionType, int> NumberOfThisExtensionType {private set; get;} = [];

		internal static Stream GetEmbedded(string embeddedPath) {
			return Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraLandscapingTools.embedded."+embeddedPath);
		}

		internal static Texture2D ResizeTexture( Texture2D texture2D, int newSize, string savePath = null) {

			RenderTexture scaledRT = RenderTexture.GetTemporary( newSize, newSize );
			Graphics.Blit(texture2D, scaledRT);

			Texture2D outputTexture = new( newSize, newSize, texture2D.format, true);

			RenderTexture.active = scaledRT;
			outputTexture.ReadPixels( new Rect( 0, 0, newSize, newSize ), 0, 0 );

			outputTexture.Apply( );

			// Clean up
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary( scaledRT );

			if(savePath != null) {
				File.WriteAllBytes( savePath, outputTexture.EncodeToPNG( ) );
			}

			return outputTexture;

		}

		internal static string GetIcon(PrefabBase prefab) {


			// if(File.Exists($"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.GetType().Name}/{prefab.name}.svg")) return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.GetType().Name}/{prefab.name}.svg";

			if(prefab is SurfacePrefab) {
				return prefab.name switch
				{   
					"Agriculture Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Concrete Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Concrete Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Grass Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Grass Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Pavement Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Pavement Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Sand Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Sand Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Tiles Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Tiles Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Tiles Surface 03" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Forestry Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Landfill Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Oil Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					"Ore Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
					_ => "Media/Game/Icons/LotTool.svg",
				};
			} else if (prefab is UIAssetCategoryPrefab) {

				return prefab.name switch
				{   
					"Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetMenuPrefab/Custom Surfaces.svg",
					"Misc Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Misc Surfaces.svg",
					"Concrete Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Concrete Surfaces.svg",
					"Rock Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Rock Surfaces.svg",
					"Wood Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Wood Surfaces.svg",
					"Ground Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Ground Surfaces.svg",
					"Tiles Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Tiles Surfaces.svg",
					"Decals" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Decals.svg",
					_ => "Media/Game/Icons/LotTool.svg"
				};
			} else if(prefab is UIAssetMenuPrefab) {

				// if(File.Exists($"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetMenuPrefab/{prefab.name}.svg")) return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetMenuPrefab/{prefab.name}.svg";

				return prefab.name switch
				{   
					"Custom Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetMenuPrefab/Custom Surfaces.svg",
					_ => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg"
				};
			} else if(prefab.name.ToLower().Contains("decal") || prefab.name.ToLower().Contains("roadarrow")) {
				return prefab.name switch
				{   

					_ => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg"
				};
			}

			return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg";
		}

		public static void RegisterELTExtension( string extensionID , ELT_ExtensionType eLT_ExtensionType ) {
			if(ELT_Extensions.ContainsKey(extensionID)) return;
			ELT_Extensions.Add(extensionID, eLT_ExtensionType);
		}
	}
}