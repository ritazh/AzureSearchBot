namespace ConversationFlow.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System.Collections.Generic;

    [Serializable]
    public class RootDialog : IDialog<object>
    {

        private string name;
        private string city;

        public async Task StartAsync(IDialogContext context)
        {
            /* Your bot is always in a state of waiting for a message from the user, either the first 
             *  message sent to your bot at the start of the conversation or a message in reply to a 
             *  prompt you sent the user.
             *  
             *  IDialogContext.Wait() makes your bot wait for a message from the user and calls the resume 
             *  method when that message is received. */
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When the resume method is called, it's passed a IAwaitable<IMessageActivity>, so to get the message
                you have to await the result. */
            var message = await result;

            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            /* IDialogContext.PostAsync posts a message to the user in the conversation. */
            await context.PostAsync("Hi, I'm the ConversationFlow bot. Let's get started.");

            /* IDialogContext.Call() calls the dialog passed as a paramter and adds it to the top of the stack. Messages 
                sent from the user will be received by the dialog at the top of the stack until it is removed
                from the stack (below). The method passed as the resume parameter is called when the dialog is removed from
                the stack. */
            context.Call(new NameDialog(), this.NameDialogResumeAfter);
        }

        private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {

                this.name = await result;

                /* CityDialog has constructor that accepts a name to show how you can pass parameters to initialize
                    a dialog. Create reusable dialogs that accept parameters to change their behavior 
                    (prompts, etc.) */
                context.Call(new CityDialog(this.name), this.CityDialogResumeAfter);
            }
            catch (Exception)
            {
                await context.PostAsync("I'm sorry that I don't understand your reply. Let's try again.");

                /* */
                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task CityDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.city = await result;

                await context.PostAsync($"Your name is { name } and you are from { city }.");

            }
            catch (Exception)
            {
                await context.PostAsync("I'm sorry, I don't understand your reply. Let's try again.");

            }
            finally
            {
                await this.SendWelcomeMessageAsync(context);
            }
        }
    }
}