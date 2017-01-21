using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Microsoft.Bot.Sample.SimpleAlarmBot.Telemetry
{
    /// <summary>
    /// Helper class to log events to AppInsights.
    /// </summary>
    public static class TelemetryLogger
    {
        // TODO: talk with Mor, can we use PascalCase for Names? (like AppInsights uses out of the box?)
        private const string _messageReceived = "message.received";
        private const string _messageSent = "message.send";
        private const string _luisIntentReceived = "message.intent.received";
        private const string _luisIntentDialog = "message.intent.dialog"; // TODO: talk with Mor about this one??
        private const string _messageSentiment = "message.sentiment";
        private const string _messageConvertStarted = "message.convert.start";
        private const string _messageConvertEnded = "message.convert.end";
        private const string _otherActivity = "message.other";
        private static readonly string _textAnalyticsMinLength = ConfigurationManager.AppSettings["TextAnalyticsMinLenght"];
        private static readonly string _textAnalyticsApiKey = ConfigurationManager.AppSettings["TextAnalyticsApiKey"];

        public static TelemetryClient TelemetryClient { get; } = new TelemetryClient();

        /// <summary>
        /// Logs an IActivity to AppInishgts.
        /// </summary>
        public static async Task TrackActivity(IActivity activity, IBotData botData)
        {
            var et = BuildEventTelemetry(activity);
#if DEBUG
            if (botData != null)
            {
                await botData.LoadAsync(CancellationToken.None);
                et.Properties.Add("debugConversationData", JsonConvert.SerializeObject(botData.ConversationData));
                et.Properties.Add("debugPrivateConversationData", JsonConvert.SerializeObject(botData.PrivateConversationData));
                et.Properties.Add("debugUserData", JsonConvert.SerializeObject(botData.UserData));
                et.Properties.Add("debugActivity", JsonConvert.SerializeObject(activity));
            }
#endif
            TelemetryClient.TrackEvent(et);

            // Track sentiment only for incoming messages. 
            if (et.Name == _messageReceived)
            {
                await TrackMessageSentiment(activity);
            }
        }

        /// <summary>
        /// Helper method to track the sentiment of incoming messages.
        /// </summary>
        /// <param name="activity"></param>
        private static async Task TrackMessageSentiment(IActivity activity)
        {
            var text = activity.AsMessageActivity().Text;
            var numWords = text.Split(' ').Length;
            if (numWords >= int.Parse(_textAnalyticsMinLength) && _textAnalyticsApiKey != string.Empty)
            {
                var properties = new Dictionary<string, string>
                {
                    {"score", (await GetSentimentScore(text)).ToString(CultureInfo.InvariantCulture)}
                };

                var et = BuildEventTelemetry(activity, properties);
                et.Name = _messageSentiment;
                TelemetryClient.TrackEvent(et);
            }
        }

        /// <summary>
        /// Logs a LUIS intent to AppInisghts.
        /// </summary>
        public static void TrackLuisIntent(IActivity activity, LuisResult result)
        {
            try
            {
                var properties = new Dictionary<string, string>
                {
                    {"intent", result.Intents[0].Intent},
                    {"score", result.Intents[0].Score.ToString()},
                    {"entities", JsonConvert.SerializeObject(result.Entities)} // TODO: test this
                    // TODO: where do I get the errors from like Mor?
                };

                var eventTelemetry = BuildEventTelemetry(activity, properties);
                eventTelemetry.Name = _luisIntentReceived;
                TelemetryClient.TrackEvent(eventTelemetry);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw;
            }
        }

        private static EventTelemetry BuildEventTelemetry(IActivity activity, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
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
                    if (activity.ReplyToId == null)
                    {
                        et.Name = _messageReceived;
                        et.Properties.Add("userId", activity.From.Id);
                        et.Properties.Add("userName", activity.From.Name);
                    }
                    else
                    {
                        et.Name = _messageSent;
                    }
                    et.Properties.Add("text", messageActivity.Text);
                    et.Properties.Add("conversationId", messageActivity.Conversation.Id);
                    break;
                case ActivityTypes.ConversationUpdate:
                    et.Name = _messageConvertStarted;
                    break;
                case ActivityTypes.EndOfConversation:
                    et.Name = _messageConvertEnded;
                    break;
                default:
                    et.Name = _otherActivity;
                    break;
            }

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    et.Properties.Add(property);
                }
            }

            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    et.Metrics.Add(metric);
                }
            }

            return et;
        }

        private static string GetDateTimeAsIso8601(DateTime activity)
        {
            var s = JsonConvert.SerializeObject(activity.ToUniversalTime());
            return s.Substring(1, s.Length - 2);
        }

        private static async Task<double> GetSentimentScore(string message)
        {
            List<DocumentInput> docs = new List<DocumentInput>
            {
                new DocumentInput {Id = 1, Text = message}
            };
            BatchInput sentimentInput = new BatchInput {Documents = docs};
            var jsonSentimentInput = JsonConvert.SerializeObject(sentimentInput);
            var sentimentInfo = await GetSentiment(_textAnalyticsApiKey, jsonSentimentInput);
            var sentimentScore = sentimentInfo.Documents[0].Score;
            return sentimentScore;
        }

        private static async Task<BatchResult> GetSentiment(string apiKey, string jsonSentimentInput)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://westus.api.cognitive.microsoft.com/");

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                byte[] byteData = Encoding.UTF8.GetBytes(jsonSentimentInput);

                var uri = "text/analytics/v2.0/sentiment";
                var sentimentRawResponse = await CallEndpoint(client, uri, byteData);

                var sentimentJsonResponse = JsonConvert.DeserializeObject<BatchResult>(sentimentRawResponse);
                return sentimentJsonResponse;
            }
        }

        private static async Task<string> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}