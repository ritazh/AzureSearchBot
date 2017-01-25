module.exports = {
    logIncomingMessage: function (session, next) {
        if (/^secret/i.test(session.message.text)) {
            session.send('42');
            return;
        }
        console.log(session.message.text);
        next();
    },
    logOutgoingMessage: function (event, next) {
        console.log(event.text);
        next();
    }
}