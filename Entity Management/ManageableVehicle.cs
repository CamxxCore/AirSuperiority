using GTA;
using GTA.Native;

namespace AirSuperiority.EntityManagement
{
    public class ManageableVehicle : ManagedEntity
    {
        private Vehicle vehicle;

        public Vehicle Vehicle {
            get { return vehicle; }
        }

        public int VehicleMissionType {
            get { return Function.Call<int>((Hash)0x534AEBA6E5ED4CAB, vehicle.Handle); }
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
