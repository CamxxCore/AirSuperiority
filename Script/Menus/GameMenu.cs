using System;
using System.Windows.Forms;
using NativeUI;
using AirSuperiority.Script.GameManagement;
using GTA;

namespace AirSuperiority.Script.Menus
{
    public class GameMenu : GTA.Script
    {
        private readonly UIMenu mainMenu, statsMenu;
        private readonly MenuPool menuPool;

        public GameMenu()
        {
            mainMenu = new UIMenu("Air Superiority", "BETA");
            var menuItem = new UIMenuItem("~g~Start Script");
            menuItem.Activated += (x, y) => { GameManager.InitializeScript(); menuPool.CloseAllMenus(); };
            mainMenu.AddItem(menuItem);
            menuItem = new UIMenuItem("~r~Stop Script");
            menuItem.Activated += (x, y) => GameManager.StopScript();
            mainMenu.AddItem(menuItem);
            menuItem = new UIMenuItem("View Stats");
            mainMenu.AddItem(menuItem);
            statsMenu = new UIMenu("Player Stats", Game.Player.Name);
            menuItem = new UIMenuItem("Load Stats");
            menuItem.Activated += (x, y) => LoadStatsMenu();
            statsMenu.AddItem(menuItem);
            menuItem = new UIMenuItem("Player Kills: --");
            menuItem.Enabled = false;
            statsMenu.AddItem(menuItem);
            menuItem = new UIMenuItem("Player Deaths: --");
            menuItem.Enabled = false;
            statsMenu.AddItem(menuItem);
            menuItem = new UIMenuItem("Total Score: --");
            menuItem.Enabled = false;
            statsMenu.AddItem(menuItem);
            mainMenu.BindMenuToItem(statsMenu, mainMenu.MenuItems[2]);
            mainMenu.OnItemSelect += MainMenu_OnItemSelect;
            mainMenu.RefreshIndex();
            menuPool = new MenuPool();
            menuPool.Add(mainMenu);
            menuPool.Add(statsMenu);
            KeyDown += KeyPressed;
            Tick += OnTick;
        }

        private void LoadStatsMenu()
        {
            statsMenu.MenuItems.Clear();
            var statVal = PlayerStats.ReadPlayerStat("kills");
            var menuItem = new UIMenuItem(string.Format("Player Kills: ~y~{0}", statVal));
            menuItem.Enabled = false;
            statsMenu.AddItem(menuItem);
            statVal = PlayerStats.ReadPlayerStat("deaths");
            menuItem = new UIMenuItem(string.Format("Player Deaths: ~y~{0}", statVal));
            menuItem.Enabled = false;
            statsMenu.AddItem(menuItem);
            statVal = PlayerStats.ReadPlayerStat("score");
            menuItem = new UIMenuItem(string.Format("Player Score: ~y~{0}", statVal));
            menuItem.Enabled = false;
            statsMenu.AddItem(menuItem);

        }

        private void MainMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender == mainMenu && index == 2)
            {
                LoadStatsMenu();
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            menuPool.ProcessMenus();
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Config.ActivationKey)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
        }
    }
}
