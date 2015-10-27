using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;

public class InterpCam
{
    private int duration;
    private Camera mainCamera, secondaryCamera;
    private Vector3 position1, position2;

    public Camera MainCamera { get { return mainCamera; } }
    public Camera SecondaryCamera { get { return secondaryCamera; } }

    public bool Active
    {
        get { return mainCamera.IsActive; }
    }

    public InterpCam(Vector3 position1, Vector3 position2, int duration)
    {
        mainCamera = new Camera(Function.Call<int>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0));
        secondaryCamera = new Camera(Function.Call<int>(Hash.CREATE_CAM, "DEFAULT_SCRIPTED_CAMERA", 0));
        mainCamera.Position = this.position1 = position1;
        secondaryCamera.Position = this.position2 = position2;
        this.duration = duration;
    }

    /// <summary>
    /// Change camera view, start spline sequence.
    /// </summary>
    public void Start()
    {
        Start(1, 1);
    }

    /// <summary>
    /// Change camera view, start spline sequence.
    /// </summary>
    public void Start(int easeLoc, int easeRot)
    {
        mainCamera.IsActive = true;
        World.RenderingCamera = mainCamera;
        Function.Call(Hash.SET_CAM_ACTIVE_WITH_INTERP, secondaryCamera.Handle, mainCamera.Handle, duration, easeLoc, easeRot);
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
        secondaryCamera.Destroy();
    }
}
