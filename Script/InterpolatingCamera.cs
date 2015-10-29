using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.Script
{
    public class InterpolatingCamera
    {
        private Camera mainCamera;

        public Camera MainCamera { get { return mainCamera; } }

        public bool Active
        {
            get { return mainCamera.IsActive; }
        }

        public InterpolatingCamera(Vector3 position)
        {
            mainCamera = World.CreateCamera(position, new Vector3(), 50f);
            mainCamera.Position = position;
        }

        /// <summary>
        /// Change camera view, start spline sequence.
        /// </summary>
        public void Start()
        {
            mainCamera.IsActive = true;
            World.RenderingCamera = mainCamera;
            Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, mainCamera.Handle, 1, 1, 1);
        }

        /// <summary>
        /// Stop sequence, switch to main camera view.
        /// </summary>
        public void Stop()
        {
            mainCamera.IsActive = false;
            World.RenderingCamera = null;
        }

        /// <summary>
        /// Destroy all cameras
        /// </summary>
        public void Destroy()
        {
            mainCamera.Destroy();
        }
    }
}
