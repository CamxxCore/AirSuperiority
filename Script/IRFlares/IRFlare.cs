using GTA;
using GTA.Native;
using GTA.Math;

namespace AirSuperiority.Script.IRFlares
{
    internal sealed class IRFlare : Entity
    {
        private int tickTime;
        private bool timerActive;
        private LoopedPTFX ptfx;

        /// <summary>
        /// Max time the entity will be active before fading fx
        /// </summary>
        private const int MaxAliveTime = 500;

        /// <summary>
        /// Initialize the class
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public IRFlare(Vector3 position, Vector3 rotation) : base(Create(position, rotation))
        {
            ptfx = new LoopedPTFX("core", "proj_flare_fuse"); //"scr_trevor2", "scr_trev2_flare_L"
        }

        /// <summary>
        /// Create the flare at the given position with rotation
        /// </summary>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        /// <returns></returns>
        private static int Create(Vector3 position, Vector3 rotation)
        {
            var model = new Model("prop_flare_01b");
            if (!model.IsLoaded)
                model.Request(1000);

            var flare = World.CreateProp(model, position, false, false);
            Function.Call(Hash.SET_ENTITY_RECORDS_COLLISIONS, flare.Handle, true);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, flare.Handle, true);
            Function.Call(Hash.SET_ENTITY_LOD_DIST, flare.Handle, 1000);
            Function.Call(Hash.SET_OBJECT_PHYSICS_PARAMS, flare.Handle, -1.0f, -1.0f, -1.0f, -1.0f, 0.009888f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f);
            flare.Rotation = rotation;
            return flare.Handle;
        }

        /// <summary>
        /// Start particle fx on the entity
        /// </summary>
        public void StartFX()
        {
            if (!ptfx.IsLoaded)
                ptfx.Load();

            ptfx.Start(new Prop(Handle), 4f);
            tickTime = Game.GameTime + MaxAliveTime;
            timerActive = true;
        }

        /// <summary>
        /// Remove particle fx and delete the entity
        /// </summary>
        public void Remove()
        {
            ptfx.Remove();
            Delete();
        }

        /// <summary>
        /// Update the class
        /// </summary>
        /// <param name="exists"></param>
        public void Update(out bool exists)
        {
            if (timerActive && Game.GameTime > tickTime)
            {
                if (ptfx.Exists && ptfx.Scale > 0.0f)
                {
                    ptfx.Scale -= 0.1f;
                    exists = true;
                }

                else
                {
                    timerActive = false;
                    exists = false;
                    Remove();
                }
            }

            else
            {
                //entity active
                exists = true;
            }
        }
    }
}
