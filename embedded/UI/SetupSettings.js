
var ExtraLandscapingTools_LandscapingMenu = document.getElementsByClassName("asset-panel_VFR")[0]
// var ExtraLandscapingTools_header = ExtraLandscapingTools_LandscapingMenu.childNodes[0]
var ExtraLandscapingTools_bottomPanel = ExtraLandscapingTools_LandscapingMenu.children[1]
var ExtraLandscapingTools_header = document.getElementsByClassName("asset-category-tab-bar_IGA")[0]

if(!ExtraLandscapingTools_header.contains(document.getElementById("ExtraLandscapingTools_settingsButton"))) {
	if(document.getElementById("ExtraLandscapingTools_settingsButton") == null) {

		var ExtraLandscapingTools_settingsButton = null

		var ExtraLandscapingTools_settingsPanelBool = false;
		var ExtraLandscapingTools_settingsPanel_OpenTab = null
		var ExtraLandscapingTools_settingsPanel_SelectedTabButton = null
		var ExtraLandscapingTools_settingsPanel_TabButtonDict = {}
		var ExtraLandscapingTools_settingsPanel_TabPanelDict = {}
		
		var ExtraLandscapingTools_settingsPanel = null
		var ExtraLandscapingTools_settingsPanel_TabsButton = null
		var ExtraLandscapingTools_settingsPanel_Tabs = null


		ExtraLandscapingTools_CreateSettingsButton()

	} else {
		ExtraLandscapingTools_header.insertBefore(ExtraLandscapingTools_settingsButton, ExtraLandscapingTools_header.childNodes[0])
	}
}

function ExtraLandscapingTools_CreateSettingsButton() {
	ExtraLandscapingTools_settingsButton = document.createElement("button", ExtraLandscapingTools_header.childNodes[1]);
	var ExtraLandscapingTools_settingsButtonImage = document.createElement("div")
	ExtraLandscapingTools_settingsButton.className = "button_bvQ button_bvQ close-button_wKK close-button_kMj"
	ExtraLandscapingTools_settingsButton.id = "ExtraLandscapingTools_settingsButton"
	ExtraLandscapingTools_settingsButton.style.marginLeft = "-10rem"
	ExtraLandscapingTools_settingsButton.style.marginRight = "9rem"
	ExtraLandscapingTools_settingsButton.style.marginBottom = "5rem"
	
	ExtraLandscapingTools_settingsButton.addEventListener("click", function() {
		engine.trigger("audio.playSound", "select-item", 1);
		if(!ExtraLandscapingTools_settingsPanelBool) {
			if(ExtraLandscapingTools_settingsPanel == null) ExtraLandscapingTools_CreateSettingsPanel()
			ExtraLandscapingTools_bottomPanel.style.display = "none"
			ExtraLandscapingTools_settingsPanel.style.display = "flex"
			ExtraLandscapingTools_settingsPanelBool = true
		} else {
			ExtraLandscapingTools_bottomPanel.style.display = ""
			ExtraLandscapingTools_settingsPanel.style.display = "none"
			ExtraLandscapingTools_settingsPanelBool = false
		}
	})

	ExtraLandscapingTools_settingsButton.addEventListener("mouseenter", function() {
		engine.trigger("audio.playSound", "hover-item", 1);
	})
	
	ExtraLandscapingTools_settingsButtonImage.className = "ExtraLandscapingTools_settingsButtonImage tinted-icon_iKo icon_PhD"
	ExtraLandscapingTools_settingsButtonImage.style.maskImage = "url(Media/Glyphs/Gear.svg)"
	ExtraLandscapingTools_settingsButtonImage.style.height = "24rem"
	ExtraLandscapingTools_settingsButtonImage.style.width = "24rem"
	// ExtraLandscapingTools_settingsButtonImage.style.backgroundColor = "var(--iconColor)"
	// ExtraLandscapingTools_settingsButtonImage.style.maskSize = "contain"
	// ExtraLandscapingTools_settingsButtonImage.style.maskPosition = "50% 50%"
	
	ExtraLandscapingTools_settingsButton.appendChild(ExtraLandscapingTools_settingsButtonImage)
	
	ExtraLandscapingTools_header.insertBefore(ExtraLandscapingTools_settingsButton, ExtraLandscapingTools_header.childNodes[0])
}

