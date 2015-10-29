using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Linq;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;

namespace AirSuperiority.Script.Entities
{
    public sealed class LocalPlayer : ActiveFighter
    {
        private InterpolatingCamera interpCam;
        private Timer boostTimer = new Timer(5000);

        /// <summary>
        /// Setup fighter for the local player.
        /// </summary>
        public void Setup()
        {
            AssignTeam(TeamManager.ActiveTeams.GetRandomItem());

            var vehModel = new Model(VehicleHash.Lazer);
            var spawnPos = Team.SpawnInfo.Item1;

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            //Create the vehicle and assign it to a ManageableVehicle.
            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, Team.SpawnInfo.Item1));
            vehicle.Heading = Team.SpawnInfo.Item2;
            vehicle.Vehicle.EngineRunning = true;
            vehicle.IsInvincible = true;          
            ManagedVehicle = vehicle;

            var ped = new ManageablePed(Game.Player.Character);
            ped.Position = Team.SpawnInfo.Item1;
            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);
            ManagedPed = ped;
            ManagedPed.EnterWater += ManagedPed_EnterWater;
            ManagedPed.Alive += ManagedPed_Alive;

            interpCam = new InterpolatingCamera(vehicle.GetOffsetInWorldCoords(new Vector3(-2f, -2f, 10f)));
            interpCam.MainCamera.PointAt(vehicle);
            interpCam.Start();

            boostTimer.Start();
        }

        private void ManagedPed_Alive(object sender, EntityChangedEventArgs e)
        {
       //     Setup();
            UI.ShowSubtitle("setup");
        }

        private void ManagedPed_EnterWater(object sender, EntityChangedEventArgs e)
        {
            Scripts.FadeOutScreen(1500, 2000);
            Setup();
            Scripts.FadeInScreen(500, 1000);
        }

        public override void Update()
        {
            if (interpCam != null && !Function.Call<bool>(Hash.IS_CAM_RENDERING, interpCam.MainCamera.Handle))
            {               
                LandingGearState = LandingGearState.Closing;
                UI.ShowSubtitle("destroyed");
                interpCam.Destroy();
                interpCam = null;
            }

            if (boostTimer.Enabled)
            {
                if (Game.GameTime > boostTimer.Waiter)
                {
                    ManagedVehicle.IsInvincible = false;
                    boostTimer.Enabled = false;
                }

                else if (new Control[] { Control.MoveUpDown, Control.MoveLeftRight }.Any(x => Game.IsControlPressed(0, x)))
                    boostTimer.Enabled = false;

                else
                    ManagedVehicle.ApplyForce(ManagedVehicle.ForwardVector * 2);
            }
        
            base.Update();
        }

        /// <summary>
        /// Unload player related things.
        /// </summary>
        public void Unload()
        {
            ManagedVehicle?.Delete();
        }
    }
}
