namespace BotV2.DataTypes.GameAPIHandles.Valorant
{
    public class AgentData
    {
        public enum Agent
        {
            Astra,
            Breach,
            Brimstone,
            Chamber,
            Cypher,
            Fade,
            Gekko,
            Harbor,
            Jett,
            // ReSharper disable once InconsistentNaming
            KAYO,
            Killjoy,
            Neon,
            Omen,
            Phoenix,
            Raze,
            Reyna,
            Sage,
            Skye,
            Sova,
            Viper,
            Yoru,
            None
        }

        public enum AgentClass
        {
            Controller,
            Duelist,
            Initiator,
            Sentinel,
            None
        }

        public static Dictionary<Agent, string> AgentHeadshot = new()
        {
            { Agent.Astra, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Astra.png" },
            { Agent.Breach, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Breach.png" },
            { Agent.Brimstone, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Brimstone.png" },
            { Agent.Chamber, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Chamber.png" },
            { Agent.Cypher, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Cypher.png" },
            { Agent.Fade, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Fade.png" },
            { Agent.Gekko, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Gekko.png" },
            { Agent.Harbor, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Harbor.png" },
            { Agent.Jett, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Jett.png" },
            { Agent.KAYO, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/KAYO.png" },
            { Agent.Killjoy, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Killjoy.png" },
            { Agent.Neon, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Neon.png" },
            { Agent.Omen, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Omen.png" },
            { Agent.Phoenix, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Phoenix.png" },
            { Agent.Raze, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Raze.png" },
            { Agent.Reyna, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Reyna.png" },
            { Agent.Sage, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Sage.png" },
            { Agent.Skye, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Skye.png" },
            { Agent.Sova, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Sova.png" },
            { Agent.Viper, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Viper.png" },
            { Agent.Yoru, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Yoru.png" },
            { Agent.None, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Headshots/Sage.png" }
        };

        public static Dictionary<AgentClass, string> RoleIcon = new()
        {
            { AgentClass.Controller, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Roles/ControllerClassSymbol.png"},
            { AgentClass.Duelist, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Roles/DuelistClassSymbol.png"},
            { AgentClass.Initiator, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Roles/InitiatorClassSymbol.png"},
            { AgentClass.Sentinel, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Roles/SentinelClassSymbol.png"},
            { AgentClass.None, "https://api.irisapp.ca/Dev/ecac-scraping/ValorantAssets/Roles/SentinelClassSymbol.png" }
        };
    }

    public record ValorantAgent(string Name, ValorantAgent.ValorantRole Role, dynamic Avatar, double HoursPlayed, double WinPercentage, double KdRatio)
    {
        public enum ValorantRole
        {
            Duelist,
            Sentinel,
            Controller,
            Initiator,
            None
        }

        public string Name { get; set; } = Name;
        public ValorantRole Role { get; set; } = Role;
        public dynamic Avatar { get; set; } = Avatar;
        public double HoursPlayed { get; set; } = HoursPlayed;
        public double WinPercentage { get; set; } = WinPercentage;
        public double KdRatio { get; set; } = KdRatio;
    }
}
