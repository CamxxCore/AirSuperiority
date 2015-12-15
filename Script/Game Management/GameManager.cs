using System;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using AirSuperiority.Script.Entities;
using AirSuperiority.Script.EntityManagement;
using AirSuperiority.Script.UIManagement;
using AirSuperiority.Data;


namespace AirSuperiority.Script.GameManagement
{
    public sealed class GameManager : GTA.Script
    {
        private readonly Keys Ability1, Ability2, Ability3;

        /// <summary>
        /// Instance of the local player.
        /// </summary>
        public static LocalFighter LocalPlayer { get; private set; } = new LocalFighter();

        public GameManager()
        {
            Ability1 = INIHelper.GetConfigSetting<Keys>("KeyBinds", "EngineRepair");
            Ability2 = INIHelper.GetConfigSetting<Keys>("KeyBinds", "Countermeasures");
            //Ability3 = INIHelper.GetConfigSetting<Keys>("KeyBinds", "");
            TeamManager.SetupRelationshipGroups();
            KeyDown += KeyPressed;
            Tick += OnTick;
        }

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Ability1)
            {
                LocalPlayer.DoFireExtinguisher();
            }

            else if (e.KeyCode == Ability2)
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
                    //   var groundAssets = TeamManager.GetActiveGroundAssets().ToList();
                    //groundAssets.ForEach(x => x.Update());

                    if (World.GetAllVehicles().Length < 100)
                    {

                        if (activeFighters.Count < TeamManager.PlayerCount)
                        {
                            AIFighter aiPlayer = new AIFighter();
                            TeamManager.SetupTeam(aiPlayer);
                            aiPlayer.Setup();
                        }

                        /*   else if (groundAssets.Count < 4)
                           {
                               AIConvoy convoy = new AIConvoy();
                               convoy.Setup(1);
                               convoy.StartConvoyDrivingRoutine(new Vector3(-248.9207f, -752.2429f, 25.35142f));
                           }*/
                    }

                    for (int i = 0; i < activeFighters.Count; i++)
                    {
                        var fighter = activeFighters[i];

                        if (fighter is LocalFighter) continue;

                        fighter.Update();

                        if (fighter.ManagedPed.IsDead)
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
                                        TeamManager.RegisterScoreForTeam(LocalPlayer.Team.Index, -500);
                                    }

                                    else
                                    {
                                        UIManager.DisplayKillInfoUI(fighter, 6000);
                                        TeamManager.RegisterScoreForTeam(LocalPlayer.Team.Index, 200);
                                    }
                                }

                                else
                                {
                                    foreach (var eFighter in activeFighters)
                                    {
                                        if (eFighter.Team.Index == fighter.Team.Index) continue;
                                        if (eFighter.ManagedPed.Handle == mKiller.Handle || eFighter.ManagedVehicle.Handle == mKiller.Handle)
                                        {
                                            TeamManager.RegisterScoreForTeam(eFighter.Team.Index, 250);
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
                                            UI.Notify("~r~Team Kill Penalty!");
                                            TeamManager.RegisterScoreForTeam(killer.Team.Index, -500);
                                        }
                                    }

                                    else
                                    {
                                        if (killer == LocalPlayer)
                                        {
                                            UIManager.DisplayKillInfoUI(fighter, 6000);
                                            TeamManager.RegisterScoreForTeam(killer.Team.Index, 200);
                                        }

                                        else
                                        TeamManager.RegisterScoreForTeam(killer.Team.Index, 250);
                                    }
                                }
                            }

                            //stop updating this fighter.
                            fighter.Team.ActiveFighters.Remove(fighter);
                            fighter.MarkAsNoLongerNeeded();
                        }

                        else if (fighter.ManagedPed.IsInWater)
                        {
                            //stop updating this fighter.
                            fighter.Team.ActiveFighters.Remove(fighter);
                            fighter.MarkAsNoLongerNeeded();
                        }

                        else
                        {
                            if (LocalPlayer.ManagedVehicle.HasBeenDamagedBy(fighter.ManagedPed.Ped))
                            {
                                fighter.ManagedPed.Ped.Task.ClearAll();
                                Function.Call(Hash.CLEAR_ENTITY_LAST_DAMAGE_ENTITY, LocalPlayer.ManagedVehicle.Vehicle);
                            }


                            else if (fighter.ManagedVehicle.Position.DistanceTo(LocalPlayer.ManagedVehicle.Position) < 180f &&
                                fighter.VehicleMissionType != Types.VehicleTask.InCombat &&
                                fighter.Team.Index != LocalPlayer.Team.Index &&
                                Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY, fighter.ManagedPed.Handle, LocalPlayer.ManagedVehicle.Handle, 17))
                            {
                                fighter.ClearTasks();

                                var pos = LocalPlayer.ManagedPed.Position;
                                Function.Call(Hash.TASK_PLANE_MISSION,
                                    fighter.ManagedPed.Handle,
                                    fighter.ManagedVehicle.Handle,
                                    LocalPlayer.ManagedVehicle.Handle,
                                    LocalPlayer.ManagedPed.Handle,
                                    pos.X, pos.Y, pos.Z,
                                    6, 70.0, -1.0, -1, 80, 80);
                            }
                        }
                    }

                    #endregion
                }
            }

            catch
            {
                //notify user of the error.
                UI.Notify(string.Format("~r~Air Superiority experienced a crash."));
                //continue throwing the exception.
                throw;
            }
        }

        public static void InitializeScript()
        {
            World.GetAllVehicles().ToList().ForEach(x => x.Delete());
            Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, true);
            Function.Call(Hash._0x8BF907833BE275DE, 2.0f, 2.0f);

            UIManager.Enabled = true;
            TeamManager.Enabled = true;

            TeamManager.GetNewTeams();

            LocalPlayer.Manage(new ManageablePed(Game.Player.Character), null);

            TeamManager.SetupTeam(LocalPlayer);

            LocalPlayer.Setup();

            scriptActive = true;
        }

        public static void StopScript()
        {
            // Scripts.FadeOutScreen(1500, 800);   
            scriptActive = false;
            UIManager.Enabled = false;
            TeamManager.Enabled = false;
            LocalPlayer.Unload();
            TeamManager.GetActiveFighters().ToList().ForEach(x => { if (x != LocalPlayer) x.Remove(); });
            TeamManager.GetActiveGroundAssets().ToList().ForEach( x => { x.Leader.Remove(); x.RemoveChildren(); });
            Function.Call(Hash.TRIGGER_MUSIC_EVENT, "MP_DM_COUNTDOWN_KILL");
            //  Scripts.FadeInScreen(3000, 1900);
        }

        protected override void Dispose(bool A_0)
        {
            if (scriptActive) StopScript();

            TeamManager.RemoveRelationshipGroups();
            base.Dispose(A_0);
        }
    }
}
