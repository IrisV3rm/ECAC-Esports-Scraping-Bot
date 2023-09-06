using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace ECAC_eSports_Scraper.Classes.SavingLoading
{
    public class Loading
    {
        public static bool LoadSavedSettings()
        {
            
            string settingsFilePath = Path.Combine(GlobalProperties.Directory, "settings.json");

            if (!Directory.Exists(GlobalProperties.Directory) || !File.Exists(settingsFilePath))
            {
                return false;
            }

            try
            {
                string settingsJson = File.ReadAllText(settingsFilePath);
                App.SavedData = JObject.Parse(settingsJson);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
