using System;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Native;
using AirSuperiority.Types;
using AirSuperiority.Script.Entities;
using AirSuperiority.Script.GameManagement;
using System.Threading.Tasks;
using System.Threading;
using Font = GTA.Font;

namespace AirSuperiority.Script.UIManagement
{
    public class UIManager : GTA.Script
    {
        /// <summary>
        /// Enable the UIText script.
        /// </summary>
        public static bool Enabled { get; set; } = false;

        public static UIRankBar RankBar {  get { return rankBar; } }

        private UIText centerText;
        private static Timer killInfoTimer, notifyTimer;
        private static Timer scaleformDisplayTimer;
        private static Timer scaleformFadeTimer;
        private static string _uiText1, _uiText2, _uiText3, _uiText4;
        private static UIBox killInfoUI;
        private static Scaleform scaleform;
        private UIContainer teamInfoHUD;

        private static UIRankBar rankBar = new UIRankBar();
        private static string[] _activeHudAssets = new string[3];
        private static HUDAsset[] _hudAssets = new HUDAsset[3];

        private static int[] _teamScores = new int[Config.MaxTeams];
        private static string[] _teamNames = new string[Config.MaxTeams];
        private static string[] _teamImages = new string[Config.MaxTeams];

        public UIManager()
        {
            var bounds = Game.ScreenResolution;
            notifyTimer = new Timer(5000);
            killInfoTimer = new Timer(4500);
            scaleformDisplayTimer = new Timer(4000);
            scaleformFadeTimer = new Timer(2000);
            scaleform = new Scaleform(Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, "MP_BIG_MESSAGE_FREEMODE"));
            killInfoUI = new UIBox(new Point(bounds.Width / 2 - (bounds.Width / 2) / 2 + 200, (Game.ScreenResolution.Height - 40) - (Game.ScreenResolution.Height / 10)), new Size(200, 60));
            centerText = new UIText(null, new Point(bounds.Width / 2 - (bounds.Width / 2) / 2, UI.HEIGHT - 38), 0.70f, Color.White, Font.ChaletComprimeCologne, true);
            teamInfoHUD = new UIContainer(new Point((int)0.00115f * Game.ScreenResolution.Width + 960, UI.HEIGHT / 6 - 20), new Size(180, Config.MaxTeams * 31), Color.FromArgb(180, Color.Black));
            SetupTeamInfoHUD(Config.MaxTeams);
            _hudAssets[0] = new HUDAsset() { ActiveAsset = @"scripts\AirSuperiority\hud\fireext\1.png", InactiveAsset = @"scripts\AirSuperiority\hud\fireext\2.png" };
            _hudAssets[1] = new HUDAsset() { ActiveAsset = @"scripts\AirSuperiority\hud\irflares\1.png", InactiveAsset = @"scripts\AirSuperiority\hud\irflares\2.png" };
            _hudAssets[2] = new HUDAsset() { ActiveAsset = @"scripts\AirSuperiority\hud\rdrjam\1.png", InactiveAsset = @"scripts\AirSuperiority\hud\rdrjam\2.png" };
            _activeHudAssets = _hudAssets.Select(x => x.ActiveAsset).ToArray();
            SetHUDIcon(2, false);
            rankBar.RankedUp += RankBar_RankedUp;
            Tick += OnTick;
        }

        private void RankBar_RankedUp(UIRankBar sender, UIRankBarEventArgs e)
        {
            Game.PlaySound("MEDAL_UP", "HUD_MINI_GAME_SOUNDSET");
        }

        public static void UnloadActiveData()
        {
            Array.Clear(_teamScores, 0, _teamScores.Length);
            Array.Clear(_teamNames, 0, _teamNames.Length);
            Array.Clear(_teamImages, 0, _teamImages.Length);
        }

