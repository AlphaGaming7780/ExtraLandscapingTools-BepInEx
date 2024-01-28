using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colossal.Json;
using CustomSurfaces;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using UnityEngine;

namespace ExtraLandscapingTools;

public class CustomSurfaces
{
	internal static Dictionary<PrefabBase, string> SurfacesDataBase = [];

	internal static List<string> FolderToLoadSurface = [];

	public static void AddCustomSurfacesFolder(string path) {
		// Plugin.Logger.LogMessage(path);
		if(!FolderToLoadSurface.Contains(path)) {
			FolderToLoadSurface.Add(path);
			GameManager_InitializeThumbnails.pathToIconToLoad.Add(new DirectoryInfo(path).Parent.FullName);
		}
	}

	internal static void LoadLocalization() {

		Dictionary<string, string> csLocalisation = [];
		
		csLocalisation.Add($"SubServices.NAME[Misc Surfaces]", "Misc");
		csLocalisation.Add($"Assets.SUB_SERVICE_DESCRIPTION[Misc Surfaces]", "Misc");

		foreach(string folder in FolderToLoadSurface) {
			foreach(string surfacesCat in Directory.GetDirectories( folder )) {
				csLocalisation.Add($"SubServices.NAME[{new DirectoryInfo(surfacesCat).Name} Surfaces]", new DirectoryInfo(surfacesCat).Name);
				csLocalisation.Add($"Assets.SUB_SERVICE_DESCRIPTION[{new DirectoryInfo(surfacesCat).Name} Surfaces]", new DirectoryInfo(surfacesCat).Name);
				foreach(string filePath in Directory.GetDirectories( surfacesCat )) 
				{	
					csLocalisation.Add($"Assets.NAME[{new DirectoryInfo(filePath).Name}]", new DirectoryInfo(filePath).Name);
				}
			}
		}

		foreach(string key in Localization.localization.Keys) {
			csLocalisation.ToList().ForEach(x => Localization.localization[key].Add(x.Key, x.Value));
		}
	}

	internal static void CreateCustomSurfaces(Material material) {
		foreach(string folder in FolderToLoadSurface) {
			foreach(string surfacesCat in Directory.GetDirectories( folder )) {
				foreach(string filePath in Directory.GetDirectories( surfacesCat )) 
				{	
					CreateCustomSurface(ELT.m_PrefabSystem, filePath, material, new DirectoryInfo(surfacesCat).Name);
				}
			}
		}
	}

