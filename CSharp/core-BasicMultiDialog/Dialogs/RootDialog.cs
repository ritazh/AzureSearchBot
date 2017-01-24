namespace ConversationFlow.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootDialog : IDialog<object>
    {

        private string name;
        private int age;

        /* When a dialog becomes the active dialog on the stack, StartAsync is called and passed the dialog 
         *  context (used to manage the conversation). */
        public async Task StartAsync(IDialogContext context)
        {
            /* Your bot is always in a state of waiting for a message from the user, either the first 
             *  message sent to your bot at the start of the conversation or a message in reply to a 
             *  prompt you sent the user.
             *  
             *  IDialogContext.Wait() makes your bot wait for a message from the user and calls the resume 
             *  method when that message is received. */

            /* Wait until the first message is received from the conversation and call MessageReceviedAsync 
             *  to process that message. */
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */

            /* Await the message passed to the conversion to start the dialog flow. */
            var message = await result;

            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            /* IDialogContext.PostAsync posts a message to the user in the conversation. */

            /* Post a message to the conversation via the dialog context. */
            await context.PostAsync("Hi, I'm the Basic Multi Dialog bot. Let's get started.");

            /* IDialogContext.Call() calls the dialog passed as a paramter and adds it to the top of the stack. Messages 
                sent from the user will be received by the dialog at the top of the stack until it is removed
                from the stack (below). The method passed as the resume parameter is called when the dialog is removed from
                the stack. */

            /*
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
                context.Call(new AgeDialog(this.name), this.AgeDialogResumeAfter);
            }
            catch (Exception)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");

                /* */
                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task AgeDialogResumeAfter(IDialogContext context, IAwaitable<int> result)
        {
            try
            {
                this.age = await result;

                await context.PostAsync($"Your name is { name } and your age is { age }.");

            }
            catch (Exception)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
            }
            finally
            {
                await this.SendWelcomeMessageAsync(context);
            }
        }
    }
}