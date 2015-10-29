using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AirSuperiority.Types;
using AirSuperiority.Script.Entities;
using GTA;

namespace AirSuperiority.Script
{
    public static class TeamManager
    {
        /// <summary>
        /// Contains information about active teams.
        /// </summary>
        public static TeamInfo[] ActiveTeams = new TeamInfo[4];

        /// <summary>
        /// World relationship groups.
        /// </summary>
        private static List<int> rGroups = new List<int> {
            World.AddRelationshipGroup("team1"),
            World.AddRelationshipGroup("team2"),
            World.AddRelationshipGroup("team3"),
            World.AddRelationshipGroup("team4")
        };

        public static void SetupRelationshipGroups()
        {
            for (int i = 0; i < rGroups.Count; i++)
            {
                for (int a = 0; a < rGroups.Count; i++)
                {
                    if (a == i) continue;
                    World.SetRelationshipBetweenGroups(Relationship.Hate, rGroups[i], rGroups[a]);
                }
            }
        }

        /// <summary>
        /// Randomize active teams.
        /// </summary>
        public static void GetNewTeams()
        { 
            for (int i = 0; i < ActiveTeams.Count(); i++)
            {
                ActiveTeams[i] = default(TeamInfo);
                ActiveTeams[i] = Resources.ValidTeams.Where(x => !ActiveTeams.Contains(x)).GetRandomItem();
                ActiveTeams[i].Index = i;
                ActiveTeams[i].RelationshipGroup = rGroups[i];
                UI.Notify(ActiveTeams[i].FriendlyName);
            }
        }

        /// <summary>
        /// Find the team with the least members and assign it to the fighter.
        /// </summary>
        /// <param name="newFighter">Target fighter.</param>
        /// <param name="activeFighters">Active fighters.</param>
        public static void SetupFighterTeam(ActiveFighter newFighter, IEnumerable<ActiveFighter> activeFighters)
        {
            var memberCounts = new List<int> { GetActiveTeamMemberCount(ActiveTeams[0], activeFighters),
                GetActiveTeamMemberCount(ActiveTeams[1], activeFighters),
                GetActiveTeamMemberCount(ActiveTeams[2], activeFighters),
                GetActiveTeamMemberCount(ActiveTeams[3], activeFighters) };

            newFighter.AssignTeam(ActiveTeams[memberCounts.IndexOf(memberCounts.Min())]);
        }

        public static int GetActiveTeamMemberCount(TeamInfo team, IEnumerable<ActiveFighter> activeFighters)
        {
            var memberCount = 0;

            foreach (var item in activeFighters)
            {
                if (item.Team.Equals(team))
                    memberCount++;
            }

            return memberCount;
        }

        /// <summary>
        /// Remove all relationship groups from the game.
        /// </summary>
        public static void RemoveRelationshipGroups()
        {
            rGroups.ForEach(x => World.RemoveRelationshipGroup(x));
        }
    }
}
