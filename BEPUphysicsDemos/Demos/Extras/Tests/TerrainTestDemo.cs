using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Test class for performance analysis.
    /// </summary>
    public class TerrainTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public TerrainTestDemo(DemosGame game)
            : base(game)
        {
            //x and y, in terms of heightmaps, refer to their local x and y coordinates.  In world space, they correspond to x and z.
            //Setup the heights of the terrain.
            //int xLength = 384;
            //int zLength = 384;

            //Fix64 xSpacing = .5m;
            //Fix64 zSpacing = .5m;
            //var heights = new Fix64[xLength, zLength];
            //for (int i = 0; i < xLength; i++)
            //{
            //    for (int j = 0; j < zLength; j++)
            //    {
            //        Fix64 x = i - xLength / 2;
            //        Fix64 z = j - zLength / 2;
            //        heights[i, j] = (Fix64)(10 * (Math.Sin(x / 8) + Math.Sin(z / 8)));
            //    }
            //}
            ////Create the terrain.
            //var terrain = new Terrain(heights, new AffineTransform(
            //        new Vector3(xSpacing, 1, zSpacing),
            //        Quaternion.Identity,
            //        new Vector3(-xLength * xSpacing / 2, 0, -zLength * zSpacing / 2)));

            //Space.Add(terrain);
            //game.ModelDrawer.Add(terrain);

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Space.Add(new Box(new Vector3((i * 50).ToFix(), (-50).ToFix(), (j * 50).ToFix()), 50.ToFix(), 20.ToFix(), 50.ToFix()));
                }
            }


            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    CompoundBody cb = new CompoundBody(new CompoundShapeEntry[] 
                        {
                            new CompoundShapeEntry(new CapsuleShape(2.ToFix(), 1.ToFix()), new Vector3(0.ToFix(), 0.ToFix(),0.ToFix())),
                            new CompoundShapeEntry(new CapsuleShape(2.ToFix(), 1.ToFix()), new Vector3(2.ToFix(), 0.ToFix(),0.ToFix())),
                            new CompoundShapeEntry(new CapsuleShape(2.ToFix(), 1.ToFix()), new Vector3(4.ToFix(), 0.ToFix(),0.ToFix())),
                            new CompoundShapeEntry(new CapsuleShape(2.ToFix(), 1.ToFix()), new Vector3(6.ToFix(), 0.ToFix(),0.ToFix())),
                            new CompoundShapeEntry(new CapsuleShape(2.ToFix(), 1.ToFix()), new Vector3(8.ToFix(), 0.ToFix(),0.ToFix())),
                            new CompoundShapeEntry(new CapsuleShape(2.ToFix(), 1.ToFix()), new Vector3(10.ToFix(), 0.ToFix(),0.ToFix())),
                            new CompoundShapeEntry(new CapsuleShape(2.ToFix(), 1.ToFix()), new Vector3(12.ToFix(), 0.ToFix(),0.ToFix())),
                        }, 10.ToFix());
                    cb.Position = new Vector3((i * 15).ToFix(), 20.ToFix(), (j * 5).ToFix());

                    //CompoundBody cb = new CompoundBody(new CompoundShapeEntry[] 
                    //{
                    //    new CompoundShapeEntry(new SphereShape(1), new Vector3(0, 0,0)),
                    //    new CompoundShapeEntry(new SphereShape(1), new Vector3(2, -2,0)),
                    //    new CompoundShapeEntry(new SphereShape(1), new Vector3(4, 0,0)),
                    //    new CompoundShapeEntry(new SphereShape(1), new Vector3(6, -2,0)),
                    //    new CompoundShapeEntry(new SphereShape(1), new Vector3(8, 0,0)),
                    //    new CompoundShapeEntry(new SphereShape(1), new Vector3(10, -2,0)),
                    //    new CompoundShapeEntry(new SphereShape(1), new Vector3(12, 0,0)),
                    //}, 10);
                    //cb.Position = new Vector3(i * 15, 20, j * 5);

                    //CompoundBody cb = new CompoundBody(new CompoundShapeEntry[] 
                    //{
                    //    new CompoundShapeEntry(new CapsuleShape(0, 1), new Vector3(0, 0,0)),
                    //    new CompoundShapeEntry(new CapsuleShape(0, 1), new Vector3(2, -2,0)),
                    //    new CompoundShapeEntry(new CapsuleShape(0, 1), new Vector3(4, 0,0)),
                    //    new CompoundShapeEntry(new CapsuleShape(0, 1), new Vector3(6, -2,0)),
                    //    new CompoundShapeEntry(new CapsuleShape(0, 1), new Vector3(8, 0,0)),
                    //    new CompoundShapeEntry(new CapsuleShape(0, 1), new Vector3(10, -2,0)),
                    //    new CompoundShapeEntry(new CapsuleShape(0, 1), new Vector3(12, 0,0)),
                    //}, 10);
                    //cb.Position = new Vector3(i * 15, 20, j * 5);

                    cb.ActivityInformation.IsAlwaysActive = true;
                    cb.AngularVelocity = new Vector3(.01m.ToFix(), 0.ToFix(), 0.ToFix());

                    Space.Add(cb);
                }
            }



            game.Camera.Position = new Vector3(0.ToFix(), 30.ToFix(), 20.ToFix());

        }





        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Terrain"; }
        }
    }
}