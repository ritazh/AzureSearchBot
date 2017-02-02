using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcmaBotDTMF.Models;

namespace UcmaBotDTMF.Common
{
    public class MyAppSettings
    {
              public static BotLanguageModel BotLanguage { get; set; }

              public static string ProjectTitle { get; set; }

              public static bool IsAmazonText2Speechenabled { get; set; }
    }
}
