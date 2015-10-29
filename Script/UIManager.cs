using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GTA;
using GTA.Native;
using Font = GTA.Font;

namespace AirSuperiority.Script
{
    public class UIManager : GTA.Script
    {
        /// <summary>
        /// Enable the UIText script.
        /// </summary>
        public static bool Enabled { get; set; } = false;

        private GTA.UIText centerText;
        private UIContainer teamInfoHUD;
        private static string _UIText1, _UIText2, _UIText3, _UIText4;
        private static int[] _TeamScores = new int[4];
        private static string[] _TeamNames = new string[4];
        private static string[] _TeamImage = new string[4];
        private static string UITextConcat = null;

        public UIManager()
        {
            centerText = new GTA.UIText(null,
                new Point(Game.ScreenResolution.Width / 2, UI.HEIGHT - 38),
                0.70f,
                Color.White,
                Font.Monospace,
                true);
            teamInfoHUD = new UIContainer(new Point(Game.ScreenResolution.Width - Game.ScreenResolution.Width / 4, UI.HEIGHT / 6), new Size(130, 125), Color.FromArgb(180, Color.Black));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(48, 12), new Size(30, 11), Color.Blue));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(48, 42), new Size(30, 11), Color.Blue));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(48, 72), new Size(30, 11), Color.Blue));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(48, 102), new Size(30, 11), Color.Blue));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(22, 12), new Size(21, 13), Color.Red));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(22, 42), new Size(21, 13), Color.Green));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(22, 72), new Size(21, 13), Color.Blue));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(22, 102), new Size(21, 13), Color.Yellow));
            /* teamInfoHUD.Items.Add(new GTA.UIText("Team A:", new Point(10, 2), 0.4f, Color.White, GTA.Font.Monospace, false));
             teamInfoHUD.Items.Add(new GTA.UIText("Team B:", new Point(10, 32), 0.4f, Color.White, GTA.Font.Monospace, false));
             teamInfoHUD.Items.Add(new GTA.UIText("Team C:", new Point(10, 62), 0.4f, Color.White, GTA.Font.Monospace, false));
             teamInfoHUD.Items.Add(new GTA.UIText("Team D:", new Point(10, 92), 0.4f, Color.White, GTA.Font.Monospace, false));     */
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Enabled)
            {
                /* for (int i = 4; i < 8; i++)
                {
                    teamInfoHUD.Items[i] = new GTA.UIText(_TeamNames[i - 4] + ":", new Point(10, 2 + (30 * (i - 4))), 0.4f, Color.White, GTA.Font.Monospace, false);
                 }*/

                for (int i = 0; i < 4; i++)
                {
                    teamInfoHUD.Items[i] = new GTA.UIRectangle(new Point(48, 12 + (30 * i)), new Size(_TeamScores[i], 11), Color.Orange);
                }

                for (int i = 0; i < 4; i++)
                {
                    if (System.IO.File.Exists(_TeamImage[i]))
                        UI.DrawTexture(_TeamImage[i], 0, 0, 100, new Point(teamInfoHUD.Position.X + 10, teamInfoHUD.Position.Y + 2 + 30 * i), new Size(24, 24));
                    else
                    {
                       var text = new UIText(_TeamNames[i] + ":", new Point(teamInfoHUD.Position.X + 10, teamInfoHUD.Position.Y + 2 + 30 * i), 0.4f, Color.White, GTA.Font.Monospace, false);
                        text.Draw();
                    }
                }

                teamInfoHUD.Draw();
                UITextConcat = string.Format("{0} {1} {2} {3}", _UIText1, _UIText2, _UIText3, _UIText4);
                centerText.Caption = UITextConcat;
                centerText.Draw();
            }
        }

        /// <summary>
        /// Update the flag icon for the specified team.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="assetName"></param>
        public static void UpdateTeamInfoFlagAsset(int team, string assetName)
        {
            _TeamImage[team] = assetName;
        }

        /// <summary>
        /// Update the score progress bar for the specified team.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="score"></param>
        public static void UpdateTeamInfoProgressBar(int team, int score)
        {
            _TeamScores[team] = score;
        }

        /// <summary>
        /// Update the friendly name of the specified team.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="name"></param>
        public static void UpdateTeamInfoFriendlyName(int team, string name)
        {
            _TeamNames[team] = name;
        }

        /// <summary>
        /// Append UI text.
        /// </summary>
        /// <param name="text"></param>
        public static void Append(string text)
        {
            if (_UIText1 == null)
            {
                _UIText1 = text;
            }

            else if (_UIText2 == null)
            {
                _UIText2 = text;
            }

            else if (_UIText3 == null)
            {
                _UIText3 = text;
            }

            else if (_UIText4 == null)
            {
                _UIText4 = text;
            }

            else return;
        }

        /// <summary>
        /// Clear all UI text.
        /// </summary>
        public static void Clear()
        {
            _UIText1 = _UIText2 = _UIText3 = _UIText4 = null;
        }
    }
}
