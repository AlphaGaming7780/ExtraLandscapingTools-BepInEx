var ExtraLandscapingTools_toolOption = document.getElementsByClassName("tool-options-panel_Se6")[0]
var ExtraLandscapingTools_toolOption_first = document.getElementsByClassName("item_bZY")[0]

var ExtraLandscapingTools_brushOption_item = document.createElement("div")
var ExtraLandscapingTools_brushOption_itemContent = document.createElement("div")
var ExtraLandscapingTools_brushOption_label = document.createElement("div")
var ExtraLandscapingTools_brushOption_content = document.createElement("div")
var ExtraLandscapingTools_brushOption_content_buttonDown = document.createElement("button")
var ExtraLandscapingTools_brushOption_content_buttonDown_img = document.createElement("img")
var ExtraLandscapingTools_brushOption_content_field = document.createElement("div")
var ExtraLandscapingTools_brushOption_content_buttonUp = document.createElement("button")
var ExtraLandscapingTools_brushOption_content_buttonUp_img = document.createElement("img")

var ExtraLandscapingTools_brushAngle_item = document.createElement("div")
var ExtraLandscapingTools_brushAngle_itemContent = document.createElement("div")
var ExtraLandscapingTools_brushAngle_label = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content_SliderContainer = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content_SliderContainer_slider = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_range = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer_thumb = document.createElement("div")
var ExtraLandscapingTools_brushAngle_content_input = document.createElement("input")

var ExtraLandscapingTools_brushOption_content_buttonsDict = {}
var ExtraLandscapingTools_brushs
var ExtraLandscapingTools_index = 0
var ExtraLandscapingTools_isPressed = false
var ExtraLandscapingTools_cursorX = 0
var ExtraLandscapingTools_sliderX = 0

ExtraLandscapingTools_brushOption_item.className = "item_bZY"
ExtraLandscapingTools_brushOption_itemContent.className = "item-content_nNz"
ExtraLandscapingTools_brushOption_label.className = "label_RZX"
ExtraLandscapingTools_brushOption_label.innerHTML = "Brush"
ExtraLandscapingTools_brushOption_content.className = "content_ZIz"
ExtraLandscapingTools_brushOption_content_buttonDown.className = "button_KVN button_KVN start-button_F6t"
ExtraLandscapingTools_brushOption_content_buttonDown_img.className = "icon_Ysc"
ExtraLandscapingTools_brushOption_content_buttonDown_img.src = "Media/Glyphs/ThickStrokeArrowDown.svg"
ExtraLandscapingTools_brushOption_content_field.className = "number-field__Hd"
ExtraLandscapingTools_brushOption_content_buttonUp.className = "button_KVN button_KVN end-button_ztm"
ExtraLandscapingTools_brushOption_content_buttonUp_img.className = "icon_Ysc"
ExtraLandscapingTools_brushOption_content_buttonUp_img.src = "Media/Glyphs/ThickStrokeArrowUp.svg"

ExtraLandscapingTools_brushAngle_item.className = "item_bZY"
ExtraLandscapingTools_brushAngle_itemContent.className = "item-content_nNz"
ExtraLandscapingTools_brushAngle_label.className = "label_RZX"
ExtraLandscapingTools_brushAngle_label.innerHTML = "Brush Rotation"
ExtraLandscapingTools_brushAngle_content.className = "content_ZIz"
ExtraLandscapingTools_brushAngle_content.style.width = "40%"
ExtraLandscapingTools_brushAngle_content_SliderContainer.className = "slider-container_Q_K"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider.className = "slider_KXG slider_pUS horizontal slider_ROT"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track.className = "track-bounds_H8_"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.className = "range-bounds_lNt"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.style.width = "0%"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_range.className = "range_nHO range_iUN"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer.className = "thumb-container_aso"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer_thumb.className = "thumb_kkL"
ExtraLandscapingTools_brushAngle_content_input.className = "slider-input_DXM input_Wfi"
ExtraLandscapingTools_brushAngle_content_input.type = "text"
ExtraLandscapingTools_brushAngle_content_input.setAttributeNS(null, "vk-title", "")
ExtraLandscapingTools_brushAngle_content_input.setAttributeNS(null, "vk-description", "")
ExtraLandscapingTools_brushAngle_content_input.setAttributeNS(null, "vk-type", "text")
ExtraLandscapingTools_brushAngle_content_input.setAttributeNS(null, "rows", "5")

