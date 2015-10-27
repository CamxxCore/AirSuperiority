using GTA;
using AirSuperiority.EntityManagement;

namespace AirSuperiority.Script.Entities
{
    public class ActiveFighter
    {
        private ManageablePed ped;
        private ManageableVehicle vehicle;

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
            ManagedPed.Ped.Task.FightAgainst(fighter.ManagedPed.Ped);
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
            vehicle.CurrentBlip?.Remove();
            vehicle.Delete();
        }
    }
}