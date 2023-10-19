using System.Net.Http.Headers;
using System.Text;
using Brotli;
using ECAC_eSports_Bot.Classes.SavingLoading;
using ECAC_eSports_Bot.DataTypes.ECAC;
using ECAC_eSports_Bot.DataTypes.GameTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable PossibleInvalidCastExceptionInForeachLoop
// ReSharper disable once PossibleLossOfFraction
// ReSharper disable IdentifierTypo

namespace ECAC_eSports_Bot.Classes.ECACMethods
{
    public class EcacMethods
    {
        private const string CachedUserId = "";
        private static JToken _cachedData = JToken.Parse("{}");

        public enum HandleTypes
        {
            Activision,
            BattleNet,
            Chess,
            CallOfDutyCrossplay,
            Discord,
            ElectronicArts,
            EpicGames,
            FighterId,
            HiRez,
            LeagueOfLegends,
            Minecraft,
            Mlb,
            Nba,
            NintendoFriendCode,
            Playstation,
            PubgMobile,
            Valorant,
            Steam,
            Supercell,
            Twitch,
            UPlay,
            Wb,
            Xbox,
            Zoom
        }

        internal static Dictionary<GlobalGameData.Games, string> GameTypes = new()
        {
            [GlobalGameData.Games.Valorant] = "e86f5c37-5bac-40ae-b10b-d1f10530935c"
        };
        
        internal static async Task<JToken> SendGetNetRequest(string getUrl)
        {
            GlobalProperties.MainClient.DefaultRequestHeaders.Clear();
            GlobalProperties.MainClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            GlobalProperties.MainClient.DefaultRequestHeaders.Add("X-League-Id", "d0b8ffc0-4feb-4b69-994c-60c8a3704316");
            GlobalProperties.MainClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalProperties.EcacAuthorization);

            using HttpResponseMessage response = await GlobalProperties.MainClient.GetAsync(getUrl);
            
            byte[] decompressedContent = (await response.Content.ReadAsByteArrayAsync()).DecompressFromBrotli();

            GlobalProperties.MainClient.DefaultRequestHeaders.Clear();

            return JToken.Parse(response.Content.ReadAsStringAsync().Result.Contains("Invalid league") ? "{}" : Encoding.UTF8.GetString(decompressedContent));
        }
        
        internal static async Task<string?> GetCurrentUserId()
        {
            JToken responseBody = await SendGetNetRequest("https://api.leaguespot.gg/api/v1/users/me");
            return responseBody.Value<string>("userId");
        }

        internal static async Task FetchAndCacheAccountData(string? userId)
        {
            if (CachedUserId == userId) return;

            JToken responseBody = await SendGetNetRequest($"https://api.leaguespot.gg/api/v1/users/{userId}");
            _cachedData = responseBody;
        }

