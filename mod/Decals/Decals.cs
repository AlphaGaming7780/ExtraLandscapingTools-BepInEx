using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Colossal.AssetPipeline;
using Colossal.AssetPipeline.Importers;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using Game.Rendering;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ExtraLandscapingTools;

public class CustomDecals
{
	internal static List<string> FolderToLoadDecals = [];

	internal static void SearchForCustomDecalsFolder(string ModsFolderPath) {
		foreach(DirectoryInfo directory in new DirectoryInfo(ModsFolderPath).GetDirectories()) {
			if(File.Exists($"{directory.FullName}\\CustomDecals.zip")) {
				if(Directory.Exists($"{directory.FullName}\\CustomDecals")) Directory.Delete($"{directory.FullName}\\CustomDecals", true);
				ZipFile.ExtractToDirectory($"{directory.FullName}\\CustomDecals", directory.FullName);
				File.Delete($"{directory.FullName}\\CustomDecals.zip");
			}
			if(Directory.Exists($"{directory.FullName}\\CustomDecals") && !FolderToLoadDecals.Contains($"{directory.FullName}\\CustomDecals")) FolderToLoadDecals.Add($"{directory.FullName}\\CustomDecals");
		}
	}

	public static void CreateCustomDecals(StaticObjectPrefab DecalPrefab) {
		foreach(string folder in FolderToLoadDecals) {
			foreach(string decalsFolder in Directory.GetDirectories( folder )) {
				CreateCustomDecal(DecalPrefab, decalsFolder, new DirectoryInfo(decalsFolder).Name);
			}
		}
	}

