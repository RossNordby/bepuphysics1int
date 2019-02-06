using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities.DataStructures;
using System.Diagnostics;
using System;
using BEPUutilities;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.BroadPhaseSystems.Hierarchies;
using BEPUphysics.BroadPhaseSystems.SortAndSweep;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Benchmarks and verifies various broad phases against each other.
    /// </summary>
    public class BroadPhasesTestDemo : StandardDemo
    {
        enum Test
        {
            Update,
            RayCast,
            BoundingBoxQuery
        }
        Random rand = new Random(0);

        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public BroadPhasesTestDemo(DemosGame game)
            : base(game)
        {
            Space.Solver.IterationLimit = 0;
            Entity toAdd;
            //BoundingBox box = new BoundingBox(new Vector3(-5, 1, 1), new Vector3(5, 7, 7));
            BoundingBox box = new BoundingBox(new Vector3((-50).ToFix(), (-50).ToFix(), (-50).ToFix()), new Vector3(50.ToFix(), 50.ToFix(), 50.ToFix()));

            //DynamicHierarchyOld dhOld = new DynamicHierarchyOld(Space.ThreadManager);
            DynamicHierarchy dh = new DynamicHierarchy(Space.ParallelLooper);
            SortAndSweep1D sas1d = new SortAndSweep1D(Space.ParallelLooper);
            Grid2DSortAndSweep grid2DSAS = new Grid2DSortAndSweep(Space.ParallelLooper);
            //DynamicHierarchy dh = new DynamicHierarchy();
            //DynamicHierarchy4 dh4 = new DynamicHierarchy4();
            //SortAndSweep1D sas1d = new SortAndSweep1D();
            //Grid2DSortAndSweep grid2DSAS = new Grid2DSortAndSweep();

            //DynamicHierarchy2 dh2 = new DynamicHierarchy2();
            //DynamicHierarchy3 dh3 = new DynamicHierarchy3();
            //SortAndSweep3D sap3d = new SortAndSweep3D();

            RawList<Entity> entities = new RawList<Entity>();
            for (int k = 0; k < 100; k++)
            {
                Vector3 position = new Vector3((rand.NextDouble().ToFix()).Mul((box.Max.X.Sub(box.Min.X))).Add(box.Min.X),
(rand.NextDouble().ToFix()).Mul((box.Max.Y.Sub(box.Min.Y))).Add(box.Min.Y),
(rand.NextDouble().ToFix()).Mul((box.Max.Z.Sub(box.Min.Z))).Add(box.Min.Z));
                toAdd = new Box(position, 1.ToFix(), 1.ToFix(), 1.ToFix(), 1.ToFix());
                toAdd.CollisionInformation.CollisionRules.Personal = CollisionRule.NoNarrowPhasePair;
                toAdd.CollisionInformation.UpdateBoundingBox(0.ToFix());
                //Space.Add(toAdd);
                //dhOld.Add(toAdd.CollisionInformation);
                dh.Add(toAdd.CollisionInformation);
                sas1d.Add(toAdd.CollisionInformation);
                grid2DSAS.Add(toAdd.CollisionInformation);
                entities.Add(toAdd);

            }


            Space.ForceUpdater.Gravity = new Vector3();

            int numRuns = 10000;
            //Prime the system.
            grid2DSAS.Update();
            sas1d.Update();
            //dhOld.Update();
            dh.Update();
            var testType = Test.Update;

            double startTime, endTime;

            switch (testType)
            {
                #region Update Timing
                case Test.Update:
                    for (int i = 0; i < numRuns; i++)
                    {
                        ////DH
                        //startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                        //dhOld.Update();
                        //endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                        //DHOldTime += endTime - startTime;

                        //DH4
                        startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                        dh.Update();
                        endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                        DHtime += endTime - startTime;

                        //SAP1D
                        startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                        sas1d.Update();
                        endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                        SAS1Dtime += endTime - startTime;

                        //Grid2D SOS
                        startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                        grid2DSAS.Update();
                        endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                        grid2DSAStime += endTime - startTime;

                        //if (sap1d.Overlaps.Count != dh.Overlaps.Count)
                        //    Debug.WriteLine("SAP1D Failure");
                        //if (grid2DSOS.Overlaps.Count != dh.Overlaps.Count)
                        //    Debug.WriteLine("grid2DSOS Failure");

                        //for (int j = 0; j < dh2.Overlaps.Count; j++)
                        //{
                        //    if (!grid2DSOS.Overlaps.Contains(dh2.Overlaps[j]))
                        //        Debug.WriteLine("Break.");
                        //}
                        //for (int j = 0; j < grid2DSOS.Overlaps.Count; j++)
                        //{
                        //    if (!dh2.Overlaps.Contains(grid2DSOS.Overlaps[j]))
                        //        break;
                        //}

                        //for (int j = 0; j < grid2DSOS.Overlaps.Count; j++)
                        //{
                        //    if (!dh4.Overlaps.Contains(grid2DSOS.Overlaps[j]))
                        //        break;
                        //}

                        //for (int j = 0; j < dh.Overlaps.Count; j++)
                        //{
                        //    if (!dh.Overlaps[j].EntryA.BoundingBox.Intersects(dh.Overlaps[j].EntryB.BoundingBox))
                        //        Debug.WriteLine("Break.");
                        //}

                        //for (int j = 0; j < sap1d.Overlaps.Count; j++)
                        //{
                        //    if (!sap1d.Overlaps[j].EntryA.BoundingBox.Intersects(sap1d.Overlaps[j].EntryB.BoundingBox))
                        //        Debug.WriteLine("Break.");
                        //}

                        //for (int j = 0; j < grid2DSOS.Overlaps.Count; j++)
                        //{
                        //    if (!grid2DSOS.Overlaps[j].EntryA.BoundingBox.Intersects(grid2DSOS.Overlaps[j].EntryB.BoundingBox))
                        //        Debug.WriteLine("Break.");
                        //}

                        //MoveEntities(entities);
                    }
                    break;
                #endregion
                #region Ray cast timing
                case Test.RayCast:
					Fix64 rayLength = 100.ToFix();
                    RawList<Ray> rays = new RawList<Ray>();
                    for (int i = 0; i < numRuns; i++)
                    {
                        rays.Add(new Ray()
                        {
                            Position = new Vector3((rand.NextDouble().ToFix()).Mul((box.Max.X.Sub(box.Min.X))).Add(box.Min.X),
(rand.NextDouble().ToFix()).Mul((box.Max.Y.Sub(box.Min.Y))).Add(box.Min.Y),
(rand.NextDouble().ToFix()).Mul((box.Max.Z.Sub(box.Min.Z))).Add(box.Min.Z)),
                            Direction = Vector3.Normalize(new Vector3((rand.NextDouble() - .5).ToFix(), (rand.NextDouble() - .5).ToFix(), (rand.NextDouble() - .5).ToFix()))
                        });
                    }
                    RawList<BroadPhaseEntry> outputIntersections = new RawList<BroadPhaseEntry>();


                    ////DH
                    //startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    //for (int i = 0; i < numRuns; i++)
                    //{
                    //    dhOld.QueryAccelerator.RayCast(rays.Elements[i], rayLength, outputIntersections);
                    //    outputIntersections.Clear();

                    //}
                    //endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    //DHOldTime = endTime - startTime;

                    //DH4
                    startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    for (int i = 0; i < numRuns; i++)
                    {
                        dh.QueryAccelerator.RayCast(rays.Elements[i], rayLength, outputIntersections);
                        outputIntersections.Clear();
                    }

                    endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    DHtime = endTime - startTime;

                    //Grid2DSAS
                    startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    for (int i = 0; i < numRuns; i++)
                    {
                        grid2DSAS.QueryAccelerator.RayCast(rays.Elements[i], rayLength, outputIntersections);
                        outputIntersections.Clear();
                    }
                    endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    grid2DSAStime = endTime - startTime;
                    break;
                #endregion
                #region Bounding box query timing
                case Test.BoundingBoxQuery:
                    Fix64 boundingBoxSize = 10.ToFix();
                    var boundingBoxes = new RawList<BoundingBox>();
                    Vector3 offset = new Vector3(boundingBoxSize.Div(2.ToFix()), boundingBoxSize.Div(2.ToFix()), boundingBoxSize.Div(2.ToFix()));
                    for (int i = 0; i < numRuns; i++)
                    {
                        Vector3 center = new Vector3((rand.NextDouble().ToFix()).Mul((box.Max.X.Sub(box.Min.X))).Add(box.Min.X),
(rand.NextDouble().ToFix()).Mul((box.Max.Y.Sub(box.Min.Y))).Add(box.Min.Y),
(rand.NextDouble().ToFix()).Mul((box.Max.Z.Sub(box.Min.Z))).Add(box.Min.Z));
                        boundingBoxes.Add(new BoundingBox()
                        {
                            Min = center - offset,
                            Max = center + offset
                        });
                    }

                    outputIntersections = new RawList<BroadPhaseEntry>();

                    ////DH
                    //startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    //for (int i = 0; i < numRuns; i++)
                    //{
                    //    dhOld.QueryAccelerator.GetEntries(boundingBoxes.Elements[i], outputIntersections);
                    //    outputIntersections.Clear();

                    //}
                    //endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    //DHOldTime = endTime - startTime;

                    //DH4
                    startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    for (int i = 0; i < numRuns; i++)
                    {
                        dh.QueryAccelerator.GetEntries(boundingBoxes.Elements[i], outputIntersections);
                        outputIntersections.Clear();
                    }

                    endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    DHtime = endTime - startTime;

                    //Grid2DSAS
                    startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    for (int i = 0; i < numRuns; i++)
                    {
                        grid2DSAS.QueryAccelerator.GetEntries(boundingBoxes.Elements[i], outputIntersections);
                        outputIntersections.Clear();
                    }
                    endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
                    grid2DSAStime = endTime - startTime;
                    break;
                #endregion
            }


            DHOldTime /= numRuns;
            DH2time /= numRuns;
            DH3time /= numRuns;
            DHtime /= numRuns;
            SAS1Dtime /= numRuns;
            grid2DSAStime /= numRuns;


        }

        void MoveEntities(RawList<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
				Fix64 speed = .1m.ToFix();
                //entities[i].Position += new Vector3((Fix64)rand.NextDouble() * speed - speed * .5m, (Fix64)rand.NextDouble() * speed - speed * .5m, (Fix64)rand.NextDouble() * speed - speed * .5m);
                entities[i].Position += new Vector3(0.ToFix(), speed, 0.ToFix());
                entities[i].CollisionInformation.UpdateBoundingBox(0.ToFix());
            }
        }

        double DHOldTime;
        double DH2time;
        double DH3time;
        double DHtime;
        double SAS1Dtime;
        double SAP3Dtime;
        double grid2DSAStime;

        public override void DrawUI()
        {
            base.DrawUI();
            Game.DataTextDrawer.Draw("Time per DHold:    ", DHOldTime * 1e6f, new Microsoft.Xna.Framework.Vector2(50, 50));
            Game.DataTextDrawer.Draw("Time per DH:    ", DHtime * 1e6f, new Microsoft.Xna.Framework.Vector2(50, 80));
            Game.DataTextDrawer.Draw("Time per SAP1D: ", SAS1Dtime * 1e6f, new Microsoft.Xna.Framework.Vector2(50, 110));
            Game.DataTextDrawer.Draw("Time per Grid2DSortAndSweep: ", grid2DSAStime * 1e6f, new Microsoft.Xna.Framework.Vector2(50, 140));
        }



        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Broad Phase Testing"; }
        }

    }
}