        /// <summary>
        /// Update UI related items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e)
        {
            if (Enabled)
            {
                centerText.Caption = string.Format("{0} {1} {2} {3}", _uiText1, _uiText2, _uiText3, _uiText4);
                centerText.Draw();

                if (scaleformDisplayTimer.Enabled)
                {
                    scaleform.Render2D();

                    if (Game.GameTime > scaleformDisplayTimer.Waiter)
                    {
                        scaleform.CallFunction("TRANSITION_OUT");
                        scaleformDisplayTimer.Enabled = false;
                        scaleformFadeTimer.Start();
                    }
                }

                if (scaleformFadeTimer.Enabled)
                {
                    scaleform.Render2D();

                    if (Game.GameTime > scaleformFadeTimer.Waiter)
                    {
                        GameManager.StopScript();
                        scaleformFadeTimer.Enabled = false;
                    }
                }

                if (killInfoTimer.Enabled)
                {
                    killInfoUI.Draw();

                    if (Game.GameTime > killInfoTimer.Waiter)
                    {
                        if (killInfoUI.Color.A > 0)
                            killInfoUI.Color = Color.FromArgb(killInfoUI.Color.A - 5, killInfoUI.Color);
                        else
                        {
                            killInfoUI.Color = Color.FromArgb(255, killInfoUI.Color);
                            killInfoTimer.Enabled = false;
                        }
                    }
                }

                for (int i = 0; i < Config.MaxTeams; i++)
                {
                    teamInfoHUD.Items[i] = new UIRectangle(new Point(50, 12 + (30 * i)), new Size(_teamScores[i], 11), Color.Orange);
                }

                if (GameManager.LocalPlayer.ManagedPed.IsAlive)
                {
                    for (int i = 0; i < Config.MaxTeams; i++)
                    {
                        if (System.IO.File.Exists(_teamImages[i]))
                        {
                            UI.DrawTexture(_teamImages[i], 0, 0, 100,
                                new Point(teamInfoHUD.Position.X + 10, teamInfoHUD.Position.Y + 2 + 30 * i),
                                new Size(24, 24));
                        }
                        else
                        {
                            var text = new UIText(_teamNames[i] + ":",
                                new Point(teamInfoHUD.Position.X + 10, teamInfoHUD.Position.Y + 2 + 30 * i),
                                0.4f, Color.White, Font.Monospace, false);

                            text.Draw();
                        }
                    }

       
                    teamInfoHUD.Draw();
                    DrawHUDIcons();
                }
            }

            rankBar.Update();
        }

        /// <summary>
        /// Setup the team info HUD, based on the amount of teams specifed.
        /// </summary>
        /// <param name="maxTeams">Max amount of teams</param>
        private void SetupTeamInfoHUD(int maxTeams)
        {
            for (int i = 0; i < maxTeams; i++)
            {
                teamInfoHUD.Items.Add(new UIRectangle(new Point(51, (30 * i) + 13), new Size(120, 11), Color.FromArgb(180, Color.Orange)));
           
            }

            for (int i = 0; i < maxTeams; i++)
            {
                var cl = TeamManager.GetColorFromTeamIndex(i);
                teamInfoHUD.Items.Add(new UIRectangle(new Point(22, (30 * i) + 12), new Size(21, 13), Color.FromArgb(180, cl)));
            }
        }

        public static void DrawEntityHUD(ActiveFighter fighter)
        {
            var pos = fighter.ManagedPed.Position;
            Function.Call(Hash.SET_DRAW_ORIGIN, pos.X, pos.Y, pos.Z, 0);
            DrawSquare(new Point(0, -32), Color.Red, string.Format("dist: {0}", Game.Player.Character.Position.DistanceTo(pos)));
            Function.Call(Hash.CLEAR_DRAW_ORIGIN);
        }

        private static UIRectangle rect;
        private static UIText entityText;

