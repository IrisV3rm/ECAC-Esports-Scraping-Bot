using System.Collections.Generic;
using ECAC_eSports_Scraper.DataTypes.GameTypes;

namespace ECAC_eSports_Scraper.DataTypes.ECAC
{
    public record Team(string Id, string LogoUrl, string Name, List<User> Members, string SchoolName, GlobalGameData.Games Game)
    {
        public string Id { get; set; } = Id;
        public string LogoUrl { get; set; } = LogoUrl;
        public string Name { get; set; } = Name;
        public string SchoolName { get; set; } = SchoolName;
        public List<User> Members { get; set; } = Members;

        public GlobalGameData.Games Game { get; set; } = Game;
    }
}

