using System;
using System.Windows.Forms;
using NativeUI;
using AirSuperiority.Data;
using AirSuperiority.Script.GameManagement;

namespace AirSuperiority.Script.Menus
{
    public class GameMenu : GTA.Script
    {
        private readonly UIMenu mainMenu;
        private readonly MenuPool menuPool;
        private readonly Keys ActivationKey;

        public GameMenu()
        {
            ActivationKey = INIHelper.GetConfigSetting<Keys>("KeyBinds", "ActivateScript");
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

        private void OnTick(object sender, EventArgs e)
        {
            menuPool.ProcessMenus();
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == ActivationKey)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }
    }
}
