using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using AirSuperiority.Types;
using AirSuperiority.Script.Entities;
using AirSuperiority.Script.EntityManagement;

namespace AirSuperiority.Script
{
    public sealed class GameManager : GTA.Script
    {
        private static bool scriptActive = false;
        private List<ActiveFighter> activeFighters = new List<ActiveFighter>();
        private int[] teamScores = new int[4];

        /// <summary>
        /// Instance of the local player.
        /// </summary>
        public static LocalPlayer LocalPlayer { get; private set; } = new LocalPlayer();

        public GameManager()
        {
            TeamManager.SetupRelationshipGroups();
            LocalPlayer.Manage(new ManageablePed(Game.Player.Character), null);
            activeFighters.Add(LocalPlayer);
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                if (scriptActive)
                {
                    TeamManager.Update();
                    LocalPlayer.Update();

                    //spawn ai as necessary
                    if (activeFighters.Count < 50 && World.GetAllVehicles().Length < 100)
                    {
                        //initialize the managed entities in ActiveFighter wrapper
                        AIPlayer aiPlayer = new AIPlayer();
                        //setup team for this ai fighter.
                        TeamManager.SetupFighterTeam(aiPlayer);
                        aiPlayer.Setup();
                        activeFighters.Add(aiPlayer);
                    }

                    //update game based logic that can't be handled by the entity.
                    for (int i = 0; i < activeFighters.Count; i++)
                    {
                        var fighter = activeFighters[i];

                        if (fighter is LocalPlayer) continue;

                        fighter.Update();

                        if (fighter.ManagedPed.IsDead)
                        {
                            var mKiller = Function.Call<Entity>((Hash)0x93C8B64DEB84728C, fighter.ManagedPed.Handle);

                            if (mKiller != null)
                            {
                                var result = activeFighters.Find(item => mKiller.Handle == item.ManagedPed.Handle);

                                if (result != null)
                                    TeamManager.RegisterScoreForTeam(result.Team.Index, 100);

                                result = activeFighters.Find(item => mKiller.Handle == item.ManagedVehicle.Handle);

                                if (result != null)
                                    TeamManager.RegisterScoreForTeam(result.Team.Index, 100);
                            }

                            //stop updating this fighter.
                            fighter.MarkAsNoLongerNeeded();
                            fighter.Team.Members.Remove(fighter);
                            activeFighters.RemoveAt(i);
                        }

                        else if (fighter.ManagedPed.IsInWater)
                        {
                            //stop updating this fighter.
                            fighter.MarkAsNoLongerNeeded();
                            fighter.Team.Members.Remove(fighter);
                            activeFighters.RemoveAt(i);
                        }
                    }
                }

                else
                {
                    if (activeFighters.Count > 0)
                    {
                        activeFighters.ForEach(x =>
                        {
                            if (!(x is LocalPlayer))
                            {
                                x.Remove();
                            }
                        });

                        activeFighters.Clear();
                    }
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

            UIManager.Enabled = true;
            TeamManager.GetNewTeams();
            
            LocalPlayer.Setup();
            scriptActive = true;
        }

        public static void StopScript(bool force = false)
        {
            // Scripts.FadeOutScreen(1500, 800);   
            UIManager.Enabled = false;
            scriptActive = false;
            LocalPlayer.Unload();
            //  Scripts.FadeInScreen(3000, 1900);
        }

        protected override void Dispose(bool A_0)
        {
            StopScript();
            activeFighters.ForEach(x => x.Remove());
            TeamManager.RemoveRelationshipGroups();
            base.Dispose(A_0);
        }
    }
}
