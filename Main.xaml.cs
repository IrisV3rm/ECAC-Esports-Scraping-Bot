using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ECAC_eSports_Scraper.Classes;
using ECAC_eSports_Scraper.Classes.ECACMethods;
using ECAC_eSports_Scraper.Classes.SavingLoading;
using ECAC_eSports_Scraper.DataTypes.ECAC;
using ECAC_eSports_Scraper.DataTypes.GameAPIHandles;
using ECAC_eSports_Scraper.DataTypes.GameTypes;
using ECAC_eSports_Scraper.Methods;
using Newtonsoft.Json.Linq;
using Wpf.Ui.Controls;
using WpfAnimatedGif;
using static System.Threading.Tasks.Task;
using Button = Wpf.Ui.Controls.Button;
using Run = System.Windows.Documents.Run;

// ReSharper disable ConstantNullCoalescingCondition

namespace ECAC_eSports_Scraper
{
    public partial class Main
    {
        internal Process BotProcess = new();
        private bool _canRun;
        private bool _loaded;

        public static WebViewHandler WebViewHandler = new();

        public Main() => InitializeComponent();

        internal void AddToBotLog(string text)
        {
            LogFlowDoc.Dispatcher.Invoke(() =>
            {
                string prefixData = (text.Contains("[") && text.Contains("]")) ? text.Substring(0, text.LastIndexOf(@"]", StringComparison.Ordinal) + 2) : "[LOG]";
                string suffixData = text.Replace(prefixData, "").Replace("ERROR_DETECTED", "");

                Paragraph paragraph = new();
                Run prefixRun = new(prefixData);
                Run suffixRun = new(@" " + suffixData);

                prefixRun.FontFamily = new FontFamily("Consolas");
                suffixRun.FontFamily = new FontFamily("Consolas");

                prefixRun.FontSize = 12;
                suffixRun.FontSize = 12;

                if (text.Contains("ERROR_DETECTED"))
                    prefixRun.Foreground = Brushes.Red;
                else
                    prefixRun.Foreground = Brushes.DeepSkyBlue;

                suffixRun.Foreground = Brushes.White;

                paragraph.Inlines.Add(prefixRun);
                paragraph.Inlines.Add(suffixRun);

                paragraph.Margin = new Thickness(0, 0, 0, 5);
                LogFlowDoc.Blocks.Add(paragraph);
            });
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            Enum.TryParse(ECACGameType.Text, out GlobalGameData.Games gameType);
            Enum.TryParse(ECACApiType.Text, out GlobalApiHandles.GameApiType gameApi);
            EcacAccount ecacAccount = new(ECACUsernameTextBox.Text, ECACPasswordTextBox.Password, gameType, gameApi);

            bool successSaveSettings = Saving.SaveSettings(
                ecacAccount, 
                DiscordTokenBox.Text, 
                TopMostToggle.IsChecked ?? false,
                ScanTeamStats.IsChecked ?? true,
                ScanEnemyStats.IsChecked ?? true,
                EnemyChannelId.Text,
                TeamChannelId.Text
            );

            if (successSaveSettings)
                App.CustomMessageBox.ShowDialog("Successfully saved the settings!", messageBoxButton: MessageBoxButton.OK);
            else
                App.CustomMessageBox.ShowDialog("Failed to save the settings, please try again later!", messageBoxImage: MessageBoxImage.Error, messageBoxButton: MessageBoxButton.OK);
        }

        private bool LoadSettings()
        {
            if (Loading.LoadSavedSettings())
            {
                
                DiscordTokenBox.Password = App.SavedData.Value<string>("DiscordToken");
                EnemyChannelId.Text = App.SavedData.Value<string>("EnemyChannelId");
                TeamChannelId.Text = App.SavedData.Value<string>("TeamChannelId");

                TopMostToggle.IsChecked = App.SavedData.Value<bool>("TopMost");
                ScanEnemyStats.IsChecked = App.SavedData.Value<bool>("ScanEnemyStats");
                ScanTeamStats.IsChecked = App.SavedData.Value<bool>("ScanTeamStats");

                dynamic ecacAccount = App.SavedData["EcacAccount"];

                ECACUsernameTextBox.Text = ecacAccount?.Username ?? string.Empty;
                ECACPasswordTextBox.Password = ecacAccount?.Password ?? string.Empty;
                ECACGameType.SelectedIndex = (int)ecacAccount?.GameType!;
                ECACApiType.SelectedIndex = (int)ecacAccount.ApiType;

                Topmost = TopMostToggle.IsChecked ?? false;

                return true;
            }

            MessageBoxResult result = App.CustomMessageBox.ShowDialog("This appears to be your first start, read the first start guide!\nPress 'Yes' to launch the first start guide\nPress 'No' if you know what you're doing!", messageBoxButton: MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start("https://google.ca");
            }
            return false;
        }

