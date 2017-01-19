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
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await this.SendWelcomeMessageAsync(context);
        }

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

                context.Call(new CityDialog(this.name), this.CityDialogResumeAfter);
            }
            catch (Exception)
            {
                await context.PostAsync("I'm sorry that I don't understand your reply. Let's try again.");

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