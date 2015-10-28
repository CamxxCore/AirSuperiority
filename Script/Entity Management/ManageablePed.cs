using GTA;

namespace AirSuperiority.Script.EntityManagement
{
    public class ManageablePed : ManagedEntity
    {
        private Ped ped;

        public Ped Ped
        {
            get { return ped; }
        }

        public ManageablePed(Ped Ped) : base(Ped)
        {
            ped = Ped;
        }

        protected override void OnUpdate()
        {
 
        }
    }
}
