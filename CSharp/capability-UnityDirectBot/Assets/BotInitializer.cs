using System.Linq;
using Assets.BotDirectLine;
using UnityEngine;
using UnityEngine.UI;

public class BotInitializer : MonoBehaviour
{
    public Text BotConsoleText;
    public Text UserConsoleText;

    private const string UserToBotMessage = "Hello bot!";
    private ConversationState conversationState = new ConversationState();

    // Use this for initialization
    public void Start()
    {
        BotDirectLineManager.Initialize("YourDirectLineToken");
        BotDirectLineManager.Instance.BotResponse += this.OnBotResponse;

        if (this.conversationState.ConversationId == null)
        {
            this.StartCoroutine(BotDirectLineManager.Instance.StartConversationCoroutine());
        }
    }

    private void OnBotResponse(object sender, Assets.BotDirectLine.BotResponseEventArgs e)
    {
        Debug.Log("OnBotResponse: " + e.ToString());

        switch (e.EventType)
        {
            case EventTypes.ConversationStarted:
                // Store the ID
                this.conversationState.ConversationId = e.ConversationId;
                this.StartCoroutine(BotDirectLineManager.Instance.SendMessageCoroutine(this.conversationState.ConversationId, "UnityUserId", UserToBotMessage, "Unity User 1"));

                break;
            case EventTypes.MessageSent:
                if (!string.IsNullOrEmpty(this.conversationState.ConversationId))
                {
                    this.conversationState.PreviouslySentMessageId = e.SentMessageId;
                    if (this.UserConsoleText != null)
                    {
                        this.UserConsoleText.text = string.Format("You said: {0}!", UserToBotMessage);
                    }

                    // Get the bot's response(s)
                    this.StartCoroutine(BotDirectLineManager.Instance.GetMessagesCoroutine(this.conversationState.ConversationId));
                }

                break;
            case EventTypes.MessageReceived:
                // Handle the received message(s)
                if (this.BotConsoleText != null)
                {
                    var message = e.Messages.FirstOrDefault(x => x.ReplyToId == this.conversationState.PreviouslySentMessageId);

                    if (message != null) 
                    {
                        this.BotConsoleText.text = string.Format("Bot said: {0}", message.Text);
                    }
                }

                break;
            case EventTypes.Error:
                // Handle the error
                break;
        }
    }
}
