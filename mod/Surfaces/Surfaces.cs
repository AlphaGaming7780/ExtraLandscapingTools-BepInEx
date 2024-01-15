using System;
using System.Collections.Generic;
using System.IO;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using UnityEngine;

namespace ExtraLandscapingTools;

public class CustomSurfaces
{

	// public delegate void OnCustomSurfacesLoad(Material material);
	// public static event OnCustomSurfacesLoad CallOnCustomSurfaces;

	internal static Dictionary<PrefabBase, string> SurfacesDataBase = [];

	internal static List<string> FolderToLoadSurface = [];

	public static void AddCustomSurfacesFolder(string path) {
		// Plugin.Logger.LogMessage(path);
		if(!FolderToLoadSurface.Contains(path)) {
			FolderToLoadSurface.Add(path);
			GameManager_InitializeThumbnails.pathToIconToLoad.Add(new DirectoryInfo(path).Parent.FullName);
		}
	}

	// internal static void CallLoadCustomSurfaces(Material material) {
	// 	CallOnCustomSurfaces(material);
	// }

	// public static void LoadCustomSurfaces(Material material, string path, string coui) {
	// 	PrefabSystem_OnCreate.CreateCustomSurfaces(material, path, coui);
	// }

	internal static void CreateCustomSurface(PrefabSystem prefabSystem, string folderPath, Material material, string uIAssetCategoryPrefab) {

		if(!File.Exists(folderPath+"\\_BaseColorMap.png")) return;

		SurfacePrefab surfacePrefab = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
		surfacePrefab.name = new DirectoryInfo(folderPath).Name;//Path.GetFileNameWithoutExtension("test");
		// surfacePrefab.active = true;

		SurfacePrefab surfacePrefabPlaceHolder = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
		surfacePrefabPlaceHolder.name = surfacePrefab.name  + "_Placeholder";//Path.GetFileNameWithoutExtension("test");
		// surfacePrefabPlaceHolder.active = true;

		Material newMaterial = new(material);
		// newMaterial.SetVector("_BaseColor", new Vector4(1,1,1,1));
		// newMaterial.SetVector("_BaseColorMap_TexelSize", new Vector4(0,0,1024,1024));
		// newMaterial.SetFloat("_Metallic", 0.1f);
		// newMaterial.SetFloat("_Smoothness", 0.1f);

		byte[] fileData;

		try {
			fileData = File.ReadAllBytes(folderPath+"\\_BaseColorMap.png");
			// int imageSize = ExtraLandscapingTools.GetImageSize(filePath);
			Texture2D texture2D_BaseColorMap = new(1, 1);
			if(!texture2D_BaseColorMap.LoadImage(fileData)) {UnityEngine.Debug.LogError("Failed to Load Image"); return;}

			if(!File.Exists(folderPath+"\\icon.png")) ELT.CreateIcon(folderPath, texture2D_BaseColorMap, 64);

			// Plugin.Logger.LogMessage(imageSize);

			newMaterial.SetTexture("_BaseColorMap", texture2D_BaseColorMap);
		} catch (Exception e) {Plugin.Logger.LogWarning(e); return;}

		try {
			fileData = File.ReadAllBytes(folderPath+"\\_NormalMap.png");
			Texture2D texture2D_NormalMap = new(1, 1);
			if(texture2D_NormalMap.LoadImage(fileData)) newMaterial.SetTexture("_NormalMap", texture2D_NormalMap);
		} catch {}

		try {
			fileData = File.ReadAllBytes(folderPath+"\\_MaskMap.png");
			Texture2D texture2D_MaskMap = new(1, 1);
			if(texture2D_MaskMap.LoadImage(fileData)) {
				newMaterial.SetTexture("_MaskMap", texture2D_MaskMap);
				newMaterial.SetFloat("_Metallic", 0.75f);
				newMaterial.SetFloat("_Smoothness", 0.75f);
			}
		} catch {}

		RenderedArea renderedArea = surfacePrefabPlaceHolder.AddComponent<RenderedArea>();
		renderedArea.m_RendererPriority = -100;
		renderedArea.m_LodBias = 0;
		renderedArea.m_Roundness = 1;
		renderedArea.m_Material = newMaterial;

		// renderedArea.m_Material = new(materialPlaceHolder);
		// renderedArea.m_Material.SetTexture("_BaseColorMap", texture2D_BaseColorMap);
		// renderedArea.m_Material.SetTexture("_NormalMap", texture2D_NormalMap);
		// renderedArea.m_Material.SetTexture("_MaskMap", texture2D_MaskMap);
		// renderedArea.m_Material.SetVector("_BaseColor", new Vector4(1,1,1,1));
		// renderedArea.m_Material.SetVector("_BaseColorMap_TexelSize", new Vector4(0,0,1024,1024));
		// renderedArea.m_Material.SetFloat("_Metallic", 1);
		// renderedArea.m_Material.SetFloat("_Smoothness", 1);
		// renderedArea.m_Material.SetFloat("_DecalColorMask0", 15);
		// renderedArea.m_Material.SetFloat("_DecalColorMask1", 0);
		// renderedArea.m_Material.SetFloat("_DecalColorMask2", 0);
		// renderedArea.m_Material.SetFloat("_DecalColorMask3", 0);

		// renderedArea.active=true;

		PlaceholderArea placeholderArea = surfacePrefabPlaceHolder.AddComponent<PlaceholderArea>();
		// placeholderArea.active = true;
		// surfacePrefab.prefab = surfacePrefab;

		SpawnableArea spawnableArea = surfacePrefab.AddComponent<SpawnableArea>();
		spawnableArea.m_Placeholders = new AreaPrefab[1];
		spawnableArea.m_Placeholders[0] = surfacePrefabPlaceHolder;
		// spawnableArea.m_Placeholders[1] = surfacePrefabPlaceHolder;
		// spawnableArea.active = true;

		RenderedArea renderedArea1 = surfacePrefab.AddComponent<RenderedArea>();
		renderedArea1.m_RendererPriority = -100;
		renderedArea1.m_LodBias = 0;
		renderedArea1.m_Roundness = 1;
		renderedArea1.m_Material = newMaterial;

		if(File.Exists(folderPath+"\\icon.png")) {
			fileData = File.ReadAllBytes(folderPath+"\\icon.png");
			Texture2D texture2D_Icon = new(1, 1);
			if(texture2D_Icon.LoadImage(fileData)) {
				if(texture2D_Icon.width > 64 || texture2D_Icon.height > 64) {
					ELT.CreateIcon(folderPath, texture2D_Icon, 64);
				}
			}

		}

		UIObject surfacePrefabUI = surfacePrefab.AddComponent<UIObject>();
		surfacePrefabUI.active = true;
		surfacePrefabUI.m_IsDebugObject = false;
		surfacePrefabUI.m_Icon = File.Exists(folderPath+"\\icon.png") ? $"{GameManager_InitializeThumbnails.COUIBaseLocation}/CustomSurfaces/{uIAssetCategoryPrefab}/{new DirectoryInfo(folderPath).Name}/icon.png" : ELT.GetIcon(surfacePrefab);
		surfacePrefabUI.m_Priority = -1;
		surfacePrefabUI.m_Group = SetupUIGroupe(prefabSystem, surfacePrefab, uIAssetCategoryPrefab);

		// foreach(PrefabBase prefabBase in spawnableArea.m_Placeholders) Plugin.Logger.LogMessage(prefabBase.name);

		prefabSystem.AddPrefab(surfacePrefab);
		prefabSystem.AddPrefab(surfacePrefabPlaceHolder);

	}

	internal static UIAssetCategoryPrefab SetupUIGroupe(PrefabSystem prefabSystem, PrefabBase prefab, string cat = "Misc") {

		if(!SurfacesDataBase.ContainsKey(prefab)) SurfacesDataBase.Add(prefab, cat);

		if(FolderToLoadSurface.Count > 0) {
			// string iconPath = SurfacesDataBase[prefab] == "Misc" ? null : $"{GameManager_InitializeThumbnails.COUIBaseLocation}/CustomSurfaces/{SurfacesDataBase[prefab]}/icon.svg";
			return Prefab.GetOrCreateNewToolCategory(prefabSystem, prefab, "Custom Surfaces", SurfacesDataBase[prefab]+" Surfaces");
		} else {
			return Prefab.GetOrCreateNewToolCategory(prefabSystem, prefab, "Landscaping", "Surfaces", "Terraforming");
		}

	}

}