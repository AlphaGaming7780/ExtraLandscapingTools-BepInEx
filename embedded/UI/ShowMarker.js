var ExtraLandscapingTools_toolOption = document.getElementsByClassName("tool-options-panel_Se6")[0]
var ExtraLandscapingTools_toolOption_first = document.getElementsByClassName("item_bZY")[0]

var ExtraLandscapingTools_showMarker_item = document.createElement("div")
var ExtraLandscapingTools_showMarker_itemContent = document.createElement("div")
var ExtraLandscapingTools_showMarker_label = document.createElement("div")
var ExtraLandscapingTools_showMarker_content = document.createElement("div")
var ExtraLandscapingTools_showMarker_content_button = document.createElement("button")
var ExtraLandscapingTools_showMarker_content_button_image = document.createElement("img")

ExtraLandscapingTools_showMarker_item.className = "item_bZY"
ExtraLandscapingTools_showMarker_itemContent.className = "item-content_nNz"
ExtraLandscapingTools_showMarker_label.className = "label_RZX"
ExtraLandscapingTools_showMarker_label.innerHTML = "Show Markers"
ExtraLandscapingTools_showMarker_content.className = "content_ZIz"
ExtraLandscapingTools_showMarker_content_button.className = "button_KVN button_KVN"
ExtraLandscapingTools_showMarker_content_button_image.className = "icon_Ysc"
ExtraLandscapingTools_showMarker_content_button_image.src = "Media/Tools/Snap Options/All.svg"  

ExtraLandscapingTools_getShowMarker("elt.showmarker")

ExtraLandscapingTools_toolOption.insertBefore(ExtraLandscapingTools_showMarker_item, ExtraLandscapingTools_toolOption_first)
ExtraLandscapingTools_showMarker_item.appendChild(ExtraLandscapingTools_showMarker_itemContent)
ExtraLandscapingTools_showMarker_itemContent.appendChild(ExtraLandscapingTools_showMarker_label)
ExtraLandscapingTools_showMarker_itemContent.appendChild(ExtraLandscapingTools_showMarker_content)
ExtraLandscapingTools_showMarker_content.appendChild(ExtraLandscapingTools_showMarker_content_button)
ExtraLandscapingTools_showMarker_content_button.appendChild(ExtraLandscapingTools_showMarker_content_button_image)


ExtraLandscapingTools_showMarker_content_button.addEventListener("click", function() {
	engine.trigger("audio.playSound", "select-item", 1);
	engine.trigger("elt.showmarker", !this.classList.contains("selected"))
})

ExtraLandscapingTools_showMarker_content_button.addEventListener("mouseenter", function() {
	engine.trigger("audio.playSound", "hover-item", 1);
})

function ExtraLandscapingTools_getShowMarker(event) {
	const updateEvent = event + ".update"
	const subscribeEvent = event + ".subscribe"
	const unsubscribeEvent = event + ".unsubscribe"
	
	var sub = engine.on(updateEvent, (data) => {
		ExtraLandscapingTools_showMarker && ExtraLandscapingTools_showMarker(data)
	})

	engine.trigger(subscribeEvent)
	return () => {
		engine.trigger(unsubscribeEvent)
		sub.clear()
	};
}

function ExtraLandscapingTools_showMarker(bool) {

    if(bool) {
        ExtraLandscapingTools_showMarker_content_button.classList.add("selected")
    } else {
        ExtraLandscapingTools_showMarker_content_button.classList.remove("selected")
    }
}