var ExtraLandscapingTools_InfoSectionContainer = document.getElementsByClassName("content_gqa")[0]

if(!ExtraLandscapingTools_InfoSectionContainer.contains(document.getElementById("ExtraLandscapingTools_TransformSection"))) {

    var ExtraLandscapingTools_TransformSection = document.createElement("div")
    var ExtraLandscapingTools_TransformSection_content = document.createElement("div")
    // var ExtraLandscapingTools_TransformSection_content_position_InfoRow = document.createElement("div")
    // var ExtraLandscapingTools_TransformSection_content_position_InfoRow_left = document.createElement("div")
    // var ExtraLandscapingTools_TransformSection_content_position_InfoRow_right = document.createElement("div")
    
    ExtraLandscapingTools_TransformSection.className = "info-section_I7V"
    ExtraLandscapingTools_TransformSection.id = "ExtraLandscapingTools_TransformSection"
    ExtraLandscapingTools_TransformSection_content.className = "content_Cdk item-focused_FuT disable-focus-highlight_ik3"
    // ExtraLandscapingTools_TransformSection_content_main_InfoRow.className = "info-row_QQ9 item-focused_FuT"
    // ExtraLandscapingTools_TransformSection_content_position_InfoRow.className = "info-row_QQ9 item-focused_FuT"
    // ExtraLandscapingTools_TransformSection_content_position_InfoRow_left.className = "left_RyE uppercase_f0y"
    // ExtraLandscapingTools_TransformSection_content_position_InfoRow_right.className = "right_ZUb"

    // ExtraLandscapingTools_TransformSection_content_position_InfoRow_left.innerHTML = "position"

    
    ExtraLandscapingTools_TransformSection_MainInfoview("TransformSection")
    ExtraLandscapingTools_TransformSection_SubInfoview("Position", "elt.transformsection_getpos")
    ExtraLandscapingTools_TransformSection_SubInfoview("Rotation", "elt.transformsection_getpos")

    // ExtraLandscapingTools_TransformSection_content_position_InfoRow.appendChild(ExtraLandscapingTools_TransformSection_content_position_InfoRow_left)
    // ExtraLandscapingTools_TransformSection_content_position_InfoRow.appendChild(ExtraLandscapingTools_TransformSection_content_position_InfoRow_right)
    // ExtraLandscapingTools_TransformSection_content.appendChild(ExtraLandscapingTools_TransformSection_content_position_InfoRow)
    ExtraLandscapingTools_TransformSection.appendChild(ExtraLandscapingTools_TransformSection_content)
    ExtraLandscapingTools_InfoSectionContainer.appendChild(ExtraLandscapingTools_TransformSection)
}

function ExtraLandscapingTools_TransformSection_MainInfoview(name) {
    var ExtraLandscapingTools_TransformSection_content_InfoRow = document.createElement("div")
    var ExtraLandscapingTools_TransformSection_content_InfoRow_left = document.createElement("div")
    var ExtraLandscapingTools_TransformSection_content_InfoRow_right = document.createElement("div")

    ExtraLandscapingTools_TransformSection_content_InfoRow.className = "info-row_QQ9 item-focused_FuT"
    ExtraLandscapingTools_TransformSection_content_InfoRow_left.className = "left_RyE uppercase_f0y"
    ExtraLandscapingTools_TransformSection_content_InfoRow_right.className = "right_ZUb"

    ExtraLandscapingTools_TransformSection_content_InfoRow_left.innerHTML = name

    ExtraLandscapingTools_TransformSection_content_InfoRow.appendChild(ExtraLandscapingTools_TransformSection_content_InfoRow_left)
    ExtraLandscapingTools_TransformSection_content_InfoRow.appendChild(ExtraLandscapingTools_TransformSection_content_InfoRow_right)
    ExtraLandscapingTools_TransformSection_content.appendChild(ExtraLandscapingTools_TransformSection_content_InfoRow)
}

