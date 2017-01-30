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

The example is an update to the basic multi-dialog sample. We will add help and cancel dialogs at the end of bot.js.

## The help dialog

We want to create a centralized help conversation. We can do this by creating a normal dialog, and adding in a single step to send the help message.

``` javascript
bot.dialog('help', (session, args, next) => {
    session.endDialog('This is a simple bot that collects a name and age.');
}).triggerAction({
    matches: /^help$/,
    onSelectAction: (session, args, next) => {
        session.beginDialog(args.action, args);
    }
});
```

### triggerAction

To make this dialog available globally, we call `triggerAction` on the dialog. `triggerAction` accepts either a regular expression or the name of an intent from a recognizer. The code snipped below registers the command *help* (or anything that starts with the word *help*) as the trigger for this dialog.

``` javascript
bot.dialog('help', [
    // existing code
]).triggerAction({ matches: /^help/ });
```

#### triggerAction flow

One important thing to note about `triggerAction` is the dialog about to be executed, *help* in our case, will replace the current dialog stack. This is problematic for scenarios where we want to be able to pick up where the user left off. Imagine if the user had typed in a couple of numbers, and then typed *help* to figure out what they could do next. We don't want to throw out all of their work; we want to allow them to pick up with the same running total they had.

To alter this behavior, or perform any other cleanup work before the dialog is executed, you can provide a method for the event `onSelectedAction`. By calling `session.beginDialog` we push the *help* dialog onto the stack rather than replacing the stack.

``` javascript
bot.dialog(
    // existing code
).triggerAction({
    matches: /^help/,
    onSelectAction: (session, args, next) => {
        session.beginDialog(args.action, args);
    }
});
```

## The cancel dialog

Implementing a simple *cancel* dialog using Node is straight forward because of the behavior of `triggerAction`. As you'll remember, `triggerAction` replaces the dialog stack. Since our cancel needs to end the conversation, we can just leave that default behavior, and add in a call to `endConversation`, as you see below.

``` javascript
bot.dialog('cancel', (session, args, next) => {
    // end the conversation to cancel the operation
    session.endConversation('Operation canceled');
}).triggerAction({
    matches: /^cancel$/
});
```

## Summary

`triggerAction` enables the creation of global event handlers. They can also make it easier to create a conversation flow for complex bots, centralize help, and greater dialog reuse.