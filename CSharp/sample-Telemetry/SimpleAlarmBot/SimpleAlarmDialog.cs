using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.SimpleAlarmBot.Telemetry;

namespace Microsoft.Bot.Sample.SimpleAlarmBot
{
    [LuisModel("56c73d36-e6de-441f-b2c2-6ba7ea73a1bf", "6d0966209c6e4f6b835ce34492f3e6d9")]
    [Serializable]
    public class SimpleAlarmDialog : LuisDialog<object>
    {
        public const string DefaultAlarmWhat = "default";

        public const string EntityAlarmName = "AlarmName";
        public const string EntityAlarmTitle = "builtin.alarm.title";
        public const string EntityAlarmStartTime = "builtin.alarm.start_time";
        public const string EntityAlarmStartDate = "builtin.alarm.start_date";
        private readonly Dictionary<string, Alarm> _alarmByWhat = new Dictionary<string, Alarm>();
        private Alarm _turnOff;

        public SimpleAlarmDialog()
        {}

        public SimpleAlarmDialog(ILuisService service)
            : base(service)
        {}

        protected override Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, IntentRecommendation bestInent, LuisResult result)
        {
            TelemetryLogger.TrackLuisIntent(context.Activity, result);
            return base.DispatchToIntentHandler(context, item, bestInent, result);
        }

        public bool TryFindAlarm(LuisResult result, out Alarm alarm)
        {
            alarm = null;

            string what;

            EntityRecommendation title;
            if (result.TryFindEntity(EntityAlarmName, out title))
            {
                what = title.Entity;
            }
            else
            {
                what = DefaultAlarmWhat;
            }

            return _alarmByWhat.TryGetValue(what, out alarm);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.delete_alarm")]
        public async Task DeleteAlarm(IDialogContext context, LuisResult result)
        {
            Alarm alarm;
            if (TryFindAlarm(result, out alarm))
            {
                _alarmByWhat.Remove(alarm.What);
                await context.PostAsync($"alarm {alarm} deleted");
            }
            else
            {
                await context.PostAsync("did not find alarm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.find_alarm")]
        public async Task FindAlarm(IDialogContext context, LuisResult result)
        {
            Alarm alarm;
            if (TryFindAlarm(result, out alarm))
            {
                await context.PostAsync($"found alarm {alarm}");
            }
            else
            {
                await context.PostAsync("did not find alarm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("alarm.set_alarm")]
        public async Task SetAlarm(IDialogContext context, LuisResult result)
        {
            EntityRecommendation title;
            if (!result.TryFindEntity(EntityAlarmName, out title))
            {
                title = new EntityRecommendation(type: EntityAlarmName) {Entity = DefaultAlarmWhat};
            }

            EntityRecommendation date;
            if (!result.TryFindEntity(EntityAlarmStartDate, out date))
            {
                date = new EntityRecommendation(type: EntityAlarmStartDate) {Entity = string.Empty};
            }

            EntityRecommendation time;
            if (!result.TryFindEntity(EntityAlarmStartTime, out time))
            {
                time = new EntityRecommendation(type: EntityAlarmStartTime) {Entity = string.Empty};
            }

            var parser = new Parser();
            var span = parser.Parse(date.Entity + " " + time.Entity);

            if (span != null)
            {
                var when = span.Start ?? span.End;
                var alarm = new Alarm {What = title.Entity, When = when.Value};
                _alarmByWhat[alarm.What] = alarm;

                string reply = $"alarm {alarm} created";
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync("could not find time for alarm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.snooze")]
        public async Task AlarmSnooze(IDialogContext context, LuisResult result)
        {
            Alarm alarm;
            if (TryFindAlarm(result, out alarm))
            {
                alarm.When = alarm.When.Add(TimeSpan.FromMinutes(7));
                await context.PostAsync($"alarm {alarm} snoozed!");
            }
            else
            {
                await context.PostAsync("did not find alarm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.time_remaining")]
        public async Task TimeRemaining(IDialogContext context, LuisResult result)
        {
            Alarm alarm;
            if (TryFindAlarm(result, out alarm))
            {
                var now = DateTime.UtcNow;
                if (alarm.When > now)
                {
                    var remaining = alarm.When.Subtract(DateTime.UtcNow);
                    await context.PostAsync($"There is {remaining} remaining for alarm {alarm}.");
                }
                else
                {
                    await context.PostAsync($"The alarm {alarm} expired already.");
                }
            }
            else
            {
                await context.PostAsync("did not find alarm");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.turn_off_alarm")]
        public async Task TurnOffAlarm(IDialogContext context, LuisResult result)
        {
            if (TryFindAlarm(result, out _turnOff))
            {
                PromptDialog.Confirm(context, AfterConfirming_TurnOffAlarm, "Are you sure?", promptStyle: PromptStyle.None);
            }
            else
            {
                await context.PostAsync("did not find alarm");
                context.Wait(MessageReceived);
            }
        }

        public async Task AfterConfirming_TurnOffAlarm(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                _alarmByWhat.Remove(_turnOff.What);
                await context.PostAsync($"Ok, alarm {_turnOff} disabled.");
            }
            else
            {
                await context.PostAsync("Ok! We haven't modified your alarms!");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("builtin.intent.alarm.alarm_other")]
        public async Task AlarmOther(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("what ?");
            context.Wait(MessageReceived);
        }

        [Serializable]
        public sealed class Alarm : IEquatable<Alarm>
        {
            public DateTime When { get; set; }
            public string What { get; set; }

            public bool Equals(Alarm other)
            {
                return other != null
                       && When == other.When
                       && What == other.What;
            }

            public override string ToString()
            {
                return $"[{What} at {When}]";
            }

            public override bool Equals(object other)
            {
                return Equals(other as Alarm);
            }

            public override int GetHashCode()
            {
                return What.GetHashCode();
            }
        }
    }
}