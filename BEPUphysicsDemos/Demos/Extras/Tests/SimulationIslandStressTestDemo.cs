using System;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using System.Diagnostics;
using BEPUphysics.NarrowPhaseSystems;


namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Spheres fall onto a large terrain.  Try driving around on it!
    /// </summary>
    public class SimulationIslandStressTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public SimulationIslandStressTestDemo(DemosGame game)
            : base(game)
        {

            Space.Add(new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 1000.ToFix(), 10.ToFix(), 1000.ToFix()));
            //MotionSettings.DefaultPositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;

            NarrowPhaseHelper.Factories.SphereSphere.EnsureCount(35000);
            NarrowPhaseHelper.Factories.BoxSphere.EnsureCount(5000);
            //NarrowPhaseHelper.Factories.BoxBox.EnsureCount(35000);
            //Space.BroadPhase = new Grid2DSortAndSweep(Space.ThreadManager);

            ConfigurationHelper.ApplySuperSpeedySettings(Space);
            //Space.ForceUpdater.Gravity = new Vector3();
            Space.Solver.IterationLimit = 1;



            Random rand = new Random();


            //Fix64 width = 200;
            //Fix64 height = 200;
            //Fix64 length = 200;
            //for (int i = 0; i < 5000; i++)
            //{
            //    Vector3 position =
            //        new Vector3((Fix64)rand.NextDouble() * width - width * .5m,
            //            (Fix64)rand.NextDouble() * height + 20,
            //            (Fix64)rand.NextDouble() * length - length * .5m);
            //    var sphere = new Sphere(position, 1, 1);// { Tag = "noDisplayObject" };
            //    sphere.ActivityInformation.IsAlwaysActive = true;
            //    Space.Add(sphere);
            //}

            Fix32 width = 10.ToFix();
            Fix32 height = 50.ToFix();
            Fix32 length = 10.ToFix();
            for (int i = 0; i.ToFix() < width; i++)
            {
                for (int j = 0; j.ToFix() < height; j++)
                {
                    for (int k = 0; k.ToFix() < length; k++)
                    {
                        Vector3 position =
                            new Vector3((i * 3 + j * .2m).ToFix(),
(20 + j * 3).ToFix(),
(k * 3 + j * .2m).ToFix());
                        var e = new Sphere(position, 1.ToFix(), 1.ToFix());// { Tag = "noDisplayObject" };
                        //var e = new Box(position, 1, 1, 1, 1) { Tag = "noDisplayObject" };
                        e.ActivityInformation.IsAlwaysActive = true;
                        Space.Add(e);
                    }
                }
            }





            game.Camera.Position = new Vector3(0.ToFix(), 30.ToFix(), 20.ToFix());

            //Pre-simulate.
            for (int i = 0; i < 30; i++)
            {
                //Space.Update();
            }

            int numRuns = 500;

            double startTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            for (int i = 0; i < numRuns; i++)
            {
                //Space.Update();
            }

            double endTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            time = endTime - startTime;
        }

        double time;

        public override void DrawUI()
        {
            //Game.DataTextDrawer.Draw("Simulation time: ", time, 5, new Vector2(50, 50));
            base.DrawUI();
        }


        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Simulation Island Stress Test"; }
        }
    }
}