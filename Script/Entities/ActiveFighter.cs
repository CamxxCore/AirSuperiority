using GTA;
using GTA.Native;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;

namespace AirSuperiority.Script.Entities
{
    public class ActiveFighter
    {
        private ManageablePed ped;
        private ManageableVehicle vehicle;
        private TeamInfo team;

        /// <summary>
        /// Managed ped object associated to this fighter.
        /// </summary>
        public ManageablePed ManagedPed
        {
            get { return ped; }
            set { ped = value; }
        }

        /// <summary>
        /// Managed vehicle object associated to this fighter.
        /// </summary>
        public ManageableVehicle ManagedVehicle
        {
            get { return vehicle; }
            set { vehicle = value; }
        }

        /// <summary>
        /// Fighters team.
        /// </summary>
        public TeamInfo Team
        {
            get { return team; }
        }

        /// <summary>
        /// State of the fighters landing gear.
        /// </summary>
        public LandingGearState LandingGearState
        {
            get { return (LandingGearState)Function.Call<int>(Hash._GET_VEHICLE_LANDING_GEAR, vehicle.Handle); }
            set { Function.Call(Hash._SET_VEHICLE_LANDING_GEAR, vehicle.Handle, (int)value); }
        }

        /// <summary>
        /// Active vehicle mission ID.
        /// </summary>
        public int VehicleMissionType
        {
            get { return Function.Call<int>((Hash)0x534AEBA6E5ED4CAB, vehicle.Handle); }
        }

        /// <summary>
        /// If the ped is a local player/ human.
        /// </summary>
        public bool IsLocalPlayer
        {          
            get { return (ped.Handle == Game.Player.Character.Handle); }
        }

        /// <summary>
        /// Assign a ManageablePed and ManageableVehicle object to this fighter.
        /// </summary>
        /// <param name="Pilot"></param>
        /// <param name="Vehicle"></param>
        /// <returns></returns>
        public ActiveFighter Manage(ManageablePed Pilot, ManageableVehicle Vehicle)
        {
            ped = Pilot;
            vehicle = Vehicle;
            return this;
        }

        /// <summary>
        /// Fight against the specifed fighter.
        /// </summary>
        /// <param name="fighter"></param>
        public void FightAgainst(ActiveFighter fighter)
        {
         //   ManagedPed.
            ManagedPed.Ped.Task.FightAgainst(fighter.ManagedPed.Ped);
        }

        /// <summary>
        /// Assign this fighter a team.
        /// </summary>
        /// <param name="newTeam"></param>
        public void AssignTeam(TeamInfo newTeam)
        {
            team = newTeam;
        }

        /// <summary>
        /// Update ped and vehicle related information.
        /// </summary>
        public virtual void Update()
        {
            ped.Update();
            vehicle.Update();
        }

        /// <summary>
        /// Removes the ped and vehicle from the world.
        /// </summary>
        public void Remove()
        {
            ped.Delete();
            ped.CurrentBlip?.Remove();
            vehicle.CurrentBlip?.Remove();
            vehicle.Delete();
        }
    }
}