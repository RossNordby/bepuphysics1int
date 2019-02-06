using System;
using System.Collections.Generic;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUutilities.DataStructures;
using BEPUphysics.CollisionShapes;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo testing convex hulls.
    /// </summary>
    public class ConvexHullTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public ConvexHullTestDemo(DemosGame game)
            : base(game)
        {

            var random = new Random(5);

            for (int i = 0; i < 500000; ++i)
            {
                List<Vector3> points = new List<Vector3>();
                for (int k = 0; k < random.Next(8, 60); k++)
                {
                    points.Add(new Vector3((-100).ToFix().Add(30.ToFix().Mul(random.NextDouble().ToFix())), 100.ToFix().Add(500.ToFix().Mul(random.NextDouble().ToFix())), 100.ToFix().Add(30.ToFix().Mul(random.NextDouble().ToFix()))));
                }
                var convexHull = new ConvexHull(new Vector3(0.ToFix(), 7.ToFix(), 0.ToFix()), points, 10.ToFix());
                Console.WriteLine(convexHull.CollisionInformation.Shape.Vertices.Count);
            }
            
            var vertices = new[] 
            { 
                new Vector3(0.ToFix(), (-1.750886E-9m).ToFix(), (-1.5m).ToFix()),
                new Vector3(1.ToFix(), 1.ToFix(), 0.5m.ToFix()), 
                new Vector3(1.ToFix(), (-1).ToFix(), 0.5m.ToFix()),
                new Vector3((-1).ToFix(), 1.ToFix(), 0.5m.ToFix()), 
                new Vector3((-1).ToFix(), (-1).ToFix(), 0.5m.ToFix()), 
            };

            var hullVertices = new RawList<Vector3>();
            ConvexHullHelper.GetConvexHull(vertices, hullVertices);

            ConvexHull hull = new ConvexHull(vertices, 5.ToFix());
            Space.Add(hull);

            Box ground = new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix());
            Space.Add(ground);
            game.Camera.Position = new Vector3(0.ToFix(), 6.ToFix(), 15.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Convex Hull Testing"; }
        }
    }
}