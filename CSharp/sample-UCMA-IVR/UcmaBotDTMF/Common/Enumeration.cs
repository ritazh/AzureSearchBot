using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UcmaBotDTM
{
    [DataContract]
    public enum PromptType
    {
        [EnumMember(Value = "CreditCard")]
        CreditCard,
        [EnumMember(Value = "AccountNumber")]
        AccountNumber,
        [EnumMember(Value = "ExpirationDate")]
        ExpirationDate,
        [EnumMember(Value = "Pin")]
        Pin,
        [EnumMember(Value = "Menu")]
        Menu,
        [EnumMember(Value = "EndDialog")]
        EndDialog,
        [EnumMember(Value = "CCV")]
        CCV,
        [EnumMember(Value = "ConfirmPayment")]
        ConfirmPayment

    }
    [DataContract]
    public enum InputFormat
    {
        [EnumMember(Value = "Number")]
        Number,
        [EnumMember(Value = "String")]
        String,
        [EnumMember(Value = "None")]
        None
    }
}
