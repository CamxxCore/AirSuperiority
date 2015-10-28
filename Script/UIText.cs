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
    public class UIText : GTA.Script
    {
        /// <summary>
        /// Enable the UIText script.
        /// </summary>
        public static bool Enabled { get; set; } = false;

        private GTA.UIText centerText;
        private UIContainer teamInfoHUD;
        private static string _UIText1, _UIText2, _UIText3, _UIText4;
        private static string UITextConcat = null;

        public UIText()
        {
            centerText = new GTA.UIText(null,
                new Point(Game.ScreenResolution.Width / 2, UI.HEIGHT - 38),
                0.70f,
                Color.White,
                Font.Monospace,
                true);
            teamInfoHUD = new UIContainer(new Point(Game.ScreenResolution.Width - Game.ScreenResolution.Width / 4, UI.HEIGHT / 6), new Size(130, 117), Color.Black);
            teamInfoHUD.Items.Add(new GTA.UIText("Team A:", new Point(10, 2), 0.4f, Color.White, GTA.Font.Monospace, false));
            teamInfoHUD.Items.Add(new GTA.UIText("Team B:", new Point(10, 32), 0.4f, Color.White, GTA.Font.Monospace, false));
            teamInfoHUD.Items.Add(new GTA.UIText("Team C:", new Point(10, 62), 0.4f, Color.White, GTA.Font.Monospace, false));
            teamInfoHUD.Items.Add(new GTA.UIText("Team D:", new Point(10, 92), 0.4f, Color.White, GTA.Font.Monospace, false));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(55, 7), new Size(30, 11), Color.Blue));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(55, 37), new Size(30, 11), Color.Blue));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(55, 67), new Size(30, 11), Color.Blue));
            teamInfoHUD.Items.Add(new GTA.UIRectangle(new Point(55, 97), new Size(30, 11), Color.Blue));
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Enabled)
            {
                teamInfoHUD.Draw();
                UITextConcat = string.Format("{0} {1} {2} {3}", _UIText1, _UIText2, _UIText3, _UIText4);
                centerText.Caption = UITextConcat;
                centerText.Draw();
            }
        }

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

        public static void Clear()
        {
            _UIText1 = _UIText2 = _UIText3 = _UIText4 = null;
        }
    }
}
