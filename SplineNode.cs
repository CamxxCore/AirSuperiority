using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA.Native;
using GTA.Math;
using GTA;

public class SplineNode
{
    private int index;
    private int duration;
    private Vector3 position;
    private Vector3 rotation;

    public int Index { get { return index; } }

    public int Duration { get { return duration; } }

    public Vector3 Position { get { return position; } }

    public Vector3 Rotation { get { return rotation; } }

    public SplineNode(Vector3 position, Vector3 rotation, int duration, int index)
    {
        this.position = position;
        this.rotation = rotation;
        this.duration = duration;
        this.index = index;
    }


}
