﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using MessageRouting;
using IntermediatorBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;

namespace IntermediatorBotSample
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public MessagesController()
        {
            // Note: This class is constructed every time there is a new activity (Post called).
        }

        /// <summary>
        /// Handles the received message.
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                // Get the message router manager instance and let it handle the activity
                MessageRouterResult result = await MessageRouterManager.Instance.HandleActivityAsync(activity, false);

                if (result.Type == MessageRouterResultType.NoActionTaken)
                {
                    // No action was taken by the message router manager. This means that the user
                    // is not engaged in a 1:1 conversation with a human (e.g. customer service
                    // agent) yet.
                    //
                    // You can, for example, check if the user (customer) needs human assistance
                    // here or forward the activity to a dialog. You could also do the check in
                    // the dialog too...
                    //
                    // Here's an example:
                    if (!string.IsNullOrEmpty(activity.Text) && activity.Text.ToLower().Contains("human"))
                    {
                        await MessageRouterManager.Instance.InitiateEngagementAsync(activity);
                    }
                    else
                    {
                        await Conversation.SendAsync(activity, () => new RootDialog());
                    }
                }
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
        {
            MessageRouterManager messageRouterManager = MessageRouterManager.Instance;

            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
                Party senderParty = MessagingUtils.CreateSenderParty(message);

                if (await messageRouterManager.RemovePartyAsync(senderParty))
                {
                    return message.CreateReply($"Data of user {senderParty.ChannelAccount?.Name} removed");
                }
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                if (message.MembersRemoved != null && message.MembersRemoved.Count > 0)
                {
                    foreach (ChannelAccount channelAccount in message.MembersRemoved)
                    {
                        Party party = new Party(
                            message.ServiceUrl, message.ChannelId, channelAccount, message.Conversation);

                        if (await messageRouterManager.RemovePartyAsync(party))
                        {
                            System.Diagnostics.Debug.WriteLine($"Party {party.ToString()} removed");
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}