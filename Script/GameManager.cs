using System;
using System.Linq;
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

        private List<ActiveFighter> activeQueue = new List<ActiveFighter>();

        /// <summary>
        /// Instance of the local player. Inherits from ActiveFighter.
        /// </summary>
        public static LocalPlayer LocalPlayer { get; private set; } = new LocalPlayer();

        public GameManager()
        {
            LocalPlayer.Manage(new ManageablePed(Game.Player.Character), null);
            activeFighters.Add(LocalPlayer);
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (scriptActive)
            {

                if (LocalPlayer.ManagedPed.IsDead)
                {
                   StopScript();
                }

                else
                    LocalPlayer.Update();

                //check for ai awaiting placement in entity list
                if (activeQueue.Count > 0)
                {
                    activeFighters.AddRange(activeQueue);
                    activeQueue.Clear();
                }

                //spawn ai as necessary
                if (activeFighters.Count < 15 && World.GetAllVehicles().Length < 100)
                {

                    //initialize the managed entities in ActiveFighter wrapper
                    AIPlayer aiPlayer = new AIPlayer();

                    //setup team for this ai fighter.
                    TeamManager.SetupFighterTeam(aiPlayer, activeFighters);

                    aiPlayer.Setup();

                    //fight against a random fighter or the local player if non exist.
                   // aiPlayer.ManagedPed.Ped.Task.FightAgainstHatedTargets(-1);

                    activeFighters.Add(aiPlayer);
                    UI.ShowSubtitle("add ai " + aiPlayer.ManagedVehicle.Position);
                }
                
                //update game based logic that can't be handled by the entity.
                for (int i = 0; i < activeFighters.Count; i++)
                {
                    var fighter = activeFighters[i];

                    //only perform this logic on AI players.
                    if (fighter is LocalPlayer) continue;

                    if (fighter.ManagedPed.IsDead || fighter.ManagedVehicle.IsDead)
                    {
                        //stop updating this fighter.
                        fighter.ManagedPed.MarkAsNoLongerNeeded();
                        fighter.ManagedVehicle.MarkAsNoLongerNeeded();
                        fighter.LandingGearState = LandingGearState.Closing;
                        activeFighters.RemoveAt(i);
                        UI.Notify("dead");
                    }

                    else
                    {
                        //   if (fighter.ManagedPed.Position.DistanceTo(LocalPlayer.ManagedPed.Position) > 1000f)
                        //  {
                        //fighter too far, remove from world and stop updating.
                        //     fighter.Remove();
                        //      activeFighters.RemoveAt(i);
                        //   }

                        // else
                        // {
                        //fighter isnt in combat, find a new opponent.
                        if (fighter.VehicleMissionType == 0)
                        {
                            fighter.FightAgainst(activeFighters.GetRandomItem() ?? LocalPlayer);
                            //fighter.FightAgainst(activeFighters.OrderBy(x => x.ManagedVehicle.Position.DistanceTo(fighter.ManagedVehicle.Position)).First());
                            UI.ShowSubtitle("find new opponent");
                        }

                            //update base
                          fighter.Update();
                      //  }
                    }
                }                        
            }

            else
            {
                if (activeQueue.Count > 0)
                {
                    activeQueue.ForEach(x => {
                        if (!x.IsLocalPlayer)
                        {
                            x.Remove();
                        }
                    });

                    activeQueue.Clear();
                }

                if (activeFighters.Count > 0)
                {
                    activeFighters.ForEach(x => {
                        if (!x.IsLocalPlayer)
                        {
                            x.Remove();
                        }
                    });

                    activeFighters.Clear();
                }
            }
        }

        public static void InitializeScript()
        {
            UIText.Enabled = true;

            TeamManager.GetNewTeams();
            
            //     Scripts.FadeOutScreen(3000, 1900);
            LocalPlayer.Setup();
         //   LocalPlayer.AssignTeam(TeamA);
            scriptActive = true;
        //    Scripts.FadeInScreen(1500, 800);
        }

        public static void StopScript(bool force = false)
        {
            // Scripts.FadeOutScreen(1500, 800);   

            scriptActive = false;
            LocalPlayer.Unload();
           // Scripts.DisableHospitalRestart(true);
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
