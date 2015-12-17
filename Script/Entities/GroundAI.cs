using GTA.Native;
using GTA.Math;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;

namespace AirSuperiority.Script.Entities
{
    public class GroundAI
    {
        private ManageablePed ped;
        private ManageableVehicle vehicle;
        private ActiveTeamData team;

        /// <summary>
        /// Managed ped object associated to this ground asset.
        /// </summary>
        public ManageablePed ManagedPed
        {
            get { return ped; }
            set { ped = value; }
        }

        /// <summary>
        /// Managed vehicle object associated to this ground asset.
        /// </summary>
        public ManageableVehicle ManagedVehicle
        {
            get { return vehicle; }
            set { vehicle = value; }
        }

        /// <summary>
        /// Ground asset team.
        /// </summary>
        public ActiveTeamData Team
        {
            get { return team; }
        }

        /// <summary>
        /// Active vehicle mission ID.
        /// </summary>
        public VehicleTask VehicleMissionType
        {
            get { return (VehicleTask)Function.Call<int>((Hash)0x534AEBA6E5ED4CAB, vehicle.Handle); }
        }

        /// <summary>
        /// Assign a ManageablePed and ManageableVehicle object to this ground asset.
        /// </summary>
        /// <param name="Pilot"></param>
        /// <param name="Vehicle"></param>
        /// <returns></returns>
        public GroundAI Manage(ManageablePed Driver, ManageableVehicle Vehicle)
        {
            ped = Driver;
            vehicle = Vehicle;
            return this;
        }

        /// <summary>
        /// Clear all ped tasks.
        /// </summary>
        public void ClearTasks()
        {
            ManagedPed.Ped.Task.ClearAllImmediately();
        }

        /// <summary>
        /// Assign this ground asset a team.
        /// </summary>
        /// <param name="newTeam"></param>
        public void AssignTeam(ActiveTeamData newTeam)
        {
            team = newTeam;
        }

        /// <summary>
        /// Follow another ground asset.
        /// </summary>
        /// <param name="asset"></param>
        public void StartFollow(GroundAI asset)
        {
            Function.Call(Hash._TASK_VEHICLE_FOLLOW, ManagedPed.Handle, ManagedVehicle.Handle, asset.ManagedPed.Handle, 40f, 262275, 10);
        }

        /// <summary>
        /// Drive to the location.
        /// </summary>
        /// <param name="asset"></param>
        public void DriveTo(Vector3 location)
        {
            Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, ManagedPed.Handle, ManagedVehicle.Handle, location.X, location.Y, location.Z, 40.0, 262275, 10.0);
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