	private static void CreateCustomSurface(PrefabSystem prefabSystem, string folderPath, Material material, string uIAssetCategoryPrefab) {

		if(!File.Exists(folderPath+"\\_BaseColorMap.png")) return;

		SurfacePrefab surfacePrefab = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
		surfacePrefab.name = new DirectoryInfo(folderPath).Name;

		SurfacePrefab surfacePrefabPlaceHolder = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
		surfacePrefabPlaceHolder.name = surfacePrefab.name  + "_Placeholder";

		Material newMaterial = new(material);
		newMaterial.SetVector("_BaseColor", new Vector4(1,1,1,1));
		newMaterial.SetFloat("_Metallic", 0.5f);
		newMaterial.SetFloat("_Smoothness", 0.5f);
		newMaterial.SetFloat("colossal_UVScale", 0.5f);

		if(File.Exists(folderPath+"\\surface.json")) {
			JSONMaterail jSONMaterail = Decoder.Decode(File.ReadAllText(folderPath+"\\surface.json")).Make<JSONMaterail>();
			// foreach(string key in jSONMaterail.Int.Keys) {newMaterial.SetInt(key, jSONMaterail.Int[key]);}
			foreach(string key in jSONMaterail.Float.Keys) {newMaterial.SetFloat(key, jSONMaterail.Float[key]);}
			foreach(string key in jSONMaterail.Vector.Keys) {newMaterial.SetVector(key, jSONMaterail.Vector[key]);}
		} 

		byte[] fileData; 

		try {
			fileData = File.ReadAllBytes(folderPath+"\\_BaseColorMap.png");
			Texture2D texture2D_BaseColorMap = new(1, 1);
			if(!texture2D_BaseColorMap.LoadImage(fileData)) {UnityEngine.Debug.LogError("Failed to Load Image"); return;}

			if(!File.Exists(folderPath+"\\icon.png")) ELT.ResizeTexture(texture2D_BaseColorMap, 64, folderPath+"\\icon.png");
			// if(texture2D_BaseColorMap.width > 512 || texture2D_BaseColorMap.height > 512) texture2D_BaseColorMap = ELT.ResizeTexture(texture2D_BaseColorMap, 512, folderPath+"\\_BaseColorMap.png");

			newMaterial.SetTexture("_BaseColorMap", texture2D_BaseColorMap);
		} catch (Exception e) {Plugin.Logger.LogWarning(e); return;}

		try {
			fileData = File.ReadAllBytes(folderPath+"\\_NormalMap.png");
			Texture2D texture2D_NormalMap = new(1, 1);
			if(texture2D_NormalMap.LoadImage(fileData)) {
				// if(texture2D_NormalMap.width > 512 || texture2D_NormalMap.height > 512) texture2D_NormalMap = ELT.ResizeTexture(texture2D_NormalMap, 512, folderPath+"\\_NormalMap.png");
				newMaterial.SetTexture("_NormalMap", texture2D_NormalMap);
			}
		} catch {}

		try {
			fileData = File.ReadAllBytes(folderPath+"\\_MaskMap.png");
			Texture2D texture2D_MaskMap = new(1, 1);
			if(texture2D_MaskMap.LoadImage(fileData)) {
				// if(texture2D_MaskMap.width > 512 || texture2D_MaskMap.height > 512) texture2D_MaskMap = ELT.ResizeTexture(texture2D_MaskMap, 512, folderPath+"\\_MaskMap.png");
				newMaterial.SetTexture("_MaskMap", texture2D_MaskMap);
				newMaterial.SetFloat("_Metallic", 0.75f);
				newMaterial.SetFloat("_Smoothness", 0.75f);
			}
		} catch {}

		RenderedArea renderedArea = surfacePrefabPlaceHolder.AddComponent<RenderedArea>();
		renderedArea.m_RendererPriority = GetRendererPriorityByCat(uIAssetCategoryPrefab);
		renderedArea.m_LodBias = 0;
		renderedArea.m_Roundness = 1;
		renderedArea.m_Material = newMaterial;

		PlaceholderArea placeholderArea = surfacePrefabPlaceHolder.AddComponent<PlaceholderArea>();

		SpawnableArea spawnableArea = surfacePrefab.AddComponent<SpawnableArea>();
		spawnableArea.m_Placeholders = new AreaPrefab[1];
		spawnableArea.m_Placeholders[0] = surfacePrefabPlaceHolder;

		RenderedArea renderedArea1 = surfacePrefab.AddComponent<RenderedArea>();
		renderedArea1.m_RendererPriority = GetRendererPriorityByCat(uIAssetCategoryPrefab);
		renderedArea1.m_LodBias = 0;
		renderedArea1.m_Roundness = 1;
		renderedArea1.m_Material = newMaterial;

		if(File.Exists(folderPath+"\\icon.png")) {
			fileData = File.ReadAllBytes(folderPath+"\\icon.png");
			Texture2D texture2D_Icon = new(1, 1);
			if(texture2D_Icon.LoadImage(fileData)) {
				if(texture2D_Icon.width > 64 || texture2D_Icon.height > 64) {
					ELT.ResizeTexture(texture2D_Icon, 64, folderPath+"\\icon.png");
				}
			}

		}

		UIObject surfacePrefabUI = surfacePrefab.AddComponent<UIObject>();
		surfacePrefabUI.active = true;
		surfacePrefabUI.m_IsDebugObject = false;
		surfacePrefabUI.m_Icon = File.Exists(folderPath+"\\icon.png") ? $"{GameManager_InitializeThumbnails.COUIBaseLocation}/CustomSurfaces/{uIAssetCategoryPrefab}/{new DirectoryInfo(folderPath).Name}/icon.png" : ELT.GetIcon(surfacePrefab);
		surfacePrefabUI.m_Priority = -1;
		surfacePrefabUI.m_Group = SetupUIGroupe(prefabSystem, surfacePrefab, uIAssetCategoryPrefab);

		prefabSystem.AddPrefab(surfacePrefab);
		prefabSystem.AddPrefab(surfacePrefabPlaceHolder);

	}

	internal static UIAssetCategoryPrefab SetupUIGroupe(PrefabSystem prefabSystem, PrefabBase prefab, string cat = "Misc") {

		if(!SurfacesDataBase.ContainsKey(prefab)) SurfacesDataBase.Add(prefab, cat);

		if(FolderToLoadSurface.Count > 0) {
			return Prefab.GetOrCreateNewToolCategory(prefabSystem, prefab, "Custom Surfaces", SurfacesDataBase[prefab]+" Surfaces");
		} else {
			return Prefab.GetOrCreateNewToolCategory(prefabSystem, prefab, "Landscaping", "Surfaces", "Terraforming");
		}
	}

	internal static int GetRendererPriorityByCat(string cat) {
		return cat switch
		{   
			"Ground" => -100,
			"Grass" => -99,
			"Sand" => -98,
			"Concrete" => -97,
			"Wood" => -97,
			"Pavement" => -96,
			"Tiles" => -95,
			_ => -100
		};
	}
}