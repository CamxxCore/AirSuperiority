using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Math;
using GTA;

namespace AirSuperiority.Types
{
    public struct TeamInfo
    {
        public BlipColor BlipColor;
        public string FriendlyName;
        public string ImageAsset;
        public int Index;
        public int MemberCount;
        public int RelationshipGroup;
        public Tuple<Vector3, float> SpawnInfo;
        public Team Team;
    }
}
