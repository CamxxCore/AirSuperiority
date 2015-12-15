using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AirSuperiority.Types;
using AirSuperiority.Script.Entities;
using AirSuperiority.Script.UIManagement;
using AirSuperiority.Data;
using GTA;
using GTA.Math;

namespace AirSuperiority.Script.GameManagement
{
    public static class TeamManager
    {
        public static bool Enabled { get; set; } = false;

        public const int TeamCount = 4;

        public const int PlayerCount = 50;

        public const float SpawnDist = 0.2f;

        /// <summary>
        /// Contains information about active teams.
        /// </summary>
        private static ActiveTeamData[] _activeTeams = new ActiveTeamData[TeamCount];

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
        /// Coordinate of map center.
        /// </summary>
        private static readonly Vector3 CenterMap = new Vector3(-248.9207f, -752.2429f, 421.6384f);

        /// <summary>
        /// World position and heading for team spawn locations.
        /// </summary>
        private static Tuple<Vector3, float>[] fighterSpawns = new Tuple<Vector3, float>[] {
             new Tuple<Vector3, float>(Vector3.Lerp(new Vector3(-1722.227f, -1353.99f, 481.6384f), CenterMap, SpawnDist), -72f),
            new Tuple<Vector3, float>(Vector3.Lerp(new Vector3(1350.13f, -596.34f, 481.6384f), CenterMap, SpawnDist), 96.70607f/*295.498f*/),
            new Tuple<Vector3, float>(Vector3.Lerp(new Vector3(-372.45f, 523.23f, 481.6384f), CenterMap, SpawnDist), 160f/*239.1252f*/),
            new Tuple<Vector3, float>(Vector3.Lerp(new Vector3(-161.3713f, -2634.05f, 481.6384f), CenterMap, SpawnDist), 10f/*357.9745f*/)
        };

        /// <summary>
        /// World position and heading for team spawn locations.
        /// </summary>
        private static Tuple<Vector3, float>[] groundSpawns = new Tuple<Vector3, float>[] {
            new Tuple<Vector3, float>(new Vector3(1318.396f, -557.2538f, 72.28039f), 58.57626f),
            new Tuple<Vector3, float>(new Vector3(-1721.894f, -1094.84f, 13.07793f), 316.236f),
            new Tuple<Vector3, float>(new Vector3(-1413.372f, 228.6124f, 215.7202f), 215.7202f),
            new Tuple<Vector3, float>(new Vector3(-94.15818f, -2623.103f, 6.00071f), 269.5972f)
        };

        /// <summary>
        /// Get the team by its index.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static ActiveTeamData GetTeamByIndex(int team)
        {
            return _activeTeams[team];
        }

        /// <summary>
        /// Get all active teams.
        /// </summary>
        /// <returns></returns>
        public static ActiveTeamData[] GetActiveTeams()
        {
            return _activeTeams;
        }

        /// <summary>
        /// Get all active fighters.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ActiveFighter> GetActiveFighters()
        {
            return _activeTeams.SelectMany(x => x.ActiveFighters);
        }

        /// <summary>
        /// Get all active ground assets.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AIConvoy> GetActiveGroundAssets()
        {
            return _activeTeams.SelectMany(x => x.ActiveGroundAssets);
        }
        /// <summary>
        /// Randomize active teams.
        /// </summary>
        public static void GetNewTeams()
        {
            var teamData = XMLHelper.ReadValues<TeamInfo>(@"scripts\AirSuperiority\assets.xml", "TeamInfo", "name", "imageAsset", "altAsset");

            for (int i = 0; i < TeamCount; i++)
            {
                _activeTeams[i] = default(ActiveTeamData);
                _activeTeams[i].TeamInfo = teamData.Where(x => !_activeTeams.Any(y => y.TeamInfo.FriendlyName == x.FriendlyName)).GetRandomItem();
                _activeTeams[i].Index = i;
                _activeTeams[i].RelationshipGroup = rGroups[i];
                _activeTeams[i].Score = 0;
                _activeTeams[i].FighterSpawn = fighterSpawns[i].Item1;
                _activeTeams[i].SpawnHeading = fighterSpawns[i].Item2;
                _activeTeams[i].GroundSpawn = groundSpawns[i].Item1;
                _activeTeams[i].GroundHeading = groundSpawns[i].Item2;
                _activeTeams[i].ActiveFighters = new List<ActiveFighter>();
                _activeTeams[i].ActiveGroundAssets = new List<AIConvoy>();
                UIManager.UpdateTeamInfoFriendlyName(i, _activeTeams[i].TeamInfo.FriendlyName);
                UIManager.UpdateTeamInfoFlagAsset(i, _activeTeams[i].TeamInfo.ImageAsset);
            }
        }

