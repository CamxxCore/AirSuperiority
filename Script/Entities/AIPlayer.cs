using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Linq;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;

namespace AirSuperiority.Script.Entities
{
    public sealed class AIPlayer : ActiveFighter
    {
        /// <summary>
        /// Setup fighter for the AI player.
        /// </summary>
        public void Setup()
        {
            var pedModel = new Model(PedHash.Pilot02SMM);
            var vehModel = new Model(VehicleHash.Lazer);

            var spawnPos = GetValidSpawnPos();

            if (!pedModel.IsLoaded)
                pedModel.Request(1000);

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, spawnPos));
            vehicle.Heading = Team.SpawnInfo.Item2;
            vehicle.Vehicle.EngineRunning = true;
            vehicle.AddBlip();
            vehicle.CurrentBlip.Sprite = BlipSprite.Plane;

            vehicle.CurrentBlip.Color = Team.Index == 0 ? 
                BlipColor.Red : Team.Index == 1 ? 
                BlipColor.Green : Team.Index == 3 ? 
                BlipColor.Blue : BlipColor.Yellow;

            ManagedVehicle = vehicle;

            var ped = new ManageablePed(World.CreatePed(pedModel, spawnPos));
            ped.Ped.BlockPermanentEvents = true;
            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);
            ped.Ped.Task.FightAgainstHatedTargets(-1);

            ManagedPed = ped;
        }

        /// <summary>
        /// Generate a random spawn position for AI
        /// </summary>
        /// <returns></returns>
        private Vector3 GetValidSpawnPos()
        {
            for (int i = 0; i < 20; i++)
            {
                for (int x = 0; x < 15; x++)
                {
                    var pos = Team.SpawnInfo.Item1.Around(30 * i);

                    if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, pos.X, pos.Y, pos.Z, 5f, 5f, 5f, 0) &&
                        !World.GetAllVehicles().Where(y => y.Model == VehicleHash.Lazer).Any(v => v.Position.DistanceTo(pos) < 30f))
                    {
                        return pos;
                    }
                }
            }

            return default(Vector3);
        }

        public override void Update()
        {
            if (LandingGearState != (LandingGearState.Closing | LandingGearState.Retracted))
            {
                LandingGearState = LandingGearState.Closing;
            }

            base.Update();
        }

        /// <summary>
        /// Unload player related things.
        /// </summary>
        public void Unload()
        {
        }
    }
}
