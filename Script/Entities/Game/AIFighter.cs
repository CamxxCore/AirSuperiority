using GTA;
using GTA.Math;
using GTA.Native;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;
using AirSuperiority.Script.GameManagement;
using AirSuperiority.Script.UIManagement;

namespace AirSuperiority.Script.Entities
{
    public sealed class AIFighter : ActiveFighter
    {
        private const float MaxSpeed = 125;
        private const int MaxHealth = 1;

        private bool entityHUDEnabled = false;

        private Timer losChecker = new Timer(6500);

        /// <summary>
        /// Setup fighter for the AI player.
        /// </summary>
        public void Setup()
        {
            var pedModel = new Model(PedHash.Pilot02SMM);

            var vehModel = new Model(new VehicleHash[] {
                VehicleHash.Lazer,
                VehicleHash.Besra,
                VehicleHash.Hydra
            }.GetRandomItem());

            var spawnPos = Scripts.GetValidSpawnPos(Team.JetSpawnPoint.Position);

            if (!pedModel.IsLoaded)
                pedModel.Request(1000);

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, spawnPos));
            vehicle.LodDistance = 2000;
            vehicle.Heading = Team.JetSpawnPoint.Heading;
            vehicle.Vehicle.EngineRunning = true;
            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle.Handle, true);

            vehicle.Vehicle.BodyHealth = 0.01f;
            vehicle.MaxHealth = MaxHealth;

            vehicle.LandingGearState = LandingGearState.Retracted;

            var blip = vehicle.AddBlip();
            blip.Sprite = BlipSprite.Jet;
            blip.Scale = 0.8f;
            blip.Alpha = 220;

            Function.Call(Hash.SET_BLIP_SECONDARY_COLOUR, blip.Handle, Team.Color.R, Team.Color.G, Team.Color.B);
            Function.Call((Hash)0xB81656BC81FE24D1, blip.Handle, 1);

            ManagedVehicle = vehicle;

            var ped = new ManageablePed(World.CreatePed(pedModel, spawnPos));
            ped.Ped.RelationshipGroup = Team.RelationshipGroup;

            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 1, 1);
            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped.Handle, 52, 0);
            Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped.Ped.Handle, false);

            ManagedPed = ped;

            losChecker.Enabled = true;
        }

        bool wentTooFar = false;
        public override void Update()
        {
            if (losChecker.Enabled && Game.GameTime > losChecker.Waiter)
            {
                var localPlayer = GameManager.LocalPlayer;

                if (ActiveTarget != null &&
                    !Function.Call<bool>(Hash.IS_PED_IN_COMBAT, ManagedPed.Handle, ActiveTarget.ManagedPed.Handle))
                {
                    ClearActiveTarget();
                }

                if (Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 1000) > 700 &&
                    Function.Call<bool>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY, ManagedPed.Handle, localPlayer.ManagedVehicle.Handle, 17) &&
                    ManagedVehicle.Position.DistanceTo(localPlayer.ManagedVehicle.Position) < 180f &&
                    Team.Index != localPlayer.Team.Index &&
                    ActiveTarget == null)
                {
                    SetActiveTarget(localPlayer);
                }

                losChecker.Reset();
            }

            if (entityHUDEnabled)
            {
                UIManager.DrawEntityHUD(this);
            }

            var outArg = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, ManagedPed.Handle, outArg))
            {
                var weaponHash = Function.Call<int>(Hash.GET_HASH_KEY, "vehicle_weapon_plane_rocket");

                if (outArg.GetResult<int>() != weaponHash)
                    Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, ManagedPed.Handle, weaponHash);
            }

            if (ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) > 2400f)
            {
                Team.ActiveFighters.Remove(this);
                Remove();
                return;
            }

            if (ManagedVehicle.MissionType == VehicleTask.Injured && ManagedVehicle.Vehicle.Speed < 70f)
            {
                ManagedVehicle.Vehicle.Speed++;
            }

            if (ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) > 700f && !wentTooFar)
            {
                if (ActiveTarget == null)
                {
                    var pos = TeamManager.CenterMap;
                    Function.Call(Hash.TASK_PLANE_MISSION,
                                ManagedPed.Handle,
                                ManagedVehicle.Handle,
                                0,
                                0,
                                pos.X, pos.Y, pos.Z,
                                6, 5.0, 15.0, 30.0, 500, 80);
                }
                       
                wentTooFar = true;
            }

            else if (wentTooFar && ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, ManagedVehicle.Position.Z)) < 700f)
            {
                if (!ManagedPed.Ped.IsInCombat)
                {
                    ManagedPed.Ped.Task.FightAgainstHatedTargets(2000f);
                }

                wentTooFar = false;
            }

            base.Update();
        }

        public void SetEntityHUDEnabled()
        {
            entityHUDEnabled = true;
        }

        /// <summary>
        /// Unload player related things.
        /// </summary>
        public void Unload()
        {
        }
    }
}
