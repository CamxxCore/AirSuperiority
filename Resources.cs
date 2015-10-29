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
                FriendlyName = "Canada",
                ImageAsset = @"scripts\flags\flag_can.png",
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(1350.13f, -596.34f, 421.6384f), 96.70607f)
            },

             new TeamInfo()
            {
                FriendlyName = "America",
                ImageAsset =  @"scripts\flags\flag_usa.png",
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(-1722.227f, -1353.99f, 421.6384f), 295.498f)
            },

             new TeamInfo()
            {
                FriendlyName = "China",
                ImageAsset = @"scripts\flags\flag_prc.png",
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(-113.6285f, 462.3756f, 421.6384f), 185.7956f)
            },

           new TeamInfo()
            {
                FriendlyName = "England",
                ImageAsset = @"scripts\flags\flag_uk.png",
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(634.7925f, -1776.049f, 421.6384f), 38.44354f)
            },
           
             new TeamInfo()
            {
                FriendlyName = "France",
                ImageAsset = @"scripts\flags\flag_fr.png",
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(-161.3713f, -2634.05f, 421.6384f), 357.9745f)
            },

              new TeamInfo()
            {
                FriendlyName = "Germany",
                ImageAsset = @"scripts\flags\flag_ger.png",
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(-121.3991f, 544.3475f, 421.6384f), 186.3273f)
            },

              new TeamInfo()
            {
                FriendlyName = "Russia",
                ImageAsset = @"scripts\flags\flag_ru.png",
                SpawnInfo = new Tuple<Vector3, float>(new Vector3(-1490.731f, 344.9524f, 421.6384f), 239.1252f)
           }
        };
    }
}
