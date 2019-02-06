using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.NarrowPhaseSystems;
using BEPUphysics.UpdateableSystems.ForceFields;
using BEPUphysicsDemos.SampleCode;
using System.Diagnostics;
using System;
using BEPUutilities.Threading;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Benchmarks and verifies various broad phases against each other.
    /// </summary>
    public class MultithreadedScalingTestDemo : StandardDemo
    {
        Func<Space, int>[] simulationBuilders;
        double[,] testResults;

        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public MultithreadedScalingTestDemo(DemosGame game)
            : base(game)
        {
            simulationBuilders = new Func<Space, int>[]
                                     {
                                         BuildPileSimulation,
                                         BuildWallSimulation,
                                         BuildPlanetSimulation
                                     };

#if WINDOWS
            int coreCountMax = Environment.ProcessorCount;

            testResults = new double[coreCountMax, simulationBuilders.Length];

            int reruns = 1;
            for (int i = 0; i < reruns; i++)
            {
                GC.Collect();
                var looper = new ParallelLooper();



                //Try different thread counts.
                for (int j = 0; j < coreCountMax; j++)
                {
                    looper.AddThread();
                    for (int k = 0; k < simulationBuilders.Length; k++)
                        testResults[j, k] = RunTest(looper, simulationBuilders[k]);
                    GC.Collect();

                }
            }
#else
            testResults = new double[4, simulationBuilders.Length];
            int reruns = 10;
            for (int i = 0; i < reruns; i++)
            {
                GC.Collect();
                var threadManager = new SpecializedThreadManager();

                threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 1 }); }, null);

                for (int k = 0; k < simulationBuilders.Length; k++)
                    testResults[0, k] += RunTest(threadManager, simulationBuilders[k]);
                GC.Collect();

                threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 3 }); }, null);
                for (int k = 0; k < simulationBuilders.Length; k++)
                    testResults[1, k] += RunTest(threadManager, simulationBuilders[k]);
                GC.Collect();
                threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 5 }); }, null);
                for (int k = 0; k < simulationBuilders.Length; k++)
                    testResults[2, k] += RunTest(threadManager, simulationBuilders[k]);
                GC.Collect();
                threadManager.AddThread(delegate { Thread.CurrentThread.SetProcessorAffinity(new[] { 4 }); }, null);
                for (int k = 0; k < simulationBuilders.Length; k++)
                    testResults[3, k] += RunTest(threadManager, simulationBuilders[k]);
                GC.Collect();
            }
#endif





        }

        int BuildPileSimulation(Space space)
        {
            Random rand = new Random(0);

#if WINDOWS
            NarrowPhaseHelper.Factories.BoxBox.EnsureCount(30000);
            BoundingBox box = new BoundingBox(new Vector3((-5).ToFix(), 10.ToFix(), (-5).ToFix()), new Vector3(5.ToFix(), 300.ToFix(), 5.ToFix()));
            for (int k = 0; k < 5000; k++)
#else        
            NarrowPhaseHelper.Factories.BoxBox.EnsureCount(1500);
            BoundingBox box = new BoundingBox(new Vector3(-5, 10, -5), new Vector3(5, 20, 5));
            for (int k = 0; k < 250; k++)
#endif
            {
                Vector3 position = new Vector3((rand.NextDouble().ToFix()).Mul((box.Max.X.Sub(box.Min.X))).Add(box.Min.X),
(rand.NextDouble().ToFix()).Mul((box.Max.Y.Sub(box.Min.Y))).Add(box.Min.Y),
(rand.NextDouble().ToFix()).Mul((box.Max.Z.Sub(box.Min.Z))).Add(box.Min.Z));
                var toAdd = new Box(position, 1.ToFix(), 1.ToFix(), 1.ToFix(), 1.ToFix());
                toAdd.ActivityInformation.IsAlwaysActive = true;

                space.Add(toAdd);


            }

            Box ground = new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 300.ToFix(), 10.ToFix(), 300.ToFix());
            space.Add(ground);

#if WINDOWS
            return 700;
#else
            return 350;
