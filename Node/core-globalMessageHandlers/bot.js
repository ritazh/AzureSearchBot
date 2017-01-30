'use strict';
const builder = require('botbuilder');

const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

const bot = module.exports = new builder.UniversalBot(connector, [
    (session, args, next) => {
        session.send(`Hi there! I'm a sample bot showing how multiple dialogs work.`);
        session.send(`Let's start the first dialog, which will ask you your name.`);
        session.beginDialog('getName');
    },
    (session, results, next) => {
        if (results.response) {
            const name = session.privateConversationData.name = results.response;
            session.beginDialog('getAge', { name: name });
        } else {
            session.endConversation(`Sorry, I didn't understand the response. Let's start over.`);
        }
    },
    (session, results, next) => {
        if (results.response) {
            const age = session.privateConversationData.age = results.response;
            const name = session.privateConversationData.name;
            session.endConversation(`Hello ${name}. You are ${age}`);
        } else {
            session.endConversation(`Sorry, I didn't understand the response. Let's start over.`);
        }
    },
]);

bot.dialog('getName', [
    (session, args, next) => {
        if(args) {
            session.dialogData.isReprompt = args.isReprompt;
        }
        builder.Prompts.text(session, 'What is your name?');
    },
    (session, results, next) => {
        const name = results.response;
        if (!name || name.trim().length < 3) {
            if (session.dialogData.isReprompt) {
                session.endDialogWithResult({ response: '' });
            } else {
                session.send('Sorry, name must be at least 3 characters.');
                session.replaceDialog('getName', { isReprompt: true });
            }
        } else {
            session.endDialogWithResult({ response: name.trim() });
        }
    }
]);

bot.dialog('getAge', [
    (session, args, next) => {
        let name = session.dialogData.name = 'User';
        if (args) {
            session.dialogData.isReprompt = args.isReprompt;
            name = session.dialogData.name = args.name;
        }
        builder.Prompts.number(session, `How old are you, ${name}?`);
    },
    (session, results, next) => {
        const age = results.response;
        if (!age || age < 13 || age > 90) {
            if (session.dialogData.isReprompt) {
                session.endDialogWithResult({ response: '' });
            } else {
                session.dialogData.didReprompt = true;
                session.send(`Sorry, that doesn't look right.`);
                session.replaceDialog('getAge', 
                    { name: session.dialogData.name, isReprompt: true });
            }
        } else {
            session.endDialogWithResult({ response: age });
        }
    }
]);

bot.dialog('help', (session, args, next) => {
    // send help message to the user and end this dialog
    session.endDialog('This is a simple bot that collects a name and age.');
}).triggerAction({
    matches: /^help$/,
    onSelectAction: (session, args, next) => {
        // overrides default behavior of replacing the dialog stack
        // This will add the help dialog to the stack
        session.beginDialog(args.action, args);
    }
});

bot.dialog('cancel', (session, args, next) => {
    // end the conversation to cancel the operation
    session.endConversation('Operation canceled');
}).triggerAction({
    matches: /^cancel$/
});