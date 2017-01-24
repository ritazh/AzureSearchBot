﻿using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace GlobalMessageHandlersBot.Dialogs
{
    public class SettingsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("This is the Settings Dialog. Reply with anything to return to prior dialog.");

            context.Wait(this.MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.Trim().Length > 0)
            {
                context.Done<object>(null);
            }
            else
            {
                context.Fail(new Exception("Message was not a string or was an empty string."));
            }
        }
    }
}