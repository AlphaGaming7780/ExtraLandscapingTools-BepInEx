if(document.getElementById("ExtraLandscapingTools_showMarker_item") == null) {

	var ExtraLandscapingTools_toolOption = document.getElementsByClassName("tool-options-panel_Se6")[0]
	var ExtraLandscapingTools_toolOption_first = document.getElementsByClassName("item_bZY")[0]
	
	var ExtraLandscapingTools_showMarker_item = document.createElement("div")
	var ExtraLandscapingTools_showMarker_itemContent = document.createElement("div")
	var ExtraLandscapingTools_showMarker_label = document.createElement("div")

	ExtraLandscapingTools_showMarker_item.className = "item_bZY"
	ExtraLandscapingTools_showMarker_item.id = "ExtraLandscapingTools_showMarker_item"
	ExtraLandscapingTools_showMarker_itemContent.className = "item-content_nNz"
	ExtraLandscapingTools_showMarker_label.className = "label_RZX"
	ExtraLandscapingTools_showMarker_label.innerHTML = "Show Markers"

	ExtraLandscapingTools_CreateToolOptionButton("Media/Tools/Snap Options/All.svg", "elt.showmarker")
	ExtraLandscapingTools_CreateToolOptionButton("Media/Tools/Snap Options/All.svg", "elt.selectsurfacereplacertool")

	ExtraLandscapingTools_toolOption.insertBefore(ExtraLandscapingTools_showMarker_item, ExtraLandscapingTools_toolOption_first)
	ExtraLandscapingTools_showMarker_item.appendChild(ExtraLandscapingTools_showMarker_itemContent)
	ExtraLandscapingTools_showMarker_itemContent.appendChild(ExtraLandscapingTools_showMarker_label)

}

function ExtraLandscapingTools_CreateToolOptionButton(icon, event) {
	var ExtraLandscapingTools_ToolOptionButton_content = document.createElement("div")
	var ExtraLandscapingTools_ToolOptionButton_content_button = document.createElement("button")
	var ExtraLandscapingTools_ToolOptionButton_content_button_image = document.createElement("img")


	ExtraLandscapingTools_ToolOptionButton_content.className = "content_ZIz"
	ExtraLandscapingTools_ToolOptionButton_content_button.className = "button_KVN button_KVN"
	ExtraLandscapingTools_ToolOptionButton_content_button_image.className = "icon_Ysc"
	ExtraLandscapingTools_ToolOptionButton_content_button_image.src = icon  

	ExtraLandscapingTools_getShowMarker(event, ExtraLandscapingTools_showMarker, ExtraLandscapingTools_ToolOptionButton_content_button)
	
	ExtraLandscapingTools_showMarker_itemContent.appendChild(ExtraLandscapingTools_ToolOptionButton_content)
	ExtraLandscapingTools_ToolOptionButton_content.appendChild(ExtraLandscapingTools_ToolOptionButton_content_button)
	ExtraLandscapingTools_ToolOptionButton_content_button.appendChild(ExtraLandscapingTools_ToolOptionButton_content_button_image)
	
	
	ExtraLandscapingTools_ToolOptionButton_content_button.addEventListener("click", function() {
		engine.trigger("audio.playSound", "select-item", 1);
		engine.trigger(event, !this.classList.contains("selected"))
	})
	
	ExtraLandscapingTools_ToolOptionButton_content_button.addEventListener("mouseenter", function() {
		engine.trigger("audio.playSound", "hover-item", 1);
	})
}

function ExtraLandscapingTools_getShowMarker(event, method, element) {
	const updateEvent = event + ".update"
	const subscribeEvent = event + ".subscribe"
	const unsubscribeEvent = event + ".unsubscribe"
	
	var sub = engine.on(updateEvent, (data) => {
		method && method(data, element)
	})

	engine.trigger(subscribeEvent)
	return () => {
		engine.trigger(unsubscribeEvent)
		sub.clear()
	};
}

function ExtraLandscapingTools_showMarker(bool, element) {

    if(bool) {
        element.classList.add("selected")
    } else {
        element.classList.remove("selected")
    }
}