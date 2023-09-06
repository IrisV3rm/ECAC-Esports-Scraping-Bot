using ECAC_eSports_Scraper.CustomControls;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace ECAC_eSports_Scraper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static CustomMessageBox CustomMessageBox;

        public static JObject SavedData;

        public static string EcacAuthorization { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CustomMessageBox = new CustomMessageBox();
        }
    }
}
