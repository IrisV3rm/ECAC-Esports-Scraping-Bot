using System.Windows;
using ECAC_eSports.CustomControls;
using Newtonsoft.Json.Linq;

namespace ECAC_eSports
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
