using System.Collections.Generic;
using ECAC_eSports_Scraper.DataTypes.GameTypes;

namespace ECAC_eSports_Scraper.DataTypes.ECAC
{
    public class Team
    {
        public string Id { get; set; }
        public string LogoUrl { get; set; }
        public string Name { get; set; }
        public string SchoolName { get; set; }
        public List<User> Members { get; set; }
        
        public GlobalGameData.Games Game { get; set; }

        public Team(string id, string logoUrl, string name, List<User> members, string schoolName, GlobalGameData.Games teamGame)
        {
            Id = id;
            LogoUrl = logoUrl;
            Name = name;
            Members = members;
            SchoolName = schoolName;
            Game = teamGame;
        }
    }
}

