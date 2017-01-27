var builder = require('botbuilder');

module.exports = {
    logIncomingMessage: function (event, next, bot) {
        if (/^secret/i.test(event.text)) {
            bot.send(new builder.Message()
            .address(event.address)
            .text('42'));
            return;
        }
        console.log(event.text);
        next();
    },
    logOutgoingMessage: function (event, next) {
        console.log(event.text);
        next();
    }
}