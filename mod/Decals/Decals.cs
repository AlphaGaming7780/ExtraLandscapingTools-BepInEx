using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colossal.AssetPipeline;
using Colossal.AssetPipeline.Importers;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using Game.Rendering;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

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

	public static void CreateCustomDecal(StaticObjectPrefab DecalPrefab, string folderPath, string decalName) {

		RenderPrefab DecalRenderPrefab = (RenderPrefab)DecalPrefab.m_Meshes[0].m_Mesh;
		SpawnableObject DecalSpawnableObjectPrefab = DecalPrefab.GetComponent<SpawnableObject>();
		DecalProperties DecalPropertiesPrefab = DecalRenderPrefab.GetComponent<DecalProperties>();



		// Material material = new(DecalRenderPrefab.ObtainMaterial(0))  //new(Shader.Find( "BH/Decals/DefaultDecalShader" )); 
		// {
		// 	name = $"{decalName}_Material",
		// };

		// foreach (string s in material.GetPropertyNames(MaterialPropertyType.Float)) {Plugin.Logger.LogMessage($"Float : {s} | {material.GetFloat(s)}");}
		// foreach(string s in material.GetPropertyNames(MaterialPropertyType.Vector)) {Plugin.Logger.LogMessage($"Vector : {s} | {material.GetVector(s)}");}
		// foreach(string s in material.GetPropertyNames(MaterialPropertyType.Texture)) {Plugin.Logger.LogMessage($"Texture : {s}");}

		byte[] fileData;

		fileData = File.ReadAllBytes(folderPath+"\\_BaseColorMap.png");
		Texture2D texture2D_BaseColorMap_Temp = new(1, 1);
		if (!texture2D_BaseColorMap_Temp.LoadImage(fileData)) {UnityEngine.Debug.LogError($"[ELT] Failed to Load the BaseColorMap image for the {decalName} decal."); return;}
		
		// texture2D_BaseColorMap_Temp.GetRawTextureData();

		Texture2D texture2D_BaseColorMap = new(texture2D_BaseColorMap_Temp.width, texture2D_BaseColorMap_Temp.height, texture2D_BaseColorMap_Temp.isDataSRGB ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_SNorm, texture2D_BaseColorMap_Temp.mipmapCount, TextureCreationFlags.MipChain)
		{
			name = $"{decalName}_BaseColorMap"
		};

		for(int i = 0; i < texture2D_BaseColorMap_Temp.mipmapCount; i++) {
			texture2D_BaseColorMap.SetPixels(texture2D_BaseColorMap_Temp.GetPixels(i), i);
		}
		texture2D_BaseColorMap_Temp.Apply();

		
		// Graphics.ConvertTexture(texture2D_BaseColorMap_Temp, texture2D_BaseColorMap);

		// texture2D_BaseColorMap.LoadRawTextureData(texture2D_BaseColorMap_Temp.GetRawTextureData());

		if(!File.Exists(folderPath+"\\icon.png")) ELT.ResizeTexture(texture2D_BaseColorMap_Temp, 64, folderPath+"\\icon.png");
		// material.SetTexture("_BaseColorMap", texture2D_BaseColorMap);

		// Texture2D texture2D_NormalMap = new(1, 1);
		// texture2D_NormalMap.name = $"{name}_NormalMap";
		// if(File.Exists(folderPath+"\\_NormalMap.png")) {
		// 	fileData = File.ReadAllBytes(folderPath+"\\_NormalMap.png");
		// 	texture2D_NormalMap.LoadImage(fileData);
		// } else {
		// 	for(int x = 0; x < texture2D_NormalMap.width; x++) {
		// 		for(int y = 0; y < texture2D_NormalMap.height; y++) {
		// 			Color color = new(0,0,0,0);
		// 			texture2D_NormalMap.SetPixel(x, y, color);
		// 		}
		// 	}

		// 	texture2D_NormalMap.Apply();
		// }
		// material.SetTexture("_NormalMap", texture2D_NormalMap);

		// Texture2D texture2D_MaskMap = new(1, 1);
		// texture2D_MaskMap.name = $"{name}_MaskMap";
		// if(File.Exists(folderPath+"\\_MaskMap.png")) {
		// 	fileData = File.ReadAllBytes(folderPath+"\\_MaskMap.png");
		// 	texture2D_MaskMap.LoadImage(fileData);
		// } else {
		// 	for(int x = 0; x < texture2D_MaskMap.width; x++) {
		// 		for(int y = 0; y < texture2D_MaskMap.height; y++) {
		// 			Color color = new(0,0,0,0);
		// 			texture2D_MaskMap.SetPixel(x, y, color);
		// 		}
		// 	}

		// 	texture2D_MaskMap.Apply();
		// }
		// material.SetTexture("_MaskMap", texture2D_MaskMap);

		// foreach(string s in material.GetPropertyNames(MaterialPropertyType.Texture)) {
		// 	if(material.GetTexture(s) && material.GetTexture(s) != null) continue;
		// 	try {
		// 		Texture2D texture2D = new(texture2D_BaseColorMap.width, texture2D_BaseColorMap.height)
		// 		{
		// 			name = $"{name}{s}"
		// 		};

		// 		for(int x = 0; x < texture2D.width; x++) {
		// 			for(int y = 0; y < texture2D.height; y++) {
		// 				Color color = new(0,0,0,0);
		// 				texture2D.SetPixel(x, y, color);
		// 			}
		// 		}

		// 		texture2D.Apply();

		// 		material.SetTexture(s, texture2D);
		// 	} catch(Exception e) {Plugin.Logger.LogError(e);}
		// }

		// MaterialDescription materialDescription = new()
		// {
		// 	m_Material = material,
		// 	m_Hash = CalculateHash(material) + material.name.GetHashCode(),
		// 	m_SupportsVT = false,
		// 	m_Stacks = [],
		// 	m_MipBiasOverride = (int)texture2D_BaseColorMap.mipMapBias
		// };

		// AssetDatabase.global.resources.materialLibrary.m_Materials.Add(materialDescription);

		TextureImporter.Texture texture = new($"{decalName}_BaseColorMap", folderPath+"\\"+"_BaseColorMap.png", texture2D_BaseColorMap);
		// TextureImporter.Texture texture = new($"{decalName}_BaseColorMap", folderPath+"\\"+"_BaseColorMap.png", new FileInfo(folderPath+"\\"+"_BaseColorMap.png").Length, texture2D_BaseColorMap.isDataSRGB);

		Surface decalSurface = new(decalName, "DefaultDecal");
		if(File.Exists(folderPath+"\\decal.json")) {
			JSONSurfacesMaterail jSONMaterail = Decoder.Decode(File.ReadAllText(folderPath+"\\decal.json")).Make<JSONSurfacesMaterail>();
			foreach(string key in jSONMaterail.Float.Keys) {decalSurface.AddProperty(key, jSONMaterail.Float[key]);}
			foreach(string key in jSONMaterail.Vector.Keys) {decalSurface.AddProperty(key, jSONMaterail.Vector[key]);}
		}

		decalSurface.AddProperty("_BaseColorMap", texture);


		Plugin.Logger.LogMessage("Texture format = " + texture.format);

		Plugin.Logger.LogMessage("SurfaceAsset");
		SurfaceAsset surfaceAsset = new()
		{
			guid = Guid.NewGuid(), //DecalRenderPrefab.surfaceAssets.ToArray()[0].guid, //
			database = DecalRenderPrefab.surfaceAssets.ToArray()[0].database,
		};

        // Surface
        // AssetDatabase.global.resources.materialLibrary.GetMaterialHash

		// Colossal.IO.AssetDatabase.AssetData.GetReadStream();
		//  Colossal.IO.AssetDatabase.TextureAsset.Load

		// surfaceAsset.database.GetReadStream()

        // surfaceAsset.SetMaterialHash(DecalRenderPrefab.surfaceAssets.ToArray()[0].materialTemplateHash);
        // surfaceAsset.SetData(material);
		AssetDataPath assetDataPath = AssetDataPath.Create($"ELT/CustomDecals/{decalName}", "SurfaceAsset");
		surfaceAsset.database.AddAsset<SurfaceAsset>(assetDataPath, surfaceAsset.guid);
        surfaceAsset.SetData(decalSurface);
		// surfaceAsset.SetData((Surface)surfaceAsset.database.GetAsset("10df77a53394eca40a4f73fd116763b3"));
		// surfaceAsset.SaveData();
		surfaceAsset.Save(force: false, saveTextures: true, vt: false);

		// TextureAsset textureAsset = surfaceAsset.database.AddAsset(base.subPath, kvp.Value, uniqueName: true);

		// Colossal.IO.AssetDatabase.TextureAssetExtensions.AddAsset

		// Plugin.Logger.LogMessage(SystemInfo.GetCompatibleFormat( ,FormatUsage.Sample));

		Vector4 MeshSize = decalSurface.GetVectorProperty("colossal_MeshSize");
		Vector4 TextureArea = decalSurface.GetVectorProperty("colossal_TextureArea");
		Mesh[] meshes = [ConstructMesh(MeshSize.x, MeshSize.y, MeshSize.z)];
		GeometryAsset geometryAsset = new()
		{
			guid = Guid.NewGuid(),
			database = DecalRenderPrefab.geometryAsset.database
		};

		AssetDataPath assetDataPath2 = AssetDataPath.Create($"ELT/CustomDecals/{decalName}", "geometryAsset");
		geometryAsset.database.AddAsset<GeometryAsset>(assetDataPath2, geometryAsset.guid);
		geometryAsset.SetData(meshes);
		// DecalRenderPrefab.geometryAsset.SetData(meshes);
		// DecalRenderPrefab.geometryAsset.ReleaseMeshes();
		// DecalRenderPrefab.geometryAsset.database.AddAsset(assetDataPath, geometryAsset.guid);
		geometryAsset.Save(true);

		Plugin.Logger.LogMessage("RenderPrefab");
		RenderPrefab renderPrefab = (RenderPrefab)ScriptableObject.CreateInstance("RenderPrefab");
		renderPrefab.geometryAsset = new AssetReference<GeometryAsset>(geometryAsset.guid);//DecalRenderPrefab.geometryAsset;
		renderPrefab.surfaceAssets = [surfaceAsset];//DecalRenderPrefab.surfaceAssets; //
		// renderPrefab.surfaceArea = DecalRenderPrefab.surfaceArea;
		renderPrefab.bounds = new(new(-MeshSize.x * 0.5f, -MeshSize.y * 0.5f, -MeshSize.z * 0.5f), new(MeshSize.x * 0.5f, MeshSize.y * 0.5f, MeshSize.z * 0.5f));
		renderPrefab.meshCount = 1;
		renderPrefab.vertexCount = geometryAsset.GetVertexCount(0); //DecalRenderPrefab.vertexCount;
		renderPrefab.indexCount = 1; //DecalRenderPrefab.indexCount;
		renderPrefab.manualVTRequired = false;

		DecalProperties decalProperties = renderPrefab.AddComponent<DecalProperties>();
		// decalProperties.m_TextureArea = new(new(-TextureArea.x * 0.5f, -TextureArea.y * 0.5f), new(TextureArea.x * 0.5f, TextureArea.y * 0.5f));
		decalProperties.m_TextureArea = new(new(TextureArea.x, TextureArea.y), new(TextureArea.z, TextureArea.w));
		decalProperties.m_LayerMask = (DecalLayers)decalSurface.GetFloatProperty("colossal_DecalLayerMask");
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
		staticObjectPrefab.name = decalName;
		staticObjectPrefab.m_Meshes = [objectMeshInfo];

		StaticObjectPrefab placeholder = (StaticObjectPrefab)ScriptableObject.CreateInstance("StaticObjectPrefab");
		placeholder.name = $"{decalName}_Placeholders";
		placeholder.m_Meshes = [objectMeshInfo];
		placeholder.AddComponent<PlaceholderObject>();

		SpawnableObject spawnableObject = staticObjectPrefab.AddComponent<SpawnableObject>();
		spawnableObject.m_Placeholders = [placeholder];  //DecalSpawnableObjectPrefab.m_Placeholders;

		Plugin.Logger.LogMessage("UIObject");
		UIObject surfacePrefabUI = staticObjectPrefab.AddComponent<UIObject>();
		surfacePrefabUI.m_IsDebugObject = false;
		surfacePrefabUI.m_Icon = File.Exists(folderPath+"\\icon.png") ? $"{GameManager_InitializeThumbnails.COUIBaseLocation}/CustomDecals/{decalName}/icon.png" : ELT.GetIcon(staticObjectPrefab);
		surfacePrefabUI.m_Priority = -1;

		// SubObjectDefaultProbability subObjectDefaultProbability = staticObjectPrefab.AddComponent<SubObjectDefaultProbability>();

		// staticObjectPrefab.m_Meshes[0].m_Mesh is RenderPrefab renderPrefab

		// ELT.m_PrefabSystem.AddPrefab(placeholder);
		ELT.m_PrefabSystem.AddPrefab(staticObjectPrefab);

	}

	private static Mesh ConstructMesh( float length, float height, float width) {
		Mesh mesh = new();

		//3) Define the co-ordinates of each Corner of the cube 
		Vector3[] c =
		[
			new Vector3(-length * .5f, -height * .5f, width * .5f),
			new Vector3(length * .5f, -height * .5f, width * .5f),
			new Vector3(length * .5f, -height * .5f, -width * .5f),
			new Vector3(-length * .5f, -height * .5f, -width * .5f),
			new Vector3(-length * .5f, height * .5f, width * .5f),
			new Vector3(length * .5f, height * .5f, width * .5f),
			new Vector3(length * .5f, height * .5f, -width * .5f),
			new Vector3(-length * .5f, height * .5f, -width * .5f),
		];


		//4) Define the vertices that the cube is composed of:
		//I have used 16 vertices (4 vertices per side). 
		//This is because I want the vertices of each side to have separate normals.
		//(so the object renders light/shade correctly) 
		Vector3[] vertices =
		[
			c[0], c[1], c[2], c[3], // Bottom
			c[7], c[4], c[0], c[3], // Left
			c[4], c[5], c[1], c[0], // Front
			c[6], c[7], c[3], c[2], // Back
			c[5], c[6], c[2], c[1], // Right
			c[7], c[6], c[5], c[4]  // Top
		];


		//5) Define each vertex's Normal
		Vector3 up = Vector3.up;
		Vector3 down = Vector3.down;
		Vector3 forward = Vector3.forward;
		Vector3 back = Vector3.back;
		Vector3 left = Vector3.left;
		Vector3 right = Vector3.right;


		Vector3[] normals =
		[
			down, down, down, down,             // Bottom
			left, left, left, left,             // Left
			forward, forward, forward, forward,	// Front
			back, back, back, back,             // Back
			right, right, right, right,         // Right
			up, up, up, up	                    // Top
		];


		//6) Define each vertex's UV co-ordinates
		Vector2 uv00 = new(0f, 0f);
		Vector2 uv10 = new(1f, 0f);
		Vector2 uv01 = new(0f, 1f);
		Vector2 uv11 = new(1f, 1f);

		Vector2[] uvs =
		[
			uv11, uv01, uv00, uv10, // Bottom
			uv11, uv01, uv00, uv10, // Left
			uv11, uv01, uv00, uv10, // Front
			uv11, uv01, uv00, uv10, // Back	        
			uv11, uv01, uv00, uv10, // Right 
			uv11, uv01, uv00, uv10  // Top
		];


		//7) Define the Polygons (triangles) that make up the our Mesh (cube)
		//IMPORTANT: Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
		//This means that a polygon's vertices must be defined in 
		//a clockwise order (relative to the camera) in order to be rendered/visible.
		int[] triangles =
		[
			3, 1, 0,        3, 2, 1,        // Bottom	
			7, 5, 4,        7, 6, 5,        // Left
			11, 9, 8,       11, 10, 9,      // Front
			15, 13, 12,     15, 14, 13,     // Back
			19, 17, 16,     19, 18, 17,	    // Right
			23, 21, 20,     23, 22, 21,	    // Top
		];


		//8) Build the Mesh
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.Optimize();
		mesh.RecalculateNormals();

		return mesh;
	}
}