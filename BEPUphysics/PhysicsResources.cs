using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.Entities;
 
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DeactivationManagement;
using BEPUutilities.DataStructures;
using BEPUutilities;
using BEPUutilities.ResourceManagement;

namespace BEPUphysics
{
    /// <summary>
    /// Handles allocation and management of commonly used resources.
    /// </summary>
    public static class PhysicsResources
    {
        static PhysicsResources()
        {
            ResetPools();
        }

        public static void ResetPools()
        {

            SubPoolRayCastResultList = new UnsafeResourcePool<RawList<RayCastResult>>();
            SubPoolBroadPhaseEntryList = new UnsafeResourcePool<RawList<BroadPhaseEntry>>();
            SubPoolCollidableList = new UnsafeResourcePool<RawList<Collidable>>();
            SubPoolCompoundChildList = new UnsafeResourcePool<RawList<CompoundChild>>();

            SubPoolEntityRawList = new UnsafeResourcePool<RawList<Entity>>(16);
            SubPoolTriangleShape = new UnsafeResourcePool<TriangleShape>();
            SubPoolTriangleCollidables = new UnsafeResourcePool<TriangleCollidable>();
            SimulationIslandConnections = new UnsafeResourcePool<SimulationIslandConnection>();
        }

        static ResourcePool<RawList<RayCastResult>> SubPoolRayCastResultList;
        static ResourcePool<RawList<BroadPhaseEntry>> SubPoolBroadPhaseEntryList;
        static ResourcePool<RawList<Collidable>> SubPoolCollidableList;
        static ResourcePool<RawList<Entity>> SubPoolEntityRawList;
        static ResourcePool<TriangleShape> SubPoolTriangleShape;
        static ResourcePool<RawList<CompoundChild>> SubPoolCompoundChildList;
        static ResourcePool<TriangleCollidable> SubPoolTriangleCollidables;
        static ResourcePool<SimulationIslandConnection> SimulationIslandConnections;
        //#endif
        /// <summary>
        /// Retrieves a ray cast result list from the resource pool.
        /// </summary>
        /// <returns>Empty ray cast result list.</returns>
        public static RawList<RayCastResult> GetRayCastResultList()
        {
            return SubPoolRayCastResultList.Take();
        }

        /// <summary>
        /// Returns a resource to the pool.
        /// </summary>
        /// <param name="list">List to return.</param>
        public static void GiveBack(RawList<RayCastResult> list)
        {
            list.Clear();
            SubPoolRayCastResultList.GiveBack(list);
        }

        /// <summary>
        /// Retrieves an BroadPhaseEntry list from the resource pool.
        /// </summary>
        /// <returns>Empty BroadPhaseEntry list.</returns>
        public static RawList<BroadPhaseEntry> GetBroadPhaseEntryList()
        {
            return SubPoolBroadPhaseEntryList.Take();
        }

        /// <summary>
        /// Returns a resource to the pool.
        /// </summary>
        /// <param name="list">List to return.</param>
        public static void GiveBack(RawList<BroadPhaseEntry> list)
        {
            list.Clear();
            SubPoolBroadPhaseEntryList.GiveBack(list);
        }

        /// <summary>
        /// Retrieves a Collidable list from the resource pool.
        /// </summary>
        /// <returns>Empty Collidable list.</returns>
        public static RawList<Collidable> GetCollidableList()
        {
            return SubPoolCollidableList.Take();
        }

        /// <summary>
        /// Returns a resource to the pool.
        /// </summary>
        /// <param name="list">List to return.</param>
        public static void GiveBack(RawList<Collidable> list)
        {
            list.Clear();
            SubPoolCollidableList.GiveBack(list);
        }

        /// <summary>
        /// Retrieves an CompoundChild list from the resource pool.
        /// </summary>
        /// <returns>Empty information list.</returns>
        public static RawList<CompoundChild> GetCompoundChildList()
        {
            return SubPoolCompoundChildList.Take();
        }

        /// <summary>
        /// Returns a resource to the pool.
        /// </summary>
        /// <param name="list">List to return.</param>
        public static void GiveBack(RawList<CompoundChild> list)
        {
            list.Clear();
            SubPoolCompoundChildList.GiveBack(list);
        }

      

        /// <summary>
        /// Retrieves an Entity RawList from the resource pool.
        /// </summary>
        /// <returns>Empty Entity raw list.</returns>
        public static RawList<Entity> GetEntityRawList()
        {
            return SubPoolEntityRawList.Take();
        }

        /// <summary>
        /// Returns a resource to the pool.
        /// </summary>
        /// <param name="list">List to return.</param>
        public static void GiveBack(RawList<Entity> list)
        {
            list.Clear();
            SubPoolEntityRawList.GiveBack(list);
        }
        
      

        /// <summary>
        /// Retrieves a TriangleCollidable from the resource pool.
        /// </summary>
        /// <returns>Initialized TriangleCollidable.</returns>
        public static TriangleCollidable GetTriangleCollidable()
        {
            return SubPoolTriangleCollidables.Take();
        }

        /// <summary>
        /// Returns a resource to the pool.
        /// </summary>
        /// <param name="triangle">Triangle collidable to return.</param>
        public static void GiveBack(TriangleCollidable triangle)
        {
            triangle.CleanUp();
            SubPoolTriangleCollidables.GiveBack(triangle);
        }

        /// <summary>
        /// Retrieves a simulation island connection from the resource pool.
        /// </summary>
        /// <returns>Uninitialized simulation island connection.</returns>
        public static SimulationIslandConnection GetSimulationIslandConnection()
        {
            return SimulationIslandConnections.Take();

        }

        /// <summary>
        /// Returns a resource to the pool.
        /// </summary>
        /// <param name="connection">Connection to return.</param>
        public static void GiveBack(SimulationIslandConnection connection)
        {
            connection.CleanUp();
            SimulationIslandConnections.GiveBack(connection);

        }
    }
}