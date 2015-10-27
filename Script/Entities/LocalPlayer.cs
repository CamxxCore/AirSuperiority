using GTA;
using GTA.Math;
using GTA.Native;
using System;
using AirSuperiority.EntityManagement;

namespace AirSuperiority.Script.Entities
{
    public sealed class LocalPlayer : ActiveFighter
    {
        /// <summary>
        /// Setup fighter for the local player.
        /// </summary>
        public void Setup()
        {
            var vehModel = new Model(VehicleHash.Lazer);
            var vehPos = GetRandomSpawnPos();

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            //Create the vehicle and assign it to a ManageableVehicle.
            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, vehPos.Item1));
            vehicle.Heading = vehPos.Item2;
            vehicle.Vehicle.EngineRunning = true;
            vehicle.ApplyForce(vehicle.ForwardVector * 2);

            var ped = new ManageablePed(Game.Player.Character);
            ped.Position = vehPos.Item1;
            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);

            ManagedVehicle = vehicle;
            ManagedPed = ped;
        }

        /// <summary>
        /// Get random item from known spawn locations.
        /// </summary>
        /// <returns></returns>
        private Tuple<Vector3, float> GetRandomSpawnPos()
        {
            return new Tuple<Vector3, float>[] {
                new Tuple<Vector3, float>(new Vector3(-3232.42f, -1755.673f, 226.5322f), 292.7778f),
                new Tuple<Vector3, float>(new Vector3(2287.492f, 7661.41f, 579.9888f), 162.8432f),
                new Tuple<Vector3, float>(new Vector3(3220.731f, 7935.237f, 421.6384f), 160.8841f)
            }.GetRandomItem();
        }

        /// <summary>
        /// Unload player related things.
        /// </summary>
        public void Unload()
        { }
    }
}
