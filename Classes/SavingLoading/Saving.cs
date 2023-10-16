using System.IO;
using ECAC_eSports.DataTypes.ECAC;
using Newtonsoft.Json;

namespace ECAC_eSports.Classes.SavingLoading
{
    internal class SaveData
    {
        public EcacAccount EcacAccount { get; set; }
        public string DiscordToken { get; set; }
        public string TeamChannelId { get; set; }
        public string EnemyChannelId { get; set; }
        public bool TopMost { get; set; }
        public bool ScanTeamStats { get; set; }
        public bool ScanEnemyStats { get; set; }
        public SaveData(EcacAccount ecacAccount, string discordToken, bool topMost, bool scanTeamStats, bool scanEnemyStats, string enemyChannelId, string teamChannelId)
        {
            EcacAccount = ecacAccount;
            DiscordToken = discordToken;
            TopMost = topMost;
            ScanTeamStats = scanTeamStats;
            ScanEnemyStats = scanEnemyStats;
            EnemyChannelId = enemyChannelId;
            TeamChannelId = teamChannelId;
        }
    }

    public class Saving
    {
        public static bool SaveSettings(EcacAccount ecacAccount, string discordToken, bool topMost, bool scanTeamStats, bool scanEnemyStats, string enemyChannelId, string teamChannelId)
        {
            try
            {
                Directory.CreateDirectory(GlobalProperties.Directory);
                string settingsFilePath = Path.Combine(GlobalProperties.Directory, "settings.json");

                using StreamWriter saveWriter = new(settingsFilePath);

                SaveData data = new(ecacAccount, discordToken, topMost, scanTeamStats, scanEnemyStats, enemyChannelId, teamChannelId);

                saveWriter.Write(JsonConvert.SerializeObject(data));
                saveWriter.Flush();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
