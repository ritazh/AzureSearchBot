'use strict';
const builder = require('botbuilder');

const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

const bot = module.exports = new builder.UniversalBot(connector);

// In a bot, a conversation can hold a collection of dialogs.

// Each dialog is designed to be a self-contained unit that can
// perform an action that might take multiple steps, such as collecting
// information from a user or performing an action on her behalf.

bot.dialog('/', [
    (session, args, next) => {
        session.send(`Hi there! I'm a sample bot showing how multiple dialogs work.`);
        session.send(`Let's start the first dialog, which will ask you your name.`);

        // Launch the getName dialog using beginDialog
        // When beginDialog completes, control will be passed
        // to the next function in the waterfall
        session.beginDialog('/getName');
    },
    (session, results, next) => {
        // executed when getName dialog completes
        // results parameter contains the object passed into endDialogWithResults

        // check for a response
        if (results.response) {
            const name = session.privateConversationData.name = results.response;

            // When calling another dialog, you can pass arguments in the second parameter
            session.beginDialog('/getCity', { name: name });
        } else {
            // no valid response received - End the conversation
            session.endConversation(`Sorry, I didn't understand the response. Let's start over.`);
        }
    },
    (session, results, next) => {
        // executed when getCity dialog completes
        // results parameter contains the object passed into endDialogWithResults

        // check for a response
        if (results.response) {
            const city = session.privateConversationData.city = results.response;
            const name = session.privateConversationData.name;

            session.endConversation(`Hello ${name} from ${city}`);
        } else {
            // no valid response received - End the conversation
            session.endConversation(`Sorry, I didn't understand the response. Let's start over.`);
        }
    },
]);

bot.dialog('/getName', [
    (session, args, next) => {
        builder.Prompts.text(session, 'What is your name?');
    },
    (session, results, next) => {
        const name = results.response;

        // Basic validation - did we get a response?
        if (!name || name.trim().length === 0) {
            // Bad response. Logic for single re-prompt
            if (session.dialogData.didReprompt) {
                // Re-prompt ocurred
                // Send back empty string
                session.endDialog({ response: '' });
            } else {
                // Set the flag
                session.dialogData.didReprompt = true;
                // Call replaceDialog to start the dialog over
                // This will replace the active dialog on the stack
                session.replaceDialog('/getName');
            }
        } else {
            // Valid name received
            // Return control to calling dialog
            // Pass the name in the response property of results
            session.endDialogWithResult({ response: name.trim() });
        }
    }
]);

bot.dialog('/getCity', [
    (session, args, next) => {
        builder.Prompts.text(session, 'Where do you live?');
    },
    (session, results, next) => {
        const city = results.response;

        // Basic validation - did we get a response?
        if (!city || city.trim().length === 0) {
            // Bad response. Logic for single re-prompt
            if (session.dialogData.didReprompt) {
                // Re-prompt ocurred
                // Send back empty string
                session.endDialog({ response: '' });
            } else {
                // Set the flag
                session.dialogData.didReprompt = true;
                // Call replaceDialog to start the dialog over
                // This will replace the active dialog on the stack
                session.replaceDialog('/getCity');
            }
        } else {
            // Valid city received
            // Return control to calling dialog
            // Pass the city in the response property of results
            session.endDialogWithResult({ response: city.trim() });
        }
    }
]);