const builder = require('botbuilder');

const connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

const bot = new builder.UniversalBot(connector, (session, args, next) => {
    // default message
    session.send('Say "add" to start adding numbers.');
});

// global help
bot.dialog('help', [
    (session, args, next) => {
        // args.action is the name of the action being called
        // this is a very useful technique to centralize logic
        switch(args.action) {
            default:
                // no action, provide default help message
                session.endDialog(`I'm a simple calculator bot. I can add numbers if you type "add".`);
            case 'addHelp':
                // addHelp action. Provide help for add
                session.endDialog('Adds numbers. You can type "help" to get this message or "total" to see the total and start over.');
        }
    }
]).triggerAction({ 
    // registered to respond globally to the word "help"
    matches: /^help/,
    onSelectAction: (session, args, next) => {
        // By default, the flow is interrupted and dialog stack is reset
        // This allows us to push a new dialog onto the stack and resume
        session.beginDialog('help', args);
    }
});

bot.dialog('addNumber', [
    (session, args, next) => {
        if(!session.privateConversationData.runningTotal) {
            session.privateConversationData.runningTotal = 0;
            builder.Prompts.number(session, `I'll add all the numbers you give me. Give me the first number.`);
        } else {
            builder.Prompts.number(session, [`Give me the next one.`, `What's the next number?`, `Give me another.`]);            
        }
    },
    (session, results, next) => {
        // retrieve the running total and new number
        let runningTotal = parseInt(session.privateConversationData.runningTotal);
        const newNumber = parseInt(results.response);

        // update the running total in privateConversationData
        session.privateConversationData.runningTotal = runningTotal += newNumber;

        // restart dialog to get the next number
        session.replaceDialog('addNumber');
    },
]).triggerAction({
    // Registers global handler for **this** dialog
    matches: /^add/
}).cancelAction('cancel', 'Operation cancelled', {
    // Register cancel for this dialog
    matches: /^cancel/,
    confirmPrompt: 'Are you sure you wish to cancel?',
}).beginDialogAction('addHelp', 'help', { matches: /^help/ });

bot.dialog('displayTotal', [
    (session, args, next) => {
        session.endConversation(`The total is ${session.privateConversationData.runningTotal}`);
    }
]).triggerAction({ matches: /^total/ });

module.exports = bot;