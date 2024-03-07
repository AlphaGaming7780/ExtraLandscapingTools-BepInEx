using System;
using System.IO;
using System.Reflection;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Game.UI.InGame;
using Unity.Entities;
using UnityEngine;

namespace ExtraLandscapingTools
{
	public class ELT
	{

		public static PrefabSystem m_PrefabSystem;
		public static RenderingSystem m_RenderingSystem;
		public static EntityManager m_EntityManager;
		public static ToolSystem m_ToolSystem;
		public static ToolUISystem m_ToolUISystem;
		public static ToolbarUISystem m_ToolbarUISystem;
		public static SurfaceReplacerTool m_SurfaceReplacerTool;

		internal delegate string OnGetIcon(PrefabBase prefabBase);
		internal static OnGetIcon onGetIcon;

		internal static Stream GetEmbedded(string embeddedPath) {
			return Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraLandscapingTools.embedded."+embeddedPath);
		}

		internal static void ClearData() {
			if(Directory.Exists(GameManager_Awake.ELTGameDataPath)) {
				Directory.Delete(GameManager_Awake.ELTGameDataPath, true);
			}
			if(Directory.Exists(GameManager_Awake.ELTUserDataPath)) {
				Directory.Delete(GameManager_Awake.ELTUserDataPath, true);
			}			
		}

		internal static void ExtractZip(string pathToZip) {

		}

		public static long MapUlongToLong(ulong ulongValue)
		{
			return unchecked((long)ulongValue + long.MinValue);
		}

		public static Texture2D ResizeTexture( Texture2D texture, int newSize, string savePath = null) {

			if(texture is null) {
				// Plugin.Logger.LogWarning("The input texture2D is null @ ResizeTexture");
				return null;
			}

			// Plugin.Logger.LogMessage(savePath);

			// if(texture is not Texture2D texture2D) {
			// 	texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, true);
			// 	Graphics.CopyTexture(texture, texture2D);
			// }

			// int height = (int)((float)texture.height/texture.width * newSize);

			RenderTexture scaledRT = RenderTexture.GetTemporary( newSize, newSize );
			Graphics.Blit(texture, scaledRT);

			Texture2D outputTexture = new( newSize, newSize, texture.format, true);

			RenderTexture.active = scaledRT;
			outputTexture.ReadPixels( new Rect( 0, 0, newSize, newSize ), 0, 0 );

			outputTexture.Apply( );

			// Clean up
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary( scaledRT );

			if(savePath != null) {
				SaveTexture(outputTexture, savePath);
			}

			return outputTexture;

		}

		public static Texture2D GetTextureFromNonReadable(Texture2D texture2D) {
			RenderTexture scaledRT = RenderTexture.GetTemporary( texture2D.width, texture2D.height );
			Graphics.Blit(texture2D, scaledRT);

			Texture2D outputTexture = new( texture2D.width, texture2D.height, TextureFormat.RGBA32, true);

			RenderTexture.active = scaledRT;
			outputTexture.ReadPixels( new Rect( 0, 0, texture2D.width, texture2D.height ), 0, 0 );

			outputTexture.Apply( );

			// Clean up
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary( scaledRT );

			return outputTexture;
		}

		public static void SaveTexture(Texture2D texture2D, string path) {
			Directory.CreateDirectory(new FileInfo(path).DirectoryName);
			File.WriteAllBytes( path, texture2D.EncodeToPNG( ) );
		}

