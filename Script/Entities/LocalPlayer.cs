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
            AssignTeam(TeamManager.GetTeamByIndex(0));

            var vehModel = new Model(VehicleHash.Lazer);

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            var spawnPos = Team.TeamInfo.SpawnInfo.Item1;
            var spawnHeading = Team.TeamInfo.SpawnInfo.Item2;

            //Create the vehicle and assign it to a ManageableVehicle.
            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, spawnPos));
            vehicle.Heading = spawnHeading;
            vehicle.Vehicle.EngineRunning = true;
            vehicle.IsInvincible = true;          
            ManagedVehicle = vehicle;

            ManagedVehicle.EnterWater += ManagedVehicle_EnterWater;

            var ped = new ManageablePed(Game.Player.Character);
            ped.Ped.RelationshipGroup = Team.RelationshipGroup;
           // ped.Position = spawnPos;
            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);
            ManagedPed = ped;

            interpCam = new InterpolatingCamera(vehicle.GetOffsetInWorldCoords(new Vector3(-2f, -2f, 10f)));
            interpCam.MainCamera.PointAt(vehicle);
            interpCam.Start();

            boostTimer.Start();
        }


        private void ManagedVehicle_EnterWater(object sender, EntityChangedEventArgs e)
        {
            Scripts.FadeOutScreen(1500, 2000);
            Setup();
            Scripts.FadeInScreen(500, 1000);
        }

        public override void Update()
        {
            if (ManagedPed.IsDead)
            {

                GTA.Script.Wait(5000);
                Setup();
            }

            else
            {
                if (interpCam != null && !Function.Call<bool>(Hash.IS_CAM_RENDERING, interpCam.MainCamera.Handle))
                {
                    LandingGearState = LandingGearState.Closing;
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
                    {
                        ManagedVehicle.IsInvincible = false;
                        boostTimer.Enabled = false;
                    }

                    else
                        ManagedVehicle.ApplyForce(ManagedVehicle.ForwardVector * 1f);
                }
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
