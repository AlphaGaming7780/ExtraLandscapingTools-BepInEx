using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colossal.IO.AssetDatabase;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using Unity.Mathematics;
using UnityEngine;
using static Colossal.IO.AssetDatabase.MaterialLibrary;

namespace ExtraLandscapingTools;

public class CustomDecals
{
	internal static List<string> FolderToLoadDecals = [];
	public static void CreateCustomDecals(StaticObjectPrefab DecalPrefab) {
		foreach(string folder in FolderToLoadDecals) {
			foreach(string decalsFolder in Directory.GetDirectories( folder )) {
				CreateCustomDecal(DecalPrefab, decalsFolder, new DirectoryInfo(decalsFolder).Name);
			}
		}
	}

	public static void CreateCustomDecal(StaticObjectPrefab DecalPrefab, string folderPath, string name) {

		RenderPrefab DecalRenderPrefab = (RenderPrefab)DecalPrefab.m_Meshes[0].m_Mesh;
		SpawnableObject DecalSpawnableObjectPrefab = DecalPrefab.GetComponent<SpawnableObject>();
		DecalProperties DecalPropertiesPrefab = DecalRenderPrefab.GetComponent<DecalProperties>();

		// Plugin.Logger.LogWarning(DecalPrefab);
		// foreach(ObjectMeshInfo objectMeshInfo99 in DecalPrefab.m_Meshes) {
		// 	Plugin.Logger.LogWarning(objectMeshInfo99);
		// 	if(objectMeshInfo99 == null) continue;
		// 	if(objectMeshInfo99.m_Mesh is RenderPrefab renderPrefab99) {
				// if(renderPrefab99.surfaceAssets == null) continue;
				// foreach(SurfaceAsset surfaceAsset99 in renderPrefab99.surfaceAssets.ToArray()) {
				// 	if(surfaceAsset99.textures == null) continue;
				// 	foreach(string s in surfaceAsset99.textures.Keys) {
				// 		if(surfaceAsset99.textures[s].rawData == null) continue;
				// 		Texture2D texture2D = new(surfaceAsset99.textures[s].width, surfaceAsset99.textures[s].height);
				// 		texture2D.LoadRawTextureData(surfaceAsset99.textures[s].rawData);
				// 		Plugin.Logger.LogMessage(s);
				// 		ELT.SaveTexture(texture2D, $"{GameManager_Awake.resources}\\Decals\\{DecalPrefab.name}\\{objectMeshInfo99}\\{surfaceAsset99}\\{s}.png");
				// 	}
				// }

		// 		foreach(Material material99 in renderPrefab99.ObtainMaterials(false)) {
		// 			Plugin.Logger.LogWarning(material99);
		// 			foreach(string s in material99.GetPropertyNames(MaterialPropertyType.Texture)) {
		// 				if(material99.HasTexture(s)) { 
		// 					Plugin.Logger.LogMessage($"Texture : {s} | {material99.GetTexture(s).name}");
		// 					Texture2D texture2D = (Texture2D)material99.GetTexture(s);
		// 					if(!texture2D.isReadable) texture2D = ELT.GetTextureFromNonReadable(texture2D);
		// 					ELT.SaveTexture(texture2D, $"{GameManager_Awake.resources}\\Decals\\{DecalPrefab.name}\\{objectMeshInfo99}\\{material99.name}\\{s}.png");
		// 				}
		// 			}
		// 		}
		// 	}
		// }

		// Plugin.Logger.LogWarning("Static Object Prefab in component");
		// foreach(StaticObjectPrefab staticObjectPrefab99 in DecalPrefab.GetComponent<SpawnableObject>().m_Placeholders.Cast<StaticObjectPrefab>()) {
		// 	Plugin.Logger.LogWarning(DecalPrefab);
		// 	foreach(ObjectMeshInfo objectMeshInfo99 in staticObjectPrefab99.m_Meshes) {
		// 		Plugin.Logger.LogWarning(objectMeshInfo99);
		// 		if(objectMeshInfo99 == null) continue;
		// 		if(objectMeshInfo99.m_Mesh is RenderPrefab renderPrefab99) {
					// if(renderPrefab99.surfaceAssets == null) continue;
					// foreach(SurfaceAsset surfaceAsset in renderPrefab.surfaceAssets.ToArray()) {
					// 	if(surfaceAsset.textures == null) continue;
					// 	foreach(string s in surfaceAsset.textures.Keys) {
					// 		if(surfaceAsset.textures[s].rawData == null) continue;
					// 		Texture2D texture2D = new(surfaceAsset.textures[s].width, surfaceAsset.textures[s].height);
					// 		texture2D.LoadRawTextureData(surfaceAsset.textures[s].rawData.ToArray());
					// 		Plugin.Logger.LogMessage(s);
					// 		ELT.SaveTexture(texture2D, $"{GameManager_Awake.resources}\\Decals\\{DecalPrefab.name}\\{staticObjectPrefab}\\{objectMeshInfo}\\{surfaceAsset}\\{s}.png");
					// 	}
					// }
		// 			foreach(Material material99 in renderPrefab99.ObtainMaterials(false)) {
		// 				Plugin.Logger.LogWarning(material99);
		// 				foreach(string s in material99.GetPropertyNames(MaterialPropertyType.Texture)) {
		// 					if(material99.HasTexture(s)) {
		// 						Plugin.Logger.LogMessage($"Texture : {s} | {material99.GetTexture(s).name}");
		// 						Texture2D texture2D = (Texture2D)material99.GetTexture(s);
		// 						if(!texture2D.isReadable) texture2D = ELT.GetTextureFromNonReadable(texture2D);
		// 						ELT.SaveTexture(texture2D, $"{GameManager_Awake.resources}\\Decals\\{DecalPrefab.name}\\{staticObjectPrefab99}\\{objectMeshInfo99}\\{material99.name}\\{s}.png");
		// 					}
		// 				}
		// 			}
		// 		}
		// 	}
		// }

		Mesh mesh = new()
		{
			vertices = [
			new Vector3(-0.5f, -0.5f, 0),
			new Vector3(0.5f, -0.5f, 0),
			new Vector3(-0.5f, 0.5f, 0),
			new Vector3(0.5f, 0.5f, 0)
		],
			uv = [
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(1, 1)
		],
			triangles = [
			0, 2, 1,
			2, 3, 1
		]
		};
		mesh.RecalculateNormals();

		Material material = DecalRenderPrefab.ObtainMaterial(0); //new(Shader.Find( "BH/Decals/DefaultDecalShader" ));
		foreach(string s in material.GetPropertyNames(MaterialPropertyType.Float)) {Plugin.Logger.LogMessage($"Float : {s} | {material.GetFloat(s)}");}
		foreach(string s in material.GetPropertyNames(MaterialPropertyType.Vector)) {Plugin.Logger.LogMessage($"Vector : {s} | {material.GetVector(s)}");}
		foreach(string s in material.GetPropertyNames(MaterialPropertyType.Texture)) {Plugin.Logger.LogMessage($"Texture : {s}");}

		byte[] fileData; 

		fileData = File.ReadAllBytes(folderPath+"\\_BaseColorMap.png");
		Texture2D texture2D_BaseColorMap = new(1, 1)
		{
			name = $"{name}_BaseColorMap"
		};

		if (!texture2D_BaseColorMap.LoadImage(fileData)) {UnityEngine.Debug.LogError($"[ELT] Failed to Load the BaseColorMap image for the {name} decal."); return;}

		if(!File.Exists(folderPath+"\\icon.png")) ELT.ResizeTexture(texture2D_BaseColorMap, 64, folderPath+"\\icon.png");
		material.SetTexture("_BaseColorMap", texture2D_BaseColorMap);

		Texture2D texture2D_NormalMap = new(1, 1);
		texture2D_NormalMap.name = $"{name}_NormalMap";
		if(File.Exists(folderPath+"\\_NormalMap.png")) {
			fileData = File.ReadAllBytes(folderPath+"\\_NormalMap.png");
			texture2D_NormalMap.LoadImage(fileData);
		} else {
			for(int x = 0; x < texture2D_NormalMap.width; x++) {
				for(int y = 0; y < texture2D_NormalMap.height; y++) {
					Color color = new(0,0,0,0);
					texture2D_NormalMap.SetPixel(x, y, color);
				}
			}

			texture2D_NormalMap.Apply();
		}
		material.SetTexture("_NormalMap", texture2D_NormalMap);

		Texture2D texture2D_MaskMap = new(1, 1);
		texture2D_MaskMap.name = $"{name}_MaskMap";
		if(File.Exists(folderPath+"\\_MaskMap.png")) {
			fileData = File.ReadAllBytes(folderPath+"\\_MaskMap.png");
			texture2D_MaskMap.LoadImage(fileData);
		} else {
			for(int x = 0; x < texture2D_MaskMap.width; x++) {
				for(int y = 0; y < texture2D_MaskMap.height; y++) {
					Color color = new(0,0,0,0);
					texture2D_MaskMap.SetPixel(x, y, color);
				}
			}

			texture2D_MaskMap.Apply();
		}
		material.SetTexture("_MaskMap", texture2D_MaskMap);

		foreach(string s in material.GetPropertyNames(MaterialPropertyType.Texture)) {
			if(material.GetTexture(s) && material.GetTexture(s) != null) continue;
			try {
				Texture2D texture2D = new(texture2D_BaseColorMap.width, texture2D_BaseColorMap.height)
				{
					name = $"{name}{s}"
				};

				for(int x = 0; x < texture2D.width; x++) {
					for(int y = 0; y < texture2D.height; y++) {
						Color color = new(0,0,0,0);
						texture2D.SetPixel(x, y, color);
					}
				}

				texture2D.Apply();

				material.SetTexture(s, texture2D);
				Plugin.Logger.LogMessage("FUCCKKK");
			} catch(Exception e) {Plugin.Logger.LogError(e);}
		}

        MaterialDescription materialDescription = new()
        {
            m_Material = material,
            m_Hash = CalculateHash(material),
			m_SupportsVT = false,
			m_Stacks = []
        };

        AssetDatabase.global.resources.materialLibrary.m_Materials.Add(materialDescription);

		Plugin.Logger.LogMessage("SurfaceAsset");
		SurfaceAsset surfaceAsset = new()
		{
			guid = Guid.NewGuid(), //DecalRenderPrefab.surfaceAssets.ToArray()[0].guid, //
			database = DecalRenderPrefab.surfaceAssets.ToArray()[0].database,
		};
		surfaceAsset.SetData(material);
		AssetDataPath assetDataPath = AssetDataPath.Create($"ELT/CustomDecals/{name}", "SurfaceAsset");
		surfaceAsset.database.AddAsset<SurfaceAsset>(assetDataPath, surfaceAsset.guid);
		surfaceAsset.Save(force: true);
		// Colossal.IO.AssetDatabase.TextureAssetExtensions.AddAsset

		Plugin.Logger.LogMessage("RenderPrefab");
		RenderPrefab renderPrefab = (RenderPrefab)ScriptableObject.CreateInstance("RenderPrefab");

		Mesh[] meshes = [mesh];
        // GeometryAsset geometryAsset = new()
        // {
        //     guid = Guid.NewGuid(),
        //     database = DecalRenderPrefab.geometryAsset.database
        // };

		// AssetDataPath assetDataPath = AssetDataPath.Create($"ELT/CustomDecals/{name}", "geometryAsset.cok");

        // geometryAsset.SetData(meshes);
		// DecalRenderPrefab.geometryAsset.SetData(meshes);
		// DecalRenderPrefab.geometryAsset.ReleaseMeshes();
		// DecalRenderPrefab.geometryAsset.database.AddAsset(assetDataPath, geometryAsset.guid);
		// geometryAsset.Save(true);
		renderPrefab.geometryAsset = DecalRenderPrefab.geometryAsset; //new AssetReference<GeometryAsset>(geometryAsset.guid);

		Plugin.Logger.LogMessage("RenderPrefab 2.5");
		if(renderPrefab.geometryAsset == null) Plugin.Logger.LogWarning("renderPrefab.geometryAsset is null");
		Plugin.Logger.LogMessage("RenderPrefab 3");
		renderPrefab.surfaceAssets = [new AssetReference<SurfaceAsset>(surfaceAsset.guid)];//DecalRenderPrefab.surfaceAssets; //
		renderPrefab.surfaceArea = DecalRenderPrefab.surfaceArea;
		renderPrefab.bounds = DecalRenderPrefab.bounds;
		renderPrefab.meshCount = 1;
		renderPrefab.vertexCount = DecalRenderPrefab.vertexCount;
		renderPrefab.indexCount = DecalRenderPrefab.indexCount;

		DecalProperties decalProperties = renderPrefab.AddComponent<DecalProperties>();
		decalProperties.m_TextureArea = DecalPropertiesPrefab.m_TextureArea;
		decalProperties.m_LayerMask = DecalPropertiesPrefab.m_LayerMask;
		decalProperties.m_RendererPriority = DecalPropertiesPrefab.m_RendererPriority;
		decalProperties.m_EnableInfoviewColor = DecalPropertiesPrefab.m_EnableInfoviewColor;

		Plugin.Logger.LogMessage("ObjectMeshInfo");
		ObjectMeshInfo objectMeshInfo = new()
		{
			m_Mesh = renderPrefab,
			m_Position = float3.zero,
			m_RequireState = Game.Objects.ObjectState.None
		};

		Plugin.Logger.LogMessage("StaticObjectPrefab");
		StaticObjectPrefab staticObjectPrefab = (StaticObjectPrefab)ScriptableObject.CreateInstance("StaticObjectPrefab");
		staticObjectPrefab.name = name;
		staticObjectPrefab.m_Meshes = [objectMeshInfo];

		StaticObjectPrefab placeholder = (StaticObjectPrefab)ScriptableObject.CreateInstance("StaticObjectPrefab");
		placeholder.name = $"{name}_Placeholders";
		placeholder.m_Meshes = [objectMeshInfo];
		placeholder.AddComponent<PlaceholderObject>();

		SpawnableObject spawnableObject = staticObjectPrefab.AddComponent<SpawnableObject>();
		spawnableObject.m_Placeholders = [placeholder];  //DecalSpawnableObjectPrefab.m_Placeholders;

		Plugin.Logger.LogMessage("UIObject");
		UIObject surfacePrefabUI = staticObjectPrefab.AddComponent<UIObject>();
		surfacePrefabUI.m_IsDebugObject = false;
		surfacePrefabUI.m_Icon = File.Exists(folderPath+"\\icon.png") ? $"{GameManager_InitializeThumbnails.COUIBaseLocation}/CustomDecals/{name}/icon.png" : ELT.GetIcon(staticObjectPrefab);
		surfacePrefabUI.m_Priority = -1;

		// SubObjectDefaultProbability subObjectDefaultProbability = staticObjectPrefab.AddComponent<SubObjectDefaultProbability>();

		// staticObjectPrefab.m_Meshes[0].m_Mesh is RenderPrefab renderPrefab

		// ELT.m_PrefabSystem.AddPrefab(placeholder);
		ELT.m_PrefabSystem.AddPrefab(staticObjectPrefab);

	}
}