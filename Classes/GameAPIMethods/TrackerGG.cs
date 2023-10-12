using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using ECAC_eSports_Scraper.DataTypes.GameAPIHandles.Valorant;
using Newtonsoft.Json.Linq;

namespace ECAC_eSports_Scraper.Classes.GameAPIMethods
{
    public class TrackerGg
    {
        private static string _cachedData = "";

        public static ValorantTrackerStats GetDefaultTrackerStats()
        {
            return new ValorantTrackerStats(
                new ValorantAgent("Sage", ValorantAgent.ValorantRole.Sentinel, "", 0.0, 0.0, 0.0),
                ValorantAgent.ValorantRole.Sentinel,
                new TrackerScore(0, 0.0, 0.0, 0.0, 0.0),
                "0%",
                "0%",
                "0",
                "0",
                new ValorantRank("Unranked", "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png"),
                new ValorantRank("Unranked", "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png")
            );
        }

        public static async Task<ValorantRank> GetCurrentRank(string riotId)
        {
            HttpResponseMessage response = await WebViewHandler.PublicClient.GetAsync($"https://api.kyroskoh.xyz/valorant/v1/mmr/NA/{riotId.Replace(" ", "%20").Replace("#", "/")}?show=rankonly&display=0");
            string rank = await response.Content.ReadAsStringAsync();
            
            return !rank.Contains("failed") ? new ValorantRank(rank, Properties.Resources.ResourceManager.GetObject(rank.Replace(" ", "_"))) : new ValorantRank("Unranked", "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png");
        }

        public static async Task<ValorantRank> GetPeakRank()
        {
            return await GetRank("peakRank");
        }

        public static async Task<string> GetHeadshotPercentage()
        {
            string hsPercentage = await GetDisplayValue("headshotsPercentage");
            return _cachedData.Contains("errors") ? "N/A" : hsPercentage; 
        }

        public static async Task<string> GetKdRatio()
        {
            string kd = await GetDisplayValue("kDRatio");

            return _cachedData.Contains("errors") ? "N/A" : kd;
        }

        public static async Task<bool> IsValidUser(string riotId, bool checkTracker)
        {
            if (string.IsNullOrEmpty(riotId)) return false;
            if (string.IsNullOrWhiteSpace(riotId)) return false;

            if (!checkTracker)
            {
                HttpResponseMessage response = await WebViewHandler.PublicClient.GetAsync($"https://api.kyroskoh.xyz/valorant/v1/mmr/NA/{riotId.Replace(" ", "%20").Replace("#", "/")}?show=rankonly&display=0");
                string rank = await response.Content.ReadAsStringAsync();
                return !rank.Contains("failed");
            }
            _cachedData = await FetchAndCacheData($"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{riotId}?forceCollect=true&source=web");

            return !_cachedData.Contains("errors");
        }

        public static Task<string> GetDisplayValue(string statValue)
        {
            JObject userData = JObject.Parse(_cachedData);
            JToken segments = userData["data"]?["segments"];
            JToken segment = segments?[0]?["stats"]?[statValue];

            return Task.FromResult(segment?.Value<string>("displayValue"));
        }

        private static Task<ValorantRank> GetRank(string rankType)
        {
            Debug.WriteLine("Starting 'GetRank'");
            
            JObject userData = JObject.Parse(_cachedData);
            JToken segments = userData["data"]?["segments"];
            if (segments is not { HasValues: true }) return Task.FromResult(new ValorantRank("Unranked", "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png"));

            JToken segment = segments[0]?["stats"]?[rankType]?["metadata"];
            if (segment == null) return Task.FromResult(new ValorantRank("Unranked", "https://trackercdn.com/cdn/tracker.gg/valorant/icons/tiersv2/0.png"));

            string tierName = segment.Value<string>("tierName");
            string iconUrl = segment.Value<string>("iconUrl");

            Debug.WriteLine("Starting 'Finished'");

            return Task.FromResult(new ValorantRank(tierName, iconUrl));
        }


        public static Task<TrackerScore> GetTrackerScore()
        {
            Debug.WriteLine("Starting 'GetTrackerScore'");
            
            JObject userData = JObject.Parse(_cachedData);
            JToken segments = userData["data"]?["segments"]?[0]?["stats"];

            int trnPerformanceScore = segments?["trnPerformanceScore"]?.Value<int>("value") ?? 0;
            double roundsWinPct = segments?["roundsWinPct"]?.Value<double>("value") ?? 0.0;
            double kAst = segments?["kAST"]?.Value<double>("value") ?? 0.0;
            double scorePerRound = segments?["scorePerRound"]?.Value<double>("value") ?? 0.0;
            double damageDeltaPerRound = segments?["damageDeltaPerRound"]?.Value<double>("value") ?? 0.0;

            Debug.WriteLine("Finished 'GetTrackerScore'");
            return Task.FromResult(new TrackerScore(trnPerformanceScore, roundsWinPct, kAst, scorePerRound, damageDeltaPerRound));
        }
        
        public static Task<string> GetWinPercentage()
        {
            Debug.WriteLine("Starting 'GetWinPercentage'");

            JObject userData = JObject.Parse(_cachedData);
            JToken segments = userData["data"]?["segments"]?[0]?["stats"];
            string roundsWinPct = segments?["roundsWinPct"]?.Value<string>("displayValue") ?? "0";

            Debug.WriteLine("Finished 'GetWinPercentage'");

            return Task.FromResult($"{roundsWinPct}%");
        }

        public static Task<string> GetAverageDamage()
        {
            Debug.WriteLine("Starting 'GetAverageDamage'");
            
            JObject userData = JObject.Parse(_cachedData);
            JToken segments = userData["data"]?["segments"]?[0]?["stats"];
            string roundsWinPct = segments?["damagePerRound"]?.Value<string>("displayValue") ?? "0";

            Debug.WriteLine("Finished 'GetAverageDamage'");

            return Task.FromResult($"{roundsWinPct}%");
        }

        public static Task<ValorantAgent.ValorantRole> GetTopRole()
        {
            Debug.WriteLine("Starting 'GetTopRole'");
            
            double winPercent = 0.0;
            JToken agentSegment = null;
            JObject userData = JObject.Parse(_cachedData);
            IEnumerable<JToken> segments = userData["data"]?["segments"]?.Reverse();

            if (segments is null)
                return Task.FromResult(ValorantAgent.ValorantRole.Controller);

            foreach (JToken segment in segments)
            {
                if (segment.Value<string>("type") != "agent-role")
                    continue;

                double? foundWinPct = segment["stats"]?["matchesWinPct"]?.Value<double>("value");
                winPercent = foundWinPct > winPercent ? foundWinPct.Value : winPercent;

                agentSegment = segment;
            }

            Enum.TryParse(agentSegment?["metadata"]?.Value<string>("role"), out ValorantAgent.ValorantRole role);

            Debug.WriteLine("Finished 'GetTopRole'");
            return Task.FromResult(role);
        }

        public static Task<ValorantAgent> GetAgentData()
        {
            Debug.WriteLine("Starting 'GetAgentData'");
            
            double winPercent = 0.0;
            JToken agentSegment = null;
            JObject userData = JObject.Parse(_cachedData);
            IEnumerable<JToken> segments = userData["data"]?["segments"]?.Reverse();

            if (segments is null)
                return Task.FromResult<ValorantAgent>(null);

            foreach (JToken segment in segments)
            {
                if (segment.Value<string>("type") != "agent")
                    continue;

                double? foundWinPct = segment["stats"]?["matchesWinPct"]?.Value<double>("value");
                winPercent = foundWinPct > winPercent ? foundWinPct.Value : winPercent;

                agentSegment = segment;
            }

            Enum.TryParse(agentSegment?["metadata"]?.Value<string>("role"), out ValorantAgent.ValorantRole role);

            Debug.WriteLine("Finished 'GetAgentData'");

            return Task.FromResult(new ValorantAgent(
                agentSegment?["metadata"]?.Value<string>("name"),
                role,
                agentSegment?["metadata"]?.Value<string>("imageUrl"),
                0.0,
                0.0,
                0.0
            ));
        }

        public static async Task<bool> TrackerRateLimitCheck(string riotId)
        {
            string siteBody = await Main.WebViewHandler.NavigateAndGetSource($"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{riotId}?forceCollect=true&source=web".Replace("#", "%23").Replace(" ", "%20"));
            
            return siteBody.Contains("You are being rate limited");
        }

        public static async Task<string> GetTrackerJson(string riotId, bool custom = false)
        {
            if (custom) 
                return await FetchAndCacheData($"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{riotId}?type=custom&season=&agent=all&map=all".Replace("#", "%23").Replace(" ", "%20"));

            return await FetchAndCacheData($"https://api.tracker.gg/api/v2/valorant/standard/profile/riot/{riotId}?forceCollect=true&source=web".Replace("#", "%23").Replace(" ", "%20"));
        }

        public static async Task<ValorantTrackerStats> GetStats(string riotId)
        {
            if (!await IsValidUser(riotId, false))
            {
                Debug.WriteLine("Invalid Riot Id");
                return GetDefaultTrackerStats();
            }
            if (!await IsValidUser(riotId, true))
            {
                Debug.WriteLine("Tracker Invalid");
                return GetDefaultTrackerStats();
            }
            if (await TrackerRateLimitCheck(riotId))
            {
                MessageBox.Show("Rate limit");
                Process.GetCurrentProcess().Kill();
            }

            _cachedData = null;
            _cachedData = await GetTrackerJson(riotId);
            while (_cachedData == null) await Task.Delay(25);

            Task<ValorantAgent> agentDataTask = GetAgentData();
            Task<ValorantAgent.ValorantRole> topRoleTask = GetTopRole();
            Task<TrackerScore> trackerScoreTask = GetTrackerScore();
            Task<ValorantRank> currentRankTask = GetCurrentRank(riotId);
            Task<ValorantRank> peakRankTask = GetPeakRank();
            Task<string> winPercentageTask = GetWinPercentage();
            Task<string> headshotPercentageTask = GetHeadshotPercentage();
            Task<string> kdRatioTask = GetKdRatio();
            Task<string> averageDamageTask = GetAverageDamage();

            Debug.WriteLine("Starting tracker stats...");

            await Task.WhenAll(
                agentDataTask,
                topRoleTask,
                trackerScoreTask,
                winPercentageTask,
                headshotPercentageTask,
                kdRatioTask,
                averageDamageTask,
                currentRankTask,
                peakRankTask
            );

            Debug.WriteLine("Finished tracker stats...");
            
            return new ValorantTrackerStats(
                agentDataTask.Result,
                topRoleTask.Result,
                trackerScoreTask.Result,
                winPercentageTask.Result,
                headshotPercentageTask.Result,
                kdRatioTask.Result,
                averageDamageTask.Result,
                currentRankTask.Result,
                peakRankTask.Result
            );
        }

        public static async Task<string> FetchAndCacheData(string url)
        {
            Debug.WriteLine($"Fetching: {url}");
            
            string siteBody = await Main.WebViewHandler.NavigateAndGetSource(url.Replace("#", "%23").Replace(" ", "%20"));
            siteBody = siteBody.Substring(siteBody.IndexOf('{'));

            int endIndex = siteBody.Contains("\\u0")
                ? siteBody.IndexOf("}\\u0", StringComparison.Ordinal) + 1
                : siteBody.LastIndexOf("}", StringComparison.Ordinal) + 1;


            string jsonData = siteBody.Substring(0, endIndex).Replace("\\\"", "\"");

            if (!jsonData.Contains("\\")) return jsonData;

            Console.WriteLine(jsonData);
            MessageBox.Show("what the fuck");

            return jsonData;
        }
    }
}
