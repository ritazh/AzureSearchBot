using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace ConversationFlow.Dialogs
{
    /* All implementations of IDialog must be serializable so the conversation state (the dialog stack) can be 
        serialized when the bot is waiting for the user to respond. */
    [Serializable]
    /* All implementations of IDialog are defined with a type for the type of value they return when complete. */
    public class NameDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("What is your name?");

            context.Wait(this.MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            /* Have logic in your dialog to parse the response to see if it's a valid result for the dialog 
                to return. What if the user replied with an image attached? */
            if (message.Text.Trim().Length > 0)
            {
                /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling
                    dialog. */
                context.Done(message.Text);
            }
            else
            {
                /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                    parent/calling dialog. */
                context.Fail(new Exception("Message was not a string or was an empty string."));
            }
        }
    }
}