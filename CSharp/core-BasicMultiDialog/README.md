# Basic Multi-Dialog Sample

A sample that shows how to use the [Bot Builder for .NET SDK](https://dev.botframework.com/)'s [Dialog](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dialogs.html) system from the  to model a conversation.

### Prerequisites

To run this sample, install the prerequisites by following the steps in the [Getting Started in .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html) section of the documentation.

### Code Highlights

The Bot Builder for .NET SDK provides the Dialogs namespace to allows developers to easily model a conversation in the bots they develop. 
Dialogs are classes that implement the IDialog interface and are used to send and receive messages to and from the conversation. 
Dialogs can be simple classes that prompt the user for information and validate the response, or can be more complex conversation flows composed of other dialogs.

All dialogs accept an implementation of the IDialogContext interface, used to managed the context of the conversation. 
This context object manages the dialog stack, by implementing the IDialogStack interface. 
The dialog at the top of the stack is the active dialog in the conversation and can:
•	Post messages to the conversation.
•	Wait for messages from the conversation, suspending the conversation until the message arrives.
•	Call children dialogs, pushing them onto the stack and making them the active dialog in the conversation.
•	Mark them selves as done, popping them from the stack, and passing control back to the parent dialog.

The [`RootDialog`](Dialogs/RootDialog.cs) class, which represents our conversation, is wired into the `MessageController.Post()` method. Check out the [MessagesController](Controllers/MessagesController.cs#L22) class passing a delegate to the `Conversation.SendAsync()` method that will be used to construct a `RootDialog` and execute the dialog's `StartAsync()` method.


````C#
public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
{
    if (activity.Type == ActivityTypes.Message)
    {
        await Conversation.SendAsync(activity, () => new RootDialog());
    }
    else
    {
        this.HandleSystemMessage(activity);
    }

    var response = Request.CreateResponse(HttpStatusCode.OK);
    return response;
}
````

In the `StartAsync()` method, we are telling the bot to wait for a message from the user and call the `MessageReceivedAsync` resume method when the message ins received.

````C#
public async Task StartAsync(IDialogContext context)
{
    context.Wait(this.MessageReceivedAsync);
}
````
The Bot Framework comes with a number of built-in prompts encapsulated in the [PromptDialog](https://github.com/Microsoft/BotBuilder/blob/84e0973b7e4473b3a02c4e21233b82f439014c95/CSharp/Library/Microsoft.Bot.Builder/Dialogs/PromptDialog.cs) class, than can be used to collect input from a user.  Check out the [RootDialog](Dialogs/RootDialog.cs#L19-L28) class, in the `MessageReceivedAsync` method the usage of the [`PromptChoice`](https://github.com/Microsoft/BotBuilder/blob/84e0973b7e4473b3a02c4e21233b82f439014c95/CSharp/Library/Microsoft.Bot.Builder/Dialogs/PromptDialog.cs#L548) dialog to asks the user to pick up an option from a list.

````C#
private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
{
    PromptDialog.Choice(
        context, 
        this.AfterChoiceSelected, 
        new[] { ChangePasswordOption, ResetPasswordOption }, 
        "What do yo want to do today?", 
        "I am sorry but I didn't understand that. I need you to select one of the options below",
        attempts: 2);
}
````

Once the user picks an option the `PromptChoice` dialog ends and return the result to the parent dialog (in this case the RootDialog) by calling to the `ResumeAfter<T>` delegate passed when calling to the child dialog.  The `IDialogContext.Call()` method can be used to Call a child dialog and add it to the top of the stack transferring control to the new dialog.

Check out the [`AfterChoiceSelected`](Dialogs/RootDialog.cs#L30-L52) resume method retrieving the user selection and the usage of of `context.Call()` to give control of the conversation to a new dialog depending on the selected option.

````C#
private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
{
    try
    {
        var selection = await result;

        switch (selection)
        {
            case ChangePasswordOption:
                await context.PostAsync("This functionality is not yet implemented! Try resetting your password.");
                await this.StartAsync(context);
                break;

            case ResetPasswordOption:
                context.Call(new ResetPasswordDialog(), this.AfterResetPassword);
                break;
        }
    }
    catch (TooManyAttemptsException)
    {
        await this.StartAsync(context);
    }
}
````

The [`ResetPasswordDialog`](Dialogs/ResetPassword.cs) uses a set of custom Prompts dialogs (classes inheriting from `Prompt<T,U>`) to ask the user for her phone number and her date of birth and validate their input. Prompts implement a retry mechanish and after X attemps they throw a `TooManyAttempsException`. Dialog exceptions can be handled in the `ResumeAfter<T>` delegate passed to the `Call` method



Once the child dialog finishes the `IDialogContext.Done()` should be called to complete the current dialog and return a result to the parent dialog. 

The sample shows how to handle dialog exceptions by awaiting the result argument within a `try/catch` block and how the  [`ResetPasswordDialog`](Dialogs/ResetPassword.cs#L68) uses `context.Done()` to return if the reset operation was successful or not to the parent dialog

````C#
private async Task AfterDateOfBirthEntered(IDialogContext context, IAwaitable<DateTime> result)
{
    try
    {
        var dateOfBirth = await result;

        if (dateOfBirth != DateTime.MinValue)
        {
            await context.PostAsync($"The date of birth you provided is: {dateOfBirth.ToShortDateString()}");

            // Add your custom reset password logic here.
            var newPassword = Guid.NewGuid().ToString().Replace("-", string.Empty);

            await context.PostAsync($"Thanks! Your new password is _{newPassword}_");

            context.Done(true);
        }
        else
        {
            context.Done(false);
        }
    }
    catch (TooManyAttemptsException)
    {
        context.Done(false);
    }
}
````


### Outcome

You will see the following result in the Bot Framework Emulator when opening and running the sample solution.

![Sample Outcome](images/outcome.png)

### More Information

To get more information about how to get started in Bot Builder for .NET and Conversations please review the following resources:
* [Bot Builder for .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/index.html)
* [Dialogs](https://docs.botframework.com/en-us/csharp/builder/sdkreference/dialogs.html)
* [IDialogContext Interface](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d1/dc6/interface_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_i_dialog_context.html)
* [PromptDialog](https://docs.botframework.com/en-us/csharp/builder/sdkreference/d9/d03/class_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_prompt_dialog.html)
