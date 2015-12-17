using GTA;
using GTA.Math;
using GTA.Native;
using System.Linq;
using System.Collections.Generic;
using AirSuperiority.Script.EntityManagement;
using AirSuperiority.Script.GameManagement;

namespace AirSuperiority.Script.Entities
{
    public sealed class AIConvoy
    {
        private GroundAI leader;
        private List<GroundAI> children = new List<GroundAI>();

        public GroundAI Leader { get { return leader; } }      
        public List<GroundAI> Children {  get { return children; } }

        public AIConvoy Setup(int children)
        {
            leader = new GroundAI();
            TeamManager.SetupTeam(this);

            var spawnPos = leader.Team.GroundSpawn;
            var pedModel = new Model(PedHash.Pilot02SMM);
            var vehModel = new Model(VehicleHash.Barracks);
                    
            var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, spawnPos));

            vehicle.Vehicle.EngineRunning = true;
            vehicle.Vehicle.Heading = leader.Team.GroundHeading;

            vehicle.AddBlip();
            vehicle.CurrentBlip.Sprite = BlipSprite.BountyHit;

            var ped = new ManageablePed(World.CreatePed(pedModel, spawnPos));
            ped.Ped.RelationshipGroup = leader.Team.RelationshipGroup;
            ped.Ped.BlockPermanentEvents = false;    
            ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);

            leader.Manage(ped, vehicle);

            AddChildren(children);

            return this;
        }

        public void AddChildren(int count = 0)
        {
            var spawnPos = leader.Team.GroundSpawn;
            var pedModel = new Model(PedHash.Pilot02SMM);
            var vehModel = new Model(VehicleHash.Barracks3);  

            if (!pedModel.IsLoaded)
                pedModel.Request(1000);

            if (!vehModel.IsLoaded)
                vehModel.Request(1000);

            for (int i = 0; i < count; i++)
            {
                var parent = children.Count > 0 ? children.Last() : leader ;

                var childAsset = new GroundAI();
                var pos = parent.ManagedVehicle.Position - parent.ManagedVehicle.ForwardVector * 6;
                var vehicle = new ManageableVehicle(World.CreateVehicle(vehModel, pos));

                vehicle.Vehicle.EngineRunning = true;
                vehicle.Vehicle.Heading = leader.Team.GroundHeading;
                vehicle.AddBlip();
                vehicle.CurrentBlip.Sprite = BlipSprite.BountyHit;

                Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, vehicle.Handle, true);

                var ped = new ManageablePed(new Ped(Function.Call<int>(Hash.CREATE_PED_INSIDE_VEHICLE, vehicle.Handle, 6, pedModel.Hash, -1, 0, 0)));
                ped.Ped.RelationshipGroup = leader.Team.RelationshipGroup;
                ped.Ped.BlockPermanentEvents = false;
                ped.Ped.SetIntoVehicle(vehicle.Vehicle, VehicleSeat.Driver);

                childAsset.Manage(ped, vehicle);
                children.Add(childAsset);
            }
        }

        public void StartConvoyDrivingRoutine(Vector3 destination)
        {
            leader.DriveTo(destination);

            for (int i = 0; i < children.Count; i++)
            {
                if (i == 0)
                    children[i].StartFollow(leader);
                else
                children[i].StartFollow(children[i - 1]);

            }
        }

        public void Update()
        {
            if (leader.ManagedVehicle.Position.DistanceTo(new Vector3(-248.9207f, -752.2429f, 25.35142f)) == 100f)
            {
                UI.Notify(string.Format("{0} team's ground asset's have almost reached the city.", TeamManager.GetColorFromTeamIndex(leader.Team.Index)));
                           
            }
        }

        public void RemoveChildren()
        {
            children.ForEach(x => x.Remove());
            children.Clear();
        }
    }
}
