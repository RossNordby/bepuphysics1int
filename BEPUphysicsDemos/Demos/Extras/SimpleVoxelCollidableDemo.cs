using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.Events;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.Constraints.Collision;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.NarrowPhaseSystems;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.OtherSpaceStages;
using BEPUphysics.PositionUpdating;
using BEPUphysics.Settings;
using BEPUphysicsDrawer.Models;
using BEPUutilities;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseSystems;
using BEPUutilities.DataStructures;
using BEPUutilities.ResourceManagement;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;


namespace BEPUphysicsDemos.Demos.Extras
{
    public struct Int3 : IEquatable<Int3>
    {
        public int X;
        public int Y;
        public int Z;

        public override int GetHashCode()
        {
            return (X * 533000401) ^ (Y * 920419813) ^ (Z * 694847539);
        }
        public bool Equals(Int3 other)
        {
            return other.X == X && other.Y == Y && other.Z == Z;
        }
    }

    /// <summary>
    /// Extremely simple and unoptimized voxel grid shape.
    /// Shapes can be shared between multiple collidables.
    /// </summary>
    public class VoxelGridShape : CollisionShape
    {
        /// <summary>
        /// Three dimensional grid of cells. A true value means the cell is filled, and false means it's empty.
        /// </summary>
        public bool[, ,] Cells { get; private set; }
        //Note: This representation is very inefficient. Each bool occupies a full byte, and there is no space skipping.
        //Large blocks of empty space take just as much space as high frequency data.
        //If memory is a concern, it would be a good idea to optimize this. It would be pretty easy to get an order of magnitude (or three) improvement.

        /// <summary>
        /// Width of a single voxel cell.
        /// </summary>
        public Fix32 CellWidth { get; private set; }

        public void GetBoundingBox(ref Vector3 position, out BoundingBox boundingBox)
        {
            var size = new Vector3(CellWidth.Mul(Cells.GetLength(0).ToFix()), CellWidth.Mul(Cells.GetLength(1).ToFix()), CellWidth.Mul(Cells.GetLength(2).ToFix()));
            boundingBox.Min = position;
            Vector3.Add(ref size, ref position, out boundingBox.Max);
        }

        public VoxelGridShape(bool[, ,] cells, Fix32 cellWidth)
        {
            Cells = cells;
            CellWidth = cellWidth;
        }

