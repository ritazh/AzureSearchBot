# Global Message Handlers Sample

A global message handler allows you to inspect every incoming message to your bot in order to control the conversation by responding to specific commands.

For example, you can define a global command in your bot to add a  

A sample that shows how to create global messages handlers to launch bot features from anywhere in the bot by using the [Dialog](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dialogs.html) system in the [Bot Builder for .NET SDK](https://dev.botframework.com/).

Settings - Interupts active dialog, when complete returns to the interupted dialog.
Cancel - Empties the dialog stack and returns the conversation to the RootDialog.

### Prerequisites

To run this sample, install the prerequisites by following the steps in the [Getting Started in .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html) section of the documentation.

This sample is based on the Basic Multi-Dialog Sample, so be sure to review that sample before getting started with this one.

### Overview

The Bot Builder for .NET SDK uses [AutoFac](https://autofac.org/) for [inversion of control and dependency injection](https://martinfowler.com/articles/injection.html). If you're not famililar with AutoFac, you can learn more in this [Quick Start Guide](http://autofac.readthedocs.io/en/latest/getting-started/index.html).

One of the ways BotF using AutoFac is Scroables.

Scorables - intercept every message in the bot, scored to see if the message should be processed by the Scroable or passed on to the dialog stack.

### Create the Settings Dialog
Dialog we'd like to add to the dialog stack whenever the user responds with 'settings' anywhere in the bot.

### Create SettingsScroable Score Incoming Messages

IScorable - defined as service to score messages.

Scroable - Implment IScorable as a component with AutoFac.

Explain code flow here.

### Define GlobalMessageHandlersBotModule to Register Scorables w/ Container

### Register Module with Conversation Container via Global ASAX.

### Cancel works the same.

But resets the stack.




It's often helpful to define global messages within a bot to allow users to access bot features from anywhere in the bot simply by replying with a specific word or phrase. For example, you could allow users to access their settings (account information, preferenes, etc.) anywhere in the bot by responding to any Dialog with the word 'settings'.

In this sample, we'll define two global message handlers that will modify the conversation by changing the [`IDialogStack`](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d1/dc6/interface_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_i_dialog_context.html):
* Settings - If the user anywhere in the bot with the word 'settings', the message handler

The Bot Builder for .NET SDK provides the Dialogs namespace to allows developers to easily model a conversation in the bots they develop. Dialogs are classes that implement the IDialog interface and are used to manage the messages sent and received from the conversation. 
Dialogs can be simple classes that prompt the user for information and validate the response, or they can be more complex conversation flows composed of other dialogs.

When a dialog is called, it's passed an instance of the [`IDialogContext`](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d1/dc6/interface_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_i_dialog_context.html) interface. This context object manages all dialogs in the conversation as a stack, by implementing the [`IDialogStack`](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d1/dc6/interface_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_i_dialog_context.html) interface. In this dialog stack, the dialog on the top of the stack is the active dialog and has access to the dialog context. The active dialog can use the dialog context to:
* Post messages to the conversation.
* Wait for messages from the conversation, suspending the bot until the message arrives.
* Call children dialogs, pushing them onto the stack and making them the active dialog.
* Mark themselves as complete, popping them from the stack, and passing control back to the parent dialog.

Let's look at how these concepts are used to manage a simple conversation in a bot.

#### Create the Root Dialog

When managing a conversation using the Dialog system, the conversation is rooted in a single dialog, often called the Root Dialog. The Root Dialog is the first dialog added to the dialog stack for the conversation. All other dialogs in the conversation are called from the Root Dialog, either directly or indirectly (in the case of a child dialog calling another dialog) and return to the Root Dialog (either directly or indirectly). The Root Dialog doesn't complete until your bot process ends.

To create the [`RootDialog`](Dialogs/RootDialog.cs) class, create a class that is marked with the `[Serializable]` attribute (so the dialog can be [serialized to state](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dialogs.html#Serialization)) and implement the `IDialog` interface. 

To implement the `IDialog` interface, you implement the `StartAsync()` methond. `StartAsync()` is called when the dialog becomes active. The method is passed the `IDialogContext` object, used to manage the conversation.

To wait for a message from the conversation, call `context.Wait()` and pass it the method you called when the message is received. When `MessageReceivedAsync()` is called, it's passed the dialog context and an [`IAwaitable`](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d9/d4e/interface_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_i_awaitable.html) of type [`IMessageActivity`](https://docs.botframework.com/en-us/csharp/builder/sdkreference/db/d11/_i_message_activity_8cs_source.html). To get the message, await the result.

````C#
[Serializable]
public class RootDialog : IDialog<object>
{
    public async Task StartAsync(IDialogContext context)
    {
        context.Wait(this.MessageReceivedAsync);
    }

    private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
    {
        var message = await result;
    }
}	
````

#### Add the Root Dialog to the Conversation

The `RootDialog` is added to the conversation in the [`MessageController`](Controllers/MessageController.cs) class via the `Post()` method. In the `Post()` method, the call to  `Conversation.SendAsync()` creates an instance of the `RootDialog`, adds it to the dialog stack to make it the active dialog, calling the `RootDialog.StartAsync()`, passing the message.

````C#
public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
{
    if (activity.Type == ActivityTypes.Message)
    {
        await Conversation.SendAsync(activity, () => new RootDialog());
    }
    else
    {
        HandleSystemMessage(activity);
    }
    var response = Request.CreateResponse(HttpStatusCode.OK);
    return response;
}	
````

At this point, you have a `RootDialog` that's added to the conversation and able to interact with the conversation via `IDialogContext`. Next, you can start creating other dialogs to call in order to manage the conversation with the user.

#### Create the Name Dialog

The [`NameDialog`](Dialogs/NameDialog.cs) class is used to ask for the user's name. We'll create the `NameDialog` class just like the `RootDialog` above, but we'll implement `IDialog<string>` to designate that the dialog returns with a string value when done. 

We'll use `context.PostAsync()` to post a message to the conversation ("What's your name?")and wait for a response by `calling context.Wait()`. `MessageReceivedAsync()` is called when the message is received from the user. Note that our bot stops and waits until that message is received.

When `MessageReceivedAsync()` is called, we validate the message to be a valid name by making sure the message has text (vs. an image as an attachment) and that the text isn't empty. If the message is a valid name, our dialog has completed successfully and calls `context.Done()` to pass the name as a string back to the calling dialog.

If the message isn't a valid name, we'll reprompt the user and wait for a response. Note that we're calling `MessageReceivedAsnc()` recursively until we get a valid response or after 3 attempts. After 3 attempts, we let the calling dialog know this dialog failed by calling `context.Fail()` and pass an exception that describes the issue.

Note: All dialogs should limit the number of retries they perform to avoid the bot getting stuck when a user doesn't know how to respond to a prompt.

````C#
public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
{
    [Serializable]
    public class NameDialog : IDialog<string>
    {
        private int attempts = 3;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("What is your name?");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                context.Done(message.Text);
            }
            else
            {
                --attempts;
                if (attempts > 0)
                {
                    await context.PostAsync("I'm sorry, I don't understand your reply. What is your name (e.g. 'Bill', 'Melinda')?");

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("Message was not a string or was an empty string."));
                }
            }
        }
    }
}	
````

The `AgeDialog` works the same way, but validates the reply to be a valid age and implements `IDialog<int>` to return an integer to the calling dialog.

#### Calling Dialogs from RootDialog

To manage the conversation, `RootDialog` calls the `NameDialog` and `AgeDialog` dialogs to get the user's name and age and posts the results to the conversation.

In `SendWelcomeMessageAsync()`, a welcome message is posted to the conversation and the `NameDialog` is added to the dialog stack via a call to `context.Call()`. `NameDialogResumeAfter()` is called when `NameDialog` completes successfully (calling `context.Done()`) or fails (calling `context.Fail()`).

If `NameDialog` completed by calling `context.Done()`, the name is returned as a string and the `AgeDialog` is called. If `NameDialog` completed by calling `context.Fail()`, the exception is caught and the `RootDialog` starts over by calling `SendWelcomeMessageAsync()`.

````C#
private async Task SendWelcomeMessageAsync(IDialogContext context)
{
	await context.PostAsync("Hi, I'm the Basic Multi Dialog bot. Let's get started.");

	context.Call(new NameDialog(), this.NameDialogResumeAfter);
}

private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
{
    try
    {
        this.name = await result;

        context.Call(new AgeDialog(this.name), this.AgeDialogResumeAfter);
    }
    catch (TooManyAttemptsException)
    {
        await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");

        await this.SendWelcomeMessageAsync(context);
    }
}	
````

When the `AgeDialog` completes, `AgeDialogResumeAfter` is called. If `AgeDialog` completed by calling `context.Done()`, the age is returned as an integer and the result of both dialogs is posted on the conversation. If `AgeDialog` completed by calling `context.Fail()`, the exception is caught and handled with a message to the user. 

Note: Under either path, `SendWelcomeMessageAsync()` is called, starting the `RootDialog` process all over again. This is expected for the `RootDialog`. The RootDialog is the root of all conversation, so it never ends until the bot process ends. In a real world bot, you'll add logic to manage this loop to make the conversation more engaging.

````C#
private async Task AgeDialogResumeAfter(IDialogContext context, IAwaitable<int> result)
{
    try
    {
        this.age = await result;

        await context.PostAsync($"Your name is { name } and your age is { age }.");

    }
    catch (TooManyAttemptsException)
    {
        await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
    }
    finally
    {
        await this.SendWelcomeMessageAsync(context);
    }
}	
````

### Outcome

Here's what the conversation looks like in the [Bot Framework Emulator](https://docs.botframework.com/en-us/tools/bot-framework-emulator/#navtitle) when supplying a valid name and age.

![Done Outcome](images/doneoutcome.png)

And here's what the convesation looks like when providing invalid responses to the `AgeDialog`.

![Failoutcome](images/failoutcome.PNG)

### More Information

For more information on managing the conversation using Dialogs, check out the following resources:
* [Bot Builder for .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/index.html)
* [Dialogs](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dialogs.html)
* [IDialogContext Interface](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d1/dc6/interface_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_i_dialog_context.html)

