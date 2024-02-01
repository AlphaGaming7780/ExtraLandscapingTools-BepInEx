try {
	if(ExtraLandscapingTools_header.contains(ExtraLandscapingTools_settingsButton)) {
		ExtraLandscapingTools_header.removeChild(ExtraLandscapingTools_settingsButton)
		if(ExtraLandscapingTools_settingsPanelBool) {
			ExtraLandscapingTools_bottomPanel.style.display = ""
			ExtraLandscapingTools_settingsPanel.style.display = "none"
			ExtraLandscapingTools_settingsPanelBool = false
		}
	}
} catch (error) {}

try {
	if(ExtraLandscapingTools_header.contains(ExtraLandscapingTools_settingsButton)) {
		ExtraLandscapingTools_header.removeChild(ExtraLandscapingTools_settingsButton)
	}
} catch (error) {}