        public void GetOverlaps(Vector3 gridPosition, BoundingBox boundingBox, ref QuickList<Int3> overlaps)
        {
            Vector3.Subtract(ref boundingBox.Min, ref gridPosition, out boundingBox.Min);
            Vector3.Subtract(ref boundingBox.Max, ref gridPosition, out boundingBox.Max);
            var inverseWidth = 1.ToFix().Div(CellWidth);
            var min = new Int3
            {
                X = Math.Max(0, (boundingBox.Min.X.Mul(inverseWidth)).ToInt()),
                Y = Math.Max(0, (boundingBox.Min.Y.Mul(inverseWidth)).ToInt()),
                Z = Math.Max(0, (boundingBox.Min.Z.Mul(inverseWidth)).ToInt())
            };
            var max = new Int3
            {
                X = Math.Min(Cells.GetLength(0) - 1, (boundingBox.Max.X.Mul(inverseWidth)).ToInt()),
                Y = Math.Min(Cells.GetLength(1) - 1, (boundingBox.Max.Y.Mul(inverseWidth)).ToInt()),
                Z = Math.Min(Cells.GetLength(2) - 1, (boundingBox.Max.Z.Mul(inverseWidth)).ToInt())
            };

            for (int i = min.X; i <= max.X; ++i)
            {
                for (int j = min.Y; j <= max.Y; ++j)
                {
                    for (int k = min.Z; k <= max.Z; ++k)
                    {
                        if (Cells[i, j, k])
                        {
                            overlaps.Add(new Int3 { X = i, Y = j, Z = k });
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Simple voxel grid collidable. Uses the VoxelGridShape as a data source and provides the 
    /// </summary>
    public class VoxelGrid : StaticCollidable
    {
        public new VoxelGridShape Shape
        {
            get { return (VoxelGridShape)base.Shape; }
        }

        /// <summary>
        /// Position of the minimum corner of the voxel grid.
        /// </summary>
        public Vector3 Position;

        public VoxelGrid(VoxelGridShape shape, Vector3 position)
        {
            Position = position;
            base.Shape = shape;
            events = new ContactEventManager<VoxelGrid>(this);
        }

        public override bool RayCast(Ray ray, Fix32 maximumLength, out RayHit rayHit)
        {
            //This example is primarily to show custom collidable pair management with a minimum of other complexity, and this isn't vital.
            //Note: the character controller makes significant use of ray casts. While its basic features work without ray casts, 
            //implementing them will unlock more features and improve behavior.
            rayHit = new RayHit();
            return false;
        }

        public override bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref Vector3 sweep, out RayHit hit)
        {
            //This example is primarily to show custom collidable pair management with a minimum of other complexity, and this isn't vital.
            hit = new RayHit();
            return false;
        }

        public override void UpdateBoundingBox()
        {
            Shape.GetBoundingBox(ref Position, out boundingBox);
        }

        //For simplicity, the event manager is read only. The other collidables like StaticMesh and InstancedMesh have a setter, but it complicates things
        //and doesn't add a lot. For example implementations of setters, check those classes out.
        protected internal ContactEventManager<VoxelGrid> events;
        ///<summary>
        /// Gets the event manager of the mesh.
        ///</summary>
        public ContactEventManager<VoxelGrid> Events
        {
            get { return events; }
        }
        protected override IContactEventTriggerer EventTriggerer
        {
            get { return events; }
        }

        protected override IDeferredEventCreator EventCreator
        {
            get { return events; }
        }
    }

    public class ReusableBoxCollidable : ConvexCollidable<BoxShape>
    {
        public ReusableBoxCollidable()
            : base(new BoxShape(1.ToFix(), 1.ToFix(), 1.ToFix()))
        {
        }

        protected override void UpdateBoundingBoxInternal(Fix32 dt)
        {
            Shape.GetBoundingBox(ref worldTransform, out boundingBox);
        }

    }

    /// <summary>
    /// Manages the contacts associated with a convex-voxelgrid collision.
    /// This is a slightly different kind of manifold- instead of directly managing collision, it manages
    /// a set of testers for each box near the opposing convex.
    /// </summary>
    public class VoxelGridConvexContactManifold : ContactManifold
    {


        static LockingResourcePool<ReusableBoxCollidable> boxCollidablePool = new LockingResourcePool<ReusableBoxCollidable>();
        static LockingResourcePool<GeneralConvexPairTester> testerPool = new LockingResourcePool<GeneralConvexPairTester>();

        private VoxelGrid voxelGrid;
        private ConvexCollidable convex;


        public QuickDictionary<Int3, GeneralConvexPairTester> ActivePairs;
        private QuickDictionary<Int3, GeneralConvexPairTester> activePairsBackBuffer;
        protected RawValueList<ContactSupplementData> supplementData = new RawValueList<ContactSupplementData>(4);

        public VoxelGridConvexContactManifold()
        {
            contacts = new RawList<Contact>(4);
            unusedContacts = new UnsafeResourcePool<Contact>(4);
            contactIndicesToRemove = new RawList<int>(4);
        }


        private GeneralConvexPairTester GetPair(ref Int3 position)
        {
            var pair = testerPool.Take();
            var boxCollidable = boxCollidablePool.Take();
            boxCollidable.Shape.Width = voxelGrid.Shape.CellWidth;
            boxCollidable.Shape.Height = voxelGrid.Shape.CellWidth;
            boxCollidable.Shape.Length = voxelGrid.Shape.CellWidth;
            pair.Initialize(convex, boxCollidable);
            boxCollidable.WorldTransform = new RigidTransform(new Vector3(
voxelGrid.Position.X.Add((position.X.ToFix().Add(F64.C0p5)).Mul(voxelGrid.Shape.CellWidth)),
voxelGrid.Position.Y.Add((position.Y.ToFix().Add(F64.C0p5)).Mul(voxelGrid.Shape.CellWidth)),
voxelGrid.Position.Z.Add((position.Z.ToFix().Add(F64.C0p5)).Mul(voxelGrid.Shape.CellWidth))));
            return pair;
        }


        private void ReturnPair(GeneralConvexPairTester pair)
        {
            boxCollidablePool.GiveBack((ReusableBoxCollidable)pair.CollidableB);
            pair.CleanUp();
            testerPool.GiveBack(pair);
        }



        public override void Initialize(Collidable newCollidableA, Collidable newCollidableB)
        {
            convex = newCollidableA as ConvexCollidable;
            voxelGrid = newCollidableB as VoxelGrid;


            if (convex == null || voxelGrid == null)
            {
                convex = newCollidableB as ConvexCollidable;
                voxelGrid = newCollidableA as VoxelGrid;
                if (convex == null || voxelGrid == null)
                    throw new ArgumentException("Inappropriate types used to initialize contact manifold.");
            }
            ActivePairs = new QuickDictionary<Int3, GeneralConvexPairTester>(BufferPools<Int3>.Locking, BufferPools<GeneralConvexPairTester>.Locking, BufferPools<int>.Locking, 3);
            activePairsBackBuffer = new QuickDictionary<Int3, GeneralConvexPairTester>(BufferPools<Int3>.Locking, BufferPools<GeneralConvexPairTester>.Locking, BufferPools<int>.Locking, 3);

        }

        public override void CleanUp()
        {
            convex = null;

            for (int i = ActivePairs.Count - 1; i >= 0; --i)
            {
                ReturnPair(ActivePairs.Values[i]);
                ActivePairs.Values[i].CleanUp();
            }
            //Clear->dispose is technically unnecessary now, but it may avoid some pain later on when this behavior changes in v2.
            //This will be a very sneaky breaking change...
            ActivePairs.Clear();
            ActivePairs.Dispose();
            Debug.Assert(activePairsBackBuffer.Count == 0);
            activePairsBackBuffer.Dispose();
            base.CleanUp();
        }


        public override void Update(Fix32 dt)
        {
            //Refresh the contact manifold for this frame.
            var transform = new RigidTransform(voxelGrid.Position);
            var convexTransform = convex.WorldTransform;
            ContactRefresher.ContactRefresh(contacts, supplementData, ref convexTransform, ref transform, contactIndicesToRemove);
            RemoveQueuedContacts();

            //Collect the set of overlapped cell indices.
            //Not the fastest way to do this, but it's relatively simple and easy.
            var overlaps = new QuickList<Int3>(BufferPools<Int3>.Thread);
            voxelGrid.Shape.GetOverlaps(voxelGrid.Position, convex.BoundingBox, ref overlaps);

            var candidatesToAdd = new QuickList<ContactData>(BufferPools<ContactData>.Thread, BufferPool<int>.GetPoolIndex(overlaps.Count));
            for (int i = 0; i < overlaps.Count; ++i)
            {
                GeneralConvexPairTester manifold;
                if (!ActivePairs.TryGetValue(overlaps.Elements[i], out manifold))
                {
                    //This manifold did not previously exist.
                    manifold = GetPair(ref overlaps.Elements[i]);
                }
                else
                {
                    //It did previously exist.
                    ActivePairs.FastRemove(overlaps.Elements[i]);
                }
                activePairsBackBuffer.Add(overlaps.Elements[i], manifold);
                ContactData contactCandidate;
                if (manifold.GenerateContactCandidate(out contactCandidate))
                {

                    candidatesToAdd.Add(ref contactCandidate);
                }
            }
            overlaps.Dispose();
            //Any pairs remaining in the activePairs set no longer exist. Clean them up.
            for (int i = ActivePairs.Count - 1; i >= 0; --i)
            {
                ReturnPair(ActivePairs.Values[i]);
                ActivePairs.FastRemove(ActivePairs.Keys[i]);
            }
            //Swap the pair sets.
            var temp = ActivePairs;
            ActivePairs = activePairsBackBuffer;
            activePairsBackBuffer = temp;

            //Check if adding the new contacts would overflow the manifold.
            if (contacts.Count + candidatesToAdd.Count > 4)
            {
                //Adding all the contacts would overflow the manifold.  Reduce to the best subset.
                var reducedCandidates = new QuickList<ContactData>(BufferPools<ContactData>.Thread, 3);
                ContactReducer.ReduceContacts(contacts, ref candidatesToAdd, contactIndicesToRemove, ref reducedCandidates);
                RemoveQueuedContacts();
                for (int i = reducedCandidates.Count - 1; i >= 0; i--)
                {
                    Add(ref reducedCandidates.Elements[i]);
                    reducedCandidates.RemoveAt(i);
                }
                reducedCandidates.Dispose();
            }
            else if (candidatesToAdd.Count > 0)
            {
                //Won't overflow the manifold, so just toss it in.
                for (int i = 0; i < candidatesToAdd.Count; i++)
                {
                    Add(ref candidatesToAdd.Elements[i]);
                }
            }


            candidatesToAdd.Dispose();

        }


        protected override void Add(ref ContactData contactCandidate)
        {
            ContactSupplementData supplement;
            supplement.BasePenetrationDepth = contactCandidate.PenetrationDepth;
            var convexTransform = convex.WorldTransform;
            var gridTransform = new RigidTransform(voxelGrid.Position);
            RigidTransform.TransformByInverse(ref contactCandidate.Position, ref convexTransform, out supplement.LocalOffsetA);
            RigidTransform.TransformByInverse(ref contactCandidate.Position, ref gridTransform, out supplement.LocalOffsetB);
            supplementData.Add(ref supplement);
            base.Add(ref contactCandidate);
        }
        protected override void Remove(int contactIndex)
        {
            supplementData.RemoveAt(contactIndex);
            base.Remove(contactIndex);
        }


    }

    public class VoxelGridConvexPairHandler : StandardPairHandler
    {
        public static void EnsurePairsAreRegistered()
        {
            //Assume if one has been added, all have.
            if (!NarrowPhaseHelper.CollisionManagers.ContainsKey(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(VoxelGrid))))
            {
                var factory = new NarrowPhasePairFactory<VoxelGridConvexPairHandler>();
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<CapsuleShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<TriangleShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<CylinderShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConeShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<TransformableShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<MinkowskiSumShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<WrappedShape>), typeof(VoxelGrid)), factory);
                NarrowPhaseHelper.CollisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConvexHullShape>), typeof(VoxelGrid)), factory);
            }
        }

        private VoxelGrid voxelGrid;
        private ConvexCollidable convex;
        public override Collidable CollidableA
        {
            get { return convex; }
        }

        public override Collidable CollidableB
        {
            get { return voxelGrid; }
        }

        public override Entity EntityA
        {
            get { return convex.Entity; }
        }

        public override Entity EntityB
        {
            get { return null; }
        }

        public override void UpdateTimeOfImpact(Collidable requester, Fix32 dt)
        {
            //Complicated and not vital. Leaving it out for simplicity. Check out InstancedMeshPairHandler for an example implementation.
            //Notice that we don't test for convex entity null explicitly.  The convex.IsActive property does that for us.
            if (convex.IsActive && convex.Entity.PositionUpdateMode == PositionUpdateMode.Continuous)
            {
                //Only perform the test if the minimum radii are small enough relative to the size of the velocity.
                Vector3 velocity = convex.Entity.LinearVelocity * dt;
                Fix32 velocitySquared = velocity.LengthSquared();

                var minimumRadius = convex.Shape.MinimumRadius.Mul(MotionSettings.CoreShapeScaling);
                timeOfImpact = 1.ToFix();
                if (minimumRadius.Mul(minimumRadius) < velocitySquared)
                {
                    for (int i = 0; i < contactManifold.ActivePairs.Count; ++i)
                    {
                        var pair = contactManifold.ActivePairs.Values[i];
                        //In the contact manifold, the box collidable is always put into the second slot.
                        var boxCollidable = (ReusableBoxCollidable)pair.CollidableB;
                        RayHit rayHit;
                        var worldTransform = boxCollidable.WorldTransform;
                        if (GJKToolbox.CCDSphereCast(new Ray(convex.WorldTransform.Position, velocity), minimumRadius, boxCollidable.Shape, ref worldTransform, timeOfImpact, out rayHit) &&
                            rayHit.T > Toolbox.BigEpsilon)
                        {
                            timeOfImpact = rayHit.T;
                        }
                    }
                }
            }
        }

        protected override void GetContactInformation(int index, out ContactInformation info)
        {
            info.Contact = contactManifold.Contacts[index];
            //Find the contact's normal and friction forces.
            info.FrictionImpulse = 0.ToFix();
            info.NormalImpulse = 0.ToFix();

            for (int i = 0; i < constraint.ContactFrictionConstraints.Count; i++)
            {
                if (constraint.ContactFrictionConstraints[i].PenetrationConstraint.Contact == info.Contact)
                {
                    info.FrictionImpulse = constraint.ContactFrictionConstraints[i].TotalImpulse;
                    info.NormalImpulse = constraint.ContactFrictionConstraints[i].PenetrationConstraint.NormalImpulse;
                    break;
                }
            }

            //Compute relative velocity
            if (convex.Entity != null)
            {
                info.RelativeVelocity = Toolbox.GetVelocityOfPoint(info.Contact.Position, convex.Entity.Position, convex.Entity.LinearVelocity, convex.Entity.AngularVelocity);
            }
            else
                info.RelativeVelocity = new Vector3();


            info.Pair = this;
        }

        public VoxelGridConvexPairHandler()
        {
            constraint = new NonConvexContactManifoldConstraint(this);
        }

        private VoxelGridConvexContactManifold contactManifold = new VoxelGridConvexContactManifold();
        private NonConvexContactManifoldConstraint constraint;
        public override ContactManifold ContactManifold
        {
            get { return contactManifold; }
        }

        public override ContactManifoldConstraint ContactConstraint
        {
            get { return constraint; }
        }


        public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
        {

            voxelGrid = entryA as VoxelGrid;
            convex = entryB as ConvexCollidable;

            if (voxelGrid == null || convex == null)
            {
                voxelGrid = entryB as VoxelGrid;
                convex = entryA as ConvexCollidable;

                if (voxelGrid == null || convex == null)
                    throw new ArgumentException("Inappropriate types used to initialize pair.");
            }

            //Contact normal goes from A to B.
            broadPhaseOverlap = new BroadPhaseOverlap(convex, voxelGrid, broadPhaseOverlap.CollisionRule);

            UpdateMaterialProperties(convex.Entity != null ? convex.Entity.Material : null, voxelGrid.Material);


            base.Initialize(entryA, entryB);





        }


        ///<summary>
        /// Cleans up the pair handler.
        ///</summary>
        public override void CleanUp()
        {
            base.CleanUp();
            voxelGrid = null;
            convex = null;

        }
    }

    /// <summary>
    /// Demo showing the voxel grid in action.
    /// </summary>
    public class SimpleVoxelCollidableDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public SimpleVoxelCollidableDemo(DemosGame game)
            : base(game)
        {
            VoxelGridConvexPairHandler.EnsurePairsAreRegistered();
            int cellCountX = 16;
            int cellCountY = 16;
            int cellCountZ = 16;
            var cells = new bool[cellCountX, cellCountY, cellCountZ];
            for (int i = 0; i < cellCountX; ++i)
            {
                for (int j = 0; j < cellCountY; ++j)
                {
                    for (int k = 0; k < cellCountZ; ++k)
                    {
                        cells[i, j, k] = (Fix32Ext.Sin(((i.ToFix().Mul(0.55m.ToFix())).Add(6.ToFix())).Add(j.ToFix().Mul((-0.325m).ToFix()))).Add(Fix32Ext.Sin(((j.ToFix().Mul(0.35m.ToFix())).Sub(0.5m.ToFix())).Add(MathHelper.PiOver2))).Add(Fix32Ext.Sin((((k.ToFix().Mul(0.5m.ToFix())).Add(MathHelper.Pi)).Add(6.ToFix())).Add(j.ToFix().Mul(0.25f.ToFix()))))) > 0.ToFix();
                    }
                }
            }
            var cellWidth = 1;
            var shape = new VoxelGridShape(cells, cellWidth.ToFix());
            var grid = new VoxelGrid(shape, new Vector3((-cellCountX * cellWidth).ToFix().Mul(0.5m.ToFix()), (-cellCountY * cellWidth).ToFix(), (-cellCountZ * cellWidth).ToFix().Mul(0.5m.ToFix())));
            Space.Add(grid);

            int width = 10;
            int height = 10;
            Fix32 blockWidth = 2.ToFix();
            Fix32 blockHeight = 1.ToFix();
            Fix32 blockLength = 1.ToFix();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var toAdd =
                        new Box(
                            new Vector3(
((i.ToFix().Mul(blockWidth)).Add((0.5m.ToFix()).Mul(blockWidth).Mul(((j % 2).ToFix())))).Sub((width.ToFix().Mul(blockWidth)).Mul(0.5m.ToFix())),
blockHeight.Mul(0.5m.ToFix()).Add(j.ToFix().Mul((blockHeight))),
0.ToFix()),
                            blockWidth, blockHeight, blockLength, 10.ToFix());
                    Space.Add(toAdd);
                }
            }

            game.Camera.Position = new Vector3(0.ToFix(), 0.ToFix(), 25.ToFix());

            for (int i = 0; i < cellCountX; ++i)
            {
                for (int j = 0; j < cellCountY; ++j)
                {
                    for (int k = 0; k < cellCountZ; ++k)
                    {
                        if (shape.Cells[i, j, k])
                        {
                            //This is a turbo-inefficient way to render things, but good enough for now. If you want to visualize a larger amount... you'll probably have to write your own.
                            game.ModelDrawer.Add(new DisplayModel(game.Content.Load<Model>("cube"), game.ModelDrawer)
                                {
                                    WorldTransform = Matrix.CreateWorldRH(grid.Position + new Vector3((i.ToFix().Add(0.5m.ToFix())).Mul(cellWidth.ToFix()), (j.ToFix().Add(0.5m.ToFix())).Mul(cellWidth.ToFix()), (k.ToFix().Add(0.5m.ToFix())).Mul(cellWidth.ToFix())), Vector3.Forward, Vector3.Up)
                                });
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Voxel Grid Demo"; }
        }
    }
}