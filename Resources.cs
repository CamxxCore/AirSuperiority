using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirSuperiority.Types;
using GTA.Math;
using GTA;

namespace AirSuperiority
{
    public static class Resources
    {
        public static TeamInfo[] ValidTeams = new TeamInfo[]
        {
            new TeamInfo()
            {
                Team = Team.Canada,
                FriendlyName = "Canada",
                ImageAsset = "flag_can",    
                Index = -1,   
                RelationshipGroup = -1,
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(-3232.42f, -1755.673f, 226.5322f), 292.7778f)
            },

             new TeamInfo()
            {
                Team = Team.America,
                FriendlyName = "America",
                ImageAsset = "flag_usa",
                Index = -1,
                RelationshipGroup = -1,
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(2287.492f, 7661.41f, 579.9888f), 162.8432f)
            },

             new TeamInfo()
            {
                Team = Team.China,
                FriendlyName = "China",
                ImageAsset = "flag_prc",
                Index = -1,
                RelationshipGroup = -1,  
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(3220.731f, 7935.237f, 421.6384f), 160.8841f)
            },

          /* new TeamInfo()
            {
                Team = Team.England,
                FriendlyName = "England",
                ImageAsset = "flag_uk",
                RelationshipGroup = -1,
            },

             new TeamInfo()
            {
                Team = Team.France,
                FriendlyName = "France",
                ImageAsset = "flag_fr",
                RelationshipGroup = -1,
            },

              new TeamInfo()
            {
                Team = Team.Germany
                FriendlyName = "Germany",
                ImageAsset = "flag_ger",
                RelationshipGroup = -1,               
            },

              new TeamInfo()
            {
                Team = Team.Russia
                FriendlyName = "Russia",
                ImageAsset = "flag_rus",
                RelationshipGroup = -1,

            }*/
        };
    }
}
