using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Colossal.Json;
using ExtraLandscapingTools.Patches;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine;

namespace ExtraLandscapingTools;

public class CustomSurfaces
{
	internal static Dictionary<PrefabBase, string> SurfacesDataBase = [];

	internal static List<string> FolderToLoadSurface = [];

	internal static void SearchForCustomSurfacesFolder(string ModsFolderPath) {
		foreach(DirectoryInfo directory in new DirectoryInfo(ModsFolderPath).GetDirectories()) {
			if(File.Exists($"{directory.FullName}\\CustomSurfaces.zip")) {
				if(Directory.Exists($"{directory.FullName}\\CustomSurfaces")) Directory.Delete($"{directory.FullName}\\CustomSurfaces", true);
				ZipFile.ExtractToDirectory($"{directory.FullName}\\CustomSurfaces.zip", directory.FullName);
				File.Delete($"{directory.FullName}\\CustomSurfaces.zip");
			}
			if(Directory.Exists($"{directory.FullName}\\CustomSurfaces")) AddCustomSurfacesFolder($"{directory.FullName}\\CustomSurfaces");
		}
	}

	// internal static void ClearSurfacesCache() {
	// 	if(Directory.Exists($"{GameManager_Awake.resourcesCache}/Surfaces")) {
	// 		Directory.Delete($"{GameManager_Awake.resourcesCache}/Surfaces", true);
	// 	}
	// }

	public static void AddCustomSurfacesFolder(string path) {
		if(FolderToLoadSurface.Contains(path)) return;
		FolderToLoadSurface.Add(path);
		GameManager_InitializeThumbnails.AddNewIconsFolder(new DirectoryInfo(path).Parent.FullName);
	}

	internal static void LoadLocalization() {

		Dictionary<string, string> csLocalisation = [];

		foreach(string folder in FolderToLoadSurface) {
			foreach(string surfacesCat in Directory.GetDirectories( folder )) {
				
				if(!csLocalisation.ContainsKey($"SubServices.NAME[{new DirectoryInfo(surfacesCat).Name} Surfaces]")) {
					csLocalisation.Add($"SubServices.NAME[{new DirectoryInfo(surfacesCat).Name} Surfaces]", $"{new DirectoryInfo(surfacesCat).Name} Surfaces");
				}

				if(!csLocalisation.ContainsKey($"Assets.SUB_SERVICE_DESCRIPTION[{new DirectoryInfo(surfacesCat).Name} Surfaces]")) {
					csLocalisation.Add($"Assets.SUB_SERVICE_DESCRIPTION[{new DirectoryInfo(surfacesCat).Name} Surfaces]", $"{new DirectoryInfo(surfacesCat).Name} Surfaces");
				}

				foreach(string filePath in Directory.GetDirectories( surfacesCat )) 
				{	
					string surfaceName = $"{new DirectoryInfo(folder).Parent.Name}_{new DirectoryInfo(surfacesCat).Name}_{new DirectoryInfo(filePath).Name}";

					if(!csLocalisation.ContainsKey($"Assets.NAME[{surfaceName}]")) csLocalisation.Add($"Assets.NAME[{surfaceName}]", new DirectoryInfo(filePath).Name);
					if(!csLocalisation.ContainsKey($"Assets.DESCRIPTION[{surfaceName}]")) csLocalisation.Add($"Assets.DESCRIPTION[{surfaceName}]", new DirectoryInfo(filePath).Name);
				}
			}
		}

		foreach(string key in Localization.localization.Keys) {

			foreach(string s in csLocalisation.Keys) {
				if(!Localization.localization[key].ContainsKey(s)) Localization.localization[key].Add(s, csLocalisation[s]);
			}
		}
	}

	internal static void CreateCustomSurfaces(Material material) {
		foreach(string folder in FolderToLoadSurface) {
			foreach(string surfacesCat in Directory.GetDirectories( folder )) {
				foreach(string filePath in Directory.GetDirectories( surfacesCat )) 
				{	
					CreateCustomSurface(filePath, material, new DirectoryInfo(surfacesCat).Name, new DirectoryInfo(folder).Parent.Name);
				}
			}
		}
	}

