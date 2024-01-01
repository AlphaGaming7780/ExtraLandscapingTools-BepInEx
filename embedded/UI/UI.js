var ExtraLandscapingTools_toolOption = document.getElementsByClassName("tool-options-panel_Se6")[0]

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

ExtraLandscapingTools_brushOption_item.className = "item_bZY"
ExtraLandscapingTools_brushOption_itemContent.className = "item-content_nNz"
ExtraLandscapingTools_brushOption_label.className = "label_RZX"
ExtraLandscapingTools_brushOption_label.innerHTML = "Brushs"
ExtraLandscapingTools_brushOption_content.className = "content_ZIz"
ExtraLandscapingTools_brushOption_content_buttonDown.className = "button_KVN button_KVN start-button_F6t"
ExtraLandscapingTools_brushOption_content_buttonDown_img.className = "icon_Ysc"
ExtraLandscapingTools_brushOption_content_buttonDown_img.src = "Media/Glyphs/ThickStrokeArrowDown.svg"
ExtraLandscapingTools_brushOption_content_field.className = "number-field__Hd"
ExtraLandscapingTools_brushOption_content_buttonUp.className = "button_KVN button_KVN end-button_ztm"
ExtraLandscapingTools_brushOption_content_buttonUp_img.className = "icon_Ysc"
ExtraLandscapingTools_brushOption_content_buttonUp_img.src = "Media/Glyphs/ThickStrokeArrowUp.svg"

ExtraLandscapingTools_getBrush("tool.brushes")
ExtraLandscapingTools_getSelectedBrush("tool.selectedBrush", ExtraLandscapingTools_selectedBrush)

ExtraLandscapingTools_brushAngle_item.className = "item_bZY"
ExtraLandscapingTools_brushAngle_itemContent.className = "item-content_nNz"
ExtraLandscapingTools_brushAngle_label.className = "label_RZX"
ExtraLandscapingTools_brushAngle_label.innerHTML = "Brush angle"
ExtraLandscapingTools_brushAngle_content.className = "content_ZIz"
ExtraLandscapingTools_brushAngle_content_SliderContainer.className = "slider-container_Q_K"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider.className = "slider_KXG slider_pUS horizontal slider_ROT"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track.className = "track-bounds_H8_"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds.className = "range-bounds_lNt"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_range.className = "range_nHO range_iUN"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer.className = "thumb-container_aso"
ExtraLandscapingTools_brushAngle_content_SliderContainer_slider_track_rangeBounds_thumbContainer_thumb.className = "thumb_kkL"
ExtraLandscapingTools_brushAngle_content_input.className = "slider-input_DXM input_Wfi"
ExtraLandscapingTools_brushAngle_content_input.type = "text"
// ExtraLandscapingTools_brushAngle_content_input += "vk-title vk-description vk-type='text' rows='5'"

ExtraLandscapingTools_toolOption.appendChild(ExtraLandscapingTools_brushOption_item)
ExtraLandscapingTools_brushOption_item.appendChild(ExtraLandscapingTools_brushOption_itemContent)
ExtraLandscapingTools_brushOption_itemContent.appendChild(ExtraLandscapingTools_brushOption_label)
ExtraLandscapingTools_brushOption_itemContent.appendChild(ExtraLandscapingTools_brushOption_content)
ExtraLandscapingTools_brushOption_content.appendChild(ExtraLandscapingTools_brushOption_content_buttonDown)
ExtraLandscapingTools_brushOption_content.appendChild(ExtraLandscapingTools_brushOption_content_field)
ExtraLandscapingTools_brushOption_content.appendChild(ExtraLandscapingTools_brushOption_content_buttonUp)
ExtraLandscapingTools_brushOption_content_buttonDown.appendChild(ExtraLandscapingTools_brushOption_content_buttonDown_img)
ExtraLandscapingTools_brushOption_content_buttonUp.appendChild(ExtraLandscapingTools_brushOption_content_buttonUp_img)

// ExtraLandscapingTools_toolOption.appendChild(ExtraLandscapingTools_brushAngle_item)
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

ExtraLandscapingTools_brushOption_content_buttonDown.addEventListener("click", function() {
	ExtraLandscapingTools_index--
	if(ExtraLandscapingTools_index < 0) ExtraLandscapingTools_index = 0
	engine.trigger("tool.selectBrush", ExtraLandscapingTools_brushs[ExtraLandscapingTools_index].entity)
})

ExtraLandscapingTools_brushOption_content_buttonUp.addEventListener("click", function() {
	ExtraLandscapingTools_index++
	if(ExtraLandscapingTools_index > ExtraLandscapingTools_brushs.length-1) ExtraLandscapingTools_index = ExtraLandscapingTools_brushs.length-1
	engine.trigger("tool.selectBrush", ExtraLandscapingTools_brushs[ExtraLandscapingTools_index].entity)
})

function ExtraLandscapingTools_selectedBrush(brush) {
	ExtraLandscapingTools_index = ExtraLandscapingTools_brushs.findIndex(aBrush => aBrush.entity.index == brush.index)
	if(ExtraLandscapingTools_index < 0) ExtraLandscapingTools_index = 0
	ExtraLandscapingTools_brushOption_content_field.innerHTML = ExtraLandscapingTools_brushs[ExtraLandscapingTools_index].name
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