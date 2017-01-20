using Autofac;
using GlobalMessageHandlersBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalMessageHandlersBot
{
    public class GlobalMessageHandlersBotModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SettingsScorable>()
                .As<IScorable<double>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CancelScorable>()
                .As<IScorable<double>>()
                .InstancePerLifetimeScope();
        }
    }
}