ExtraLandscapingTools_getBrush("tool.brushes")
ExtraLandscapingTools_getSelectedBrush("tool.selectedBrush")
ExtraLandscapingTools_getBrushAngle("tool.brushAngle")

ExtraLandscapingTools_toolOption.insertBefore(ExtraLandscapingTools_brushOption_item, ExtraLandscapingTools_toolOption_first)
ExtraLandscapingTools_brushOption_item.appendChild(ExtraLandscapingTools_brushOption_itemContent)
ExtraLandscapingTools_brushOption_itemContent.appendChild(ExtraLandscapingTools_brushOption_label)
ExtraLandscapingTools_brushOption_itemContent.appendChild(ExtraLandscapingTools_brushOption_content)
ExtraLandscapingTools_brushOption_content.appendChild(ExtraLandscapingTools_brushOption_content_buttonDown)
ExtraLandscapingTools_brushOption_content.appendChild(ExtraLandscapingTools_brushOption_content_field)
ExtraLandscapingTools_brushOption_content.appendChild(ExtraLandscapingTools_brushOption_content_buttonUp)
ExtraLandscapingTools_brushOption_content_buttonDown.appendChild(ExtraLandscapingTools_brushOption_content_buttonDown_img)
ExtraLandscapingTools_brushOption_content_buttonUp.appendChild(ExtraLandscapingTools_brushOption_content_buttonUp_img)

ExtraLandscapingTools_toolOption.insertBefore(ExtraLandscapingTools_brushAngle_item, ExtraLandscapingTools_toolOption_first)
ExtraLandscapingTools_brushAngle_item.appendChild(ExtraLandscapingTools_brushAngle_itemContent)
ExtraLandscapingTools_brushAngle_itemContent.appendChild(ExtraLandscapingTools_brushAngle_label)
ExtraLandscapingTools_brushAngle_itemContent.appendChild(ExtraLandscapingTools_brushAngle_content)
ExtraLandscapingTools_brushAngle_content.appendChild(ExtraLandscapingTools_brushAngle_content_SliderContainer)
ExtraLandscapingTools_brushAngle_content_SliderContainer.appendChild(ExtraLandscapingTools_brushAngle_content_SliderContainer_slider)
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider.appendChild(ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track)
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track.appendChild(ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds)
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.appendChild(ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_range)
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.appendChild(ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer)
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer.appendChild(ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer_thumb)
ExtraLandscapingTools_brushAngle_content.appendChild(ExtraLandscapingTools_brushAngle_content_input)

ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer_thumb.addEventListener("mousedown", (e) => {
    engine.trigger("audio.playSound", "grabSlider", 1);
	ExtraLandscapingTools_isPressed = true;
    ExtraLandscapingTools_cursorX = e.clientX;
	ExtraLandscapingTools_sliderX = ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.clientWidth
});

document.addEventListener("mouseup", () => {
    ExtraLandscapingTools_isPressed = false;
});

ExtraLandscapingTools_toolOption.addEventListener("mousemove", (e) => {
	if (!ExtraLandscapingTools_isPressed) return;
	e.preventDefault();
	let pourcent = (ExtraLandscapingTools_sliderX + (e.clientX - ExtraLandscapingTools_cursorX))/ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track.clientWidth * 100
	if(pourcent > 100) pourcent = 100
	if(pourcent < 0) pourcent = 0
	ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.style.width = `${pourcent}%`
    engine.trigger("audio.playSound", "drag-slider", pourcent/100);
	engine.trigger("tool.setBrushAngle", pourcent*3.6)
});

ExtraLandscapingTools_brushAngle_content_input.addEventListener("input", function() {
	if(this.value > 360) this.value = 360
	if(this.value < 0) this.value = 0
	// ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.style.width = ((this.value/360)*100)+"%"
	engine.trigger("tool.setBrushAngle", parseFloat(this.value))
})

