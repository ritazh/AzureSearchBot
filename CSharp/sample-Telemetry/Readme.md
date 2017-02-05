# Telemetry Bot Sample (C#)

This sample  shows how to use log custom telemetry on a bot. The sample writes custom events to Azure Application Insights than can then be queried from PowerBI or other reporting tools to create dashboards.

PowerBI dashboard example  
![PowerBI Dashboard](Solution%20Items\Docs\Images\PowerBIDashboard.jpg)

Custom dashboard example  
![Custom Dashboard](Solution%20Items\Docs\Images\CustomDashboard.jpg)

## Prerequisites
The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* Azure Application Insights Service account (for more information on App Insights see [Set up Application Insights for ASP.NET](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-asp-net))
* (Optional) Text Analytics key (if you want to track sentiment)
* (Optional) Download and install [PowerBI desktop](https://powerbi.microsoft.com/en-us/desktop/) if you will be authoring reports for PowerBI. 

## Highlights
By default, the bot framework and the bot connector log limited information to AppInsights, this code provides helper classes that can be used to log additional data about your bot's activity. 

To be more precise, sample shows you how to log incoming and outgoing messages from any class that derives from `IDialog`, log LUIS intents and log other messages that are sent or received from the user outside the context of a dialog. We use a very simple alarm bot that allows the user to set or delete alarms and sends and proactively sends a message when the alarm triggers. 

The data is persisted in Azure Application Insights, so be aware that this may not always be desired and depends on the  information being exchanged and your security and privacy requirements. If needed, you can customize the code to avoid logging sensitive information. 

## Key code elements
This section describes the portions of the code that are related to capture bot telemetry, you can apply this changes to your bot code and start creating reports in only a few steps.

### Configuring logging settings.
The parameters the `TelemetryLogger` needs to work are stored in your bot's `web.config` file.
```xml
    <!-- AppInsights InstrumentationKey-->
    <add key="InstrumentationKey" value="3e1a0acb-52f5-41fc-982a-3b078ff8fb43" />
    
    <!-- Text Analytics data for message sentiment analysis -->
    <add key="TextAnalyticsApiKey" value="a501dfbedcb446eb8ff2bb2174d3add1" />
    <add key="TextAnalyticsMinLenght" value="3" />
```
* `InstrumentationKey`. Is your Application Insights instrumentation key, you can obtain this key from Azure Portal. Note that the `TelemetryLogger` class initializes AppInsights using code, so you will need to remove the TelemetryKey element from the `ApplicationInsights.config` file, this allows you to have different keys for different environments and change them through configuration if needed.
* `TextAnalyticsApiKey`. To track sentiment analysis, the telemetry code calls the Text Analytics API, you can obtain this key from the Azure Portal. The bot won't log any sentiment data if this value is empty.
* `TextAnalyticsMinLenght`. You normally don't want to track sentiment for short phrases like "yes", "no", etc. In the example above, this parameter tells the logger to only track sentiment for messages that have 3 words or more. 

### Initialize the telemetry logger
Once you have updated your `web.config`, you need to initialize the `TelemetryLogger` class by calling the `Initialize()` methods as follows in `Global.asax.cs`:
```cs
protected void Application_Start()
{
    // Initialize telemetry subsytem.
    TelemetryLogger.Initialize(ConfigurationManager.AppSettings["InstrumentationKey"], ConfigurationManager.AppSettings["TextAnalyticsApiKey"], ConfigurationManager.AppSettings["TextAnalyticsMinLenght"]);

    // Configure Web API.
    GlobalConfiguration.Configure(WebApiConfig.Register);
}
```
The call above, will initialize Application Insights with the key provided and will use the Text Analytics API to analyze sentiment on incoming messages. It will also register the `DialogActivityLogger` class that is responsible for tracking incoming and outgoing messages from any class that implements `IDialog`. 

### Tracking LUIS intents
To tack and LUIS intents you will need to override the `DispatchToIntentHandler()` method of the dialogs that derive from `LuisDialog<object>` and call `TelemetryLogger.TrackLuisIntent()` as follows:
```cs
protected override Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, IntentRecommendation bestInent, LuisResult result)
{
    // Log the resolved intent. 
    TelemetryLogger.TrackLuisIntent(context.Activity, result);
    return base.DispatchToIntentHandler(context, item, bestInent, result);
}
```
This call will log `LuisResults` to application insights, including their score and any entities identified by LUIS. 

### Tracking other messages
There are situations where you may need to send a message to a user outside the context of a dialog posting directly to the conversation. In these cases, you will need to manually invoke `TelemetryLogger.TrackActivity()`.  
In the alarm bot example provided, we do this in two situations:
* When we receive a message that is not of type `ActivityTypes.Message` (see the HandleSystemMessage method inside the `MessagesController.cs` class). 
* When the alarm fires and we send a message to the users (see the FireAlarm() method in `Alarm.cs`).


### Telemetry namespace
This folder contains the helper classes that are needed to track events, call text analytics, add custom dimensions and metrics, etc. You normally don't need to change anything here unless you want to customize your data with additional properties. There are two classes that are relevant in this namespace:

#### TelemetryLogger.cs
This is a static class with the helper methods you need to track custom events in AppInsights.

When in debug mode, the `TrackActivity` method also stores Json serialized properties for `ConversationData`, `PrivateConversationData`, `UserData` and the entire Activity, you can use this information to explore the properties during development and add them to the `TelemetryEvent` properties if you like to customize your reports.

#### DialogActivityLogger.cs
This class intercepts incoming and outgoing messages and calls `TelemetryLogger.TrackActivity` to log incoming and outgoing messages from classes that implement `IDialog`.


## PowerBI integration
The code includes a sample PowerBI report located under `Solution Items\PowerBIReports` that you can use as a base to create your own reports.
* For more details on how to integrate PowerBI with AppInsights see [Feed Power BI from Application Insights](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-export-power-bi)
* To learn more about how to write PowerBI reports see [Guided Learning](https://powerbi.microsoft.com/en-us/guided-learning/)

## Configuration
TODO

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

