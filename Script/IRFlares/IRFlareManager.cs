using System.Collections.Generic;
using GTA;
using GTA.Math;

namespace AirSuperiority.Script.IRFlares
{
    /// <summary>
    /// Class library making it easy to implement IR countermeasures with ScriptHookVDotNet scripts.
    /// </summary>
    public sealed class IRFlareManager
    {
        /// <summary>
        /// Max time for dropping flares (ms). <b>Default =</b> 5200
        /// </summary>
        public int MaxDropTime { get; set; } = 3000;

        /// <summary>
        /// Frequency of flare drops (ms). <b>Default =</b> 250
        /// </summary>
        public int FlareDropInterval { get; set; } = 263;

        /// <summary>
        /// Initializes the class for the given vehicle.
        /// </summary>
        /// <param name="vehicle">Target vehicle</param>
        /// <returns></returns>
        public IRFlareManager SetupWithVehicle(Vehicle vehicle)
        {
            this.vehicle = vehicle;
            activeFlareSequence = new List<IRFlareSequence>();
            activeFlareSequence.Add(new IRFlareSequence(vehicle, FlareDropInterval, MaxDropTime, new Vector3(0, 0.6f, -2.0f), IRFlareSequence.ForceType.Left, 15f));
            activeFlareSequence.Add(new IRFlareSequence(vehicle, FlareDropInterval, MaxDropTime, new Vector3(0, -0.6f, -2.0f), IRFlareSequence.ForceType.Right, 15f));
            activeFlareSequence.Add(new IRFlareSequence(vehicle, FlareDropInterval, MaxDropTime, new Vector3(0, 0.4f, -2.0f), IRFlareSequence.ForceType.Left, 9f));
            activeFlareSequence.Add(new IRFlareSequence(vehicle, FlareDropInterval, MaxDropTime, new Vector3(0, -0.4f, -2.0f), IRFlareSequence.ForceType.Right, 9f));
            activeFlareSequence.Add(new IRFlareSequence(vehicle, FlareDropInterval, MaxDropTime, new Vector3(0, 0.0f, -2.0f), IRFlareSequence.ForceType.Down, 2.5f));
            return this;
        }

        private List<IRFlareSequence> activeFlareSequence;
        private Vehicle vehicle;

        /// <summary>
        /// Update the class.
        /// </summary>
        public void Update()
        {
            activeFlareSequence?.ForEach(x => x.Update());
        }

        /// <summary>
        /// Start flare sequence.
        /// </summary>
        public void Start()
        {
            RemoveAll();
            activeFlareSequence?.ForEach(x => x.Start());
        }

        /// <summary>
        /// Remove all flares.
        /// </summary>
        public void RemoveAll()
        {
            activeFlareSequence?.ForEach(x => x.RemoveAll());
        }
    }
}