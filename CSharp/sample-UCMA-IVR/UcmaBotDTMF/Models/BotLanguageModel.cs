using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UcmaBotDTMF.Models
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
