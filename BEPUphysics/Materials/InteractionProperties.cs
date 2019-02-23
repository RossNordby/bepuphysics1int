

namespace BEPUphysics.Materials
{
    ///<summary>
    /// Contains the blended friction and bounciness of a pair of objects.
    ///</summary>
    public struct InteractionProperties
    {
        ///<summary>
        /// Kinetic friction between the pair of objects.
        ///</summary>
        public Fix KineticFriction;
        ///<summary>
        /// Static friction between the pair of objects.
        ///</summary>
        public Fix StaticFriction;
        ///<summary>
        /// Bounciness between the pair of objects.
        ///</summary>
        public Fix Bounciness;
    }
}
