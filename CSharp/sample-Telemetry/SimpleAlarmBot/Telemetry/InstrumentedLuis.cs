using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Microsoft.Bot.Sample.SimpleAlarmBot.Telemetry
{
    public class InstrumentedLuis : ILuisService
    {
        public Uri BuildUri(LuisRequest luisRequest)
        {
            throw new NotImplementedException();
        }

        public Task<LuisResult> QueryAsync(Uri uri, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}