function ExtraLandscapingTools_CreateSettingsPanel() {
    ExtraLandscapingTools_settingsPanel = document.createElement("div");
	ExtraLandscapingTools_settingsPanel.className = "content_XD5 content_AD7 child-opacity-transition_nkS"
	// ExtraLandscapingTools_settingsPanel.style.height = "100%"
	// ExtraLandscapingTools_settingsPanel.style.width = "100%"
	ExtraLandscapingTools_settingsPanel.style.display = "none"
	ExtraLandscapingTools_settingsPanel.style.flexDirection = "row"

	ExtraLandscapingTools_settingsPanel_Tabs = document.createElement("div")
	ExtraLandscapingTools_settingsPanel_Tabs.className = "ExtraLandscapingTools_settingsPanel_Tabs" //detail_eTp
	ExtraLandscapingTools_settingsPanel_Tabs.style.overflowY = "auto"
	ExtraLandscapingTools_settingsPanel_Tabs.style.height = "100%"
	ExtraLandscapingTools_settingsPanel_Tabs.style.width = "80%"
	ExtraLandscapingTools_settingsPanel_Tabs.style.display = "flex"
	ExtraLandscapingTools_settingsPanel_Tabs.style.flexDirection = "column"
	// ExtraLandscapingTools_settingsPanel_Tabs.style.backgroundColor = "rgba(0,0,255, 0.2)"

	ExtraLandscapingTools_settingsPanel_TabsButton = document.createElement("div")
	ExtraLandscapingTools_settingsPanel_TabsButton.className = "ExtraLandscapingTools_settingsPanel_TabsButton" //menu_hb1
	ExtraLandscapingTools_settingsPanel_TabsButton.style.height = "100%"
	ExtraLandscapingTools_settingsPanel_TabsButton.style.width = "20%"
	ExtraLandscapingTools_settingsPanel_TabsButton.style.overflowY = "auto"
	ExtraLandscapingTools_settingsPanel_TabsButton.style.borderRightStyle = "solid"
	ExtraLandscapingTools_settingsPanel_TabsButton.style.borderRightWidth = "var(--stroke2)"
	ExtraLandscapingTools_settingsPanel_TabsButton.style.borderRightColor = "rgba(0, 0, 0, 0.100000)"
	ExtraLandscapingTools_settingsPanel_TabsButton.style.display = "flex"
	ExtraLandscapingTools_settingsPanel_TabsButton.style.flexDirection = "column"
	// ExtraLandscapingTools_settingsPanel_TabsButton.style.backgroundColor = "rgba(0,0,255, 0.2)"


	ExtraLandscapingTools_CreateSettingsCat("ELT Base")
	ExtraLandscapingTools_CreateSettingsCatToggle("ELT Base", "Load Custom Surfaces", "elt.loadcustomsurfaces")
	ExtraLandscapingTools_CreateSettingsCatToggle("ELT Base", "Enable Transfrom Section", "elt.enabletransformsection")
	// ExtraLandscapingTools_CreateSettingsCatToggle("Global", "Load last radio on startup", "extended_radio_settings.SaveLastRadio")
	// ExtraLandscapingTools_CreateSettingsCatButton("ELT Base", "Reload Radio", "extended_radio.reloadradio")

	ExtraLandscapingTools_SetDefaultOpenSettingsPanel("ELT Base")

	ExtraLandscapingTools_settingsPanel.appendChild(ExtraLandscapingTools_settingsPanel_TabsButton)
	ExtraLandscapingTools_settingsPanel.appendChild(ExtraLandscapingTools_settingsPanel_Tabs)

    ExtraLandscapingTools_LandscapingMenu.appendChild(ExtraLandscapingTools_settingsPanel)
}



function ExtraLandscapingTools_CreateSettingsCat(name) {
	var ExtraLandscapingTools_settingsPanel_Tabs_catButton = document.createElement("button")
	var ExtraLandscapingTools_settingsPanel_Tabs_catPanel = document.createElement("div")

	ExtraLandscapingTools_settingsPanel_Tabs_catButton.className = name
	ExtraLandscapingTools_settingsPanel_Tabs_catButton.style.color = "rgba(255,255,255,255)"
	ExtraLandscapingTools_settingsPanel_Tabs_catPanel.className = name

	ExtraLandscapingTools_settingsPanel_TabButtonDict[name] = ExtraLandscapingTools_settingsPanel_Tabs_catButton
	ExtraLandscapingTools_settingsPanel_TabPanelDict[name] = ExtraLandscapingTools_settingsPanel_Tabs_catPanel

	ExtraLandscapingTools_setupTab(ExtraLandscapingTools_settingsPanel_Tabs_catPanel)
	ExtraLandscapingTools_setupTabButton(ExtraLandscapingTools_settingsPanel_Tabs_catButton, ExtraLandscapingTools_settingsPanel_Tabs_catPanel)
}

