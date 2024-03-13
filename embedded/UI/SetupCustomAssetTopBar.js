
// var ExtraLandscapingTools_AssetCategory = document.getElementsByClassName("asset-category-tab-bar_IGA")[0]
// var ExtraLandscapingTools_AssetCategory_CloseButton = document.getElementsByClassName("close-button_kMj")[0]

var ExtraLandscapingTools_Header = document.getElementsByClassName("header_H_U")[0]

if(!ExtraLandscapingTools_Header.contains(document.getElementById("ExtraLandscapingTools_CustomAssetCategory"))) {
	if(document.getElementById("ExtraLandscapingTools_CustomAssetCategory") == null) {

		var ExtraLandscapingTools_CustomAssetCategory = null
		var ExtraLandscapingTools_CustomAssetCategory_Items = null

		var ExtraLandscapingTools_CustomCatButtonDict = {}
		var ExtraLandscapingTools_SelectedCustomAssetCategory;

		ExtraLandscapingTools_CreateCustomAssetsCat()

	} else {
		if(ExtraLandscapingTools_Header.children[0] != null) {
			ExtraLandscapingTools_Header.insertBefore(ExtraLandscapingTools_CustomAssetCategory, ExtraLandscapingTools_Header.children[0])
		} else {
			ExtraLandscapingTools_Header.appendChild(ExtraLandscapingTools_CustomAssetCategory)
		}
	}
}


function ExtraLandscapingTools_CreateCustomAssetsCat() {
	ExtraLandscapingTools_CustomAssetCategory = document.createElement("div");
	ExtraLandscapingTools_CustomAssetCategory_Items = document.createElement("div")
	
	ExtraLandscapingTools_CustomAssetCategory.id = "ExtraLandscapingTools_CustomAssetCategory"
	ExtraLandscapingTools_CustomAssetCategory.className = "asset-category-tab-bar_IGA";
	ExtraLandscapingTools_CustomAssetCategory_Items.className = "items_gPf"
	
	ExtraLandscapingTools_getterValue("elt.assetscat", ExtraLandscapingTools_CustomAssetCategory, ExtraLandscapingTools_CreateCustomCatButtonCallBack)
	if(ExtraLandscapingTools_SelectedCustomAssetCategory == undefined) ExtraLandscapingTools_SelectButtonAssetCat(ExtraLandscapingTools_CustomCatButtonDict[Object.keys(ExtraLandscapingTools_CustomCatButtonDict)[0]])
	else ExtraLandscapingTools_SelectButtonAssetCat(ExtraLandscapingTools_CustomCatButtonDict[ExtraLandscapingTools_SelectedCustomAssetCategory.id], false)
	
	ExtraLandscapingTools_CustomAssetCategory.appendChild(ExtraLandscapingTools_CustomAssetCategory_Items)
	// ExtraLandscapingTools_CustomAssetCategory.appendChild(ExtraLandscapingTools_AssetCategory.children[1])
	// ExtraLandscapingTools_Header.appendChild(ExtraLandscapingTools_CustomAssetCategory)
	if(ExtraLandscapingTools_Header.children[0] != null) {
		ExtraLandscapingTools_Header.insertBefore(ExtraLandscapingTools_CustomAssetCategory, ExtraLandscapingTools_Header.children[0])
	} else {
		ExtraLandscapingTools_Header.appendChild(ExtraLandscapingTools_CustomAssetCategory)
	}
}

function ExtraLandscapingTools_CreateCustomCatButton(name, icon) {
	var ExtraLandscapingTools_CustomCatButton = document.createElement("button")
	var ExtraLandscapingTools_CustomCatButton_Icon = document.createElement("img")
	var ExtraLandscapingTools_CustomCatButton_Div = document.createElement("div")

	ExtraLandscapingTools_CustomCatButton.className = "button_Yym button_Yym"
	ExtraLandscapingTools_CustomCatButton.id = name
	ExtraLandscapingTools_CustomCatButton_Icon.className = "icon_LAz"
	ExtraLandscapingTools_CustomCatButton_Icon.src = icon
	ExtraLandscapingTools_CustomCatButton_Div.className = "item-inner_NKx"

	ExtraLandscapingTools_CustomCatButton.addEventListener("click", function() {

		engine.trigger("audio.playSound", "select-item", 1);
		ExtraLandscapingTools_SelectButtonAssetCat(this)
	})

	ExtraLandscapingTools_CustomCatButton.addEventListener("mouseenter", function() {
		ExtraLandscapingTools_ShowCustomCatInfo(name, icon)
		engine.trigger("audio.playSound", "hover-item", 1);
	})

	ExtraLandscapingTools_CustomCatButton.addEventListener("mouseleave", function() {
		ExtraLandscapingTools_HideCustomCatInfo(name)
	})

	ExtraLandscapingTools_CustomCatButtonDict[name] = ExtraLandscapingTools_CustomCatButton

	ExtraLandscapingTools_CustomCatButton.appendChild(ExtraLandscapingTools_CustomCatButton_Icon)
	ExtraLandscapingTools_CustomCatButton.appendChild(ExtraLandscapingTools_CustomCatButton_Div)
	ExtraLandscapingTools_CustomAssetCategory_Items.appendChild(ExtraLandscapingTools_CustomCatButton)
}

