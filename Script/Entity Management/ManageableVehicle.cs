using GTA;
using GTA.Native;
using AirSuperiority.Types;

namespace AirSuperiority.Script.EntityManagement
{
    public class ManageableVehicle : ManagedEntity
    {
        public Vehicle Vehicle { get { return vehicle; } }

        private Vehicle vehicle;

        /// <summary>
        /// Active vehicle mission type.
        /// </summary>
        public VehicleTask MissionType
        {
            get { return (VehicleTask)Function.Call<int>((Hash)0x534AEBA6E5ED4CAB, vehicle.Handle); }
        }

        /// <summary>
        /// State of the vehicle landing gear.
        /// </summary>
        public LandingGearState LandingGearState
        {
            get { return (LandingGearState)Function.Call<int>(Hash._GET_VEHICLE_LANDING_GEAR, vehicle.Handle); }
            set { Function.Call(Hash._SET_VEHICLE_LANDING_GEAR, vehicle.Handle, (int)value); }
        }

        public ManageableVehicle(Vehicle Vehicle) : base(Vehicle)
        {
            vehicle = Vehicle;
        }

        protected override void OnUpdate()
        {
        }
    }
}