        private static void DrawSquare(Point location, Color color, string subText)
        {
            rect = new UIRectangle(new Point(location.X - 25, location.Y), new Size(4, 52));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X + 25, location.Y), new Size(4, 52));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 25, location.Y + 50), new Size(52, 4));
            rect.Color = color;
            rect.Draw();
            rect = new UIRectangle(new Point(location.X - 25, location.Y), new Size(52, 4));
            rect.Color = color;
            rect.Draw();
            entityText = new UIText(subText, new Point(location.X - 5, location.Y + 70), 0.3f, Color.White, Font.ChaletComprimeCologne, false);
            entityText.Draw();
         //   entityText = new UIText(subText1, new Point(location.X - 30, location.Y + 70), 0.22f);
        //    entityText.Draw();
        }

        /// <summary>
        /// Set that status of a player ability 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="active"></param>
        public static void SetHUDIcon(int index, bool active)
        {
            _activeHudAssets[index] = active ? _hudAssets[index].ActiveAsset : _hudAssets[index].InactiveAsset;
        }

        /// <summary>
        /// Draw interactive player ability HUD assets.
        /// </summary>
        private static void DrawHUDIcons()
        {
            var val = -0.0045f;
            val *= Game.ScreenResolution.Width;
            UI.DrawTexture(_activeHudAssets[0], 0, 0, 100, new Point((int)val + 240, 602), new Size(24, 24));
            UI.DrawTexture(_activeHudAssets[1], 0, 0, 100, new Point((int)val + 240, 637), new Size(24, 24));
            UI.DrawTexture(_activeHudAssets[2], 0, 0, 100, new Point((int)val + 240, 672), new Size(24, 24));
        }

        /// <summary>
        /// Show the UI for team victory, given the specified winner.
        /// </summary>
        /// <param name="team">Target team</param>
        public static void ShowWinnerInfoUI(ActiveTeamData team)
        {
            Function.Call(Hash.PLAY_MISSION_COMPLETE_AUDIO, "MICHAEL_BIG_01");
            Wait(1550);
            scaleform.CallFunction("SHOW_MISSION_PASSED_MESSAGE", string.Format("{0} takes Victory!", team.Color.Name), "", 100, true, 0, true);
            scaleformDisplayTimer.Start();
        }

        /// <summary>
        /// Draw the kill info UI for the time given in milliseconds.
        /// </summary>
        public static void DisplayKillInfoUI(ActiveFighter fighter, int timeout, bool teamKilled = false)
        {
            killInfoUI.AddFighterInfo(fighter);
            killInfoTimer.Interval = timeout;
            killInfoTimer.Start();
        }

        /// <summary>
        /// Update friendly name in the UI for the specified team.
        /// </summary>
        /// <param name="team"></param>
        public static void UpdateTeamInfoFriendlyName(ActiveTeamData team)
        {
            _teamNames[team.Index] = team.TeamInfo.FriendlyName;
        }

        /// <summary>
        /// Update the UI flag icon for the specified team.
        /// </summary>
        /// <param name="team">Target team</param>
        public static void UpdateTeamInfoFlagAsset(ActiveTeamData team)
        {
            _teamImages[team.Index] = team.TeamInfo.ImageAsset;
        }

        /// <summary>
        /// Interpolate team score UI from the current value to the desired new value
        /// </summary>
        /// <param name="team">Target team</param>
        /// <param name="score">Total new score</param>
        /// <returns></returns>
        private static int InterpolateTeamScore(int team, int score)
        {
            var value = _teamScores[team];

            for (int i = value; i < score; i++)
            {
                _teamScores[team] = i;
                Thread.Sleep(100);
            }

            return _teamScores[team];
        }

        /// <summary>
        /// Add a team score interpolation to the queue.
        /// </summary>
        /// <param name="team">Target team</param>
        /// <param name="score">Amount of score to add</param>
        /// <returns></returns>
        public static async Task<int> QueueTeamInfoProgressBar(ActiveTeamData team, int score)
        {
            return await Task.Factory.StartNew(() => InterpolateTeamScore(team.Index, team.Score + score));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeout"></param>
        public static void NotifyWithWait(string message, int timeout)
        {
            if (notifyTimer.Enabled) return;
            UI.Notify(message);
            notifyTimer.Interval = timeout;
            notifyTimer.Start();
        }

        /// <summary>
        /// Append subtitle UI text.
        /// </summary>
        /// <param name="text"></param>
        public static void Append(string text)
        {
            if (_uiText1 == null)
            {
                _uiText1 = text;
            }

            else if (_uiText2 == null)
            {
                _uiText2 = text;
            }

            else if (_uiText3 == null)
            {
                _uiText3 = text;
            }

            else if (_uiText4 == null)
            {
                _uiText4 = text;
            }

            else return;
        }

        /// <summary>
        /// Clear all subtitle UI text.
        /// </summary>
        public static void Clear()
        {
            _uiText1 = _uiText2 = _uiText3 = _uiText4 = null;
        }
    }
}
