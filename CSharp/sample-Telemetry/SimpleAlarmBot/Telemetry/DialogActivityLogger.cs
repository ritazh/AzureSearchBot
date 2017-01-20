using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.SimpleAlarmBot.Telemetry
{
    /// <summary>
    /// A generic logger for Dialog activities. 
    /// </summary>
    public class DialogActivityLogger : IActivityLogger
    {
        private readonly IBotData _botData;

        public DialogActivityLogger(IBotData botData)
        {
            _botData = botData;
        }

        public async Task LogAsync(IActivity activity)
        {
            // Questions
            // IDialogContext?
            //    ctx.ConversationData?
            //    ctx.PrivateConversationData?
            //    ctx.UserData?
            // Luis?
            // Forms?
            //StateClient stateClient = activity.GetStateClient();
            await _botData.LoadAsync(CancellationToken.None);
            var p = _botData.ConversationData;
            TelemetryLogger.TrackActivity(activity);
        }
    }
}