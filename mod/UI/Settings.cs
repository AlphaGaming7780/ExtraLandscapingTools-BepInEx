using System;
using System.Collections.Generic;

namespace ExtraLandscapingTools.UI;


[Serializable]
public class  SettingsUI {
    public string TabName;
    public List<SettingUI> settings;

    public SettingsUI(string TabName, List<SettingUI> settings) {
        this.TabName = TabName;
        this.settings = settings;
    }

    public SettingsUI()
    {
    }

}

[Serializable]
public class SettingUI
{

    public enum SettingUIType {
        CheckBox,
        Button,
    }

    public SettingUIType settingUIType;
    public string displayName;
    public string name;

    public SettingUI(SettingUIType settingUIType, string displayName, string name) {
        this.settingUIType = settingUIType;
        this.displayName = displayName;
        this.name = name;
    }

    public SettingUI()
    {
    }
}

public class SettingsButton : SettingUI
{
    public string buttonDescription;

    public SettingsButton(string displayName, string name, string buttonDescription = null) {
        settingUIType = SettingUIType.Button;
        this.displayName = displayName;
        this.name = name;
        this.buttonDescription = buttonDescription;
    }

    public SettingsButton()
    {
    }
}

public class SettingsCheckBox : SettingUI
{
    public SettingsCheckBox(string displayName, string name) {
        settingUIType = SettingUIType.CheckBox;
        this.displayName = displayName;
        this.name = name;
    }

    public SettingsCheckBox()
    {
    }
}