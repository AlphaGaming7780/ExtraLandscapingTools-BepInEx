using System;
using System.Collections.Generic;
using System.Reflection;
using Colossal.IO.AssetDatabase;
using Game.Prefabs;
using Unity.Entities;

namespace ExtraLandscapingTools;

public abstract class Extension {

	public enum ExtensionType : int
	{
		Other = 0,
		Surfaces = 1,
		// Decals = 2,
		Assets = 2
	}

	public static List<Extension> Extensions {private set; get;} = [];
	public abstract string ExtensionID { get; }
	public abstract ExtensionType Type { get; }
	public virtual ExtensionSettings ExtensionSettings { get; internal set;}

	protected virtual void OnCreate() {
		Prefab.onAddPrefab += OnAddPrefab;
		ELT.onGetIcon += OnGetIcon;
		if (ExtensionSettings != null) ExtensionSettings = Settings.LoadSettings(ExtensionID, ExtensionSettings);
	}

	// public virtual void OnLoadLocalization(LocaleAsset localeAsset) {}
	public virtual bool OnAddPrefab(PrefabBase prefabBase) {return true;}
	public virtual string OnGetIcon(PrefabBase prefabBase) {return null;}

	public virtual void OnELTSettings() {}

	public void SaveSettings() {
		Settings.SaveSettings(ExtensionID, ExtensionSettings);
	}

	public void Test() {
		Plugin.Logger.LogMessage(Assembly.GetExecutingAssembly().FullName);
	}

	public static void RegisterELTExtension( Extension extension ) {
		if(Extensions.Contains(extension)) return;
		Extensions.Add(extension);
		extension.OnCreate();
	}
}

[Serializable]
public class ExtensionSettings;