        /// <summary>
        /// Find the team with the least members and assign it to the fighter.
        /// </summary>
        /// <param name="newFighter">Target fighter.</param>
        /// <param name="activeFighters">Active fighters.</param>
        public static void SetupTeam(ActiveFighter newFighter)
        {
            var memberCounts = _activeTeams.Select(x => x.ActiveFighters.Count).ToList();
            var team = _activeTeams[memberCounts.IndexOf(memberCounts.Min())];
            newFighter.AssignTeam(team);
            team.ActiveFighters.Add(newFighter);
        }

        /// <summary>
        /// Assign the ground asset to the specified team
        /// </summary>
        /// <param name="newAsset">Target ground asset.</param>
        /// <param name="team">Team index.</param>
        public static void SetupTeam(AIConvoy newAsset)
        {
            var memberCounts = _activeTeams.Select(x => x.ActiveGroundAssets.Count).ToList();
            var team = _activeTeams[memberCounts.IndexOf(memberCounts.Min())];
            newAsset.Leader.AssignTeam(team);
            team.ActiveGroundAssets.Add(newAsset);
        }

        /// <summary>
        /// Assign the ground asset to the specified team
        /// </summary>
        /// <param name="newAsset">Target ground asset.</param>
        /// <param name="team">Team index.</param>
        public static void SetupTeam(AIConvoy newAsset, int team)
        {
            newAsset.Leader.AssignTeam(_activeTeams[team]);
            _activeTeams[team].ActiveGroundAssets.Add(newAsset);
        }

        /// <summary>
        /// Register score for the team.
        /// </summary>
        /// <param name="score"></param>
        public static void RegisterScoreForTeam(int team, int score)
        {
            var mScore = _activeTeams[team].Score + score / 100;
            if (mScore > 0)
                _activeTeams[team].Score = mScore;
        }

        /// <summary>
        /// Gets the color representing the team at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Color GetColorFromTeamIndex(int index)
        {
            switch (index)
            {
                case 0: return Color.Green;
                case 1: return Color.Red;
                case 2: return Color.Blue;
                case 3: return Color.Yellow;
                default: throw new ArgumentOutOfRangeException(string.Format("index: Out of range.", index));
            }
        }

        /// <summary>
        /// Gets the blip color representing the team at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static BlipColor GetBlipColorFromTeamIndex(int index)
        {
            return index == 0 ?
              BlipColor.Green : index == 1 ?
              BlipColor.Red : index == 3 ?
              BlipColor.Blue : BlipColor.Yellow;
        }


        /// <summary>
        /// Update team related information.
        /// </summary>
        public static void Update()
        {
            if (Enabled)
            {
                var scores = _activeTeams.Select(x => x.Score).ToArray();
                //update team score UI
                for (int i = 0; i < TeamCount; i++)
                {
                    if (_activeTeams[i].Score > 0 && _activeTeams[i].Score == scores.Max())
                    {
                        if (_activeTeams[i].Score > 100)
                        {
                            UIManager.ShowWinnerInfoUI(_activeTeams[i]);
                            Enabled = false;
                            break;
                        }

                        if (!_activeTeams[i].InControl)
                        {
                            _activeTeams[i].InControl = true;
                            string teamCl = GetColorFromTeamIndex(_activeTeams[i].Index).Name;
                            UIManager.NotifyWithWait(string.Format("{0} took the lead", teamCl), 5000);    
                        }
                    }

                    else
                        _activeTeams[i].InControl = false;

                    UIManager.UpdateTeamInfoProgressBar(i, scores[i]);
                }
            }
        }

        /// <summary>
        /// Reset all team scores.
        /// </summary>
        public static void ResetAllScores()
        {
            for (int i = 0; i < _activeTeams.Length; i++)
                _activeTeams[i].Score = 0;
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
