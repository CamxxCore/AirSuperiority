using System.Drawing;
using GTA;
using AirSuperiority.Script.Entities;

namespace AirSuperiority.Script.UIManagement
{
    public class UIBox : UIContainer
    {
        private string imageAsset;
        private Point position;
        private Size size;
        
        public UIBox(Point position, Size size) : base(position, size, Color.FromArgb(240, Color.Black))
        {
            this.position = position;
            this.size = size;
        }

        /// <summary>
        /// Setup the UI box for the specified fighter
        /// </summary>
        /// <param name="fighter"></param>
        /// <returns></returns>
        public UIBox AddFighterInfo(ActiveFighter fighter)
        {
            Items.Clear();
            Items.Add(new UIText("AI Player", new Point(14, 16), 0.4f, Color.White, GTA.Font.ChaletComprimeCologne, false));
            Items.Add(new UIText(fighter.Team.TeamInfo.FriendlyName, new Point(14, 35), 0.4f, Color.White, GTA.Font.ChaletComprimeCologne, false));
            Items.Add(new UIText("KILLED", new Point(78, 5), 0.34f, Color.White, GTA.Font.Monospace, false));
            Items.Add(new UIRectangle(new Point(166, 40), new Size(21, 13), Color.FromArgb(180, fighter.Team.Color)));
            imageAsset = fighter.Team.TeamInfo.AltImageAsset;
            return this;
        }

        /// <summary>
        /// Draw the UI box.
        /// </summary>
        public override void Draw()
        {
            if (System.IO.File.Exists(imageAsset))
            UI.DrawTexture(imageAsset, 0, 0, 100, new Point(Position.X + 158, Position.Y + 28), new Size(24, 24));
            base.Draw();
        }
    }
}
