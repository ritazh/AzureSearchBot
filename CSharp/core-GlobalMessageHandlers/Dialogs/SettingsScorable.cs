using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Scorables.Internals;

namespace GlobalMessageHandlersBot.Dialogs
{
    //
    // Summary:
    //     Allow the scoring of items, with external comparison of scores, and enable the
    //     winner to take some action.
    //
    // Remarks:
    //     We avoided the traditional "bool TryScore(Item item, object state, out Score
    //     score)" pattern to allow for Score generic type parameter covariance.
    public class SettingsScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogStack stack;

        /* Construct w/ IDialogStack, registerd by the BotF, resolved at runtime. */
        public SettingsScorable(IDialogStack stack)
        {
            SetField.NotNull(out this.stack, nameof(stack), stack);
        }

        // Summary:
        //     Perform some asynchronous work to analyze the item and produce some opaque state.
        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken token)
        {
            var message = activity as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (message.Text.Equals("settings", StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text;
                }
            }

            return null;
        }
        //
        // Summary:
        //     Returns whether this scorable wants to participate in scoring this item.
        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }
        //
        // Summary:
        //     Gets the score for this item.
        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }
        //
        // Summary:
        //     If this scorable wins, this method is called.
        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null)
            {
                var settingsDialog = new SettingsDialog();

                var interruption = settingsDialog.Void<object, IMessageActivity>();

                this.stack.Call(interruption, null);

                await this.stack.PollAsync(token);
            }
        }
        //
        // Summary:
        //     The scoring process has completed - dispose of any scoped resources.
        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}