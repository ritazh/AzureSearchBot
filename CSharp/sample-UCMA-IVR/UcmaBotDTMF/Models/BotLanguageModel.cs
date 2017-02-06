using System;
using System.Runtime.Serialization;

namespace UcmaBotDtmf.Models
{
    [DataContract]
    public class BotLanguageModel
    {
        [DataMember(Name = "lang")]
        public string Language { get; set; }

        [DataMember(Name = "directlineSecret")]
        public string Secret { get; set; }

        [DataMember(Name = "local")]
        public string Local { get; set; }
    }
}
