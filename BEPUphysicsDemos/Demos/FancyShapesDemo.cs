using System;
using System.Collections.Generic;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.CollisionShapes.ConvexShapes;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Non-standard shapes like MinkowskiSums, WrappedBodies, and ConvexHulls.
    /// </summary>
    public class FancyShapesDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public FancyShapesDemo(DemosGame game)
            : base(game)
        {


            var points = new List<Vector3>();


            //Setup a random distribution in a cube and compute a convex hull.
            var random = new Random(0);
            for (int k = 0; k < 40; k++)
            {
                points.Add(new Vector3(3.ToFix().Mul(random.NextDouble().ToFix()), 5.ToFix().Mul(random.NextDouble().ToFix()), 3.ToFix().Mul(random.NextDouble().ToFix())));
            }
            var convexHull = new ConvexHull(new Vector3(0.ToFix(), 7.ToFix(), 0.ToFix()), points, 10.ToFix());

            Space.Add(convexHull);

            points.Clear();


            //Create another random distribution, but this time with more points.
            points.Clear();
            for (int k = 0; k < 400; k++)
            {
                points.Add(new Vector3(1.ToFix().Mul(random.NextDouble().ToFix()), 3.ToFix().Mul(random.NextDouble().ToFix()), 1.ToFix().Mul(random.NextDouble().ToFix())));
            }


            convexHull = new ConvexHull(new Vector3(4.ToFix(), 7.ToFix(), 0.ToFix()), points, 10.ToFix());
            Space.Add(convexHull);



            //Minkowski Sums are fancy 'combinations' of objects, where the result is the sum of the individual points making up shapes.
            //Think of it as sweeping one shape around and through another; a sphere and a box would produce a rounded-edge box.
            var minkowskiSum = new MinkowskiSum(new Vector3(4.ToFix(), (-3).ToFix(), 0.ToFix()),
                    new OrientedConvexShapeEntry(new BoxShape(2.ToFix(), 2.ToFix(), 2.ToFix())),
                    new OrientedConvexShapeEntry(new ConeShape(2.ToFix(), 2.ToFix())), 10.ToFix());
            Space.Add(minkowskiSum);

            minkowskiSum = new MinkowskiSum(new Vector3(0.ToFix(), 3.ToFix(), 0.ToFix()),
                    new OrientedConvexShapeEntry(Quaternion.CreateFromYawPitchRoll(1.ToFix(), 2.ToFix(), 3.ToFix()), new ConeShape(1.ToFix(), 1.ToFix())),
                    new OrientedConvexShapeEntry(new TriangleShape(Vector3.Zero, Vector3.Right, Vector3.Forward)), 1.ToFix());
            Space.Add(minkowskiSum);

            //Note how this minkowski sum is composed of a cylinder, and another minkowski sum shape.
            minkowskiSum = new MinkowskiSum(new Vector3((-4).ToFix(), 10.ToFix(), 0.ToFix()),
                    new OrientedConvexShapeEntry(new CylinderShape(1.ToFix(), 2.ToFix())),
                    new OrientedConvexShapeEntry(new MinkowskiSumShape(
                        new OrientedConvexShapeEntry(new TriangleShape(new Vector3(1.ToFix(), 1.ToFix(), 1.ToFix()), new Vector3((-2).ToFix(), 0.ToFix(), 0.ToFix()), new Vector3(0.ToFix(), (-1).ToFix(), 0.ToFix()))),
                        new OrientedConvexShapeEntry(new BoxShape(.3m.ToFix(), 1.ToFix(), .3m.ToFix())))), 10.ToFix());
            Space.Add(minkowskiSum);

            //Minkowski sums can also be used on more than two shapes at once.  The two-shape constructor is just a convenience wrapper.


            //Wrapped objects use an implicit convex hull around a set of shapes.

            //Oblique cone:
            var cone = new List<ConvexShapeEntry>
            {
                new ConvexShapeEntry(new CylinderShape(0.ToFix(), 1.ToFix())),
                new ConvexShapeEntry(new RigidTransform(new Vector3(1.ToFix(), 2.ToFix(), 0.ToFix())), new SphereShape(0.ToFix())) 
            };
            Space.Add(new WrappedBody(new Vector3((-5).ToFix(), 0.ToFix(), 0.ToFix()), cone, 10.ToFix()));



            //Rather odd shape:
            var oddShape = new List<ConvexShapeEntry>();
            var bottom = new ConvexShapeEntry(new Vector3((-2).ToFix(), 2.ToFix(), 0.ToFix()), new SphereShape(2.ToFix()));
            var middle = new ConvexShapeEntry(
                new RigidTransform(
                    new Vector3((-2).ToFix(), 3.ToFix(), 0.ToFix()),
                    Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.Pi.Div(6.ToFix()))),
                    new CylinderShape(0.ToFix(), 3.ToFix()));
            var top = new ConvexShapeEntry(new Vector3((-2).ToFix(), 4.ToFix(), 0.ToFix()), new SphereShape(1.ToFix()));
            oddShape.Add(bottom);
            oddShape.Add(middle);
            oddShape.Add(top);
            Space.Add(new WrappedBody(new Vector3((-3).ToFix(), 4.ToFix(), 0.ToFix()), oddShape, 10.ToFix()));

            //Transformable shapes can be any other kind of convex primitive transformed by any affine transformation.
            Matrix3x3 transform;
            transform = Matrix3x3.Identity;
            transform.M23 = .5m.ToFix();
            transform.M13 = .5m.ToFix();
            var transformable = new TransformableEntity(new Vector3(0.ToFix(), 0.ToFix(), 4.ToFix()), new BoxShape(1.ToFix(), 1.ToFix(), 1.ToFix()), transform, 10.ToFix());
            Space.Add(transformable);


            Space.Add(new Box(new Vector3(0.ToFix(), (-10).ToFix(), 0.ToFix()), 70.ToFix(), 5.ToFix(), 70.ToFix()));

            game.Camera.Position = new Vector3(0.ToFix(), 0.ToFix(), 30.ToFix());

        }


        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Fancy Shapes"; }
        }
    }
}