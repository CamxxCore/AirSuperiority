using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using AirSuperiority.Script.Entities;
using AirSuperiority.EntityManagement;

namespace AirSuperiority.Script
{
    public sealed class GameManager : GTA.Script
    {
        /// <summary>
        /// Instance of the local player. Inherits from ActiveFighter.
        /// </summary>
        public static LocalPlayer LocalPlayer { get; private set; } = new LocalPlayer();

        /// <summary>
        /// All active participants.
        /// </summary>
        private List<ActiveFighter> activeFighters = new List<ActiveFighter>();

        /// <summary>
        /// Queued participants.
        /// </summary>
        private List<ActiveFighter> activeQueue = new List<ActiveFighter>();

        /// <summary>
        /// Whether the script is running. This shouldn't be changed outside of toggle script methods.
        /// </summary>
        private static bool scriptActive = false;

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
                    StopScript(); 

                //check for ai awaiting placement in entity list
                if (activeQueue.Count > 0)
                {
                    activeFighters.AddRange(activeQueue);
                    activeQueue.Clear();
                }

                //spawn ai as necessary
                if (activeFighters.Count < 15)
                {
                    var spawnPos = Scripts.GetRandomPositionNearEntity(LocalPlayer.ManagedPed, 2f);
                    var aiData = CreateAI(spawnPos);

                    //initialize the managed entities in ActiveFighter wrapper
                    ActiveFighter aiPlayer = new ActiveFighter();
                    aiPlayer.Manage(aiData.Item1, aiData.Item2);
                    aiPlayer.ManagedVehicle.AddBlip();
                    aiPlayer.ManagedVehicle.CurrentBlip.Sprite = BlipSprite.Plane;

                    if (activeFighters.Count > 1)
                    aiPlayer.FightAgainst(activeFighters.GetRandomItem());

                    activeFighters.Add(aiPlayer);

                    UI.ShowSubtitle("add ai " + aiPlayer.ManagedVehicle.Position);
                }
                
                for (int i = 0; i < activeFighters.Count; i++)
                {
                    var fighter = activeFighters[i];

                    if (fighter.ManagedPed.IsDead || fighter.ManagedVehicle.IsDead)
                    {
                        //stop updating this fighter.
                        activeFighters.RemoveAt(i);
                        UI.Notify("dead");
                    }

                    else
                    {                      
                        fighter.Update();

                        if (!fighter.IsLocalPlayer)
                        {
                            if (fighter.ManagedPed.Position.DistanceTo(LocalPlayer.ManagedPed.Position) > 1000f && 
                                !Function.Call<bool>(Hash.IS_PED_IN_COMBAT, fighter.ManagedPed.Handle, LocalPlayer.ManagedPed.Handle))
                            {
                                fighter.FightAgainst(LocalPlayer);
                                UI.ShowSubtitle("too far");
                            }

                            if (fighter.ManagedVehicle.VehicleMissionType == 0)
                            {
                                fighter.FightAgainst(activeFighters.GetRandomItem());
                                UI.ShowSubtitle("find new opponent");
                            }
                        }
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

        /// <summary>
        /// Generate a random spawn position for AI
        /// </summary>
        /// <returns></returns>
        private Vector3 GenerateRandomSpawnPosition()
        {
            var player = LocalPlayer.ManagedPed;

            Vector3[] directionalVectors = new Vector3[] { player.ForwardVector,
                -player.ForwardVector,
                 player.RightVector,
                -player.RightVector
            };

            OutputArgument outPos = new OutputArgument();
            Vector3 randomDir = directionalVectors.GetRandomItem();

            Function.Call(Hash.FIND_SPAWN_POINT_IN_DIRECTION,
                player.Position.X,
                player.Position.Y,
                player.Position.Z,
                randomDir.X,
                randomDir.Y,
                randomDir.Z,
                200f,
                outPos);

            var vectorResult = outPos.GetResult<Vector3>();
            return new Vector3(vectorResult.X, vectorResult.Y, World.GetGroundHeight(new Vector2(vectorResult.X, vectorResult.Y)) + 200f);
        }

        private Tuple<ManageablePed, ManageableVehicle> CreateAI(Vector3 position)
        {
            var pedModel = new Model(PedHash.Pilot02SMM);
            var vehModel = new Model(VehicleHash.Lazer);

            if (!pedModel.IsLoaded)
                pedModel.Request(1000);

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            var aiPed = new ManageablePed(World.CreatePed(pedModel, position));
            var aiVehicle = new ManageableVehicle(World.CreateVehicle(vehModel, position));

            aiVehicle.Vehicle.EngineRunning = true;
            aiPed.Ped.BlockPermanentEvents = true;
            aiPed.Ped.SetIntoVehicle(aiVehicle.Vehicle, VehicleSeat.Driver);

            return new Tuple<ManageablePed, ManageableVehicle>(aiPed, aiVehicle);
        }

        public static void InitializeScript()
        {
       //     Scripts.FadeOutScreen(3000, 1900);
            LocalPlayer.Setup();            
            scriptActive = true;
        //    Scripts.FadeInScreen(1500, 800);
        }

        public static void StopScript(bool force = false)
        {
           // Scripts.FadeOutScreen(1500, 800);   
            scriptActive = false;                   
            LocalPlayer.Unload();
          //  Scripts.FadeInScreen(3000, 1900);
        }

        protected override void Dispose(bool A_0)
        {
            LocalPlayer.ManagedVehicle?.Delete();
            activeFighters.ForEach(x => {
                if (!x.IsLocalPlayer)
                {
                    x.Remove();
                }
            });

            base.Dispose(A_0);
        }
    }
}