		public static string GetIcon(PrefabBase prefab) {

			if(prefab is null) return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg";

			if(onGetIcon is not null) foreach(Delegate @delegate in onGetIcon.GetInvocationList()) {

				object result = @delegate.DynamicInvoke(prefab);

				if(result is string s && s is not null) return s;
			}

			if(File.Exists($"{GameManager_Awake.resourcesIcons}/{prefab.GetType().Name}/{prefab.name}.svg")) return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/{prefab.GetType().Name}/{prefab.name}.svg";

			if(prefab is SurfacePrefab) {

				// return prefab.name switch
				// {   
				// 	"Agriculture Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Concrete Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Concrete Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Grass Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Grass Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Pavement Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Pavement Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Sand Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Sand Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Tiles Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Tiles Surface 02" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Tiles Surface 03" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Forestry Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Landfill Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Oil Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	"Ore Surface 01" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/SurfacePrefab/{prefab.name}.svg",
				// 	_ => "Media/Game/Icons/LotTool.svg",
				// };

				return "Media/Game/Icons/LotTool.svg";

			} else if (prefab is UIAssetCategoryPrefab) {
				// return prefab.name switch
				// {   
				// 	"Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetMenuPrefab/Custom Surfaces.svg",
				// 	"Misc Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Misc Surfaces.svg",
				// 	"Concrete Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Concrete Surfaces.svg",
				// 	"Rock Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Rock Surfaces.svg",
				// 	"Wood Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Wood Surfaces.svg",
				// 	"Ground Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Ground Surfaces.svg",
				// 	"Tiles Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Tiles Surfaces.svg",
				// 	"Decals" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetCategoryPrefab/Decals.svg",
				// 	_ => "Media/Game/Icons/LotTool.svg"
				// };
				return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg";
			} else if(prefab is UIAssetMenuPrefab) {

				// return prefab.name switch
				// {   
				// 	"Custom Surfaces" => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/UIAssetMenuPrefab/Custom Surfaces.svg",
				// 	_ => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg"
				// };
				return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg";
			} else if(prefab.name.ToLower().Contains("decal") || prefab.name.ToLower().Contains("roadarrow")) {



				// return prefab.name switch
				// {   

				// 	_ => $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg"
				// };
				try {
					if(prefab is StaticObjectPrefab staticObjectPrefab) {

						// SpawnableObject spawnableObject = staticObjectPrefab.GetComponent<SpawnableObject>();
						// if(spawnableObject is not null) {
						// 	foreach(ObjectPrefab objectPrefab in spawnableObject.m_Placeholders) {
						// 		if(objectPrefab is StaticObjectPrefab staticObjectPrefab1) {
						// 			foreach(ObjectMeshInfo objectMeshInfo in staticObjectPrefab1.m_Meshes) {
						// 				if(objectMeshInfo.m_Mesh is RenderPrefab renderPrefab) {
						// 					foreach(Material material in renderPrefab.ObtainMaterials()) {
						// 						foreach(string s in material.GetPropertyNames(MaterialPropertyType.Texture)) {ResizeTexture(material.GetTexture(s), 64, $"{GameManager_Awake.resourcesIcons}\\Decals\\{prefab.name}\\{s}.png");} //\\{staticObjectPrefab1.name}\\{objectMeshInfo}
						// 					}
						// 				}
						// 			}
						// 		}
						// 	}
						// }

						// foreach(ObjectMeshInfo objectMeshInfo in staticObjectPrefab.m_Meshes) {
						// 	if(objectMeshInfo.m_Mesh is RenderPrefab renderPrefab) {
						// 		foreach(Material material in renderPrefab.ObtainMaterials()) {
						// 			foreach(string s in material.GetPropertyNames(MaterialPropertyType.Texture)) {ResizeTexture(material.GetTexture(s), 64, $"{GameManager_Awake.resourcesIcons}\\Decals\\{prefab.name}\\{s}.png");}
						// 		}
						// 	}
						// }

						// if(staticObjectPrefab.m_Meshes[0].m_Mesh is RenderPrefab) {


							

							// ResizeTexture(renderPrefab.ObtainMaterial(0).GetTexture("_BaseColorMap"), 64, $"{GameManager_Awake.resourcesIcons}\\Decals\\{prefab.name}.png");
							// return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Decals/{prefab.name}.png";
						// }
					}
				} catch(Exception e) {Plugin.Logger.LogError(e);}

				return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg";
			}

			return $"{GameManager_InitializeThumbnails.COUIBaseLocation}/resources/Icons/Misc/placeholder.svg";
		}


	}
}