function ExtraLandscapingTools_TransformSection_SubInfoview(name, event) {
    var ExtraLandscapingTools_TransformSection_content_InfoRow = document.createElement("div")
    var ExtraLandscapingTools_TransformSection_content_InfoRow_left = document.createElement("div")
    var ExtraLandscapingTools_TransformSection_content_InfoRow_right = document.createElement("div")

    ExtraLandscapingTools_TransformSection_content_InfoRow.className = "info-row_QQ9 item-focused_FuT subRow_NJI link_ICj"
    ExtraLandscapingTools_TransformSection_content_InfoRow_left.className = "left_RyE link_ICj"
    ExtraLandscapingTools_TransformSection_content_InfoRow_right.className = "right_ZUb link_ICj"
    ExtraLandscapingTools_TransformSection_content_InfoRow_right.style.justifyContent = "flex-end"
    ExtraLandscapingTools_TransformSection_content_InfoRow_right.style.alignContent = "flex-end"

    ExtraLandscapingTools_TransformSection_content_InfoRow_left.innerHTML = name

    ExtraLandscapingTools_TransformSection_SubInfoview_CreateInput(event, ExtraLandscapingTools_TransformSection_content_InfoRow_right)
    ExtraLandscapingTools_TransformSection_SubInfoview_CreateInput(event, ExtraLandscapingTools_TransformSection_content_InfoRow_right)
    ExtraLandscapingTools_TransformSection_SubInfoview_CreateInput(event, ExtraLandscapingTools_TransformSection_content_InfoRow_right)

    ExtraLandscapingTools_getterValue( event, ExtraLandscapingTools_TransformSection_content_InfoRow_right, ExtraLandscapingTools_TransformSection_UpdatePosition)

    ExtraLandscapingTools_TransformSection_content_InfoRow.appendChild(ExtraLandscapingTools_TransformSection_content_InfoRow_left)
    ExtraLandscapingTools_TransformSection_content_InfoRow.appendChild(ExtraLandscapingTools_TransformSection_content_InfoRow_right)
    ExtraLandscapingTools_TransformSection_content.appendChild(ExtraLandscapingTools_TransformSection_content_InfoRow)
}

function ExtraLandscapingTools_TransformSection_SubInfoview_CreateInput(event, element) {
    var ExtraLandscapingTools_TransformSection_content_InfoRow_right_input = document.createElement("input")
    ExtraLandscapingTools_TransformSection_content_InfoRow_right_input.className = "slider-input_DXM input_Wfi"
    ExtraLandscapingTools_TransformSection_content_InfoRow_right_input.type = "text"
    ExtraLandscapingTools_TransformSection_content_InfoRow_right_input.style.width = "32.5%"
    ExtraLandscapingTools_TransformSection_content_InfoRow_right_input.setAttributeNS(null, "vk-title", "")
    ExtraLandscapingTools_TransformSection_content_InfoRow_right_input.setAttributeNS(null, "vk-description", "")
    ExtraLandscapingTools_TransformSection_content_InfoRow_right_input.setAttributeNS(null, "vk-type", "text")
    ExtraLandscapingTools_TransformSection_content_InfoRow_right_input.setAttributeNS(null, "rows", "5")

    ExtraLandscapingTools_TransformSection_content_InfoRow_right_input.addEventListener("input", function() {
        ExtraLandscapingTools_TransformSection_OnInput(event, element)
    })

    element.appendChild(ExtraLandscapingTools_TransformSection_content_InfoRow_right_input)

}

function ExtraLandscapingTools_TransformSection_OnInput(event, element) {
    var data = {
        x: 0.0,
        y: 0.0,
        z: 0.0
    }
    data.x = parseFloat(element.children[0].value)
    data.y = parseFloat(element.children[1].value)
    data.z = parseFloat(element.children[2].value)
    engine.trigger(event, data)
}


function ExtraLandscapingTools_TransformSection_UpdatePosition(element, data) {
    element.children[0].value = data.x
    element.children[1].value = data.y
    element.children[2].value = data.z
    // element.innerHTML = data.x + " | " + data.y + " | " + data.z
}
