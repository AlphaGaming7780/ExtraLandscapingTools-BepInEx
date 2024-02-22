var ExtraLandscapingTools_Header = document.getElementsByClassName("header_H_U")[0]
var ExtraLandscapingTools_AssetCategory = document.getElementsByClassName("asset-category-tab-bar_IGA")[0]
var ExtraLandscapingTools_AssetCategory_CloseButton = document.getElementsByClassName("close-button_kMj")[0]

var ExtraLandscapingTools_CustomAssetCategory = document.createElement("div");
var ExtraLandscapingTools_CustomAssetCategory_Items = document.createElement("div")

var ExtraLandscapingTools_CustomCatButtonDict = {}
var ExtraLandscapingTools_SelectedCustomAssetCategory;

ExtraLandscapingTools_CustomAssetCategory.id = "ExtraLandscapingTools_CustomAssetCategory"
ExtraLandscapingTools_CustomAssetCategory.className = "asset-category-tab-bar_IGA";
ExtraLandscapingTools_CustomAssetCategory_Items.className = "items_gPf"

ExtraLandscapingTools_getterValue("elt.assetscat", ExtraLandscapingTools_CustomAssetCategory, ExtraLandscapingTools_CreateCustomCatButtonCallBack)
ExtraLandscapingTools_SelectButtonAssetCat(ExtraLandscapingTools_CustomCatButtonDict[Object.keys(ExtraLandscapingTools_CustomCatButtonDict)[0]])

ExtraLandscapingTools_CustomAssetCategory.appendChild(ExtraLandscapingTools_CustomAssetCategory_Items)
// ExtraLandscapingTools_CustomAssetCategory.appendChild(ExtraLandscapingTools_AssetCategory.children[1])
// ExtraLandscapingTools_Header.appendChild(ExtraLandscapingTools_CustomAssetCategory)
if(ExtraLandscapingTools_Header.children[0] != null) {
    ExtraLandscapingTools_Header.insertBefore(ExtraLandscapingTools_CustomAssetCategory, ExtraLandscapingTools_Header.children[0])
} else {
    ExtraLandscapingTools_Header.appendChild(ExtraLandscapingTools_CustomAssetCategory)
}

function ExtraLandscapingTools_CreateCustomCatButton(name, icon) {
    var ExtraLandscapingTools_CustomCatButton = document.createElement("button")
    var ExtraLandscapingTools_CustomCatButton_Icon = document.createElement("img")
    var ExtraLandscapingTools_CustomCatButton_Div = document.createElement("div")

    ExtraLandscapingTools_CustomCatButton.className = "button_Yym button_Yym"
    ExtraLandscapingTools_CustomCatButton.id = (name)
    ExtraLandscapingTools_CustomCatButton_Icon.className = "icon_LAz"
    ExtraLandscapingTools_CustomCatButton_Icon.src = icon
    ExtraLandscapingTools_CustomCatButton_Div.className = "item-inner_NKx"

	ExtraLandscapingTools_CustomCatButton.addEventListener("click", function() {

		engine.trigger("audio.playSound", "select-item", 1);
        ExtraLandscapingTools_SelectButtonAssetCat(this)
	})

	ExtraLandscapingTools_CustomCatButton.addEventListener("mouseenter", function() {
		engine.trigger("audio.playSound", "hover-item", 1);
	})

    ExtraLandscapingTools_CustomCatButtonDict[name] = ExtraLandscapingTools_CustomCatButton

    ExtraLandscapingTools_CustomCatButton.appendChild(ExtraLandscapingTools_CustomCatButton_Icon)
    ExtraLandscapingTools_CustomCatButton.appendChild(ExtraLandscapingTools_CustomCatButton_Div)
    ExtraLandscapingTools_CustomAssetCategory_Items.appendChild(ExtraLandscapingTools_CustomCatButton)
}

function ExtraLandscapingTools_SelectButtonAssetCat(element) {
    if(ExtraLandscapingTools_SelectedCustomAssetCategory != undefined) {
        ExtraLandscapingTools_SelectedCustomAssetCategory.classList.remove('selected');
    }
    engine.trigger("elt.assetscat", element.id)
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