using System;
using System.Runtime.Serialization;
using UcmaBotDtmf.Common;

namespace UcmaBotDtmf.Models
{

    [DataContract]
    public class DirectlineMessage
    {
        [DataMember(Name ="text")]
        public string Prompt { get; set; }

        [DataMember(Name = "length")]
        public int Length { get; set; }

        [DataMember(Name = "promptType")]
        public PromptType PromptType { get; set; }

        [DataMember(Name = "inputFormat")]
        public InputFormat InputFormat { get; set; }
        public string PreviousPrompt { get; set; }
        public int PromptSpokenCount { get; set;     }

    }
}
