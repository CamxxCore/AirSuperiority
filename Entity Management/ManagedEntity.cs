using System;
using GTA;

namespace AirSuperiority.EntityManagement
{
    public delegate void EntityChangedEventHandler(object sender, EntityChangedEventArgs e);

    public abstract class ManagedEntity : Entity
    {
        private TimeSpan totalTime;
        private int deadTicks, aliveTicks, totalTicks;
        private int spawnTime;

        protected event EntityChangedEventHandler Alive, Dead;

        /// <summary>
        /// Total entity ticks.
        /// </summary>
        public int TotalTicks {
            get { return totalTicks; }
        }

        /// <summary>
        /// Total time entity has been available to the script.
        /// </summary>
        public TimeSpan TotalTime {
            get { return totalTime; }
        }

        /// <summary>
        /// Total ticks for which the entity has been alive.
        /// </summary>
        public int AliveTicks {
            get { return aliveTicks; }
        }

        /// <summary>
        /// Total ticks for which the entity has been dead.
        /// </summary>
        public int DeadTicks {
            get { return deadTicks; }
        }

        /// <summary>
        /// Initialize the entity for management.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ManagedEntity(Entity Entity) : base (Entity.Handle)
        {
            spawnTime = Game.GameTime;
            totalTime = default(TimeSpan);
        }

        /// <summary>
        /// Fired when the entity has died.
        /// </summary>
        protected virtual void OnDead(EntityChangedEventArgs e)
        {
            Dead?.Invoke(this, e);
        }

        /// <summary>
        /// Fired when the entity has been revived.
        /// </summary>
        protected virtual void OnAlive(EntityChangedEventArgs e)
        {
            Alive?.Invoke(this, e);
        }

        /// <summary>
        /// Fired for each tick.
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// Call this method each tick to update entity related information.
        /// </summary>
        public void Update()
        {
            OnUpdate();

            if (IsDead)
            {
                if (deadTicks == 0)
                {
                    OnDead(new EntityChangedEventArgs(this));
                }

                
                aliveTicks = 0;
                deadTicks++;
            }

            else
            {
                if (aliveTicks == 0)
                {
                    OnAlive(new EntityChangedEventArgs(this));
                }

                deadTicks = 0;
                aliveTicks++;
            }

            totalTicks++;
            totalTime = TimeSpan.FromMilliseconds(Game.GameTime - spawnTime);
        }
    }
}
