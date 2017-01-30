# Embedding bots into apps - Unity

This sample show how bot can go beyond by becoming embedded into larger applications.

### Prerequisites
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* Unity 3d. You can download it from [here](https://store.unity.com/es). Personal edition is free.
* Register your bot with the Microsoft Bot Framework. Please refer to [this](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#registering) for the instructions.
* Enable the Direct Line channel, edit the settings  ![DirectLine Channel](images/chatwidget-directline-channel.png) and add a new site to get the Direct Line Embed code.
![DirectLine Token](images/chatwidget-directline-token.png)

Refer to [this](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#channels) for more information on how to configure channels. 
* Update the 'Start' source of the [BotInitializer.cs](Assets/BotInitializer.cs) with the embed code.

````C#
public void Start()
{
	BotDirectLineManager.Initialize("YourDirectLineToken");
	BotDirectLineManager.Instance.BotResponse += this.OnBotResponse;

	if (this.conversationState.ConversationId == null)
	{
		this.StartCoroutine(BotDirectLineManager.Instance.StartConversationCoroutine());
	}
}
````

### Code Highlights
The Direct Line API is a simple REST API for connecting directly to a single bot. This API is intended for developers writing their own client applications, web chat controls, mobile apps, or service-to-service applications that will talk to their bot.
Within the Direct Line API, you will find:
* An authentication mechanism using standard secret/token patterns
* The ability to send messages from your client to your bot via an HTTP POST message
* The ability to receive messages by WebSocket stream, if you choose
* The ability to receive messages by polling HTTP GET, if you choose
* A stable schema, even if your bot changes its protocol version

#### Authentication: Secrets and Tokens
Direct Line allows you to authenticate all calls with either a secret (retrieved from the Direct Line channel configuration page) or a token (which you may get at runtime by converting your secret).

A Direct Line secret is a master key that can access any conversation, and create tokens. Secrets do not expire.

A Direct Line token is a key for a single conversation. It expires but can be refreshed.

If you're writing a service-to-service application, using the secret may be simplest. If you're writing an application where the client runs in a web browser or mobile app, you may want to exchange your secret for a token, which only works for a single conversation and will expire unless refreshed. You choose which security model works best for you.
Your secret or token is communicated in the Authorization header of every call, with the Bearer scheme. Example below.

Your secret or token is communicated in the Authorization header of every call, with the Bearer scheme. Example below.

````
-- connect to directline.botframework.com --
POST /v3/directline/conversations/abc123/activities HTTP/1.1
Authorization: Bearer RCurR_XV9ZA.cwA.BKA.iaJrC8xpy8qbOF5xnR2vtCX7CZj0LdjAPGfiCpg4Fv0
[other HTTP headers, omitted]
````

#### Starting a conversation
Clients begin by explicitly starting a conversation. If successful, the Direct Line service replies with a JSON object containing a conversation ID, a token, and a WebSocket URL that may be used later.

````
-- connect to directline.botframework.com --
POST /v3/directline/conversations HTTP/1.1
Authorization: Bearer RCurR_XV9ZA.cwA.BKA.iaJrC8xpy8qbOF5xnR2vtCX7CZj0LdjAPGfiCpg4Fv0y8qbOF5xPGfiCpg4Fv0y8qqbOF5x8qbOF5xn
[other headers]

-- response from directline.botframework.com --
HTTP/1.1 201 Created
[other headers]

{
  "conversationId": "abc123",
  "token": "RCurR_XV9ZA.cwA.BKA.iaJrC8xpy8qbOF5xnR2vtCX7CZj0LdjAPGfiCpg4Fv0y8qbOF5xPGfiCpg4Fv0y8qqbOF5x8qbOF5xn",
  "expires_in": 1800,
  "streamUrl": "https://directline.botframework.com/v3/directline/conversations/abc123/stream?t=RCurR_XV9ZA.cwA..."
}
````

If the conversation was started, an HTTP 201 status code is returned. HTTP 201 is the code that clients will receive under most circumstances, as the typical use case is for a client to start a new conversation. Under certain conditions -- specifically, when the client has a token scoped to a single conversation AND when that conversation was started with a prior call to this URL -- this method will return HTTP 200 to signify the request was acceptable but that no conversation was created (as it already existed).

You have 60 seconds to connect to the WebSocket URL. If the connection cannot be established during this time, use the reconnect method below to generate a new stream URL.

This call is similar to /v3/directline/tokens/generate. The difference is that the call to /v3/directline/conversations starts the conversation, contacts the bot, and creates a streaming WebSocket URL, none of which occur when generating a token.

* Call /v3/directline/conversations if you will distribute the token to client(s) and want them to initiate the conversation.
* Call /v3/directline/conversations if you intend to start the conversation immediately.


#### Sending an Activity to the bot
Using the Direct Line 3.0 protocol, clients and bots may exchange many different Bot Framework v3 Activites, including Message Activities, Typing Activities, and custom activities that the bot supports.

To send any one of these activities to the bot,

* the client formulates the Activity according to the Activity schema (see below)
* the client issues a POST message to /v3/directline/conversations/{id}/activities
* the service returns when the activity was delivered to the bot, with an HTTP status code reflecting the bot's status code. If the POST was successful, the service returns a JSON payload containing the ID of the Activity that was sent.

Example follows.
````
-- connect to directline.botframework.com --
POST /v3/directline/conversations/abc123/activities HTTP/1.1
Authorization: Bearer RCurR_XV9ZA.cwA.BKA.iaJrC8xpy8qbOF5xnR2vtCX7CZj0LdjAPGfiCpg4Fv0
[other headers]

{
  "type": "message",
  "from": {
    "id": "user1"
  },
  "text": "hello"
}

-- response from directline.botframework.com --
HTTP/1.1 200 OK
[other headers]

{
  "id": "0001"
}
````

#### Receiving Activities from the bot
The GET mechanism is useful for clients who are unable to use the WebSocket, or for clients wishing to retrieve the conversation history.

To retrieve messages, issue a GET call to the conversation endpoint. Optionally supply a watermark, indicating the most recent message seen. The watermark field accompanies all GET/WebSocket messages as a property in the ActivitySet.

````
-- connect to directline.botframework.com --
GET /v3/directline/conversations/abc123/activities?watermark=0001a-94 HTTP/1.1
Authorization: Bearer RCurR_XV9ZA.cwA.BKA.iaJrC8xpy8qbOF5xnR2vtCX7CZj0LdjAPGfiCpg4Fv0
[other headers]

-- response from directline.botframework.com --
HTTP/1.1 200 OK
[other headers]

{
  "activities": [{
    "type": "message",
    "channelId": "directline",
    "conversation": {
      "id": "abc123"
    },
    "id": "abc123|0000",
    "from": {
      "id": "user1"
    },
    "text": "hello"
  }, {
    "type": "message",
    "channelId": "directline",
    "conversation": {
      "id": "abc123"
    },
    "id": "abc123|0001",
    "from": {
      "id": "bot1"
    },
    "text": "Nice to see you, user1!"
  }],
  "watermark": "0001a-95"
}
````
Clients should page through the available activities by advancing the watermark value until no activities are returned.

#### Inside the Unity code 
When the BotInitializer component is called, then conversation with the bot is started. This send a POST request to the DirectLine and the response returns the conversationId used to send and receive messages.

When the ConversationStarted event is attended, the message **"Hello bot!"** is sent from the Unity User to the bot. This send a POST request to the DirectLine with the message. Following the BotInitializer retrieve the message sending a GET request with the conversationId. The response contains all the history of messages in the conversation if no watermark is passed in the request.

### Outcome
You will see the following result if you open the project in Unity 3d and run it. 

![Outcome](images/outcome-directline-unity.png)

### More Information
To get more information about how to get started with the embeddable web chat control for the Microsoft Bot Framework and Xamarin's WebView please review the following resources:

* [Bot Connector - Direct Line API - v3.0](https://docs.botframework.com/en-us/restapi/directline3/)
* [Registering your Bot with the Microsoft Bot Framework](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#registering)
* [Unity 3d](https://unity3d.com/es)
* [SimpleJSON](http://wiki.unity3d.com/index.php/SimpleJSON)
