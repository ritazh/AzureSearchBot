using System;
using UcmaBotDtmf.Models;

namespace UcmaBotDtmf.Common
{
    public class MyAppSettings
    {
        public static BotLanguageModel BotLanguage { get; set; }

        public static string ProjectTitle { get; set; }

        public static bool IsAmazonText2Speechenabled { get; set; }
    }
}