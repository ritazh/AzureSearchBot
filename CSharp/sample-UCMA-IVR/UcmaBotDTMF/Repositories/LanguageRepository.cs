using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcmaBotDTMF.Models;

namespace UcmaBotDTMF.Repositories
{
    public class LanguageRepository
    {
        private List<BotLanguageModel> languages = new List<BotLanguageModel>();
        public LanguageRepository()
        {
            Load();
        }

        private void Load()
        {
            var dir = Directory.GetCurrentDirectory() + "\\Data\\languages.json";

            string content = File.ReadAllText(dir);

            languages = JsonConvert.DeserializeObject<List<BotLanguageModel>>(content);

        }
        public BotLanguageModel GetLanguage(string language)
        {
            return languages.FirstOrDefault(l => l.Language.ToLower() == language.ToLower());
        }

        public void Update(List<BotLanguageModel> updatedLanguages)
        {
            var dir = Directory.GetCurrentDirectory() + "\\Data\\languages.json";

            var content = JsonConvert.SerializeObject(updatedLanguages);

            File.WriteAllText(dir, content);

            Load();
        }
        public List<BotLanguageModel> GetBotLanguages()
        {
            return languages;
        }
    }
}
