using System;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Native;
using AirSuperiority.Types;
using AirSuperiority.Script.Entities;
using AirSuperiority.Script.GameManagement;
using Font = GTA.Font;

namespace AirSuperiority.Script.UIManagement
{
    public class UIManager : GTA.Script
    {
        /// <summary>
        /// Enable the UIText script.
        /// </summary>
        public static bool Enabled { get; set; } = false;

        private UIText centerText;
        private static Timer killInfoTimer, notifyTimer;
        private static Timer scaleformDisplayTimer;
        private static Timer scaleformFadeTimer;
        private static string _uiText1, _uiText2, _uiText3, _uiText4;
        private static UIBox killInfoUI;
        private static Scaleform scaleform;
        private UIContainer teamInfoHUD;

        private static string[] _activeHudAssets = new string[3];
        private static HUDAsset[] _hudAssets = new HUDAsset[3];

        private static int[] _teamScores = new int[TeamManager.TeamCount];
        private static string[] _teamNames = new string[TeamManager.TeamCount];
        private static string[] _teamImages = new string[TeamManager.TeamCount];

        public UIManager()
        {

            notifyTimer = new Timer(5000);
            killInfoTimer = new Timer(4500);
            scaleformDisplayTimer = new Timer(4000);
            scaleformFadeTimer = new Timer(2000);
            scaleform = new Scaleform(Function.Call<int>(Hash.REQUEST_SCALEFORM_MOVIE, "MP_BIG_MESSAGE_FREEMODE"));
            killInfoUI = new UIBox(new Point((Game.ScreenResolution.Width / 2) - (Game.ScreenResolution.Width / 10), (Game.ScreenResolution.Height - 40) - (Game.ScreenResolution.Height / 10)), new Size(200, 60));
            centerText = new UIText(null, new Point(Game.ScreenResolution.Width / 2, UI.HEIGHT - 38), 0.70f, Color.White, Font.ChaletComprimeCologne, true);
            teamInfoHUD = new UIContainer(new Point(Game.ScreenResolution.Width - Game.ScreenResolution.Width / 4, UI.HEIGHT / 6), new Size(180, 125), Color.FromArgb(180, Color.Black));
            /* teamInfoHUD.Items.Add(new UIRectangle(new Point(51, 13), new Size(120, 11), Color.Orange));
             teamInfoHUD.Items.Add(new UIRectangle(new Point(51, 43), new Size(120, 11), Color.Orange));
             teamInfoHUD.Items.Add(new UIRectangle(new Point(51, 73), new Size(120, 11), Color.Orange));
             teamInfoHUD.Items.Add(new UIRectangle(new Point(51, 103), new Size(120, 11), Color.Orange));
             teamInfoHUD.Items.Add(new UIRectangle(new Point(22, 12), new Size(21, 13), Color.FromArgb(180, Color.Green)));
             teamInfoHUD.Items.Add(new UIRectangle(new Point(22, 42), new Size(21, 13), Color.FromArgb(180, Color.Red)));
             teamInfoHUD.Items.Add(new UIRectangle(new Point(22, 72), new Size(21, 13), Color.FromArgb(180, Color.Blue)));
             teamInfoHUD.Items.Add(new UIRectangle(new Point(22, 102), new Size(21, 13), Color.FromArgb(180, Color.Yellow)));*/
            SetupTeamInfoHUD();
            _hudAssets[0] = new HUDAsset() { ActiveAsset= @"scripts\AirSuperiority\hud\fireext\1.png", InactiveAsset = @"scripts\AirSuperiority\hud\fireext\2.png" };
            _hudAssets[1] = new HUDAsset() { ActiveAsset = @"scripts\AirSuperiority\hud\irflares\1.png", InactiveAsset = @"scripts\AirSuperiority\hud\irflares\2.png" };
            _hudAssets[2] = new HUDAsset() { ActiveAsset = @"scripts\AirSuperiority\hud\rdrjam\1.png", InactiveAsset = @"scripts\AirSuperiority\hud\rdrjam\2.png" };
            _activeHudAssets = _hudAssets.Select(x => x.ActiveAsset).ToArray();
            SetHUDStatus(2, false);
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Enabled)
            {
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

                if (notifyTimer.Enabled && Game.GameTime > notifyTimer.Waiter)
                {
                    notifyTimer.Enabled = false;
                }

                centerText.Caption = string.Format("{0} {1} {2} {3}", _uiText1, _uiText2, _uiText3, _uiText4);
                centerText.Draw();


                for (int i = 0; i < TeamManager.TeamCount; i++)
                {
                    teamInfoHUD.Items[i] = new UIRectangle(new Point(50, 12 + (30 * i)), new Size(_teamScores[i], 11), Color.Orange);
                }

                if (GameManager.LocalPlayer.ManagedPed.IsAlive)
                {

                    for (int i = 0; i < TeamManager.TeamCount; i++)
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

                    if (GameManager.LocalPlayer.IsInVehicle)
                    DrawHUDAssets();
                }
            }
        }

