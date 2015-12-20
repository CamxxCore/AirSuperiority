using GTA;
using GTA.Native;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;

namespace AirSuperiority.Script.Entities
{
    public abstract class ActiveFighter
    {
        private ManageablePed ped;
        private ManageableVehicle vehicle;
        private ActiveTeamData team;

        public ActiveFighter ActiveTarget { get; private set; }

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
        /// Entity team.
        /// </summary>
        public ActiveTeamData Team
        {
            get { return team; }
        }

        /// <summary>
        /// If the ped is a local player/ human.
        /// </summary>
        public bool IsLocalPlayer
        {          
            get { return (ped.Handle == Game.Player.Character.Handle); }
        }

        /// <summary>
        /// If the ped is in the assigned vehicle.
        /// </summary>
        public bool IsInVehicle
        {
            get { return (ped.Ped.IsInVehicle(vehicle.Vehicle)); }
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
            ManagedPed.Ped.Task.FightAgainst(fighter.ManagedPed.Ped);
        }

        /// <summary>
        /// Clear all ped tasks.
        /// </summary>
        public void ClearTasks()
        {
            ManagedPed.Ped.Task.ClearAll();
        }

        /// <summary>
        /// Assign this fighter a team.
        /// </summary>
        /// <param name="newTeam"></param>
        public void AssignTeam(ActiveTeamData newTeam)
        {
            team = newTeam;
        }

        /// <summary>
        /// Set active target for vehicle mission natives.
        /// </summary>
        /// <param name="fighter"></param>
        public void SetActiveTarget(ActiveFighter fighter)
        {
            var pos = fighter.ManagedPed.Position;
            Function.Call(Hash.TASK_PLANE_MISSION,
                ManagedPed.Handle,
                ManagedVehicle.Handle,
                fighter.ManagedVehicle.Handle,
                fighter.ManagedPed.Handle,
                pos.X, pos.Y, pos.Z,
                6, 70.0, -1.0, 30.0, 500, 50);

            ActiveTarget = fighter;
        }

        /// <summary>
        /// Set active target for vehicle mission natives.
        /// </summary>
        /// <param name="fighter"></param>
        public void ClearActiveTarget()
        {
            ActiveTarget = null;
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
            vehicle.CurrentBlip?.Remove();
            ped.Delete();
            vehicle.Delete();
        }

        /// <summary>
        /// Set the ped and vehicle as no longer needed. Let the engine cleanup.
        /// </summary>
        public void MarkAsNoLongerNeeded()
        {
            vehicle.CurrentBlip?.Remove();
            ped.MarkAsNoLongerNeeded();
            vehicle.MarkAsNoLongerNeeded();
        }
    }
}