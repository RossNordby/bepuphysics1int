

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
        public Fix32 KineticFriction;
        ///<summary>
        /// Static friction between the pair of objects.
        ///</summary>
        public Fix32 StaticFriction;
        ///<summary>
        /// Bounciness between the pair of objects.
        ///</summary>
        public Fix32 Bounciness;
    }
}
