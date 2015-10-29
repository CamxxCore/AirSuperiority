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
            var spawnHeading = Team.TeamInfo.SpawnInfo.Item2;

            if (!pedModel.IsLoaded)
                pedModel.Request(1000);

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, spawnPos));
            vehicle.LodDistance = 1000;
            vehicle.Heading = spawnHeading;
            vehicle.Vehicle.EngineRunning = true;
            vehicle.AddBlip();
            vehicle.CurrentBlip.Sprite = BlipSprite.Plane;

            vehicle.CurrentBlip.Color = Team.Index == 0 ?
                BlipColor.Red : Team.Index == 1 ?
                BlipColor.Green : Team.Index == 3 ?
                BlipColor.Blue : BlipColor.Yellow;



            ManagedVehicle = vehicle;
            LandingGearState = LandingGearState.Closing;
            var ped = new ManageablePed(World.CreatePed(pedModel, spawnPos));
            ped.Ped.RelationshipGroup = Team.RelationshipGroup;
            ped.Ped.BlockPermanentEvents = true;
            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);
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

                var pos = Team.TeamInfo.SpawnInfo.Item1.Around(20 * i);

                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, pos.X, pos.Y, pos.Z, 5f, 5f, 5f, 0) &&
                    !World.GetAllVehicles().Where(y => y.Model == VehicleHash.Lazer).Any(v => v.Position.DistanceTo(pos) < 20f))
                {
                    return pos;
                }
            }

            return default(Vector3);
        }

        public override void Update()
        {
            // Function.Call(Hash.SET_BLIP_ROTATION, ManagedVehicle.CurrentBlip?.Handle, (float) Math.Round(ManagedVehicle.Heading));

            if (LandingGearState != LandingGearState.Retracted)
            {
                LandingGearState = LandingGearState.Closing;
            }

            //fighter isn't in combat, find a new opponent.
            if (VehicleMissionType == 0)
            {
                ManagedPed.Ped.Task.FightAgainstHatedTargets(100000f);
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
