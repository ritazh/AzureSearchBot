var builder = require('botbuilder');
var restify = require('restify');

// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat bot
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

const ChangePasswordOption = 'Change Password';
const ResetPasswordOption = 'Reset Password';

var bot = new builder.UniversalBot(connector, [function (session) {
    builder.Prompts.choice(session, 
        'What do yo want to do today?',
        [ChangePasswordOption, ResetPasswordOption]);
    },
    function (session, result) {
        if (result.response) {
            switch(result.response.entity) {
                case ChangePasswordOption:
                    session.send('This functionality is not yet implemented! Try resetting your password.');
                    break;
                case ResetPasswordOption:
                    session.beginDialog('resetPassword:/');
                    break;
            }
        } else {
            session.send('Didn\'t understand what you mean.');
        }
    }
]);

bot.set('persistUserData', true);

//bot.reloadAction('cancel', null, { matches: /^cancel/i });

//Sub-Dialogs
bot.library(require('./dialogs/reset-password'));

//Validators
bot.library(require('./validators'));

server.post('/api/messages', connector.listen());