        private void SetupNav()
        {
            Dictionary<NavigationItem, Canvas> navPages = new()
            {
                { TeamViewerNav, TeamViewerCanvas },
                { UpcomingTeamNav, UpcomingTeamCanvas },
                { SettingsNav, SettingsCanvas },
                { BotLog, BotLogPage}
            };

            foreach (NavigationItem navItem in RootNavigation.Items.OfType<NavigationItem>())
            {
                navItem.Click += delegate
                {
                    switch (_loaded)
                    {
                        case false when navItem == SettingsNav:
                            LoadingCanvas.Visibility = Visibility.Collapsed;
                            break;
                        case false:
                            return;
                    }

                    foreach (NavigationItem internalNavItem in RootNavigation.Items.OfType<NavigationItem>())
                    {
                        internalNavItem.IsActive = internalNavItem == navItem;
                        navPages[internalNavItem].Visibility = internalNavItem.IsActive ? Visibility.Visible : Visibility.Collapsed;
                    }
                };
            }
        }

        private void SetupSettingsNav()
        {
            Dictionary<Button, Canvas> settingsNavPages = new()
            {
                { GeneralSettingsNav, GeneralSettingsCanvas },
                { ECACSettingsNav, ECACSettingsCanvas },
                { BotSettingsNav, BotSettingsCanvas }
            };

            foreach (Button navButton in SettingsTabHandler.Children.OfType<Button>())
            {
                navButton.Click += delegate
                {
                    foreach (Button settingsButton in settingsNavPages.Keys)
                    {
                        settingsNavPages[settingsButton].Visibility = settingsButton == navButton ? Visibility.Visible : Visibility.Collapsed;
                    }

                    double targetLeftProperty = navButton == GeneralSettingsNav
                        ? 23.0
                        : (double)navButton.GetValue(Canvas.LeftProperty) + navButton.Width / 4;

                    DoubleAnimation animation = new()
                    {
                        To = targetLeftProperty,
                        Duration = TimeSpan.FromMilliseconds(150)
                    };

                    SettingsButtonAccent.BeginAnimation(Canvas.LeftProperty, animation);
                };
            }
        }

        private static int GetTextLength(Run element)
        {
            return element.Dispatcher.Invoke(() => element.Text.Length);
        }