	private static void CreateCustomSurface(string folderPath, Material material, string CatName, string modName) {

		if(!File.Exists(folderPath+"\\"+"_BaseColorMap.png")) {Plugin.Logger.LogError($"No _BaseColorMap.png file for the {new DirectoryInfo(folderPath).Name} surface in {CatName} category in {modName}.");return;}

		string surfaceName = $"{modName}_{CatName}_{new DirectoryInfo(folderPath).Name}";

		Dictionary<string, object> SurfaceInformation = [];

		SurfacePrefab surfacePrefab = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
		surfacePrefab.name = surfaceName;
		surfacePrefab.m_Color = new(255f,255f,255f,0.05f);

		SurfacePrefab surfacePrefabPlaceHolder = (SurfacePrefab)ScriptableObject.CreateInstance("SurfacePrefab");
		surfacePrefabPlaceHolder.name = surfacePrefab.name  + "_Placeholder";

		Material newMaterial = new(material);
		newMaterial.SetVector("_BaseColor", new Vector4(1,1,1,1));
		newMaterial.SetFloat("_Metallic", 0.5f);
		newMaterial.SetFloat("_Smoothness", 0.5f);
		newMaterial.SetFloat("colossal_UVScale", 0.5f);
		newMaterial.SetFloat("_DrawOrder", GetRendererPriorityByCat(CatName));

		if(File.Exists(folderPath+"\\surface.json")) {
			JSONSurfacesMaterail jSONMaterail = Decoder.Decode(File.ReadAllText(folderPath+"\\surface.json")).Make<JSONSurfacesMaterail>();
			foreach(string key in jSONMaterail.Float.Keys) {
				if(material.HasProperty(key)) newMaterial.SetFloat(key, jSONMaterail.Float[key]);
				else { 
					SurfaceInformation.Add(key, jSONMaterail.Float[key]); 
				}
			}
			foreach(string key in jSONMaterail.Vector.Keys) {newMaterial.SetVector(key, jSONMaterail.Vector[key]);}

			if(jSONMaterail.prefabIdentifierInfos.Count > 0) {
				ObsoleteIdentifiers obsoleteIdentifiers = surfacePrefab.AddComponent<ObsoleteIdentifiers>();
				obsoleteIdentifiers.m_PrefabIdentifiers = [..jSONMaterail.prefabIdentifierInfos];
			}
		}

		byte[] fileData; 

		try {
			fileData = File.ReadAllBytes(folderPath+"\\_BaseColorMap.png");
			Texture2D texture2D_BaseColorMap = new(1, 1);
			if(!texture2D_BaseColorMap.LoadImage(fileData)) {UnityEngine.Debug.LogError($"[ELT] Failed to Load the BaseColorMap image for the {surfacePrefab.name} surface."); return;}

			if(!File.Exists(folderPath+"\\icon.png")) ELT.ResizeTexture(texture2D_BaseColorMap, 128, folderPath+"\\icon.png");
			// if(texture2D_BaseColorMap.width > 512 || texture2D_BaseColorMap.height > 512) texture2D_BaseColorMap = ELT.ResizeTexture(texture2D_BaseColorMap, 512, folderPath+"\\_BaseColorMap.png");
			
			if(!SurfaceInformation.ContainsKey("ELT_SnowAmount")) SurfaceInformation.Add("ELT_SnowAmount", 0.15f);

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
			}
		} catch {}

		if(Settings.settings.EnableSnowSurfaces) {
			Vector4 baseColor = newMaterial.GetVector("_BaseColor");
			baseColor.w -= (float)SurfaceInformation["ELT_SnowAmount"];
			newMaterial.SetVector("_BaseColor", baseColor);
		}

		// try {
		// 	Texture2D texture2D = (Texture2D)newMaterial.GetTexture("_BaseColorMap");
		// 	Plugin.Logger.LogMessage(texture2D);


