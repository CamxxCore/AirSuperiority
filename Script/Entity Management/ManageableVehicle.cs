using GTA;
using GTA.Native;

namespace AirSuperiority.Script.EntityManagement
{
    public class ManageableVehicle : ManagedEntity
    {
        private Vehicle vehicle;

        public Vehicle Vehicle {
            get { return vehicle; }
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
