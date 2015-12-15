using System.Collections.Generic;
using AirSuperiority.Script.Entities;
using GTA.Math;

namespace AirSuperiority.Types
{
    public struct ActiveTeamData
    {
        public bool InControl;
        public int Index;
        public List<ActiveFighter> ActiveFighters;
        public List<AIConvoy> ActiveGroundAssets;
        public int RelationshipGroup;
        public int Score;
        public float SpawnHeading;
        public float GroundHeading;
        public Vector3 FighterSpawn;
        public Vector3 GroundSpawn;
        public TeamInfo TeamInfo;
    }
}
