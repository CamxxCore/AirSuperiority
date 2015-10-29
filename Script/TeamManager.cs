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
        private static TeamData[] _activeTeams = new TeamData[4];

        /// <summary>
        /// World relationship groups.
        /// </summary>
        private static List<int> rGroups = new List<int> {
            World.AddRelationshipGroup("team1"),
            World.AddRelationshipGroup("team2"),
            World.AddRelationshipGroup("team3"),
            World.AddRelationshipGroup("team4")
        };

        /// <summary>
        /// Get the team by its index.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static TeamData GetTeamByIndex(int team)
        {
            return _activeTeams[team];
        }

        /// <summary>
        /// Get all active teams.
        /// </summary>
        /// <returns></returns>
        public static TeamData[] GetActiveTeams()
        {
            return _activeTeams;
        }

        /// <summary>
        /// Randomize active teams.
        /// </summary>
        public static void GetNewTeams()
        { 
            for (int i = 0; i < 4; i++)
            {
                _activeTeams[i] = default(TeamData);
                _activeTeams[i].TeamInfo = Resources.ValidTeams.Where(x => !_activeTeams.Any(y => y.TeamInfo.FriendlyName == x.FriendlyName)).GetRandomItem();
                _activeTeams[i].Index = i;
                _activeTeams[i].RelationshipGroup = rGroups[i];
                _activeTeams[i].Score = 0;
                _activeTeams[i].Members = new List<ActiveFighter>();
                UIManager.UpdateTeamInfoFriendlyName(i, _activeTeams[i].TeamInfo.FriendlyName);
                UIManager.UpdateTeamInfoFlagAsset(i, _activeTeams[i].TeamInfo.ImageAsset);
            }
        }

        /// <summary>
        /// Find the team with the least members and assign it to the fighter.
        /// </summary>
        /// <param name="newFighter">Target fighter.</param>
        /// <param name="activeFighters">Active fighters.</param>
        public static void SetupFighterTeam(ActiveFighter newFighter)
        {
            var memberCounts = new List<int> { _activeTeams[0].Members.Count,
                _activeTeams[1].Members.Count,
                _activeTeams[2].Members.Count,
                _activeTeams[3].Members.Count};

            var team = _activeTeams[memberCounts.IndexOf(memberCounts.Min())];
            newFighter.AssignTeam(team);
            team.Members.Add(newFighter);
        }

                /// <summary>
        /// Register score for the fighters team.
        /// </summary>
        /// <param name="score"></param>
        public static void RegisterScoreForTeam(int team, int score)
        {
            _activeTeams[team].Score += score / 10;
        }

        /// <summary>
        /// Update team related information.
        /// </summary>
        public static void Update()
        {
            for (int i = 0; i < 4; i++)
                UIManager.UpdateTeamInfoProgressBar(i, _activeTeams[i].Score);
        }

        /// <summary>
        /// Setup world relationship groups.
        /// </summary>
        public static void SetupRelationshipGroups()
        {
            foreach (int group in rGroups)
            {
                foreach (int enemyGroup in rGroups.Where(x => x != group))
                {
                    World.SetRelationshipBetweenGroups(Relationship.Hate, group, enemyGroup);
                }
            }
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
