using System;
using System.Collections.Generic;
using ExtraLandscapingTools.UI;
using Game.Prefabs;
using static ExtraLandscapingTools.Extensions;

namespace ExtraLandscapingTools;

public abstract class Extension {
	public abstract string ExtensionID { get; }
	public abstract ExtensionType Type { get; }
	public virtual SettingsUI UISettings { get; internal set;}

	internal protected virtual void OnCreate() {
		if(UISettings != null) ELT_UI.settings.Add(UISettings);
		Prefab.onAddPrefab += OnAddPrefab;
		ELT.onGetIcon += OnGetIcon;
		Localization.onLoadLocalization += OnLoadLocalization;
	}
	public virtual bool OnAddPrefab(PrefabBase prefabBase) { return true; }
	public virtual string OnGetIcon(PrefabBase prefabBase) { return null; }
	public virtual Dictionary<string , Dictionary<string, string>> OnLoadLocalization() { return null; }

	public T LoadSettings<T>(T ExtensionSettings) {
		return Settings.LoadSettings(ExtensionID, ExtensionSettings);
	}

	public void SaveSettings<T>(T ExtensionSettings) {
		Settings.SaveSettings(ExtensionID, ExtensionSettings);
	}
}

[Serializable]
public class ExtensionSettings;