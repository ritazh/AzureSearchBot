# Global commands with dialogAction

One of the greatest advantages of the bot interface is it allows the user to type effectively whatever it is they want.

One of the greatest challenges of the bot interface is it allows the user to type effectively whatever it is they want.

We need to guide the user, and to make it easy for them to figure out what commands are available, and what information they're able to send to the bot. There are a few ways that we can assist the user, including providing buttons and choices. But sometimes it's just as easy as allowing the user to type *help* or *cancel*.

## Adding global commands

If you're going to add a cancel command, for example, you need to make sure the user can type it wherever they are, and trigger the block of code to inform the user what is available to them. Bot Framework allows you to do this by creating a *DialogAction*.

### What is a DialogAction?

At the end of the day a DialogAction is a global way of starting a dialog. Unlike a traditional dialog, where it will be started or stopped based on a flow you define, a DialogAction is started based on the user typing in a particular keyword, regardless of where in the flow the user currently is. DialogActions are perfect for adding commands such as *help*, *cancel* or *representative*.

## Creating a DialogAction

You register a DialogAction by using the bot function `beginDialogAction`. `beginDialogAction` accepts three parameters, a name for the DialogAction, the name of the Dialog you wish to start, and a named parameter with the regular expression the bot should look for when starting the dialog.

``` javascript
bot.beginDialogAction('cancel', '/cancel', { matches: /^cancel/ });

bot.dialog('/cancel', [
    (session) => {
        // whatever you need the dialog to do,
        // such as ending the conversation
        session.endConversation('Conversation ended. Send a message to start a new one.');
    }
]);
```

The first line registers a DialogAction named **cancel**, calling a Dialog named **cancel**. The DialogAction will be launched when the user types anything that begins with the word **cancel**.

The next line registers a dialog, named **cancel**. This dialog is just like a normal dialog. You could prompt the user at this point for additional information about what they might like, query the **message** property from **session** to determine the full text of what the user typed in order to provide more specific cancel.

## DialogAction flow

Let's talk through the sample code.

### Resuming operations

``` javascript
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
```

As indicated before, a `DialogAction` is simply a `Dialog`; the only difference is in how it's started. As such, you can perform all `Dialog` operations, such as prompting the user for information. In our sample, we prompt the user for information. While we don't use this information in this sample, you could extend the code to perform the necessary operations for that command. When `endDialog` is called, control will return back to whatever step was being executed when control was passed to the dialogAction.

### Cancelling the conversation

``` javascript
bot.beginDialogAction('cancel', '/cancel', { matches: /^cancel/ });
bot.dialog('/cancel', [
    (session, args, next) => {
        // this will end the entire conversation
        session.endConversation('Conversation ended. Send a message to start a new one.');
    },
]);
```

In this example, `endConversation` is called, which will end the conversation, and remove all dialogs from the stack. You will certainly want to add a confirmation into a production bot, as it could be pretty easy to cancel the entire operation without realizing it.

## Summary

One of the biggest issues in creating a flow with a chat bot is the fact a user can say nearly anything, or could potentially get lost and not know what messages the bot is looking to receive. A **DialogAction** allows you to add global commands, such as help or cancel, which can create a more elegant flow to the dialog.