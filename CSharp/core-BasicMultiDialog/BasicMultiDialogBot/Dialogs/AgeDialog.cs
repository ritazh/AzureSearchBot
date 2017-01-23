using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace ConversationFlow.Dialogs
{
    [Serializable]
    public class AgeDialog : IDialog<int>
    {
        private string name;

        /* Constructor to initialize the dialog. Use to create reusable dialogs and pass in prompts, 
            messages when a reply isn't understood, etc. */
        public AgeDialog(string name)
        {
            this.name = name;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"{ this.name }, what is your age?");

            context.Wait(this.MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            int age;

            if (Int32.TryParse(message.Text, out age) && (age > 0))
            {
                context.Done(age);
            }
            else
            {
                context.Fail(new Exception("Message was not a valid age."));
            }
        }
    }
}