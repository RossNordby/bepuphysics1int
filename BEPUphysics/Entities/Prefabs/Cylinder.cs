using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.EntityStateManagement;
 
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;


namespace BEPUphysics.Entities.Prefabs
{
    /// <summary>
    /// Cylinder-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
    /// </summary>
    public class Cylinder : Entity<ConvexCollidable<CylinderShape>>
    {
        /// <summary>
        /// Gets or sets the height of the cylinder.
        /// </summary>
        public Fix Height
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
        /// Gets or sets the radius of the cylinder.
        /// </summary>
        public Fix Radius
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


        private Cylinder(Fix high, Fix rad, Fix mass)
            : base(new ConvexCollidable<CylinderShape>(new CylinderShape(high, rad)), mass)
        {
        }

        private Cylinder(Fix high, Fix rad)
            : base(new ConvexCollidable<CylinderShape>(new CylinderShape(high, rad)))
        {
        }

        /// <summary>
        /// Constructs a physically simulated cylinder.
        /// </summary>
        /// <param name="position">Position of the cylinder.</param>
        /// <param name="height">Height of the cylinder.</param>
        /// <param name="radius">Radius of the cylinder.</param>
        /// <param name="mass">Mass of the object.</param>
        public Cylinder(Vector3 position, Fix height, Fix radius, Fix mass)
            : this(height, radius, mass)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a nondynamic cylinder.
        /// </summary>
        /// <param name="position">Position of the cylinder.</param>
        /// <param name="height">Height of the cylinder.</param>
        /// <param name="radius">Radius of the cylinder.</param>
        public Cylinder(Vector3 position, Fix height, Fix radius)
            : this(height, radius)
        {
            Position = position;
        }

        /// <summary>
        /// Constructs a physically simulated cylinder.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="height">Height of the cylinder.</param>
        /// <param name="radius">Radius of the cylinder.</param>
        /// <param name="mass">Mass of the object.</param>
        public Cylinder(MotionState motionState, Fix height, Fix radius, Fix mass)
            : this(height, radius, mass)
        {
            MotionState = motionState;
        }

        /// <summary>
        /// Constructs a nondynamic cylinder.
        /// </summary>
        /// <param name="motionState">Motion state specifying the entity's initial state.</param>
        /// <param name="height">Height of the cylinder.</param>
        /// <param name="radius">Radius of the cylinder.</param>
        public Cylinder(MotionState motionState, Fix height, Fix radius)
            : this(height, radius)
        {
            MotionState = motionState;
        }




    }
}