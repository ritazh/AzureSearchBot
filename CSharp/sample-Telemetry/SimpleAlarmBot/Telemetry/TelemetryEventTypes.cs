using System;

namespace Microsoft.Bot.Sample.SimpleAlarmBot.Telemetry
{
    /// <summary>
    /// Define event type names used by TelemtryLogger.
    /// </summary>
    public static class TelemetryEventTypes
    {
        // TODO: talk with Mor, can we use PascalCase for Names? (like AppInsights uses out of the box?)
        public const string MessageReceived = "message.received";
        public const string MessageSent = "message.send";
        public const string LuisIntentReceived = "message.intent.received";
        public const string LuisIntentDialog = "message.intent.dialog"; // TODO: talk with Mor about this one??
        public const string MessageSentiment = "message.sentiment";
        public const string ConvertionStarted = "message.convert.start";
        public const string ConvertionEnded = "message.convert.end";
        public const string OtherActivity = "message.other";
        public const string ConversationUpdate = "message.conversation.updated";
        public const string ConversationEnded = "message.conversation.ended";
    }
}