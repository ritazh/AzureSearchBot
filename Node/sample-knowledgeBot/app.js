 require('./config.js')();
require('./connectorSetup.js')();
require('./searchHelpers.js')();
require('./dialogs/results.js')(); 
require('./dialogs/askQuestion.js')();
require('./dialogs/musicianExplorer.js')();
require('./dialogs/musicianSearch.js')();


var request = require('request');

// Entry point of the bot
bot.dialog('/', [
    function (session) {
        session.replaceDialog('/promptButtons');
    }
]);

bot.dialog('/promptButtons', [
    function (session) {
        var choices = ["Ask a question", "Talk to a representative"]
        builder.Prompts.choice(session, "What would you like to do?", choices);
    },
    function (session, results) {
        if (results.response) {
            var selection = results.response.entity;
            // route to corresponding dialogs
            switch (selection) {
                case "Ask a question":
                    session.replaceDialog('/askQuestion');
                    break;
                case "Talk to a representative":
                    session.send("Let me connect you...");
                    break;
                default:
                    session.reset('/');
                    break;
            }
        }
    }
]);



