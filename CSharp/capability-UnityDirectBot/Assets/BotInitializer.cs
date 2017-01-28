using Assets.BotDirectLine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BotInitializer : MonoBehaviour
{
    public Text ConsoleText;

    private ConversationState _conversationState = new ConversationState();

    // Use this for initialization
    void Start () {
        BotDirectLineManager.Initialize("wUTenxJ6CoU.cwA.QA4.SbNWwPq0RuW02uVqO6lExf7mJr4_jh_VA7Ed78AgMrg");
        BotDirectLineManager.Instance.BotResponse += OnBotResponse;

        if (_conversationState.ConversationId == null)
        {
            StartCoroutine(BotDirectLineManager.Instance.StartConversationCoroutine());
        }
    }

	// Update is called once per frame
	void Update () {
        if (_conversationState.ConversationId != null && string.IsNullOrEmpty(_conversationState.PreviouslySentMessageId))
        {
            _conversationState.PreviouslySentMessageId = "ddd";

            StartCoroutine(BotDirectLineManager.Instance.SendMessageCoroutine(_conversationState.ConversationId, "UnityUserId", "Hello bot!", "Unity User 1"));
        }
	}

    private void OnBotResponse(object sender, Assets.BotDirectLine.BotResponseEventArgs e)
    {
        Debug.Log("OnBotResponse: " + e.ToString());

        switch (e.EventType)
        {
            case EventTypes.ConversationStarted:
                // Store the ID
                _conversationState.ConversationId = e.ConversationId;
                break;
            case EventTypes.MessageSent:
                if (!string.IsNullOrEmpty(_conversationState.ConversationId))
                {
                    // Get the bot's response(s)
                    StartCoroutine(BotDirectLineManager.Instance.GetMessagesCoroutine(_conversationState.ConversationId));
                }

                break;
            case EventTypes.MessageReceived:
                ConsoleText.text = string.Join(" -- ", e.Messages.Select(x => x.Text).ToArray());
                _conversationState.PreviouslySentMessageId = string.Empty;
                // Handle the received message(s)
                break;
            case EventTypes.Error:
                // Handle the error
                break;
        }
    }
}
