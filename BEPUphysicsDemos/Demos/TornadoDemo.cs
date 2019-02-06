using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.UpdateableSystems.ForceFields;
using BEPUphysicsDemos.SampleCode;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Unsuspecting blocks get ambushed by a whirlwind.
    /// </summary>
    public class TornadoDemo : StandardDemo
    {
        private readonly BoundingBoxForceFieldShape shape;
        private readonly Tornado tornado;

        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public TornadoDemo(DemosGame game)
            : base(game)
        {
            shape = new BoundingBoxForceFieldShape(new BoundingBox(new Vector3((-100).ToFix(), (-20).ToFix(), (-40).ToFix()), new Vector3((-20).ToFix(), 120.ToFix(), 40.ToFix())));
            tornado = new Tornado(shape, (shape.BoundingBox.Min + shape.BoundingBox.Max) / 2.ToFix(), new Vector3(0.ToFix(), 1.ToFix(), 0.ToFix()),
150.ToFix(), false, 50.ToFix(), 10.ToFix(), 200.ToFix(), 200.ToFix(), 80.ToFix(), 2000.ToFix(), 40.ToFix(), 10.ToFix());
            tornado.ForceWakeUp = true; //The tornado will be moving, so it should wake up things that it comes into contact with.
            Space.Add(tornado);

            //Create the unfortunate box-like citizens about to be hit by the tornado.
            int numColumns = 10;
            int numRows = 10;
            int numHigh = 1;
			Fix64 separation = 1.5m.ToFix();
            Entity toAdd;
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        toAdd = new Box(new Vector3(
(separation.Mul(i.ToFix())).Sub((numRows.ToFix().Mul(separation)).Div(2.ToFix())),
5.ToFix().Add(k.ToFix().Mul(separation)),
(separation.Mul(j.ToFix())).Sub((numColumns.ToFix().Mul(separation)).Div(2.ToFix()))),
1.ToFix(), 1.ToFix(), 1.ToFix(), 10.ToFix());
                        Space.Add(toAdd);
                    }

            //x and y, in terms of heightmaps, refer to their local x and y coordinates.  In world space, they correspond to x and z.
            //Setup the heights of the terrain.
            //[The size here is limited by the Reach profile the demos use- the drawer draws the terrain as a big block and runs into primitive drawing limits.
            //The physics can support far larger terrains!]
            int xLength = 180;
            int zLength = 180;

			Fix64 xSpacing = 8.ToFix();
			Fix64 zSpacing = 8.ToFix();
            var heights = new Fix64[xLength,zLength];
            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < zLength; j++)
                {
					Fix64 x = (i - xLength / 2).ToFix();
					Fix64 z = (j - zLength / 2).ToFix();
                    //heights[i,j] = (Fix64)Math.Pow(1.2 * Math.Sqrt(x * x + y * y), 2);
                    //heights[i,j] = -1f / (x * x + y * y);
                    //heights[i,j] = (Fix64)(x * y / 100f);
                    heights[i,j] = 5.ToFix().Mul((Fix64Ext.Sin(x.Div(8.ToFix())).Add(Fix64Ext.Sin(z.Div(8.ToFix())))));
                    //heights[i,j] = 3 * (Fix64)Math.Sin(x * y / 100f);
                    //heights[i,j] = (x * x * x * y - y * y * y * x) / 1000f;
                }
            }

            //Create the terrain.
            var terrain = new Terrain(heights, new AffineTransform(
                new Vector3(xSpacing, 1.ToFix(), zSpacing), 
                Quaternion.Identity, 
                new Vector3(((-xLength).ToFix().Mul(xSpacing)).Div(2.ToFix()), 0.ToFix(), ((-zLength).ToFix().Mul(zSpacing)).Div(2.ToFix()))));
            Space.Add(terrain);
            game.ModelDrawer.Add(terrain);
            game.Camera.Position = new Vector3(0.ToFix(), 5.ToFix(), 60.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Tornado"; }
        }

        public override void Update(Fix64 dt)
        {
            //Move the origin of the force of the tornado,
            Vector3 increment = new Vector3(10.ToFix(), 0.ToFix(), 0.ToFix()) * dt;
            //Move the detection shape as well.
            shape.BoundingBox = new BoundingBox(shape.BoundingBox.Min + increment, shape.BoundingBox.Max + increment);
            tornado.Position += increment;
            base.Update(dt);
        }
    }
}