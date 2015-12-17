using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using AirSuperiority.Script.Entities;
using AirSuperiority.Script.EntityManagement;
using AirSuperiority.Script.GameManagement;
using AirSuperiority.Types;
using GTA.Math;

namespace AirSuperiority.Script.Entities
{
    public class ObjectiveBlimp : ObjectiveEntity
    {
        /// <summary>
        /// Setup fighter for the AI player.
        /// </summary>
        public void Setup()
        {
            var vehModel = new Model(VehicleHash.Blimp);

            OutputArgument outVec = new OutputArgument(), outFlt = new OutputArgument();

            var pos = TeamManager.CenterMap;

            Function.Call(Hash.GET_RANDOM_VEHICLE_NODE, pos.X, pos.Y, pos.Z, 1200f, 1, 1, 1, outVec, outFlt);

            var spawnPos = Scripts.GetValidSpawnPos(outVec.GetResult<Vector3>() + new Vector3(0, 0, 400f));

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, spawnPos));
            vehicle.LodDistance = 2000;
            vehicle.Heading = outFlt.GetResult<float>();
            vehicle.Vehicle.EngineRunning = true;
            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle.Handle, true);

            vehicle.Vehicle.BodyHealth = 1000;
            vehicle.MaxHealth = 1000;
            vehicle.MaxSpeed = 0;


            var blip = vehicle.AddBlip();
            blip.Sprite = BlipSprite.Blimp;
            blip.Scale = 0.8f;
            blip.Alpha = 220;


            var cl = TeamManager.GetColorFromTeamIndex(Team.Index);

            Function.Call(Hash.SET_BLIP_SECONDARY_COLOUR, blip.Handle, cl.R, cl.G, cl.B);
            Function.Call((Hash)0xB81656BC81FE24D1, blip.Handle, 1);

            ManagedVehicle = vehicle;
        }

    }
}
