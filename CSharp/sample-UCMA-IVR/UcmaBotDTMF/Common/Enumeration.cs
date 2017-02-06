using System;
using System.Runtime.Serialization;

namespace UcmaBotDtmf.Common
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