function ExtraLandscapingTools_setupTabButton(tabButton) {

	tabButton.innerHTML = tabButton.className
	tabButton.className += "ExtraLandscapingTools_settingsTab_" + tabButton.className + " station-item_cOt" //item_pq7
	tabButton.style.width = "100%"
	tabButton.style.height = "auto"
	tabButton.style.display = "flex"

	tabButton.addEventListener("click", function() {

		engine.trigger("audio.playSound", "select-item", 1);
		ExtraLandscapingTools_settingsPanel_SelectedTabButton.classList.remove('selected');
		ExtraLandscapingTools_settingsPanel_SelectedTabButton = tabButton
		ExtraLandscapingTools_settingsPanel_SelectedTabButton.className = "selected " + ExtraLandscapingTools_settingsPanel_SelectedTabButton.className

		ExtraLandscapingTools_settingsPanel_OpenTab.style.display = "none"
		ExtraLandscapingTools_settingsPanel_OpenTab = ExtraLandscapingTools_settingsPanel_TabPanelDict[tabButton.innerHTML]
		ExtraLandscapingTools_settingsPanel_OpenTab.style.display = "flex"

	})

	tabButton.addEventListener("mouseenter", function() {
		engine.trigger("audio.playSound", "hover-item", 1);
	})

	ExtraLandscapingTools_settingsPanel_TabsButton.appendChild(tabButton)
}

function ExtraLandscapingTools_setupTab(tab) {

	tab.className = "ExtraLandscapingTools_settingsTab_" + tab.className
	tab.style.width = "100%"
	tab.style.height = "auto"
	tab.style.display = "none"
	tab.style.flexDirection = "column"

	ExtraLandscapingTools_settingsPanel_Tabs.appendChild(tab)

}

function ExtraLandscapingTools_SetDefaultOpenSettingsPanel(name) {

	ExtraLandscapingTools_settingsPanel_OpenTab = ExtraLandscapingTools_settingsPanel_TabPanelDict[name]
	ExtraLandscapingTools_settingsPanel_OpenTab.style.display = "flex"

	ExtraLandscapingTools_settingsPanel_SelectedTabButton = ExtraLandscapingTools_settingsPanel_TabButtonDict[name]
	ExtraLandscapingTools_settingsPanel_SelectedTabButton.className = "selected " + ExtraLandscapingTools_settingsPanel_SelectedTabButton.className
}