        private void SetupTeamInfoHUD()
        {
            var colors = new Color[] { Color.Green, Color.Red, Color.Blue, Color.Yellow };

            for (int i = 0; i < TeamManager.TeamCount; i++)
            {
                teamInfoHUD.Items.Add(new UIRectangle(new Point(51, (30 * i) + 13), new Size(120, 11), Color.Orange));
           
            }

            for (int i = 0; i < TeamManager.TeamCount; i++)
            {
                teamInfoHUD.Items.Add(new UIRectangle(new Point(22, (30 * i) + 12), new Size(21, 13), Color.FromArgb(180, colors[i])));

            }

            //  
        }

        public static void SetHUDStatus(int index, bool active)
        {
            _activeHudAssets[index] = active ? _hudAssets[index].ActiveAsset : _hudAssets[index].InactiveAsset;
        }

        private static void DrawHUDAssets()
        {
            var val = -0.0045f;
            val *= Game.ScreenResolution.Width;
            UI.DrawTexture(_activeHudAssets[0], 0, 0, 100, new Point((int)val + 240, 602), new Size(24, 24));
            UI.DrawTexture(_activeHudAssets[1], 0, 0, 100, new Point((int)val + 240, 637), new Size(24, 24));
            UI.DrawTexture(_activeHudAssets[2], 0, 0, 100, new Point((int)val + 240, 672), new Size(24, 24));
        }

        public static void NotifyWithWait(string message, int timeout)
        {
            if (notifyTimer.Enabled) return;
            UI.Notify(message);
            notifyTimer.Interval = timeout;
            notifyTimer.Start();
        }

        public static void ShowWinnerInfoUI(ActiveTeamData team)
        {
            Function.Call(Hash.PLAY_MISSION_COMPLETE_AUDIO, "MICHAEL_BIG_01");
            Wait(1550);
            string str = team.Index == 0 ?
                            "~g~Green" : team.Index == 1 ?
                            "~r~Red" : team.Index == 2 ?
                            "~b~Blue" : "Yellow";
            scaleform.CallFunction("SHOW_MISSION_PASSED_MESSAGE", string.Format("{0} takes Victory!", str), "", 100, true, 0, true);
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
        /// Update the flag icon for the specified team.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="assetName"></param>
        public static void UpdateTeamInfoFlagAsset(int team, string assetName)
        {
            _teamImages[team] = assetName;
        }

        /// <summary>
        /// Update the score progress bar for the specified team.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="score"></param>
        public static void UpdateTeamInfoProgressBar(int team, int score)
        {
            _teamScores[team] = score % 120;
        }

        /// <summary>
        /// Update the friendly name of the specified team.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="name"></param>
        public static void UpdateTeamInfoFriendlyName(int team, string name)
        {
            _teamNames[team] = name;
        }

        /// <summary>
        /// Append UI text.
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
        /// Clear all UI text.
        /// </summary>
        public static void Clear()
        {
            _uiText1 = _uiText2 = _uiText3 = _uiText4 = null;
        }
    }
}
