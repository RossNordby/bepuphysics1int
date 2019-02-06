
using System;
using System.Diagnostics;
using BEPUphysics.Constraints;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUphysics.NarrowPhaseSystems;
using BEPUphysics.Settings;
using BEPUutilities;

using Microsoft.Xna.Framework.Input;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Tests out the performance characteristics of the persistent manifold's contact management.
    /// </summary>
    public class PersistentManifoldStressTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public PersistentManifoldStressTestDemo(DemosGame game)
            : base(game)
        {
            var ground = new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), 200.ToFix(), 1.ToFix(), 200.ToFix());
            Space.Add(ground);



            var spawnVolume = new BoundingBox
                {
                    Min = new Vector3((-25).ToFix(), 2.ToFix(), (-25).ToFix()),
                    Max = new Vector3(25.ToFix(), 102.ToFix(), 25.ToFix())
                };

            var span = spawnVolume.Max - spawnVolume.Min;

            NarrowPhaseHelper.Factories.ConvexConvex.EnsureCount(30000);

            var random = new Random(5);
            for (int i = 0; i < 5000; ++i)
            {
                Vector3 position;
                position.X = spawnVolume.Min.X.Add((random.NextDouble().ToFix()).Mul(span.X));
                position.Y = spawnVolume.Min.Y.Add((random.NextDouble().ToFix()).Mul(span.Y));
                position.Z = spawnVolume.Min.Z.Add((random.NextDouble().ToFix()).Mul(span.Z));

                var entity = new Capsule(position, 2.ToFix(), 0.8m.ToFix(), 10.ToFix());
                Space.Add(entity);
            }

            for (int i = 0; i < 60; ++i)
            {
                Space.Update();
            }

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            
            var start = Stopwatch.GetTimestamp();
            for (int i = 0; i < 1000; ++i)
            {
                Space.Update();
            }
            var end = Stopwatch.GetTimestamp();
            var time = (end - start) / (double)Stopwatch.Frequency;

            Console.WriteLine("Time: {0}", time);
 
            game.Camera.Position = new Vector3((-10).ToFix(), 10.ToFix(), 10.ToFix());
            game.Camera.Yaw(MathHelper.Pi.Div((-4).ToFix()));
            game.Camera.Pitch(MathHelper.Pi.Div(9.ToFix()));
        }



        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Persistent Manifold Stress"; }
        }
    }
}