#endif
        }

        int BuildWallSimulation(Space space)
        {
#if WINDOWS
            int width = 100;
            int height = 40;
            NarrowPhaseHelper.Factories.BoxBox.EnsureCount(20000);
#else
            NarrowPhaseHelper.Factories.BoxBox.EnsureCount(2000);
            int width = 25;
            int height = 15;
#endif
            Fix64 blockWidth = 2.ToFix();
            Fix64 blockHeight = 1.ToFix();
            Fix64 blockLength = 3.ToFix();


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var toAdd =
                        new Box(
                            new Vector3(
((i.ToFix().Mul(blockWidth)).Add((.5m.ToFix().Mul(blockWidth)).Mul(((j % 2).ToFix())))).Sub((width.ToFix().Mul(blockWidth)).Mul(.5m.ToFix())),
(blockHeight.Mul(.5m.ToFix())).Add(j.ToFix().Mul((blockHeight))),
0.ToFix()),
                            blockWidth, blockHeight, blockLength, 10.ToFix());
                    toAdd.ActivityInformation.IsAlwaysActive = true;
                    space.Add(toAdd);

                }
            }

            Box ground = new Box(new Vector3(0.ToFix(), (-5).ToFix(), 0.ToFix()), 500.ToFix(), 10.ToFix(), 500.ToFix());
            space.Add(ground);
#if WINDOWS
            return 800;
#else
            return 400;
#endif
        }

        int BuildPlanetSimulation(Space space)
        {
            space.ForceUpdater.Gravity = Vector3.Zero;


            var planet = new Sphere(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 30.ToFix());
            space.Add(planet);

            var field = new GravitationalField(new InfiniteForceFieldShape(), planet.Position, (66730 / 2).ToFix(), 100.ToFix());
            space.Add(field);

            //Drop the "meteorites" on the planet.
            Entity toAdd;
#if WINDOWS
            //By pre-allocating a bunch of box-box pair handlers, the simulation will avoid having to allocate new ones at runtime.
            NarrowPhaseHelper.Factories.BoxBox.EnsureCount(30000);

            int numColumns = 25;
            int numRows = 25;
            int numHigh = 25;
#else
            NarrowPhaseHelper.Factories.BoxBox.EnsureCount(2000);
            int numColumns = 10;
            int numRows = 10;
            int numHigh = 10;
#endif
            Fix64 separation = 5.ToFix();
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        toAdd = new Box(new Vector3((separation.Mul(i.ToFix())).Sub((numRows.ToFix().Mul(separation)).Div(2.ToFix())), 40.ToFix().Add(k.ToFix().Mul(separation)), (separation.Mul(j.ToFix())).Sub((numColumns.ToFix().Mul(separation)).Div(2.ToFix()))), 1.ToFix(), 1.ToFix(), 1.ToFix(), 5.ToFix());
                        toAdd.LinearVelocity = new Vector3(30.ToFix(), 0.ToFix(), 0.ToFix());
                        toAdd.LinearDamping = 0.ToFix();
                        toAdd.AngularDamping = 0.ToFix();
                        space.Add(toAdd);
                    }
#if WINDOWS
            return 3000;
#else
            return 1000;
#endif
        }

        double RunTest(IParallelLooper threadManager, Func<Space, int> simulationBuilder)
        {

            Space space = new Space(threadManager);

            var timeStepCount = simulationBuilder(space);


            //Perform one starter frame to warm things up.
            space.Update();

            var startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            for (int i = 0; i < timeStepCount; i++)
            {
                space.Update();
            }
            var endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            return endTime - startTime;
        }

        private int timeStepsElapsed;

        public override void Update(Fix64 dt)
        {
            timeStepsElapsed++;
            base.Update(dt);
        }



        public override void DrawUI()
        {
            base.DrawUI();
            //Game.DataTextDrawer.Draw("Time steps elapsed: ", timeStepsElapsed, new Vector2(600, 600));
            //return;
            Microsoft.Xna.Framework.Vector2 origin = new Microsoft.Xna.Framework.Vector2(100, 50);
            Microsoft.Xna.Framework.Vector2 spacing = new Microsoft.Xna.Framework.Vector2(80, 50);
            //Draw the horizontal core counts.
            for (int i = 0; i < testResults.GetLength(0); i++)
            {
                Game.DataTextDrawer.Draw(i + 1, origin + new Microsoft.Xna.Framework.Vector2(spacing.X * i, -30));
            }
            for (int i = 0; i < testResults.GetLength(1); i++)
            {
                Game.DataTextDrawer.Draw(i, origin + new Microsoft.Xna.Framework.Vector2(-30, spacing.Y * i));
            }

            for (int i = 0; i < testResults.GetLength(0); i++)
            {
                int lowestTime = 0;
                for (int j = 0; j < testResults.GetLength(1); j++)
                {
                    Game.DataTextDrawer.Draw(testResults[i, j] * 1e3, 0, origin + new Microsoft.Xna.Framework.Vector2(spacing.X * i, spacing.Y * j));
                    if (testResults[i, j] < testResults[i, lowestTime])
                        lowestTime = j;
                }
            }



        }



        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Multithreaded Scaling"; }
        }

    }
}