        public static async Task<string?> SignIn(string username, string password)
        {
            GlobalProperties.MainClient.DefaultRequestHeaders.Clear();
            GlobalProperties.MainClient.DefaultRequestHeaders.Add("X-League-Id", "d0b8ffc0-4feb-4b69-994c-60c8a3704316");

            using StringContent requestContent = new($"{{\"otpCode\":\"\",\"password\":\"{password}\",\"username\":\"{username}\"}}", Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await GlobalProperties.MainClient.PostAsync("https://api.leaguespot.gg/api/v2/users/login", requestContent);

            dynamic? deserializedData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            GlobalProperties.MainClient.DefaultRequestHeaders.Clear();
            
            return deserializedData?.token;
        }


        public static async Task<string?> GetLinkedHandleByType(string? userId, HandleTypes gameHandle)
        {
            await FetchAndCacheAccountData(userId);
            
            if (_cachedData is not { HasValues: true } || _cachedData["gameHandles"] is null)
            {
                return null;
            }

            JToken? gameHandles = _cachedData["gameHandles"];
            return gameHandles?.Count() <= (int)gameHandle ? null : gameHandles?[(int)gameHandle]?.Value<string>("handle");
        }

        public static List<User> GetTeamMembers(JToken? members)
        {
            return (members ?? throw new ArgumentNullException(nameof(members))).Select(member => new User(
                member.Value<string>("gamerHandle"),
                member.Value<string>("roleId"),
                member.Value<string>("userId"),
                member.Value<string>("discordHandle"))
            ).ToList();
        }

        public static async Task<TeamStats> GetTeamStats(string? teamId, List<User> teamMembers, bool currentSeasonOnly = true)
        {
            string? seasonId = null;
            double winCount = 0.0;
            double lossCount = 0.0;

            JToken response = await SendGetNetRequest($"https://api.leaguespot.gg/api/v1/teams/{teamId}/matches");
            JArray responseBody = JArray.Parse(response.ToString());

            foreach (JToken matchToken in responseBody)
            {
                JObject? match = matchToken as JObject;

                if (seasonId is null) seasonId = match?.Value<string>("seasonId");
                else if (currentSeasonOnly && seasonId != match?.Value<string>("seasonId")) break;

                JToken? participant = match?["match"]?["matchParticipants"]?[0];
                bool isWinner = participant?.Value<bool>("isWinner") ?? false;
                string? participantTeamId = participant?.Value<string>("teamId");

                if (isWinner && participantTeamId == teamId) winCount++;
                else lossCount++;
            }

            if (lossCount == 0) lossCount = 1;

            return new TeamStats(
                winCount,
                lossCount, 
                (int)Math.Floor(winCount / (winCount + lossCount) * 100),
                teamMembers.First(member => member.RoleId == "6f4da22c-7fe5-4c78-8876-eec2c87d1096"),
                teamMembers.First(member => member.RoleId == "5a1675f0-2fa9-482b-b187-434901734a42")
            );
        }

        internal static async Task<string?> GetCurrentOpponentTeamId(GlobalGameData.Games gameType)
        {
            Team? currentTeamData = await GetCurrentUserTeam(gameType);
            string? currentTeamId = currentTeamData?.Id;

            if (currentTeamId == null) return "";

            JToken response = await SendGetNetRequest($"https://api.leaguespot.gg/api/v1/teams/{currentTeamId}/matches");
            JArray responseBody = JArray.Parse(response.ToString());

            if (!responseBody.HasValues) return currentTeamId;

            JToken? teams = responseBody[0]["match"]?["matchParticipants"];
            
            return teams?.FirstOrDefault(team => team.Value<string>("teamId") != currentTeamId)?.Value<string>("teamId");
        }

        public static async Task<Team?> GetCurrentOpponent(GlobalGameData.Games gameType)
        {
            JToken team = await SendGetNetRequest($"https://api.leaguespot.gg/api/v1/teams/{await GetCurrentOpponentTeamId(gameType)}");
            List<User> teamMembers = GetTeamMembers(team.Value<JToken>("members"));
            return new Team(
                team.Value<string>("id"),
                team.Value<string>("organizationLogoUrl"),
                team.Value<string>("name"),
                teamMembers,
                team["organization"]!.Value<string>("name"),
                GlobalGameData.Games.Valorant,
                await GetTeamStats(team.Value<string>("id"),teamMembers,false)
            );
        }

        public static async Task<Team?> GetCurrentUserTeam(GlobalGameData.Games gameType)
        {
            // Stupid shitty hack but I couldn't figure out why it wasn't working with the set client
            using HttpClient httpClient = new();
            
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("x-league-id", "d0b8ffc0-4feb-4b69-994c-60c8a3704316");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalProperties.EcacAuthorization);

            HttpResponseMessage response = await httpClient.GetAsync($"https://api.leaguespot.gg/api/v1/users/{await GetCurrentUserId()}/teams");

            response.EnsureSuccessStatusCode();
            byte[] decompressedContent = (await response.Content.ReadAsByteArrayAsync()).DecompressFromBrotli();
            JToken responseBody = JToken.Parse(Encoding.UTF8.GetString(decompressedContent));

            if (!responseBody.HasValues) return null;

            foreach (JToken team in responseBody)
            {
                if (team.Value<string>("gameId") != GameTypes[gameType]) continue;

                List<User> teamMembers = GetTeamMembers(team.Value<JToken>("members"));

                return new Team(
                    team.Value<string>("id"),
                    team.Value<string>("logoUrl"),
                    team.Value<string>("name"),
                    teamMembers,
                    team["organization"]!.Value<string>("name"),
                    GlobalGameData.Games.Valorant,
                    await GetTeamStats(team.Value<string>("id"), teamMembers)
                );
            }

            return null;
        }
    }
}
