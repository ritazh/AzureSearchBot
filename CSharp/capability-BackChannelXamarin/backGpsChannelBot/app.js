require('./connectorSetup.js')();

//Bot listening for inbound backchannel events - in this case it only listens for events named "buttonClicked"
bot.on("event", function (event) {
    var msg = new builder.Message().address(event.address);
    msg.data.textLocale = "en-us";
    if (event.name === "userLocationObtained") {
        msg.data.text = "Your location is " + event.value;
    }
    bot.send(msg);
})

//Basic root dialog which takes an inputted color and sends a getUserLocation event. No NLP, regex, validation here - just grabs input and sends it back as an event. 
bot.dialog('/', [
    function (session) {
        var reply = createEvent("getUserLocation", session.message.text, session.message.address);
        session.endDialog(reply);
    }
]);

//Creates a backchannel event
const createEvent = (eventName, value, address) => {
    var msg = new builder.Message().address(address);
    msg.data.type = "event";
    msg.data.name = eventName;
    msg.data.value = value;
    return msg;
}



