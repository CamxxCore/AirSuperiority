using GTA;

namespace AirSuperiority.Script.EntityManagement
{
    public class ManageablePed : ManagedEntity
    {
        private Ped ped;
        private int vehicleTicks;

        /// <summary>
        /// Fired when the ped has entered a vehicle.
        /// </summary>
        public event EntityChangedEventHandler EnterVehicle;

        /// <summary>
        /// Fired when the ped has exited a vehicle.
        /// </summary>
        public event EntityChangedEventHandler ExitVehicle;

        public Ped Ped
        {
            get { return ped; }
        }

        public ManageablePed(Ped Ped) : base(Ped)
        {
            ped = Ped;
        }

        protected virtual void OnEnterVehicle(EntityChangedEventArgs e)
        {
            EnterVehicle?.Invoke(this, e);
        }

        protected virtual void OnExitVehicle(EntityChangedEventArgs e)
        {
            ExitVehicle?.Invoke(this, e);
        }

        protected override void OnUpdate()
        {
            if (ped.IsInVehicle())
            {
                if (vehicleTicks == 0)
                    OnEnterVehicle(new EntityChangedEventArgs(this));
                vehicleTicks++;
            }

            else
            {
                if (vehicleTicks > 0)
                {
                    OnExitVehicle(new EntityChangedEventArgs(this));
                    vehicleTicks = 0;
                }
            }
        }
    }
}
