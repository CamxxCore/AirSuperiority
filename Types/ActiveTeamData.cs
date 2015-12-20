using System.Collections.Generic;
using AirSuperiority.Script.Entities;
using System.Drawing;

namespace AirSuperiority.Types
{
    public class ActiveTeamData
    {
        public List<ActiveFighter> ActiveFighters { get; private set; }
        public List<AIConvoy> ActiveGroundAssets { get; private set; }
        public SpawnPoint GroundSpawnPoint { get; private set; }
        public SpawnPoint JetSpawnPoint { get; private set; }
        public Color Color { get; private set; }
        public bool InControl { get; set; }
        public int Index { get; private set; }
        public int RelationshipGroup { get; private set; }
        public int Score { get; set; }
        public TeamInfo TeamInfo { get; private set; }

        public ActiveTeamData(int index, int rGroup, Color tColor, TeamInfo tInfo, SpawnPoint groundSpawn, SpawnPoint jetSpawn)
        {
            Index = index;
            RelationshipGroup = rGroup;
            Color = tColor;
            TeamInfo = tInfo;
            GroundSpawnPoint = groundSpawn;
            JetSpawnPoint = jetSpawn;
            ActiveFighters = new List<ActiveFighter>();
            ActiveGroundAssets = new List<AIConvoy>();
            InControl = false;
            Score = 0;
        }
    }
}
