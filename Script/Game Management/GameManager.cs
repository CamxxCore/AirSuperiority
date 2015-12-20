using System;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using AirSuperiority.Script.Entities;
using AirSuperiority.Script.EntityManagement;
using AirSuperiority.Script.UIManagement;

namespace AirSuperiority.Script.GameManagement
{
    public sealed class GameManager : GTA.Script
    {
        /// <summary>
        /// Instance of the local player.
        /// </summary>
        public static LocalFighter LocalPlayer { get; private set; } = new LocalFighter();

        public GameManager()
        {
            TeamManager.SetupRelationshipGroups();
            KeyDown += KeyPressed;
            Tick += OnTick;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Config.Ability1)
            {
                LocalPlayer.DoFireExtinguisher();
            }

            else if (e.KeyCode == Config.Ability2)
            {
                LocalPlayer.DoIRFlares();
            }
        }

        private static bool scriptActive = false;
        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                if (scriptActive)
                {
                    Game.Player.WantedLevel = 0;
                    Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.1f);

                    TeamManager.Update();
                    LocalPlayer.Update();

                    #region Update Fighters

                    var activeFighters = TeamManager.GetActiveFighters().ToList();

                    if (World.GetAllVehicles().Length < 100)
                    {
                        if (activeFighters.Count < Config.MaxPlayers)
                        {
                            AIFighter aiPlayer = new AIFighter();
                            TeamManager.SetupTeam(aiPlayer);
                            aiPlayer.Setup();
                        }
                    }

                    for (int i = 0; i < activeFighters.Count; i++)
                    {
                        var fighter = activeFighters[i];

                        if (fighter is LocalFighter) continue;

                        fighter.Update();

                        if (fighter.ManagedPed.IsDead || fighter.ManagedPed.IsInWater)
                        {
                            var GET_PED_KILLER = 0x93C8B64DEB84728C;
                            var mKiller = Function.Call<Entity>((Hash)GET_PED_KILLER, fighter.ManagedPed.Handle);

                            if (mKiller != null)
                            {
                                if (mKiller.Handle == LocalPlayer.ManagedPed.Handle ||
                                    mKiller.Handle == LocalPlayer.ManagedVehicle.Handle)
                                {
                                    if (fighter.Team.Index == LocalPlayer.Team.Index)
                                    {
                                        UIManager.DisplayKillInfoUI(fighter, 6000);
                                        UI.Notify("~r~Team Kill Penalty!");
                                        TeamManager.RegisterScoreForTeam(LocalPlayer.Team, -250); ;
                                    }

                                    else
                                    {
                                        UIManager.DisplayKillInfoUI(fighter, 6000);
                                        TeamManager.RegisterScoreForTeam(LocalPlayer.Team, 250);
                                        ExpRankManager.AddRankPoints(100, true);
                                    }
                                }

                                else
                                {
                                    foreach (var eFighter in activeFighters)
                                    {
                                        if (eFighter.Team.Index == fighter.Team.Index) continue;
                                        if (eFighter.ManagedPed.Handle == mKiller.Handle || eFighter.ManagedVehicle.Handle == mKiller.Handle)
                                        {
                                            TeamManager.RegisterScoreForTeam(eFighter.Team, 350);
                                        }
                                    }
                                }
                            }

                            else
                            {
                                var killer = activeFighters.Find(x => fighter.ManagedVehicle.HasBeenDamagedBy(x.ManagedPed));

                                if (killer != null)
                                {
                                    if (fighter.Team.Index == killer.Team.Index)
                                    {
                                        if (killer == LocalPlayer)
                                        {
                                            UIManager.DisplayKillInfoUI(fighter, 6000);
                                            UI.Notify("~r~Team Damage Penalty!");
                                            TeamManager.RegisterScoreForTeam(killer.Team, -250);
                                        }
                                    }

                                    else
                                    {
                                        if (killer == LocalPlayer)
                                        {
                                            UIManager.DisplayKillInfoUI(fighter, 6000);
                                            TeamManager.RegisterScoreForTeam(killer.Team, 250);
                                            ExpRankManager.AddRankPoints(100, true);
                                        }

                                        else
                                            TeamManager.RegisterScoreForTeam(killer.Team, 250);
                                    }
                                }
                            }

                            //stop updating this fighter.
                            TeamManager.StopUpdate(fighter);
                            fighter.MarkAsNoLongerNeeded();
                        }

                        else if (fighter.ManagedPed.IsInWater)
                        {
                            //stop updating this fighter.
                            TeamManager.StopUpdate(fighter);
                            fighter.MarkAsNoLongerNeeded();
                        }
                    }
                    #endregion
                }
            }

            catch
            {
                //notify user of the error.
                UI.Notify("~r~Air Superiority experienced a crash.");
                //continue throwing the exception.
                throw;
            }
        }

        public static void InitializeScript()
        {
            Scripts.FadeOutScreen(1500, 800);
            World.GetAllVehicles().ToList().ForEach(x => x.Delete());
            Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, true);
            Function.Call(Hash._0x8BF907833BE275DE, 2.0f, 2.0f);

            UIManager.Enabled = true;
            TeamManager.Enabled = true;

            TeamManager.GetNewTeams();

            LocalPlayer.Manage(new ManageablePed(Game.Player.Character), null);

            TeamManager.SetupTeam(LocalPlayer);

            LocalPlayer.Setup();

            Scripts.FadeInScreen(0, 1900);

            scriptActive = true;
        }

        public static void StopScript()
        {
            UIManager.UnloadActiveData();
            UIManager.Enabled = false;

            TeamManager.UnloadActiveData();
            TeamManager.Enabled = false;

            LocalPlayer.Unload();
            scriptActive = false;

            Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_DM_COUNTDOWN_KILL");
        }

        protected override void Dispose(bool A_0)
        {
            if (scriptActive) StopScript();
            TeamManager.RemoveRelationshipGroups();
            base.Dispose(A_0);
        }
    }
}