function ExtraLandscapingTools_CreateSettingsCatToggle(panelname, name, event) {

	var ExtraLandscapingTools_settingsContainer = document.createElement("div")
	var ExtraLandscapingTools_settingsToggleLabel = document.createElement("div")
	var ExtraLandscapingTools_settingsToggle = document.createElement("div")
	var ExtraLandscapingTools_settingsToggleCheckmark = document.createElement("div")

	ExtraLandscapingTools_settingsContainer.className = "station-item_cOt field_MBO field_UuC"
	ExtraLandscapingTools_settingsToggleLabel.className = "label_DGc label_ZLb"
	ExtraLandscapingTools_settingsToggle.className = "toggle_cca item-mouse-states_Fmi toggle_th_"
	ExtraLandscapingTools_settingsToggleCheckmark.className = "checkmark_NXV"

	ExtraLandscapingTools_settingsToggle.style.setProperty("--checkmarkColor", "rgba(80, 76, 83, 1.000000)")
	ExtraLandscapingTools_settingsToggle.style.alignItems = "flex-end"
	ExtraLandscapingTools_settingsToggle.style.backgroundColor = "rgba(236, 236, 236, 1.000000)"
	ExtraLandscapingTools_settingsToggleLabel.innerHTML = name
	ExtraLandscapingTools_settingsToggleLabel.style.color = "rgba(255,255,255,255)"

	ExtraLandscapingTools_settingsContainer.addEventListener("click", function () {
		// ExtraLandscapingTools_Check(ExtraLandscapingTools_settingsContainer.childNodes[1], !ExtraLandscapingTools_isCheck(ExtraLandscapingTools_settingsContainer.childNodes[1]))
		engine.trigger("audio.playSound", "select-item", 1);
		engine.trigger(event, !ExtraLandscapingTools_isCheck(ExtraLandscapingTools_settingsContainer.childNodes[1]))
	})

	ExtraLandscapingTools_settingsContainer.addEventListener("mouseenter", function() {
		engine.trigger("audio.playSound", "hover-item", 1);
	})

	ExtraLandscapingTools_settingsContainer.appendChild(ExtraLandscapingTools_settingsToggleLabel)
	ExtraLandscapingTools_settingsContainer.appendChild(ExtraLandscapingTools_settingsToggle)
	ExtraLandscapingTools_settingsToggle.appendChild(ExtraLandscapingTools_settingsToggleCheckmark)
	
	ExtraLandscapingTools_getterValue(event, ExtraLandscapingTools_settingsToggle, ExtraLandscapingTools_Check)

	// ExtraLandscapingTools_Check(ExtraLandscapingTools_settingsToggle, )

	ExtraLandscapingTools_settingsPanel_TabPanelDict[panelname].appendChild(ExtraLandscapingTools_settingsContainer)
}

function ExtraLandscapingTools_CreateSettingsCatButton(panelname, name, event) {

	var ExtraLandscapingTools_settingsContainer = document.createElement("div")
	var ExtraLandscapingTools_settingsButton = document.createElement("button")

	ExtraLandscapingTools_settingsContainer.className = "buttons_hd7"
	ExtraLandscapingTools_settingsButton.className = "button_WWa button_SH8"

	ExtraLandscapingTools_settingsButton.style.backgroundColor = "rgba(255, 255, 255, 1)"
	ExtraLandscapingTools_settingsButton.style.borderColor = "rgba(212, 23, 23, 1)"
	ExtraLandscapingTools_settingsButton.style.color = "rgba(212, 23, 23, 1)"
	ExtraLandscapingTools_settingsButton.innerHTML = name

	ExtraLandscapingTools_settingsButton.addEventListener("click", function () {
		engine.trigger("audio.playSound", "select-item", 1);
		engine.trigger(event)
	})

	ExtraLandscapingTools_settingsButton.addEventListener("mouseover", function () {

		this.style.backgroundColor = "rgba(212, 23, 23, 1)"
		this.style.color = "rgba(255, 255, 255, 1)"

	})

	ExtraLandscapingTools_settingsButton.addEventListener("mouseout", function () {

		this.style.backgroundColor = "rgba(255, 255, 255, 1)"
		this.style.color = "rgba(212, 23, 23, 1)"

	})

	ExtraLandscapingTools_settingsButton.addEventListener("mouseenter", function() {
		engine.trigger("audio.playSound", "hover-item", 1);
	})

	ExtraLandscapingTools_settingsContainer.appendChild(ExtraLandscapingTools_settingsButton)

	ExtraLandscapingTools_settingsPanel_TabPanelDict[panelname].appendChild(ExtraLandscapingTools_settingsContainer)
}

function ExtraLandscapingTools_Check(element, bool) {
	if(bool) {
		element.classList.remove("unchecked")
		element.classList.add("checked")
		element.childNodes[0].classList.remove("unchecked")
		element.childNodes[0].classList.add("checked")
	} else {
		element.classList.remove("checked")
		element.classList.add("unchecked")
		element.childNodes[0].classList.remove("checked")
		element.childNodes[0].classList.add("unchecked")
	}
}

function ExtraLandscapingTools_isCheck(element) {
	return element.classList.contains("checked")
}

function ExtraLandscapingTools_getterValue(event, element, onUpdate) {
    const updateEvent = event + ".update"
    const subscribeEvent = event + ".subscribe"
    const unsubscribeEvent = event + ".unsubscribe"
    
    var sub = engine.on(updateEvent, (data) => {
        element && onUpdate(element, data)
    })

    engine.trigger(subscribeEvent)
    return () => {
        engine.trigger(unsubscribeEvent)
        sub.clear()
    };
}