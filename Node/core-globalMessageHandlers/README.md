# Global commands with dialogAction

One of the greatest advantages of the bot interface is it allows the user to type effectively whatever it is they want.

One of the greatest challenges of the bot interface is it allows the user to type effectively whatever it is they want.

We need to guide the user, and to make it easy for them to figure out what commands are available, and what information they're able to send to the bot. There are a few ways that we can assist the user, including providing buttons and choices. But sometimes it's just as easy as allowing the user to type *help* or *cancel*.

## Adding global commands

If you're going to add a cancel command, for example, you need to make sure the user can type it wherever they are, and trigger the block of code to inform the user what is available to them. Bot Framework offers two ways to add a global message hook: `beginDialogAction` and `triggerAction`.

### beginDialogAction

`beginDialogAction` can be called from either a bot or a dialog, and adds a global command for that context. When the regex matches, the dialog specified in `beginDialogAction` will then be executed. The main advantage to `beginDialogAction` is it allows you to add a global command to a specific dialog, enabling context specific help.

### triggerAction

`triggerAction` can be called on a dialog, and adds a way to globally call that dialog. The most obvious case for `triggerAction` is to create a global help dialog. However, `triggerAction` can also simplify the design of a bot, allowing you to add global commands, enabling the user to bounce around to the features and functionality they need, as they need it.

## The sample

In the example, we have a simple adding machine. When the user *add*, `addDialog` is executed and prompts the user for numbers. When the user types *total*, the bot prints out the current running total, and resets the conversation.

## Breaking it down

Let's take a look at the different sections of the bot.

### The constructor

When constructing the bot, we pass in a function as a waterfall step. This step will be executed as a default, so if nothing matches, and we're not already in a dialog, the simple introduction message will be sent.

### The help dialog

We want to create a centralized help conversation. We can do this by creating a normal dialog, and adding in a single step. The step in in the sample checks the `action` property of `args`, which normally would provide the name of the action being executed. In our case, as you'll see later, we're passing that in through the use of `beginDialogAction`.

``` javascript
bot.dialog('help', [
    (session, args, next) => {
        switch(args.action) {
            default:
                session.endDialog(`I'm a simple calculator bot. I can add numbers if you type "add".`);
            case 'addHelp':
                session.endDialog('Adds numbers. You can type "help" to get this message or "total" to see the total and start over.');
        }
    }
])
```

#### triggerAction

To make this dialog available globally, we call `triggerAction` on the dialog. `triggerAction` accepts either a regular expression or the name of an intent from a recognizer. The code snipped below registers the command *help* (or anything that starts with the word *help*) as the trigger for this dialog.

``` javascript
bot.dialog('help', [
    // existing code
]).triggerAction({ matches: /^help/ });
```

##### triggerAction flow

One important thing to note about `triggerAction` is the dialog about to be executed, *help* in our case, will replace the current dialog stack. This is problematic for scenarios where we want to be able to pick up where the user left off. Imagine if the user had typed in a couple of numbers, and then typed *help* to figure out what they could do next. We don't want to throw out all of their work; we want to allow them to pick up with the same running total they had.

To alter this behavior, or perform any other cleanup work before the dialog is executed, you can provide a method for the event `onSelectedAction`. By calling `session.beginDialog` we push the *help* dialog onto the stack rather than replacing the stack.

``` javascript
bot.dialog(
    // existing code
).triggerAction({
    matches: /^help/,
    onSelectAction: (session, args, next) => {
        session.beginDialog('help', args);
    }
});
```

### The add dialog

The add dialog uses `triggerAction` just as we've seen before, registering the word *add* as the trigger for the dialog.

``` javascript
bot.dialog('addNumber', [
    (session, args, next) => {
        if(!session.privateConversationData.runningTotal) {
            session.privateConversationData.runningTotal = 0;
            builder.Prompts.number(session, `I'll add all the numbers you give me.`);
        } else {
            builder.Prompts.number(session, [`Give me the next one.`]);
        }
    },
    (session, results, next) => {
        let runningTotal = parseInt(session.privateConversationData.runningTotal);
        const newNumber = parseInt(results.response);
        session.privateConversationData.runningTotal = runningTotal += newNumber;
        session.replaceDialog('addNumber');
    },
]).triggerAction({
    matches: /^add/
})
```

The logic inside of the waterfall for *addNumber* asks the user for a number, adds it to the running total, and then starts the process over by calling `replaceDialog`. `triggerAction` registers this dialog on the word *add*. Because the running total is in `privateConversationData`, it will always be available regardless of the number of times the user types *add*.

#### Exiting by using cancelAction

As with many dialogs, it's possible the user might be in a situation where they simply want to exit out and start all over again. While this could, in theory, be done by using `triggerAction` or `beginDialogAction`, there isn't an elegant way to handle a confirmation. The last thing we want to do is have the user cancel out by accident; it's best to do a simple prompt to confirm their intent.

`cancelAction` is build for exactly that purpose. It allows you to register a word for cancelling the current dialog and returning to the start.

``` javascript
bot.dialog('addNumber', [
    // existing code
]).triggerAction({
    matches: /^add/
}).cancelAction('cancel', 'Operation cancelled', {
    matches: /^cancel/, // trigger word
    confirmPrompt: 'Are you sure you wish to cancel?'
})
```

`cancelAction` takes three parameters, the name of the action, the text to send the user when the dialog is cancelled, and finally the options for how the cancel will be triggered and confirmed.

#### Providing context specific help

While having a single global *help* command might be helpful in some circumstances, with even the most basic of bots you'll want to provide help based on what the user is doing at that time. If the user is in a dialog, I want to make sure we can give the user help specific to that dialog.

`beginDialogAction` allows you to register a command scoped to just that dialog. By using `beingDialogAction`, you can indicate that *help* only works for that dialog. In addition, if you are using a global dialog (as built in the sample), you can pass additional information to the dialog in the first parameter, which is the name of the action.

``` javascript
bot.dialog('addNumber', [
    // existing code, triggerAction and cancelAction
]).beginDialogAction('addHelp', // name of the action
    'help', // name of the dialog to execute
    {matches: /^help/} // word that triggers the action
);
```

The first parameter, which is the name of the action, will become available to the *help* dialog in `args.action`. You'll notice that we have a case statement looking for the name of the action, and then providing context specific help. This allows you to create a centralized help dialog that can provide the help the user needs when using your bot.

``` javascript
bot.dialog('help', [
    (session, args, next) => {
        switch(args.action) {
            default:
                session.endDialog(`I'm a simple calculator bot. I can add numbers if you type "add".`);
            case 'addHelp':
                session.endDialog('Adds numbers. You can type "help" to get this message or "total" to see the total and start over.');
        }
    }
]).triggerAction({
    // existing code
});
```

## Summary

`beginDialogAction`, `triggerAction` and `cancelAction` enable the creation of global event handlers. They can also make it easier to create a conversation flow for complex bots, centralize help, and greater dialog reuse.