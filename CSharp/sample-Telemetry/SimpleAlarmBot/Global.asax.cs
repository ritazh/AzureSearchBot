using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using Autofac;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Sample.SimpleAlarmBot.Telemetry;

namespace Microsoft.Bot.Sample.SimpleAlarmBot
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Set AppInsights Instrumentation Key. 
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Register activity logger
            var builder = new ContainerBuilder();
            builder.RegisterType<DialogActivityLogger>().As<IActivityLogger>().InstancePerLifetimeScope();
            builder.Update(Conversation.Container);
        }
    }
}