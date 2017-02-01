using System;

namespace Microsoft.Bot.Sample.SimpleAlarmBot.Dialogs
{
    [Serializable]
    public sealed class Alarm : IEquatable<Alarm>
    {
        public string Name { get; set; }
        public DateTime? When { get; set; }

        public bool Equals(Alarm other)
        {
            return other != null
                   && When == other.When
                   && Name == other.Name;
        }

        public override string ToString()
        {
            return $"[{Name} at {When}]";
        }

        public override bool Equals(object other)
        {
            return Equals(other as Alarm);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}