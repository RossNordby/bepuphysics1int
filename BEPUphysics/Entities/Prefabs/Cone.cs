using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.EntityStateManagement;
 
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;


namespace BEPUphysics.Entities.Prefabs
{
    /// <summary>
    /// Cone-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
    /// </summary>
    public class Cone : Entity<ConvexCollidable<ConeShape>>
    {
        /// <summary>
        /// Gets or sets the length of the cone.
        /// </summary>
        public Fix32 Height
        {
            get
            {
                return CollisionInformation.Shape.Height;
            }
            set
            {
                CollisionInformation.Shape.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the cone.
        /// </summary>
        public Fix32 Radius
        {
            get
            {
                return CollisionInformation.Shape.Radius;
            }
            set
            {
                CollisionInformation.Shape.Radius = value;
            }
        }


        private Cone(Fix32 high, Fix32 rad)
            :base(new ConvexCollidable<ConeShape>(new ConeShape(high, rad)))
        {
        }

        private Cone(Fix32 high, Fix32 rad, Fix32 mass)
            :base(new ConvexCollidable<ConeShape>(new ConeShape(high, rad)), mass)
        {
        }



        /// <summary>
        /// Constructs a physically simulated cone.
        /// </summary>
        /// <param name="position">Position of the cone.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        /// <param name="mass">Mass of the object.</param>
        public Cone(Vector3 position, Fix32 height, Fix32 radius, Fix32 mass)
            : this(height, radius, mass)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a nondynamic cone.
        /// </summary>
        /// <param name="position">Position of the cone.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        public Cone(Vector3 position, Fix32 height, Fix32 radius)
            : this(height, radius)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a physically simulated cone.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        /// <param name="mass">Mass of the object.</param>
        public Cone(MotionState motionState, Fix32 height, Fix32 radius, Fix32 mass)
            : this(height, radius, mass)
        {
            MotionState = motionState;
        }

        /// <summary>
        /// Constructs a nondynamic cone.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="height">Height of the cone.</param>
        /// <param name="radius">Radius of the cone.</param>
        public Cone(MotionState motionState, Fix32 height, Fix32 radius)
            : this(height, radius)
        {
            MotionState = motionState;
        }

 


    }
}