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

        public const float SpawnDist = 0.2f;
        public const int MaxScore = 100;

        /// <summary>
        /// Contains information about active teams.
        /// </summary>
        private static ActiveTeamData[] _activeTeams = new ActiveTeamData[Config.MaxTeams];

        /// <summary>
        /// World relationship groups.
        /// </summary>
        private static List<int> rGroups = Enumerable.Range(0, Config.MaxTeams)
            .Select(i => World.AddRelationshipGroup(string.Format("team{0}", i))).ToList();

        /// <summary>
        /// Coordinate of map center.
        /// </summary>
        public static readonly Vector3 CenterMap = new Vector3(-248.9207f, -752.2429f, 421.6384f);

        /// <summary>
        /// World position and heading for team spawn locations.
        /// </summary>
        public static readonly SpawnPoint[] FighterSpawns = new SpawnPoint[] {
            new SpawnPoint() { Position = Vector3.Lerp(new Vector3(1350.13f, -596.34f, 481.6384f), CenterMap, SpawnDist), Heading = 96.70607f },
            new SpawnPoint() { Position = Vector3.Lerp(new Vector3(-1722.227f, -1353.99f, 481.6384f), CenterMap, SpawnDist), Heading = -72f },
            new SpawnPoint() { Position = Vector3.Lerp(new Vector3(-1563.28f, 60.707f, 481.6384f), CenterMap, SpawnDist), Heading = -120f },
            new SpawnPoint() { Position = Vector3.Lerp(new Vector3(-372.45f, 523.23f, 481.6384f), CenterMap, SpawnDist), Heading = 190f },
            new SpawnPoint() { Position = Vector3.Lerp(new Vector3(-161.3713f, -2634.05f, 481.6384f), CenterMap, SpawnDist), Heading = -10f },
            new SpawnPoint() { Position = Vector3.Lerp(new Vector3(505.179f, 128.99f, 481.6384f), CenterMap, SpawnDist), Heading = 120f },
            new SpawnPoint() { Position = Vector3.Lerp(new Vector3(-886.338f, -1694.8f, 481.6384f), CenterMap, SpawnDist), Heading = 0f }
        };

        /// <summary>
        /// World position and heading for team spawn locations.
        /// </summary>
        public static readonly SpawnPoint[] GroundSpawns = new SpawnPoint[] {
            new SpawnPoint() { Position = new Vector3(1318.396f, -557.2538f, 72.28039f), Heading = 58.57626f },
            new SpawnPoint() { Position = new Vector3(-1721.894f, -1094.84f, 13.07793f), Heading = 316.236f },
            new SpawnPoint() { Position = new Vector3(-1413.372f, 228.6124f, 215.7202f), Heading = 215.7202f },
            new SpawnPoint() { Position = new Vector3(-94.15818f, -2623.103f, 6.00071f), Heading = 269.5972f }
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

            for (int i = 0; i < Config.MaxTeams; i++)
            {
                _activeTeams[i] = default(ActiveTeamData);
                _activeTeams[i].TeamInfo = teamData.Where(x => !_activeTeams.Any(y => y.TeamInfo.FriendlyName == x.FriendlyName)).GetRandomItem();
                _activeTeams[i].Index = i;
                _activeTeams[i].RelationshipGroup = rGroups[i];
                _activeTeams[i].Score = 0;
                _activeTeams[i].FighterSpawn = FighterSpawns[i].Position;
                _activeTeams[i].SpawnHeading = FighterSpawns[i].Heading;
                _activeTeams[i].ActiveFighters = new List<ActiveFighter>();
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
        public static async void RegisterScoreForTeam(ActiveTeamData team, int score)
        {
            var tScore = score / 100;
            if (tScore > 0)
             team.Score = await UIManager.QueueTeamInfoProgressBar(team, tScore);
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
                case 4: return Color.Orange;
                case 5: return Color.Purple;
                case 6: return Color.Fuchsia;
                case 7: return Color.Gold;
                case 8: return Color.Gray;
                case 9: return Color.SkyBlue;
                default: return Color.Red;
            }
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
                for (int i = 0; i < Config.MaxTeams; i++)
                {
                    if (_activeTeams[i].Score > 0 && _activeTeams[i].Score == scores.Max())
                    {
                        if (_activeTeams[i].Score > MaxScore)
                        {
                            UIManager.ShowWinnerInfoUI(_activeTeams[i]);
                            Enabled = false;
                            break;
                        }

                        if (!_activeTeams[i].InControl)
                        {
                            _activeTeams[i].InControl = true;
                            string teamCl = GetColorFromTeamIndex(_activeTeams[i].Index).Name;
                            UIManager.NotifyWithWait(string.Format("{0} team took the lead", teamCl), 5000);
                            SoundManager.Step();
                        }
                    }

                    else
                        _activeTeams[i].InControl = false;
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
