using System;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Microsoft.Bot.Sample.SimpleAlarmBot.Telemetry
{
    public static class TelemetryLogger
    {
        private const string _messageReceived = "message.received";
        private const string _messageSent = "message.send";
        private const string _messageConversationStarted = "message.convert.start";
        private const string _messageConversationEnded = "message.convert.end";
        private const string _intentDialog = "message.intent.dialog";
        private const string _messageSentiment = "message.sentiment";
        private const string _otherActivity = "message.other";
        public static TelemetryClient TelemetryClient { get; } = new TelemetryClient();

        public static void TrackActivity(IActivity activity)
        {
            var et = BuildEventTelemetry(activity);
            TelemetryClient.TrackEvent(et);
        }

        private static EventTelemetry BuildEventTelemetry(IActivity activity)
        {
            var et = new EventTelemetry();
            // TODO: check with Mor if we really need timestamp becaue it is already in the message
            if (activity.Timestamp != null) et.Properties.Add("timestamp", GetDateTimeAsIso8601(activity.Timestamp.Value));
            et.Properties.Add("type", activity.Type);
            // TODO: Mor seems to be logging channel for intents, not for messages.
            et.Properties.Add("channel", activity.ChannelId);

            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    var messageActivity = activity.AsMessageActivity();
                    et.Name = activity.ReplyToId == null ? _messageReceived : _messageSent;
                    et.Properties.Add("text", messageActivity.Text);
                    et.Properties.Add("conversationId", messageActivity.Conversation.Id);
                    break;
                case ActivityTypes.ConversationUpdate:
                    et.Name = _messageConversationStarted;
                    break;
                case ActivityTypes.EndOfConversation:
                    et.Name = _messageConversationEnded;
                    break;
                default:
                    et.Name = _otherActivity;
                    break;
            }
            return et;
        }

        private static string GetDateTimeAsIso8601(DateTime activity)
        {
            var s = JsonConvert.SerializeObject(activity.ToUniversalTime());
            return s.Substring(1, s.Length - 2);
        }
    }
}