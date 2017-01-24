'use strict';
const builder = require('botbuilder');

const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

const bot = module.exports = new builder.UniversalBot(connector);

// register the dialogAction
// each dialogAction needs a name,
// the name of the dialog to call, 
// and what regex it should look for
bot.beginDialogAction('settings', '/settings', { matches: /^settings/ });

bot.dialog('/settings', [
    // the dialog is just a normal bot dialog
    // you can perform whatever operations you need to perform
    (session, args, next) => {
        builder.Prompts.text(session, `After receiving your message, control will return to where you left off.`);
    },
    (session, results, next) => {
        // this will return to the step in the prior dialog
        session.endDialog('... returning to the prior step.');
    },
]);

bot.beginDialogAction('cancel', '/cancel', { matches: /^cancel/ });
bot.dialog('/cancel', [
    (session, args, next) => {
        // this will end the entire conversation
        session.endConversation('Conversation ended. Send a message to start a new one.');
    },
]);

// In a bot, a conversation can hold a collection of dialogs.

// Each dialog is designed to be a self-contained unit that can
// perform an action that might take multiple steps, such as collecting
// information from a user or performing an action on her behalf.

bot.dialog('/', [
    (session, args, next) => {
        session.send(`Hi there! I'm a sample bot showing how you can create global commands.`);
        session.send('Typing `cancel` will restart the conversation, while `settings` allows you to test starting a side dialog that picks up where you left off.');
        session.send(`Let's start the first dialog, which will ask you your name.`);

        session.beginDialog('/getName');
    },
    (session, results, next) => {
        if (results.response) {
            const name = session.privateConversationData.name = results.response;
            session.beginDialog('/getAge', { name: name });
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

bot.dialog('/getName', require('./getNameDialog.js'));

bot.dialog('/getAge', require('./getAgeDialog.js'));