	public static void CreateCustomDecal(StaticObjectPrefab DecalPrefab, string folderPath, string decalName) {

		RenderPrefab DecalRenderPrefab = (RenderPrefab)DecalPrefab.m_Meshes[0].m_Mesh;
		// SpawnableObject DecalSpawnableObjectPrefab = DecalPrefab.GetComponent<SpawnableObject>();
		DecalProperties DecalPropertiesPrefab = DecalRenderPrefab.GetComponent<DecalProperties>();

		Surface decalSurface = new(decalName, "DefaultDecal");
		if(File.Exists(folderPath+"\\decal.json")) {
			JSONSurfacesMaterail jSONMaterail = Decoder.Decode(File.ReadAllText(folderPath+"\\decal.json")).Make<JSONSurfacesMaterail>();
			foreach(string key in jSONMaterail.Float.Keys) {decalSurface.AddProperty(key, jSONMaterail.Float[key]);}
			foreach(string key in jSONMaterail.Vector.Keys) {decalSurface.AddProperty(key, jSONMaterail.Vector[key]);}
		}

		byte[] fileData;

		fileData = File.ReadAllBytes(folderPath+"\\_BaseColorMap.png");
		Texture2D texture2D_BaseColorMap_Temp = new(1, 1);
		if (!texture2D_BaseColorMap_Temp.LoadImage(fileData)) {UnityEngine.Debug.LogError($"[ELT] Failed to Load the BaseColorMap image for the {decalName} decal."); return;}

		Texture2D texture2D_BaseColorMap = new(texture2D_BaseColorMap_Temp.width, texture2D_BaseColorMap_Temp.height, texture2D_BaseColorMap_Temp.isDataSRGB ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_SNorm, texture2D_BaseColorMap_Temp.mipmapCount, TextureCreationFlags.MipChain)
		{
			name = $"{decalName}_BaseColorMap"
		};

		for(int i = 0; i < texture2D_BaseColorMap_Temp.mipmapCount; i++) {
			texture2D_BaseColorMap.SetPixels(texture2D_BaseColorMap_Temp.GetPixels(i), i);
		}
		texture2D_BaseColorMap.Apply();
		if(!File.Exists(folderPath+"\\icon.png")) ELT.ResizeTexture(texture2D_BaseColorMap_Temp, 64, folderPath+"\\icon.png");
		TextureImporter.Texture textureImporterBaseColorMap = new($"{decalName}_BaseColorMap", folderPath+"\\"+"_BaseColorMap.png", texture2D_BaseColorMap);
		decalSurface.AddProperty("_BaseColorMap", textureImporterBaseColorMap);

		
		Texture2D texture2D_NormalMap_temp = new(1, 1);
		if(File.Exists(folderPath+"\\_NormalMap.png")) {
			fileData = File.ReadAllBytes(folderPath+"\\_NormalMap.png");
			if(texture2D_NormalMap_temp.LoadImage(fileData)) {
				Texture2D texture2D_NormalMap = new(texture2D_NormalMap_temp.width, texture2D_NormalMap_temp.height, texture2D_NormalMap_temp.isDataSRGB ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_SNorm, texture2D_NormalMap_temp.mipmapCount, TextureCreationFlags.MipChain)
				{
					name = $"{decalName}_NormalMap"
				};

				for(int i = 0; i < texture2D_NormalMap_temp.mipmapCount; i++) {
					texture2D_NormalMap.SetPixels(texture2D_NormalMap_temp.GetPixels(i), i);
				}
				texture2D_NormalMap.Apply();
				if(!File.Exists(folderPath+"\\icon.png")) ELT.ResizeTexture(texture2D_NormalMap_temp, 64, folderPath+"\\icon.png");
				TextureImporter.Texture textureImporterNormalMap = new($"{decalName}_NormalMap", folderPath+"\\"+"_NormalMap.png", texture2D_NormalMap);
				decalSurface.AddProperty("_NormalMap", textureImporterNormalMap);

			};
		}

		Texture2D texture2D_MaskMap_temp = new(1, 1);
		if(File.Exists(folderPath+"\\_MaskMap.png")) {
			fileData = File.ReadAllBytes(folderPath+"\\_MaskMap.png");
			if(texture2D_MaskMap_temp.LoadImage(fileData)) {
				Texture2D texture2D = new(texture2D_MaskMap_temp.width, texture2D_MaskMap_temp.height, texture2D_MaskMap_temp.isDataSRGB ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_SNorm, texture2D_MaskMap_temp.mipmapCount, TextureCreationFlags.MipChain)
				{
					name = $"{decalName}_MaskMap"
				};

				for(int i = 0; i < texture2D_MaskMap_temp.mipmapCount; i++) {
					texture2D.SetPixels(texture2D_MaskMap_temp.GetPixels(i), i);
				}
				texture2D.Apply();
				if(!File.Exists(folderPath+"\\icon.png")) ELT.ResizeTexture(texture2D_MaskMap_temp, 64, folderPath+"\\icon.png");
				TextureImporter.Texture textureImporterNormalMap = new($"{decalName}_MaskMap", folderPath+"\\"+"_MaskMap.png", texture2D);
				decalSurface.AddProperty("_MaskMap", textureImporterNormalMap);

			};
		}

		AssetDataPath assetDataPath = AssetDataPath.Create($"ELT/CustomDecals/{decalName}", "SurfaceAsset");
		SurfaceAsset surfaceAsset = new()
		{
			guid = Guid.NewGuid(), //DecalRenderPrefab.surfaceAssets.ToArray()[0].guid, //
			database = DecalRenderPrefab.surfaceAssets.ToArray()[0].database,
		};
		surfaceAsset.database.AddAsset<SurfaceAsset>(assetDataPath, surfaceAsset.guid);
        surfaceAsset.SetData(decalSurface);
		surfaceAsset.Save(force: false, saveTextures: true, vt: false);

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
		geometryAsset.Save(true);

		Plugin.Logger.LogMessage("RenderPrefab");
		RenderPrefab renderPrefab = (RenderPrefab)ScriptableObject.CreateInstance("RenderPrefab");
		renderPrefab.name = $"{decalName}_RenderPrefab";
		renderPrefab.geometryAsset = new AssetReference<GeometryAsset>(geometryAsset.guid);
		renderPrefab.surfaceAssets = [surfaceAsset];
		renderPrefab.bounds = new(new(-MeshSize.x * 0.5f, -MeshSize.y * 0.5f, -MeshSize.z * 0.5f), new(MeshSize.x * 0.5f, MeshSize.y * 0.5f, MeshSize.z * 0.5f));
		renderPrefab.meshCount = 1;
		renderPrefab.vertexCount = geometryAsset.GetVertexCount(0);
		renderPrefab.indexCount = 1;
		renderPrefab.manualVTRequired = false;

		DecalProperties decalProperties = renderPrefab.AddComponent<DecalProperties>();
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
		spawnableObject.m_Placeholders = [placeholder];

		Plugin.Logger.LogMessage("UIObject");
		UIObject surfacePrefabUI = staticObjectPrefab.AddComponent<UIObject>();
		surfacePrefabUI.m_IsDebugObject = false;
		surfacePrefabUI.m_Icon = File.Exists(folderPath+"\\icon.png") ? $"{GameManager_InitializeThumbnails.COUIBaseLocation}/CustomDecals/{decalName}/icon.png" : ELT.GetIcon(staticObjectPrefab);
		surfacePrefabUI.m_Priority = -1;

		staticObjectPrefab.AddComponent<CustomDecal>();

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

internal class CustomDecal: ComponentBase
{
	public override void GetArchetypeComponents(HashSet<ComponentType> components) {}
	public override void GetPrefabComponents(HashSet<ComponentType> components) {}
}