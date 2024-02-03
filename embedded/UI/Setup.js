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