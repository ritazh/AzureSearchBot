namespace Assets.BotDirectLine
{
    using System;

    public class MessageActivity
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageActivity()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fromId"></param>
        /// <param name="text"></param>
        /// <param name="channelId"></param>
        /// <param name="timestampString"></param>
        /// <param name="fromName"></param>
        /// <param name="conversationId"></param>
        /// <param name="replyToId"></param>
        public MessageActivity(string fromId, string text, string channelId, string timestampString = null, string fromName = null, string conversationId = null, string replyToId = null)
        {
            if (string.IsNullOrEmpty(timestampString))
            {
                this.Timestamp = DateTime.Now;
            }
            else
            {
                this.Timestamp = Convert.ToDateTime(timestampString);
            }

            this.ChannelId = channelId;
            this.FromId = fromId;
            this.FromName = fromName;
            this.ConversationId = conversationId;
            this.Text = text;
            this.ReplyToId = replyToId;
        }

        public DateTime Timestamp
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public string ChannelId
        {
            get;
            set;
        }

        public string FromId
        {
            get;
            set;
        }

        public string FromName
        {
            get;
            set;
        }

        public string ConversationId
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public string ReplyToId
        {
            get;
            set;
        }

        public static MessageActivity FromJson(SimpleJSON.JSONNode activityJsonRootNode)
        {
            MessageActivity messageActivity = new MessageActivity();
            messageActivity.Id = activityJsonRootNode[BotJsonProtocol.KeyId];
            messageActivity.Timestamp = Convert.ToDateTime(activityJsonRootNode[BotJsonProtocol.KeyTimestamp]);
            messageActivity.ChannelId = activityJsonRootNode[BotJsonProtocol.KeyChannelId];

            SimpleJSON.JSONNode fromJsonRootNode = activityJsonRootNode[BotJsonProtocol.KeyFrom];

            if (fromJsonRootNode != null)
            {
                messageActivity.FromId = fromJsonRootNode[BotJsonProtocol.KeyId];
                messageActivity.FromName = fromJsonRootNode[BotJsonProtocol.KeyName];
            }

            SimpleJSON.JSONNode conversationJsonRootNode = activityJsonRootNode[BotJsonProtocol.KeyConversation];

            if (conversationJsonRootNode != null)
            {
                messageActivity.ConversationId = fromJsonRootNode[BotJsonProtocol.KeyId];
            }

            messageActivity.Text = activityJsonRootNode[BotJsonProtocol.KeyText];
            messageActivity.ReplyToId = activityJsonRootNode[BotJsonProtocol.KeyReplyToId];

            return messageActivity;
        }

        public string ToJsonString()
        {
            string asJsonString =
                "{ \"" + BotJsonProtocol.KeyActivityType + "\": \"" + BotJsonProtocol.KeyMessage + "\", \""
                + BotJsonProtocol.KeyChannelId + "\": \"" + this.ChannelId + "\", \""
                + BotJsonProtocol.KeyFrom + "\": { \""
                    + BotJsonProtocol.KeyId + "\": \"" + this.FromId
                    + (string.IsNullOrEmpty(this.FromName) ? string.Empty : ("\", \"" + BotJsonProtocol.KeyName + "\": \"" + this.FromName))
                + "\" }, \""
                + BotJsonProtocol.KeyText + "\": \"" + this.Text + "\" }";

            return asJsonString;
        }

        public override string ToString()
        {
            return this.ToJsonString();
        }
    }
}
