using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace GlobalMessageHandlersBot.Dialogs
{
    public class CancelScorable : IScorable<double>
    {
        private readonly IDialogStack stack;

        public CancelScorable(IDialogStack stack)
        {
            SetField.NotNull(out this.stack, nameof(stack), stack);
        }

        public async Task<object> PrepareAsync<Item>(Item item, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (message.Text.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text;
                }
            }

            return null;
        }

        public bool TryScore(object state, out double score)
        {
            bool matched = state != null;
            score = matched ? 1.0 : double.NaN;
            return matched;
        }

        public async Task PostAsync<Item>(Item item, object state, CancellationToken token)
        {
            this.stack.Reset();
        }
    }
}