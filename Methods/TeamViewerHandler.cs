using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ECAC_eSports_Scraper.Classes.ECACMethods;
using ECAC_eSports_Scraper.Classes.GameAPIMethods;
using ECAC_eSports_Scraper.DataTypes.ECAC;
using ECAC_eSports_Scraper.DataTypes.GameAPIHandles.Valorant;
using ECAC_eSports_Scraper.DataTypes.GameTypes;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Image = System.Windows.Controls.Image;

// ReSharper disable PossibleLossOfFraction

namespace ECAC_eSports_Scraper.Methods
{
    public class TeamViewerHandler
    {
        public bool IsLoaded;

        internal readonly Image TeamIcon;
        internal WrapPanel TeamMemberHolder;
        internal readonly Border MemberTemplate;
        internal Run SchoolRun;
        internal readonly Run TeamNameRun;
        internal readonly Run CurrentGameRun;
        internal readonly Run WinCountRun;
        internal readonly Run LossCount;
        internal readonly Run WinPercentRun;

        public Team CurrentTeam { get; private set; }
        
        public TeamViewerHandler(WrapPanel teamMemberHolder, Border memberTemplate, Run schoolRun, Run teamNameRun, Run currentGameRun, Run winCountRun, Run lossCount, Run winPercentRun, Image teamIcon)
        {
            TeamMemberHolder = teamMemberHolder;
            MemberTemplate = memberTemplate;

            TeamIcon = teamIcon;
            SchoolRun = schoolRun;
            TeamNameRun = teamNameRun;
            CurrentGameRun = currentGameRun;
            WinCountRun = winCountRun;
            LossCount = lossCount;
            WinPercentRun = winPercentRun;
        }

        public async void Initialize()
        {
            Team currentUserTeam = await EcacMethods.GetCurrentUserTeam(GlobalGameData.Games.Valorant);
            CurrentTeam = currentUserTeam;
            TeamStats currentTeamStats = await EcacMethods.GetTeamStats(currentUserTeam.Id, false);

            GlobalMethods.SetElementText(TeamNameRun, currentUserTeam.Name);
            GlobalMethods.SetElementText(CurrentGameRun, currentUserTeam.Game.ToString());
            GlobalMethods.SetElementText(WinCountRun, currentTeamStats.WinCount.ToString());
            GlobalMethods.SetElementText(LossCount, currentTeamStats.LossCount.ToString());
            GlobalMethods.SetElementText(WinPercentRun, currentTeamStats.WinPercentage + "%");
            GlobalMethods.SetImageSource(TeamIcon, new BitmapImage(new Uri(currentUserTeam.LogoUrl)));

            foreach (User user in currentUserTeam.Members)
            {
                string linkedHandle = await EcacMethods.GetLinkedHandleByType(user.UserId, EcacMethods.HandleTypes.Valorant);
                user.TrackerStats = await TrackerGg.GetStats(linkedHandle);
                ValorantRank currentRank = user.TrackerStats.CurrentRank;
                ValorantRank peakRank = user.TrackerStats.PeakRank;
                
                user.SetRiotId(linkedHandle);
                user.SetValorantCurrentRank(currentRank);
                user.SetValorantPeakRank(peakRank);
                
                Border template = MemberTemplate.CloneControl();
                Canvas memberCanvas = template.Child as Canvas;

                Run ecacNameRun = (Run)((TextBlock)memberCanvas?.Children[1])?.Inlines.Last();
                Run riotTagRun = (Run)((TextBlock)memberCanvas?.Children[2])?.Inlines.Last();
                Run kdaRun = (Run)((TextBlock)memberCanvas?.Children[9])?.Inlines.Last();
                Run hsRun = (Run)((TextBlock)memberCanvas?.Children[10])?.Inlines.Last();
                
                Image firstImage = (Image)memberCanvas?.Children[0];
                Image secondImage = (Image)memberCanvas?.Children[4];
                Image thirdImage = (Image)memberCanvas?.Children[5];

                SymbolIcon validRiotTagElement = (SymbolIcon)memberCanvas?.Children[6];

                ecacNameRun!.Text = user.EcacName;
                riotTagRun!.Text = user.RiotId;

                kdaRun!.Text = user.TrackerStats.KdRatio;
                hsRun!.Text = user.TrackerStats.HeadshotPercentage;

                firstImage.Source = user.ValorantPeakRank.RankIcon is Bitmap ? GlobalMethods.ImageSourceFromBitmap(user.ValorantPeakRank.RankIcon) : new BitmapImage(new Uri(user.ValorantPeakRank.RankIcon.ToString()));
                secondImage.Source = firstImage.Source;
                thirdImage.Source = user.ValorantCurrentRank.RankIcon is Bitmap ? GlobalMethods.ImageSourceFromBitmap(user.ValorantCurrentRank.RankIcon) : new BitmapImage(new Uri(user.ValorantCurrentRank.RankIcon.ToString()));
                

                validRiotTagElement.Symbol = await TrackerGg.IsValidUser(linkedHandle, false) ? SymbolRegular.CheckmarkCircle20 : SymbolRegular.DismissCircle20;
                if (validRiotTagElement.Symbol == SymbolRegular.CheckmarkCircle20) validRiotTagElement.Foreground = new SolidColorBrush(Colors.LimeGreen);

                template.Visibility = Visibility.Visible;

                TeamMemberHolder.Dispatcher.Invoke(() =>
                {
                    TeamMemberHolder.Children.Add(template);
                });
                Console.WriteLine(@"Finished Local: " + user.EcacName);
            }

            IsLoaded = true;
        }
    }
}
