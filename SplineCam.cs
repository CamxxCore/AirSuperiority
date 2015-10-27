using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

public class SplineCamera
{
    private Camera mainCamera;
    private List<SplineNode> nodes;

    public Camera MainCamera { get { return mainCamera; } }

    /// <summary>
    /// If the camera is currently active.
    /// </summary>
    public bool Active
    {
        get { return mainCamera.IsActive; }
    }

    /// <summary>
    /// Collection of registered camera nodes.
    /// </summary>
    public List<SplineNode> Nodes { get { return nodes; } }

    /// <summary>
    /// Overall progress of camera interpolation (0 - 1)
    /// </summary>
    public float Phase
    {
        get { return Function.Call<float>(Hash.GET_CAM_SPLINE_PHASE, mainCamera.Handle); }
        set { Function.Call(Hash.SET_CAM_SPLINE_PHASE, mainCamera.Handle, value); }
    }

    /// <summary>
    /// Phase of the current camera node.
    /// </summary>
    public float NodePhase
    {
        get { return Function.Call<float>(Hash.GET_CAM_SPLINE_PHASE, mainCamera.Handle); }
        set { Function.Call(Hash.SET_CAM_SPLINE_PHASE, mainCamera.Handle, value); }
    }

    /// <summary>
    /// Current camera node.
    /// </summary>
    public SplineNode CurrentNode
    {
        get { return Nodes[Function.Call<int>(Hash.GET_CAM_SPLINE_NODE_INDEX, mainCamera.Handle)]; }
    }

    /// <summary>
    /// Overall duration.
    /// </summary>
    public int Duration
    {
        set { Function.Call(Hash.SET_CAM_SPLINE_DURATION, mainCamera.Handle, value); }
    }

    public SplineCamera()
    {
        mainCamera = new Camera(Function.Call<int>(Hash.CREATE_CAM, "DEFAULT_SPLINE_CAMERA", 0));
        nodes = new List<SplineNode>();
    }

    /// <summary>
    /// Add camera node with position and rotation and specified duration with reference to last node.
    /// </summary>
    /// <param name="position">Position of the node.</param>
    /// <param name="rotation">Rotation of the node.</param>
    /// <param name="duration">Duration from previous node.</param>
    public void AddNode(Vector3 position, Vector3 rotation, int duration)
    {
        Function.Call(Hash.ADD_CAM_SPLINE_NODE, mainCamera.Handle, position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, duration, 3, 2);
        nodes.Add(new SplineNode(position, rotation, duration, nodes.Count));
    }

    /// <summary>
    /// Change camera view, start spline sequence.
    /// </summary>
    public void Start()
    {
        mainCamera.IsActive = true;
        World.RenderingCamera = mainCamera;
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
    /// Interpolate to player camera.
    /// </summary>
    public void TrackToPlayerView()
    {
        Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 1, 1, 1);
    }

    /// <summary>
    /// Destroy the camera.
    /// </summary>
    public void Destroy()
    {
        mainCamera.Destroy();
    }
}

