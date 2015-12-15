using GTA;
using GTA.Math;
using GTA.Native;
using System.Linq;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;
using AirSuperiority.Script.GameManagement;

namespace AirSuperiority.Script.Entities
{
    public sealed class AIFighter : ActiveFighter
    {
        private const float MaxSpeed = 117;
        private const int MaxHealth = 1;

        /// <summary>
        /// Setup fighter for the AI player.
        /// </summary>
        public void Setup()
        {
            var pedModel = new Model(PedHash.Pilot02SMM);
            var vehModel = new Model(VehicleHash.Lazer);

            var spawnPos = GetValidSpawnPos(Team.FighterSpawn);

            if (!pedModel.IsLoaded)
                pedModel.Request(1000);

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, spawnPos));
            vehicle.LodDistance = 2000;
            vehicle.Heading = Team.SpawnHeading;
            vehicle.MaxSpeed = Function.Call<float>(Hash._GET_VEHICLE_MAX_SPEED, vehicle.Model.Hash);
            vehicle.Vehicle.EngineRunning = true;
            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle.Handle, true);
            vehicle.AddBlip();
            vehicle.CurrentBlip.Sprite = BlipSprite.Plane;
            vehicle.CurrentBlip.Color = TeamManager.GetBlipColorFromTeamIndex(Team.Index);
            vehicle.Vehicle.BodyHealth = 0.1f;
            vehicle.MaxHealth = 1;
            vehicle.MaxSpeed = MaxSpeed;
            ManagedVehicle = vehicle;

            LandingGearState = LandingGearState.Retracted;

            var ped = new ManageablePed(World.CreatePed(pedModel, spawnPos));
            ped.Ped.RelationshipGroup = Team.RelationshipGroup;

            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 1, 1);
            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 52, 0);

            Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped.Ped.Handle, false);

            ManagedPed = ped;
        }

        /// <summary>
        /// Generate a random spawn position for AI
        /// </summary>
        /// <returns></returns>
        private Vector3 GetValidSpawnPos(Vector3 position)
        {
            for (int i = 0; i < 20; i++)
            {

                var pos = position.Around(20 * i);

                if (!Function.Call<bool>(Hash.IS_POINT_OBSCURED_BY_A_MISSION_ENTITY, pos.X, pos.Y, pos.Z, 5f, 5f, 5f, 0) &&
                    !World.GetAllVehicles().Where(y => y.Model == VehicleHash.Lazer).Any(v => v.Position.DistanceTo(pos) < 20f))
                {
                    return pos;
                }
            }

            return position;
        }

        bool wentToFar = false;

        public override void Update()
        {
            var outArg = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, ManagedPed.Handle, outArg))
            {
                var weaponHash = Function.Call<int>(Hash.GET_HASH_KEY, "vehicle_weapon_plane_rocket");

                if (outArg.GetResult<int>() != weaponHash)
                Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, ManagedPed.Handle, weaponHash);
            }

            if (ManagedVehicle.Vehicle.Speed < 70f)
            {
                ManagedVehicle.Vehicle.Speed++;
            }

            if (ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) > 2000f)
            {
                Team.ActiveFighters.Remove(this);
                Remove();
                return;
            }

            if (ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) > 700f && !wentToFar)
            {
               Function.Call<bool>(Hash.TASK_VEHICLE_DRIVE_TO_COORD, ManagedPed.Handle, ManagedVehicle.Handle, -248.9207f, -752.2429f, 421.6384f, MaxSpeed, 1, ManagedVehicle.Model.Hash, 262144, -1.0, -1.0);
                wentToFar = true;
            }

            else if (wentToFar && ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) < 700f)
            {     
                Function.Call(Hash.REGISTER_HATED_TARGETS_AROUND_PED, ManagedPed.Handle, 1600f);
                ManagedPed.Ped.Task.FightAgainstHatedTargets(1600f);
               // Function.Call(Hash.TASK_COMBAT_HATED_TARGETS_AROUND_PED_TIMED, ManagedPed.Handle, -1, 1600f, 0);//ManagedPed.Ped.Task.FightAgainstHatedTargets(900f);
                wentToFar = false;
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