ExtraLandscapingTools_brushOption_content_buttonDown.addEventListener("click", function() {
	if(ExtraLandscapingTools_index == 0) return
	engine.trigger("audio.playSound", "decrease-elevation", 1);
	engine.trigger("tool.selectBrush", ExtraLandscapingTools_brushs[ExtraLandscapingTools_index-1].entity)
})

ExtraLandscapingTools_brushOption_content_buttonDown.addEventListener("mouseenter", function() {
	if(ExtraLandscapingTools_index == 0) return
	engine.trigger("audio.playSound", "hover-item", 1);
})

ExtraLandscapingTools_brushOption_content_buttonUp.addEventListener("click", function() {
	if(ExtraLandscapingTools_index+1 == ExtraLandscapingTools_brushs.length) return
	engine.trigger("audio.playSound", "increase-elevation", 1);
	engine.trigger("tool.selectBrush", ExtraLandscapingTools_brushs[ExtraLandscapingTools_index+1].entity)
})

ExtraLandscapingTools_brushOption_content_buttonUp.addEventListener("mouseenter", function() {
	if(ExtraLandscapingTools_index+1 == ExtraLandscapingTools_brushs.length) return
	engine.trigger("audio.playSound", "hover-item", 1);
})

function ExtraLandscapingTools_selectedBrush(brush) {
	ExtraLandscapingTools_index = ExtraLandscapingTools_brushs.findIndex(aBrush => aBrush.entity.index == brush.index)
	if(ExtraLandscapingTools_index < 0) ExtraLandscapingTools_index = 0
	ExtraLandscapingTools_brushOption_content_field.innerHTML = ExtraLandscapingTools_brushs[ExtraLandscapingTools_index].name
	if(ExtraLandscapingTools_index == 0) ExtraLandscapingTools_brushOption_content_buttonDown.setAttribute("disabled", "")
	else ExtraLandscapingTools_brushOption_content_buttonDown.removeAttribute("disabled")
	if(ExtraLandscapingTools_index == ExtraLandscapingTools_brushs.length-1) ExtraLandscapingTools_brushOption_content_buttonUp.setAttribute("disabled", "")
	else ExtraLandscapingTools_brushOption_content_buttonUp.removeAttribute("disabled")
}

function ExtraLandscapingTools_updateBrushAngle(angle) {
	ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.style.width = angle/360*100+"%"
	ExtraLandscapingTools_brushAngle_content_input.value = angle
}

function ExtraLandscapingTools_getBrushAngle(event) {
	const updateEvent = event + ".update"
	const subscribeEvent = event + ".subscribe"
	const unsubscribeEvent = event + ".unsubscribe"
	
	var sub = engine.on(updateEvent, (data) => {
		ExtraLandscapingTools_updateBrushAngle && ExtraLandscapingTools_updateBrushAngle(data)
	})

	engine.trigger(subscribeEvent)
	return () => {
		engine.trigger(unsubscribeEvent)
		sub.clear()
	};
}

function ExtraLandscapingTools_getSelectedBrush(event) {
	const updateEvent = event + ".update"
	const subscribeEvent = event + ".subscribe"
	const unsubscribeEvent = event + ".unsubscribe"
	
	var sub = engine.on(updateEvent, (data) => {
		ExtraLandscapingTools_selectedBrush && ExtraLandscapingTools_selectedBrush(data)
	})

	engine.trigger(subscribeEvent)
	return () => {
		engine.trigger(unsubscribeEvent)
		sub.clear()
	};
}

function ExtraLandscapingTools_getBrush(event) {
	const updateEvent = event + ".update"
	const subscribeEvent = event + ".subscribe"
	const unsubscribeEvent = event + ".unsubscribe"
	
	var sub = engine.on(updateEvent, (data) => {
		ExtraLandscapingTools_brushs = data
	})

	engine.trigger(subscribeEvent)
	return () => {
		engine.trigger(unsubscribeEvent)
		sub.clear()
	};
}