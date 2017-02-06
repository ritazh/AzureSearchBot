using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using UcmaBotDtmf.Common;
using UcmaBotDtmf.Models;

namespace UcmaBotDtmf.Helpers
{
    public delegate void MessageReceivedHandler(DirectlineMessage message);
    public class DirectlineEndpoint
    {
        private static string directLineSecret = MyAppSettings.BotLanguage.Secret;    
        private DirectLineClient client;
        private Conversation conversation;
        private MessageReceivedHandler _callback;
        private bool IsListenerStarted;
        public DirectlineEndpoint(MessageReceivedHandler callback)
        {
             client = new DirectLineClient(directLineSecret);

            _callback = callback;


        }

        public string NewConversation()
        {
            conversation = client.Conversations.StartConversation();

            Listen(conversation.ConversationId);

            return conversation.ConversationId;
        }

        public void SendActivity(string message, string conversationid)
        {
            Console.WriteLine($"Directline input: {message}");
            Activity activity = new Activity();
            activity.ChannelId = "directline";
            activity.Type = ActivityTypes.Message;
            activity.From = new ChannelAccount(id: ConfigurationSettings.AppSettings["botUser1"]);
            activity.Recipient = new ChannelAccount(id: ConfigurationSettings.AppSettings["botUser2"]);
            activity.Conversation = new ConversationAccount(id: conversationid);
            activity.Text = message;
            var output = client.Conversations.PostActivityWithHttpMessagesAsync(conversationid, activity).Result;

            
        }

        public   void Listen(string conversationid)
        {
            IsListenerStarted = true;
            new System.Threading.Thread(async () => await ReadBotMessagesAsync(client, conversationid)).Start();
        }
        public void Stop()
        {
            IsListenerStarted = false;
        }
        private  async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId)
        {
            string watermark = null;
            
            while (IsListenerStarted)
            {
                var messages = await client.Conversations.GetActivitiesAsync(conversationId, watermark);

                watermark = messages?.Watermark;

                var messagesFromBotText = from x in messages.Activities                                        
                                          select x;

                foreach (Activity message in messagesFromBotText)
                {

                    if (!string.IsNullOrEmpty(message.ReplyToId))
                    {

                        try
                        {

                            if (message.ChannelData == null)
                            {
                                DirectlineMessage botMessage = new DirectlineMessage()
                                {
                                    InputFormat = InputFormat.None,
                                    Prompt = $"Channel Data is null for prompt {message.Text}",
                                    Length = 0,
                                    PromptType = PromptType.EndDialog,

                                };
                                _callback(botMessage);

                            }
                            else
                            {

                                var dmessage = JsonConvert.DeserializeObject<DirectlineMessage>(message.ChannelData.ToString());

                                Console.WriteLine($"Directline output: {message.Text}");

                                _callback(dmessage);
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                    }

               

                   
                }

                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }

        private static void RenderHeroCard(DirectLineChannelData channelData)
        {
            
            const int Width = 70;
            Func<string, string> contentLine = (content) => string.Format($"{{0, -{Width}}}", string.Format("{0," + ((Width + content.Length) / 2).ToString() + "}", content));

            Console.WriteLine("/{0}", new string('*', Width + 1));
            Console.WriteLine("*{0}*", contentLine(channelData.Content.Title));
            Console.WriteLine("*{0}*", new string(' ', Width));
            Console.WriteLine("*{0}*", contentLine(channelData.Content.Text));
            Console.WriteLine("{0}/", new string('*', Width + 1));
        }
    }
}