function ExtraLandscapingTools_ShowCustomCatInfo(name, icon) {
	var ExtraLandscapingTools_detailContainer = document.getElementsByClassName("detail-container_E9q")[0]
	var ExtraLandscapingTools_detailContainer_assetDetailPanel = document.createElement("div")
	var ExtraLandscapingTools_detailContainer_titleBar = document.createElement("div")
	var ExtraLandscapingTools_detailContainer_title = document.createElement("div")
	var ExtraLandscapingTools_detailContainer_content = document.createElement("div")
	var ExtraLandscapingTools_detailContainer_previewContainer = document.createElement("div")
	var ExtraLandscapingTools_detailContainer_preview = document.createElement("img")
	var ExtraLandscapingTools_detailContainer_column = document.createElement("div")
	var ExtraLandscapingTools_detailContainer_paragraphs = document.createElement("div")
	var ExtraLandscapingTools_detailContainer_paragraphsTexte = document.createElement("p")
	
	ExtraLandscapingTools_detailContainer_assetDetailPanel.className = "asset-detail-panel_hf8 detail-panel_izf"
	ExtraLandscapingTools_detailContainer_assetDetailPanel.id = "ExtraLandscapingTools_detailContainer_assetDetailPanel_"+name
	ExtraLandscapingTools_detailContainer_titleBar.className = "title-bar_I7O child-opacity-transition_nkS"
	ExtraLandscapingTools_detailContainer_title.className = "title_qub"
	ExtraLandscapingTools_detailContainer_content.className = "content_rep row_H0d child-opacity-transition_nkS"
	ExtraLandscapingTools_detailContainer_previewContainer.className = "preview-container_sPA"
	ExtraLandscapingTools_detailContainer_preview.className = "preview_MDY"
	ExtraLandscapingTools_detailContainer_column.className = "column_dTT"
	ExtraLandscapingTools_detailContainer_paragraphs.className = "paragraphs_nbD description_ZQn"

	ExtraLandscapingTools_detailContainer_title.innerHTML = name

	ExtraLandscapingTools_detailContainer_preview.src = icon

	ExtraLandscapingTools_detailContainer_paragraphsTexte.setAttribute("cohinline", "cohinline")
	ExtraLandscapingTools_detailContainer_paragraphsTexte.innerHTML = name


	ExtraLandscapingTools_detailContainer_titleBar.appendChild(ExtraLandscapingTools_detailContainer_title)
	ExtraLandscapingTools_detailContainer_assetDetailPanel.appendChild(ExtraLandscapingTools_detailContainer_titleBar)

	ExtraLandscapingTools_detailContainer_previewContainer.appendChild(ExtraLandscapingTools_detailContainer_preview)
	ExtraLandscapingTools_detailContainer_content.appendChild(ExtraLandscapingTools_detailContainer_previewContainer)

	ExtraLandscapingTools_detailContainer_paragraphs.appendChild(ExtraLandscapingTools_detailContainer_paragraphsTexte)
	ExtraLandscapingTools_detailContainer_column.appendChild(ExtraLandscapingTools_detailContainer_paragraphs)
	ExtraLandscapingTools_detailContainer_content.appendChild(ExtraLandscapingTools_detailContainer_column)

	ExtraLandscapingTools_detailContainer_assetDetailPanel.appendChild(ExtraLandscapingTools_detailContainer_content)

	ExtraLandscapingTools_detailContainer.appendChild(ExtraLandscapingTools_detailContainer_assetDetailPanel)

}

function ExtraLandscapingTools_HideCustomCatInfo(name) {
	var ExtraLandscapingTools_detailContainer_assetDetailPanel = document.getElementById("ExtraLandscapingTools_detailContainer_assetDetailPanel_"+name)
	ExtraLandscapingTools_detailContainer_assetDetailPanel.parentElement.removeChild(ExtraLandscapingTools_detailContainer_assetDetailPanel)
	
}


function ExtraLandscapingTools_SelectButtonAssetCat(element, bool = true) {
	if(ExtraLandscapingTools_SelectedCustomAssetCategory != undefined) {
		ExtraLandscapingTools_SelectedCustomAssetCategory.classList.remove('selected');
	}
	if(bool) engine.trigger("elt.assetscat", element.id)
	ExtraLandscapingTools_SelectedCustomAssetCategory = element
	ExtraLandscapingTools_SelectedCustomAssetCategory.classList.add("selected")
}

function ExtraLandscapingTools_CreateCustomCatButtonCallBack(element, data) {
	JSON.parse(data).forEach(ExtraLandscapingTools_CatButton => {
		if(ExtraLandscapingTools_CustomCatButtonDict[ExtraLandscapingTools_CatButton.catName] == null) {
			ExtraLandscapingTools_CreateCustomCatButton(ExtraLandscapingTools_CatButton.catName, ExtraLandscapingTools_CatButton.icon)
		}
	});
	data = null
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