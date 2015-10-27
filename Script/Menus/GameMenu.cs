using System;
using System.Windows.Forms;
using NativeUI;

namespace AirSuperiority.Script.Menus
{
    public class GameMenu : GTA.Script
    {
        private readonly UIMenu mainMenu;
        private readonly MenuPool menuPool;

        public GameMenu()
        {
            mainMenu = new UIMenu("Air Superiority", "BETA");
            var menuItem = new UIMenuItem("~g~Start Script", null);
            menuItem.Activated += (x, y) => { GameManager.InitializeScript(); menuPool.CloseAllMenus(); };
            mainMenu.AddItem(menuItem);
            menuItem = new UIMenuItem("~r~Stop Script", null);
            menuItem.Activated += (x, y) => GameManager.StopScript();
            mainMenu.AddItem(menuItem);
            menuPool = new MenuPool();
            menuPool.Add(mainMenu);
            KeyDown += KeyPressed;
            Tick += OnTick;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.K)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            menuPool.ProcessMenus();
        }
    }
}
