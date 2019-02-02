
using System;

namespace BEPUphysics.Materials
{
    ///<summary>
    /// Material properties for collidable objects.
    ///</summary>
    public class Material
    {
        internal Fix32 kineticFriction = MaterialManager.DefaultKineticFriction;
        ///<summary>
        /// Gets or sets the friction coefficient used when the object is sliding quickly and
        /// no special material relationship is defined between the colliding objects.
        ///</summary>
        public Fix32 KineticFriction
        {
            get
            {
                return kineticFriction;
            }
            set
            {
                kineticFriction = value;
                if (MaterialChanged != null)
                    MaterialChanged(this);
            }
        }

        internal Fix32 staticFriction = MaterialManager.DefaultStaticFriction;
        ///<summary>
        /// Gets or sets the friction coefficient used when the object is sliding slowly and
        /// no special material relationship is defined between the colliding objects.
        ///</summary>
        public Fix32 StaticFriction
        {
            get
            {
                return staticFriction;
            }
            set
            {
                staticFriction = value;
                if (MaterialChanged != null)
                    MaterialChanged(this);
            }
        }


        internal Fix32 bounciness = MaterialManager.DefaultBounciness;
        ///<summary>
        /// Gets or sets the coefficient of restitution between the objects when
        /// no special material relationship is defined between the colliding objects.
        ///</summary>
        public Fix32 Bounciness
        {
            get
            {
                return bounciness;
            }
            set
            {
                bounciness = value;
                if (MaterialChanged != null)
                    MaterialChanged(this);
            }
        }

        ///<summary>
        /// Gets or sets user data associated with the material.
        ///</summary>
        public object Tag { get; set; }
        int hashCode;
        ///<summary>
        /// Constructs a new material.
        ///</summary>
        public Material()
        {
            hashCode = (int)(((uint)GetHashCode()) * 19999999);
        }

        ///<summary>
        /// Constructs a new material.
        ///</summary>
        ///<param name="staticFriction">Static friction to use.</param>
        ///<param name="kineticFriction">Kinetic friction to use.</param>
        ///<param name="bounciness">Bounciness to use.</param>
        public Material(Fix32 staticFriction, Fix32 kineticFriction, Fix32 bounciness)
            : this()
        {
            this.staticFriction = staticFriction;
            this.kineticFriction = kineticFriction;
            this.bounciness = bounciness;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return hashCode;
        }

        ///<summary>
        /// Fires when the material properties change.
        ///</summary>
        public event Action<Material> MaterialChanged;

    }
}