		// 	newMaterial.SetTexture("_BaseColorMap", texture2D);
		// } catch (Exception e) {Plugin.Logger.LogError(e);}


		RenderedArea renderedArea = surfacePrefabPlaceHolder.AddComponent<RenderedArea>();
		renderedArea.m_RendererPriority = (int)newMaterial.GetFloat("_DrawOrder");
		renderedArea.m_LodBias = 0;
		renderedArea.m_Roundness = 1;
		renderedArea.m_Material = newMaterial;

		PlaceholderArea placeholderArea = surfacePrefabPlaceHolder.AddComponent<PlaceholderArea>();

		SpawnableArea spawnableArea = surfacePrefab.AddComponent<SpawnableArea>();
		spawnableArea.m_Placeholders = new AreaPrefab[1];
		spawnableArea.m_Placeholders[0] = surfacePrefabPlaceHolder;

		RenderedArea renderedArea1 = surfacePrefab.AddComponent<RenderedArea>();
		renderedArea1.m_RendererPriority = (int)newMaterial.GetFloat("_DrawOrder");
		renderedArea1.m_LodBias = 0;
		renderedArea1.m_Roundness = 1;
		renderedArea1.m_Material = newMaterial;

		if(File.Exists(folderPath+"\\icon.png")) {
			fileData = File.ReadAllBytes(folderPath+"\\icon.png");
			Texture2D texture2D_Icon = new(1, 1);
			if(texture2D_Icon.LoadImage(fileData)) {
				if(texture2D_Icon.width > 128 || texture2D_Icon.height > 128) {
					ELT.ResizeTexture(texture2D_Icon, 128, folderPath+"\\icon.png");
				}
			}

		}

		UIObject surfacePrefabUI = surfacePrefab.AddComponent<UIObject>();
		surfacePrefabUI.active = true;
		surfacePrefabUI.m_IsDebugObject = false;
		surfacePrefabUI.m_Icon = File.Exists(folderPath+"\\icon.png") ? $"{GameManager_InitializeThumbnails.COUIBaseLocation}/CustomSurfaces/{CatName}/{new DirectoryInfo(folderPath).Name}/icon.png" : ELT.GetIcon(surfacePrefab);
		surfacePrefabUI.m_Priority = -1;
		surfacePrefabUI.m_Group = SetupUIGroupe(surfacePrefab, CatName);

		surfacePrefab.AddComponent<CustomSurface>();

		ELT.m_PrefabSystem.AddPrefab(surfacePrefab);
		ELT.m_PrefabSystem.AddPrefab(surfacePrefabPlaceHolder);

	}

	internal static UIAssetCategoryPrefab SetupUIGroupe(PrefabBase prefab, string cat = null) {

		cat ??= GetCatByRendererPriority(prefab.GetComponent<RenderedArea>().m_RendererPriority);

		if(!SurfacesDataBase.ContainsKey(prefab)) SurfacesDataBase.Add(prefab, cat);

		if(CanCreateCustomSurfaces()) {
			return Prefab.GetOrCreateNewToolCategory(prefab, Prefab.GetCustomAssetMenuName(), SurfacesDataBase[prefab]+" Surfaces");
		} else {
			return Prefab.GetOrCreateNewToolCategory(prefab, "Landscaping", "Surfaces", "Terraforming");
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

	internal static string GetCatByRendererPriority( int i) {
		return i switch
		{   
			-100 => "Ground",
			-99 => "Ground", //"Grass",
			-98 => "Ground", //"Sand",
			-97 => "Concrete",
			-96 => "Concrete", //"Pavement",
			-95 => "Tiles",
			_ => "Misc"
		};
	}
	internal static bool CanCreateCustomSurfaces() {
		return Settings.settings.LoadCustomSurfaces && FolderToLoadSurface.Count > 0;
	}
}


internal class CustomSurface : ComponentBase
{
    public override void GetArchetypeComponents(HashSet<ComponentType> components) {}
    public override void GetPrefabComponents(HashSet<ComponentType> components) {}
}