        protected virtual async void SetupInitializers()
        {
            TeamViewerHandler teamViewer = new(LocalTeamMembersHolder, TeamMemberTemplate, LocalSchoolName, LocalTeamName, LocalGameType, LocalWinCount, LocalLossCount, LocalWinPercent, LocalTeamIcon);
            EnemyTeamViewerHandler enemyTeamViewer = new(EnemyTeamMembersHolder, TeamMemberTemplate, EnemySchoolName, EnemyTeamName, EnemyGameType, EnemyWinCount, EnemyLossCount, EnemyWinPercent, EnemyTeamIcon);
            teamViewer.Initialize();

#pragma warning disable CS4014
            Run(() =>
#pragma warning restore CS4014
            {
                while (!(teamViewer.IsLoaded && enemyTeamViewer.IsLoaded))
                {
                    if (!teamViewer.IsLoaded)
                    {
                        if (GetTextLength(LoadingTeamStatsRun) >= 21) GlobalMethods.SetRunText(LoadingTeamStatsRun, "Loading Team Stats");
                        else GlobalMethods.AddRunText(LoadingTeamStatsRun, ".");
                    }
                    else
                    {
                        LoadingTeamStatsRun.Dispatcher.InvokeAsync(() =>
                        {
                            if (LoadingTeamStatsRun.Text != "Successfully loaded team stats!")
                                Application.Current.Dispatcher.Invoke(enemyTeamViewer.Initialize);
                        });

                        GlobalMethods.SetRunText(LoadingTeamStatsRun, "Successfully loaded team stats!");
                        GlobalMethods.SetIconVisible(LoadingTeamStatsIcon, true);

                    }

                    if (!enemyTeamViewer.IsLoaded)
                    {
                        if (GetTextLength(LoadingEnemyStatsRun) >= 33) GlobalMethods.SetRunText(LoadingEnemyStatsRun, "Loading Enemy Team Stats Stats");
                        else GlobalMethods.AddRunText(LoadingEnemyStatsRun, ".");
                    }
                    else
                    {
                        GlobalMethods.SetRunText(LoadingEnemyStatsRun, "Successfully loaded enemy team stats!");
                        GlobalMethods.SetIconVisible(LoadingEnemyStatsIcon, true);
                    }

                    System.Threading.Thread.Sleep(300);
                }

                LoadingCanvas.Dispatcher.Invoke(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                    LoadingCanvas.Visibility = Visibility.Hidden;
                    _loaded = true;
                });
            });

            while (!_loaded) await Delay(25);

            BotProcess.StartInfo = new ProcessStartInfo
            {
                FileName = "Bot.exe",
                RedirectStandardOutput = true,
                RedirectStandardInput = false,
                RedirectStandardError = true,
                UseShellExecute = false,
                Arguments = $"{Process.GetCurrentProcess().Id}",
                CreateNoWindow = true
            };

            BotProcess.ErrorDataReceived += (_, args) =>
            {
                AddToBotLog($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR_DETECTED{args.Data}");
            };

            BotProcess.OutputDataReceived += (_, args) =>
            {
                Debug.WriteLine($"DATA RECEIVED: {args.Data}");

                string data = args.Data;
                if (string.IsNullOrEmpty(data)) return;

                switch (data)
                {
                    case "READY":
                        AddToBotLog($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Main client received connection state.");
                        using (NamedPipeClientStream pipeClient = new(".", "ECAC_BOT_PIPE", PipeDirection.InOut))
                        {
                            pipeClient.Connect();
                            StreamWriter writer = new(pipeClient);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                writer.WriteLine(
                                     $"{{" +
                                     $"\"processId\":{Process.GetCurrentProcess().Id}," +
                                     $"\"localTeamId\":\"{TeamChannelId.Text}\"," +
                                     $"\"enemyTeamId\":\"{EnemyChannelId.Text}\"," +
                                     $"\"enemyTeamViewerHandle\":{JToken.FromObject(enemyTeamViewer)}," +
                                     $"\"localTeamViewerHandle\":{JToken.FromObject(teamViewer)}," +
                                     $"\"discordToken\":\"{DiscordTokenBox.Password}\"" +
                                     $"}}"
                                    );
                                writer.Flush();
                            });
                        }
                        break;
                    default:
                        AddToBotLog(data);
                        break;
                }
            };

            BotProcess.Start();
            BotProcess.BeginOutputReadLine();


        }

        public async void SetupWebView()
        {
            if (Directory.Exists("ECAC eSports Scraper.exe.WebView2"))
                Directory.Delete("ECAC eSports Scraper.exe.WebView2", true);

            await WebViewHandler.InitWebView(MainView);
            App.EcacAuthorization = await EcacMethods.SignIn(ECACUsernameTextBox.Text, ECACPasswordTextBox.Password);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            string tempFilePath = Path.GetTempFileName() + ".gif";
            Properties.Resources.loading_gif.Save(tempFilePath);
            ImageBehavior.SetAnimatedSource(LoadingGif, new BitmapImage(new Uri(tempFilePath)));

            _canRun = LoadSettings();

            SetupSettingsNav();
            SetupNav();

            if (!_canRun) return;

            SetupWebView();
            while (string.IsNullOrEmpty(App.EcacAuthorization)) await Delay(50);
            SetupInitializers();
        }

    }
}


