using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirSuperiority.Types;
using AirSuperiority.Script.EntityManagement;
using GTA.Native;
using GTA.Math;

namespace AirSuperiority.Script.Entities
{
    public abstract class ObjectiveEntity
    {
        private ActiveTeamData team;
        private ManageableVehicle vehicle;

        /// <summary>
        /// Managed vehicle object associated to this entity
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
        /// Active vehicle mission ID.
        /// </summary>
        public VehicleTask VehicleMissionType
        {
            get { return (VehicleTask)Function.Call<int>((Hash)0x534AEBA6E5ED4CAB, vehicle.Handle); }
        }

        /// <summary>
        /// Assign a ManageableVehicle object to this objective blimp.
        /// </summary>
        /// <param name="Pilot"></param>
        /// <param name="Vehicle"></param>
        /// <returns></returns>
        public ObjectiveEntity Manage(ManageableVehicle Vehicle)
        {
            vehicle = Vehicle;
            return this;
        }

        /// <summary>
        /// Assign this ground asset a team.
        /// </summary>
        /// <param name="newTeam"></param>
        public void AssignTeam(ActiveTeamData newTeam)
        {
            team = newTeam;
        }
    }
}
