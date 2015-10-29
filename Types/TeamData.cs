using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirSuperiority.Script.Entities;
using GTA;

namespace AirSuperiority.Types
{
    public struct TeamData
    {
        public int Index;
        public List<ActiveFighter> Members;
        public int RelationshipGroup;
        public int Score;
        public TeamInfo TeamInfo;
    }
}
