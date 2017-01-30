namespace Assets.BotDirectLine
{
    using System;
    using System.Collections.Generic;

    public enum EventTypes
    {
        None,
        ConversationStarted,
        MessageSent,
        MessageReceived, // Can be 1 or more messages
        Error
    }

    public class BotResponseEventArgs : EventArgs
    {
        public BotResponseEventArgs()
        {
            this.EventType = EventTypes.None;
            this.Messages = new List<MessageActivity>();
        }

        public EventTypes EventType
        {
            get;
            set;
        }
        
        public string SentMessageId
        {
            get;
            set;
        }

        public string ConversationId
        {
            get;
            set;
        }

        /// <summary>
        /// Can contain e.g. an error code.
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Not an actual message but e.g. an error message.
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        public string Watermark
        {
            get;
            set;
        }

        public IList<MessageActivity> Messages
        {
            get;
            private set;
        }

        public override string ToString()
        {
            string retval = "[Event type: " + this.EventType;

            if (!string.IsNullOrEmpty(this.SentMessageId))
            {
                retval += "; ID of message sent: " + this.SentMessageId;
            }

            if (!string.IsNullOrEmpty(this.Code))
            {
                retval += "; Code: " + this.Code;
            }

            if (!string.IsNullOrEmpty(this.Message))
            {
                retval += "; Message: " + this.Message;
            }

            if (!string.IsNullOrEmpty(this.ConversationId))
            {
                retval += "; Conversation ID: " + this.ConversationId;
            }

            if (!string.IsNullOrEmpty(this.Watermark))
            {
                retval += "; Watermark: " + this.Watermark;
            }

            if (this.Messages.Count > 0)
            {
                retval += "; Messages: ";

                for (int i = 0; i < this.Messages.Count; ++i)
                {
                    retval += "\"" + this.Messages[i] + "\"";

                    if (i < this.Messages.Count - 1)
                    {
                        retval += ", ";
                    }
                }
            }

            retval += "]";
            return retval;
        }
    }
}
