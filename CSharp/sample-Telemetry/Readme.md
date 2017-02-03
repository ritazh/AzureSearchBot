# Telemetry Bot Sample (C#)

A sample bot that forwards the user to complete a PayPal payment and then resumes the conversation. This sample is a simple starting point for more complex PayPal transactions through bots. By redirecting to PayPal and resuming once done we ensure the bot does not handle the financial transaction itself and just facilitates it.

### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* Application Insights account.
* Text Analytics key (if you want to track sentiment)

### Highlights

TODO

### Key code elements

#### Global.asax.cs

            // Set AppInsights Instrumentation Key. 
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];

			// Register activity logger
            var builder = new ContainerBuilder();
            builder.RegisterType<DialogActivityLogger>().As<IActivityLogger>().InstancePerLifetimeScope();
            builder.Update(Conversation.Container);

#### MessagesController.cs
Track non dialog activities manually using

            await TelemetryLogger.TrackActivity(message, null);

#### RootDialog.cs
Override DispatchToIntentHandler to intercept and log Luis Events:

        protected override Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, IntentRecommendation bestInent, LuisResult result)
        {
            // Log the resolved intent. 
            TelemetryLogger.TrackLuisIntent(context.Activity, result);
            return base.DispatchToIntentHandler(context, item, bestInent, result);
        }

#### Telemetry namespace
This folder contains the helper classes that are needed to track events, call text analytics, add custom dimensions and metrics, etc.


### Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

### Copyright and license

Code released under [the MIT license](LICENSE)
