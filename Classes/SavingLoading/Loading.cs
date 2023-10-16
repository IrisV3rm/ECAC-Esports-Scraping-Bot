using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECAC_eSports.Classes.SavingLoading
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
