# Telemetry Bot Sample (C#)

Sample that shows how to use Application Insights to log custom telemetry on a bot. The sample logs custom events to app insights than can then be queried from PowerBI or other reporting tools to create custom dashboards.

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* Application Insights account.
* Text Analytics key (if you want to track sentiment)
* (Optional) Download and install [PowerBI desktop](https://powerbi.microsoft.com/en-us/desktop/) if you will be authoring reports for PowerBI. 

### Highlights

TODO

### Key code elements

#### Global.asax.cs
In Global.asax.cs we register the InstrumentationKey to be used by the TelemetryClient so you don't have to store this setting in ApplicationInsights.config allowing you to configure the key outside of code. 
Here, we also register DialogActivityLogger so we can intercept incoming and outgoing messages from anything that implements IDialog.

            // Set AppInsights Instrumentation Key. 
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];

			// Register activity logger
            var builder = new ContainerBuilder();
            builder.RegisterType<DialogActivityLogger>().As<IActivityLogger>().InstancePerLifetimeScope();
            builder.Update(Conversation.Container);

#### MessagesController.cs
Track non-dialog activities manually using

            await TelemetryLogger.TrackActivity(message, null);

#### RootDialog.cs
Override DispatchToIntentHandler to intercept and log Luis Events:

        protected override Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, IntentRecommendation bestInent, LuisResult result)
        {
            // Log the resolved intent. 
            TelemetryLogger.TrackLuisIntent(context.Activity, result);
            return base.DispatchToIntentHandler(context, item, bestInent, result);
        }

### Telemetry namespace
This folder contains the helper classes that are needed to track events, call text analytics, add custom dimensions and metrics, etc.

#### TelemetryLogger.cs
This is a static class with the helper methods you need to track custom events in AppInsights.

When in debug mode, the TrackActivity method also stores a Json for ConversationData, PrivateConversationData, UserData and the entire Activity, you can use this to explore the properties during development and add them to the TelemetryEvent properties if you like to customize your reports.

#### DialogActivityLogger.cs
This class intercepts incoming and outgoing messages and calls TelemetryLogger.TrackActivity to log messages.


### PowerBI integration
The code includes a sammple PowerBI report located under Solution Items\PowerBIReports that you can use as a base to create your own reports.
* For more details on how to integrate PowerBI with AppInsights see [Feed Power BI from Application Insights](https://docs.microsoft.com/en-us/azure/application-insights/app-insights-export-power-bi)
* To learn more about how to write PowerBI reports see [Guided Learning](https://powerbi.microsoft.com/en-us/guided-learning/)

## Configuration
TODO

### Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

### Copyright and license

Code released under [the MIT license](LICENSE)

