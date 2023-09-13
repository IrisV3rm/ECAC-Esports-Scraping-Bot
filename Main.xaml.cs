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
                string prefixData = (text.Contains("[") && text.Contains("]")) ? text.Substring(0, text.IndexOf(@"]", StringComparison.Ordinal)) : "[LOG]";
                string suffixData = text.Replace(prefixData, "");

                Paragraph paragraph = new();
                Run prefixRun = new(prefixData);
                Run suffixRun = new(@" " + suffixData);

                prefixRun.FontFamily = new FontFamily("Consolas");
                suffixRun.FontFamily = new FontFamily("Consolas");

                prefixRun.FontSize = 12;
                suffixRun.FontSize = 12;

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
            //TeamViewerHandler teamViewer = new(LocalTeamMembersHolder, TeamMemberTemplate, LocalSchoolName, LocalTeamName, LocalGameType, LocalWinCount, LocalLossCount, LocalWinPercent, LocalTeamIcon);
            //EnemyTeamViewerHandler enemyTeamViewer = new(EnemyTeamMembersHolder, TeamMemberTemplate, EnemySchoolName, EnemyTeamName, EnemyGameType, EnemyWinCount, EnemyLossCount, EnemyWinPercent, EnemyTeamIcon);
            //teamViewer.Initialize();

            //#pragma warning disable CS4014
            //            Run(() =>
            //#pragma warning restore CS4014
            //            {
            //                while (!(teamViewer.IsLoaded && enemyTeamViewer.IsLoaded))
            //                {
            //                    if (!teamViewer.IsLoaded)
            //                    {
            //                        if (GetTextLength(LoadingTeamStatsRun) >= 21) GlobalMethods.SetRunText(LoadingTeamStatsRun, "Loading Team Stats");
            //                        else GlobalMethods.AddRunText(LoadingTeamStatsRun, ".");
            //                    }
            //                    else
            //                    {
            //                        LoadingTeamStatsRun.Dispatcher.InvokeAsync(() =>
            //                        {
            //                            if (LoadingTeamStatsRun.Text != "Successfully loaded team stats!")
            //                                Application.Current.Dispatcher.Invoke(enemyTeamViewer.Initialize);
            //                        });

            //                        GlobalMethods.SetRunText(LoadingTeamStatsRun, "Successfully loaded team stats!");
            //                        GlobalMethods.SetIconVisible(LoadingTeamStatsIcon, true);

            //                    }

            //                    if (!enemyTeamViewer.IsLoaded)
            //                    {
            //                        if (GetTextLength(LoadingEnemyStatsRun) >= 33) GlobalMethods.SetRunText(LoadingEnemyStatsRun, "Loading Enemy Team Stats Stats");
            //                        else GlobalMethods.AddRunText(LoadingEnemyStatsRun, ".");
            //                    }
            //                    else
            //                    {
            //                        GlobalMethods.SetRunText(LoadingEnemyStatsRun, "Successfully loaded enemy team stats!");
            //                        GlobalMethods.SetIconVisible(LoadingEnemyStatsIcon, true);
            //                    }

            //                    System.Threading.Thread.Sleep(300);
            //                }

            //                LoadingCanvas.Dispatcher.Invoke(() =>
            //                {
            //                    System.Threading.Thread.Sleep(2000);
            //                    LoadingCanvas.Visibility = Visibility.Hidden;
            //                    _loaded = true;
            //                });
            //            });

            //while (!_loaded) await Delay(25);

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
                Debug.WriteLine($"ERROR: {args.Data}");
            };

            BotProcess.OutputDataReceived += (_, args) =>
            {
                Debug.WriteLine($"DATA RECEIVED: {args.Data}");

                string data = args.Data;
                if (string.IsNullOrEmpty(data)) return;

                switch (data)
                {
                    case "READY":
                        
                        using (NamedPipeClientStream pipeClient = new(".", "ECAC_BOT_PIPE", PipeDirection.InOut))
                        {
                            pipeClient.Connect();
                            StreamWriter writer = new(pipeClient);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                //writer.WriteLine($"{{" +
                                //                 $"\"processId\":{Process.GetCurrentProcess().Id}," +
                                //                 $"\"localTeamId\":\"{TeamChannelId.Text}\"," +
                                //                 $"\"enemyTeamId\":\"{EnemyChannelId.Text}\"," +
                                //                 $"\"enemyTeamViewerHandle\":{JToken.FromObject(enemyTeamViewer)}," +
                                //                 $"\"localTeamViewerHandle\":{JToken.FromObject(teamViewer)}," +
                                //                 $"\"discordToken\":\"{DiscordTokenBox.Password}\"" +
                                //                 $"}}");
                                writer.WriteLine("{\"processId\":21516,\"localTeamId\":\"1037208192226177054\",\"enemyTeamId\":\"1067527886581538886\",\"enemyTeamViewerHandle\":{\"IsLoaded\":true,\"CurrentTeam\":{\"Id\":\"7aea11c2-3ce0-4415-b80f-401a95c37e6d\",\"LogoUrl\":\"https://legacyplatformapiprod.blob.core.windows.net/images/organizations/4e4cbb1a-8765-430e-a230-e31ee67eadf9.jpg?v=637848045288030450\",\"Name\":\"Loyola Valorant Ehounds Grey\",\"SchoolName\":\"Loyola University Maryland\",\"Members\":[{\"EcacName\":null,\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"4926bf9d-be35-49af-a4c1-08d97e247df9\",\"DiscordHandle\":\"Gilly#1716\",\"RiotId\":\"LMD Gilly#2 Ls\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":null},{\"EcacName\":null,\"RoleId\":\"5a1675f0-2fa9-482b-b187-434901734a42\",\"UserId\":\"3ae47070-4ec1-4986-b412-08d972cd7778\",\"DiscordHandle\":\"PistolTech#6356\",\"RiotId\":\"LMD spirit#grimm\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":null},{\"EcacName\":null,\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"a3f912d8-f247-4312-a476-08d97e247df9\",\"DiscordHandle\":\"JPF#9114\",\"RiotId\":\"LMD JPF#403\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":null},{\"EcacName\":\"Bye\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"fd23aedb-ee17-4988-841a-08da92aa706f\",\"DiscordHandle\":\"chase!#7861\",\"RiotId\":\"LMD bye#aboo\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":null},{\"EcacName\":null,\"RoleId\":\"6f4da22c-7fe5-4c78-8876-eec2c87d1096\",\"UserId\":\"bfd2eb9f-7e4b-4f71-b462-08d972cd7778\",\"DiscordHandle\":\"henrypodeschi#3859\",\"RiotId\":\"dimwit#dumb\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":null},{\"EcacName\":\"Rush\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"d6b6e996-f051-4fb7-3dd8-08db008af9ff\",\"DiscordHandle\":null,\"RiotId\":\"LMD Rush#12613\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":null},{\"EcacName\":\"1hamzy\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"98ec8abb-0a43-4c30-8421-08da92aa706f\",\"DiscordHandle\":null,\"RiotId\":\"hamzy#2020\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":null}],\"Game\":0}},\"localTeamViewerHandle\":{\"IsLoaded\":true,\"CurrentTeam\":{\"Id\":\"62d42379-9906-430b-ac5d-18be8d39ab68\",\"LogoUrl\":\"https://legacyplatformapiprod.blob.core.windows.net/images/teams/62d42379-9906-430b-ac5d-18be8d39ab68.jpg?v=637998869700957471\",\"Name\":\"Kodiaks Valorant\",\"SchoolName\":\"Lethbridge College\",\"Members\":[{\"EcacName\":\"SolidVal\",\"RoleId\":\"5a1675f0-2fa9-482b-b187-434901734a42\",\"UserId\":\"bf8b20d6-b183-40eb-e2d9-08da9ce2c006\",\"DiscordHandle\":\"Solid#6532\",\"RiotId\":\"KODI SOLID#0000\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"N/A\",\"Role\":2,\"Avatar\":\"\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":2,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":0,\"Kast\":0,\"AverageCombatScore\":0,\"DdPerRound\":0},\"PeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"0%\",\"HeadshotPercentage\":\"0%\",\"KdRatio\":\"0\",\"AverageDamagePerRound\":\"0\"}},{\"EcacName\":\"ninelota\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"2e26cc3d-0fe7-49ec-e2c7-08da9ce2c006\",\"DiscordHandle\":\"lota#6163\",\"RiotId\":\"KODI LOTA#slay\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"N/A\",\"Role\":2,\"Avatar\":\"\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":2,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":0,\"Kast\":0,\"AverageCombatScore\":0,\"DdPerRound\":0},\"PeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"0%\",\"HeadshotPercentage\":\"0%\",\"KdRatio\":\"0\",\"AverageDamagePerRound\":\"0\"}},{\"EcacName\":\"iTZi\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"2b52a4ec-0b1f-4195-e8da-08da9ce358a2\",\"DiscordHandle\":\"iTZi#9792\",\"RiotId\":\"KODI iTZi#21822\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"N/A\",\"Role\":2,\"Avatar\":\"\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":2,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":0,\"Kast\":0,\"AverageCombatScore\":0,\"DdPerRound\":0},\"PeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"0%\",\"HeadshotPercentage\":\"0%\",\"KdRatio\":\"0\",\"AverageDamagePerRound\":\"0\"}},{\"EcacName\":\"KodiToast\",\"RoleId\":\"5a1675f0-2fa9-482b-b187-434901734a42\",\"UserId\":\"94fe2dc3-370d-4652-0124-08da9be6c368\",\"DiscordHandle\":\"Iris#0410\",\"RiotId\":\"Cinnamon Toast#Krunc\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Platinum 3\",\"RankIcon\":\"System.Drawing.Bitmap\",\"SmallerRank\":\"P3\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"Raze\",\"Role\":0,\"Avatar\":\"https://titles.trackercdn.com/valorant-api/agents/f94c3b30-42be-e959-889c-5aa313dba261/displayicon.png\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":0,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":100,\"Kast\":80,\"AverageCombatScore\":449,\"DdPerRound\":187.2},\"PeakRank\":{\"Rank\":\"Platinum 3\",\"RankIcon\":\"System.Drawing.Bitmap\",\"SmallerRank\":\"P3\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"100.0%%\",\"HeadshotPercentage\":\"31.0%\",\"KdRatio\":\"4.00\",\"AverageDamagePerRound\":\"273.0%\"}},{\"EcacName\":\"KODI Naevis\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"9e7250d8-5c87-4d8a-e8d9-08da9ce358a2\",\"DiscordHandle\":\"kang_lib#7486\",\"RiotId\":\"KODI Naevis#Capt\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"N/A\",\"Role\":2,\"Avatar\":\"\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":2,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":0,\"Kast\":0,\"AverageCombatScore\":0,\"DdPerRound\":0},\"PeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"0%\",\"HeadshotPercentage\":\"0%\",\"KdRatio\":\"0\",\"AverageDamagePerRound\":\"0\"}},{\"EcacName\":\"Luxely\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"19a1bede-e866-4636-c36b-08dafaf1f1e0\",\"DiscordHandle\":\"alexandra#9151\",\"RiotId\":\"KODI Luxely#Lux\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"N/A\",\"Role\":2,\"Avatar\":\"\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":2,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":0,\"Kast\":0,\"AverageCombatScore\":0,\"DdPerRound\":0},\"PeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"0%\",\"HeadshotPercentage\":\"0%\",\"KdRatio\":\"0\",\"AverageDamagePerRound\":\"0\"}},{\"EcacName\":\"s0ap\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"b7f7e496-58a8-423a-f0d9-08dafaf84b16\",\"DiscordHandle\":\"s0ap#3793\",\"RiotId\":\"KODI s0ap#s0ap\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"N/A\",\"Role\":2,\"Avatar\":\"\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":2,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":0,\"Kast\":0,\"AverageCombatScore\":0,\"DdPerRound\":0},\"PeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"0%\",\"HeadshotPercentage\":\"0%\",\"KdRatio\":\"0\",\"AverageDamagePerRound\":\"0\"}},{\"EcacName\":\"mocchael\",\"RoleId\":\"21140ea7-a115-49db-b6d7-b118979e2ede\",\"UserId\":\"50338072-3f92-4484-c36a-08dafaf1f1e0\",\"DiscordHandle\":\"Moccha#3177\",\"RiotId\":\"KODI mocchael#0001\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"N/A\",\"Role\":2,\"Avatar\":\"\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":2,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":0,\"Kast\":0,\"AverageCombatScore\":0,\"DdPerRound\":0},\"PeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"0%\",\"HeadshotPercentage\":\"0%\",\"KdRatio\":\"0\",\"AverageDamagePerRound\":\"0\"}},{\"EcacName\":\"johnwook\",\"RoleId\":\"6f4da22c-7fe5-4c78-8876-eec2c87d1096\",\"UserId\":\"e4f2faa5-c699-4cb3-340a-08da9d86f346\",\"DiscordHandle\":\"JohnWook#0726\",\"RiotId\":\"KODI JohnWook#NA1\",\"ValorantCurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"ValorantPeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"TrackerStats\":{\"TopAgent\":{\"Name\":\"N/A\",\"Role\":2,\"Avatar\":\"\",\"HoursPlayed\":0,\"WinPercentage\":0,\"KdRatio\":0},\"Role\":2,\"TrackerScore\":{\"Score\":0,\"WinPercentage\":0,\"Kast\":0,\"AverageCombatScore\":0,\"DdPerRound\":0},\"PeakRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"CurrentRank\":{\"Rank\":\"Unranked\",\"RankIcon\":\"https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png\",\"SmallerRank\":\"Unr\"},\"WinPercentage\":\"0%\",\"HeadshotPercentage\":\"0%\",\"KdRatio\":\"0\",\"AverageDamagePerRound\":\"0\"}}],\"Game\":0}},\"discordToken\":\" " + DiscordTokenBox.Password +"\"}");
                                writer.Flush();
                            });
                        }
                        break;
                }
                AddToBotLog(data);
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


