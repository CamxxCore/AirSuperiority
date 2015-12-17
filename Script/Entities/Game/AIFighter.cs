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
        private const float MaxSpeed = 125;
        private const int MaxHealth = 1;

        private Timer losCheckWaiter = new Timer(5000);

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
            vehicle.Vehicle.EngineRunning = true;
            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle.Handle, true);

            vehicle.Vehicle.BodyHealth = 0.01f;
            vehicle.MaxHealth = MaxHealth;
            vehicle.MaxSpeed = MaxSpeed;

            vehicle.LandingGearState = LandingGearState.Retracted;

            var blip = vehicle.AddBlip();
            blip.Sprite = BlipSprite.Jet;
            blip.Scale = 0.8f;
            blip.Alpha = 220;

            var cl = TeamManager.GetColorFromTeamIndex(Team.Index);

            Function.Call(Hash.SET_BLIP_SECONDARY_COLOUR, blip.Handle, cl.R, cl.G, cl.B);
            Function.Call((Hash)0xB81656BC81FE24D1, blip.Handle, 1);

            ManagedVehicle = vehicle;

            var ped = new ManageablePed(World.CreatePed(pedModel, spawnPos));
            ped.Ped.RelationshipGroup = Team.RelationshipGroup;

            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 1, 1);
            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 52, 0);
            Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped.Ped.Handle, false);

            losCheckWaiter.Enabled = true;

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
            if (losCheckWaiter.Enabled && Game.GameTime > losCheckWaiter.Waiter)
            {
                var otherTeams = TeamManager.GetActiveFighters().Where(x => x.ManagedPed.Handle != ManagedPed.Handle);

                foreach (var fighter in otherTeams)
                    if (ManagedVehicle.Position.DistanceTo(fighter.ManagedVehicle.Position) < 250f &&
                        Team.Index != fighter.Team.Index &&
                        Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY, fighter.ManagedPed.Handle, ManagedVehicle.Handle))
                    {
                        ClearTasks();
                        var pos = fighter.ManagedPed.Position;
                        Function.Call(Hash.TASK_PLANE_MISSION,
                            ManagedPed.Handle,
                            ManagedVehicle.Handle,
                            fighter.ManagedVehicle.Handle,
                            fighter.ManagedPed.Handle,
                            pos.X, pos.Y, pos.Z,
                            6, 70.0, -1.0, 30.0, 500, 50);
                    }

                losCheckWaiter.Reset();
            }

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

            if (!ManagedPed.Ped.IsInCombat)
            {
                ManagedPed.Ped.Task.FightAgainstHatedTargets(10000f);
            }

            if (ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) > 2400f)
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
