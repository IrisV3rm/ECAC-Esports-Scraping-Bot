using System;

namespace ECAC_eSports_Scraper.Classes.SavingLoading
{
    public static class GlobalProperties
    {
        public static readonly string Directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + "\\ECACScraper";
    }
}
