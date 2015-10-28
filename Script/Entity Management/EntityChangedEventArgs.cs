using System;
using GTA;

namespace AirSuperiority.Script.EntityManagement
{
    /// <summary>
    /// Event args for entity related events
    /// </summary>
    public class EntityChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize the class
        /// </summary>
        /// <param name="entity"></param>
        public EntityChangedEventArgs(Entity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// The entity that fired the event
        /// </summary>
        public Entity Entity { get; private set; }
    }
}
