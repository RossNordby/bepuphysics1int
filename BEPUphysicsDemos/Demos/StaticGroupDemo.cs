using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.CollisionShapes;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.BroadPhaseEntries;
using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Demo showing a whole bunch of efficient static geometry.
    /// </summary>
    public class StaticGroupDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public StaticGroupDemo(DemosGame game)
            : base(game)
        {


            //Creating a bunch of separate StaticMeshes or kinematic Entity objects for an environment can pollute the broad phase.
            //This is because the broad phase implementation doesn't have guarantees about what elements can collide, so it has to
            //traverse the acceleration structure all the way down to pairs to figure it out.  That can get expensive!

            //Individual objects, like StaticMeshes, can have very complicated geometry without hurting the broad phase because the broad phase
            //has no knowledge of the thousands of triangles in the mesh.  The StaticMesh itself knows that the triangles within the mesh
            //never need to collide, so it never needs to test them against each other.

            //Similarly, the StaticGroup can be given a bunch of separate collidables.  The broad phase doesn't directly know about these child collidables-
            //it only sees the StaticGroup.  The StaticGroup knows that the things inside it can't ever collide with each other, so no tests are needed.
            //This avoids the performance problem!

            //To demonstrate, we'll be creating a set of static objects and giving them to a group to manage.
            var collidables = new List<Collidable>();

			//Start with a whole bunch of boxes.  These are entity collidables, but without entities!
			Fix64 xSpacing = 6.ToFix();
			Fix64 ySpacing = 6.ToFix();
			Fix64 zSpacing = 6.ToFix();


            //NOTE: You might notice this demo takes a while to start, especially on the Xbox360.  Do not fear!  That's due to the creation of the graphics data, not the physics.
            //The physics can handle over 100,000 static objects pretty easily.  The graphics, not so much :)
            //Try disabling the game.ModelDrawer.Add() lines and increasing the number of static objects.  
            int xCount = 15;
            int yCount = 7;
            int zCount = 15;


            var random = new Random(5);
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    for (int k = 0; k < zCount; k++)
                    {
                        //Create a transform and the instance of the mesh.
                        var collidable = new ConvexCollidable<BoxShape>(new BoxShape((random.NextDouble().ToFix().Mul(6.ToFix())).Add(.5m.ToFix()), (random.NextDouble().ToFix().Mul(6.ToFix())).Add(.5m.ToFix()), random.NextDouble().ToFix().Mul(6.ToFix()).Add(.5m.ToFix())));

                        //This EntityCollidable isn't associated with an entity, so we must manually tell it where to sit by setting the WorldTransform.
                        //This also updates its bounding box.
                        collidable.WorldTransform = new RigidTransform(
                            new Vector3((i.ToFix().Mul(xSpacing)).Sub((xCount.ToFix().Mul(xSpacing)).Mul(.5m.ToFix())), (j.ToFix().Mul(ySpacing)).Add(3.ToFix()), k.ToFix().Mul(zSpacing).Sub((zCount.ToFix().Mul(zSpacing)).Mul(.5m.ToFix()))),
                            Quaternion.CreateFromAxisAngle(Vector3.Normalize(new Vector3(random.NextDouble().ToFix(), random.NextDouble().ToFix(), random.NextDouble().ToFix())), random.NextDouble().ToFix().Mul(100.ToFix())));

                        collidables.Add(collidable);
                    }
                }
            }


            //Now create a bunch of instanced meshes too.
            xSpacing = 6.ToFix();
            ySpacing = 6.ToFix();
            zSpacing = 6.ToFix();

            xCount = 10;
            yCount = 2;
            zCount = 10;

            Vector3[] vertices;
            int[] indices;
            ModelDataExtractor.GetVerticesAndIndicesFromModel(game.Content.Load<Model>("fish"), out vertices, out indices);
            var meshShape = new InstancedMeshShape(vertices, indices);

            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    for (int k = 0; k < zCount; k++)
                    {
                        //Create a transform and the instance of the mesh.
                        var transform = new AffineTransform(
                            new Vector3((random.NextDouble().ToFix().Mul(6.ToFix())).Add(.5m.ToFix()), (random.NextDouble().ToFix().Mul(6.ToFix())).Add(.5m.ToFix()), random.NextDouble().ToFix().Mul(6.ToFix()).Add(.5m.ToFix())),
                             Quaternion.CreateFromAxisAngle(Vector3.Normalize(new Vector3(random.NextDouble().ToFix(), random.NextDouble().ToFix(), random.NextDouble().ToFix())), random.NextDouble().ToFix().Mul(100.ToFix())),
                            new Vector3((i.ToFix().Mul(xSpacing)).Sub((xCount.ToFix().Mul(xSpacing)).Mul(.5m.ToFix())), (j.ToFix().Mul(ySpacing)).Add(50.ToFix()), k.ToFix().Mul(zSpacing).Sub((zCount.ToFix().Mul(zSpacing)).Mul(.5m.ToFix()))));
                        var mesh = new InstancedMesh(meshShape, transform);
                        //Making the triangles one-sided makes collision detection a bit more robust, since the backsides of triangles won't try to collide with things
                        //and 'pull' them back into the mesh.
                        mesh.Sidedness = TriangleSidedness.Counterclockwise;
                        collidables.Add(mesh);
                    }
                }
            }

            var ground = new ConvexCollidable<BoxShape>(new BoxShape(200.ToFix(), 1.ToFix(), 200.ToFix()));
            ground.WorldTransform = new RigidTransform(new Vector3(0.ToFix(), (-3).ToFix(), 0.ToFix()), Quaternion.Identity);
            collidables.Add(ground);

            var group = new StaticGroup(collidables);
            for (int i = 0; i < collidables.Count; ++i)
            {
                game.ModelDrawer.Add(collidables[i]);
            }
            Space.Add(group);




            //Create a bunch of dynamic boxes to drop on the staticswarm.
            xCount = 8;
            yCount = 3;
            zCount = 8;
            xSpacing = 3.ToFix();
            ySpacing = 5.ToFix();
            zSpacing = 3.ToFix();
            for (int i = 0; i < xCount; i++)
                for (int j = 0; j < zCount; j++)
                    for (int k = 0; k < yCount; k++)
                    {
                        Space.Add(new Box(new Vector3(
(xSpacing.Mul(i.ToFix())).Sub((((xCount - 1).ToFix()).Mul(xSpacing)).Div(2.ToFix())),
100.ToFix().Add(k.ToFix().Mul((ySpacing))),
(2.ToFix().Add(zSpacing.Mul(j.ToFix()))).Sub((((zCount - 1).ToFix()).Mul(zSpacing)).Div(2.ToFix()))),
1.ToFix(), 1.ToFix(), 1.ToFix(), 10.ToFix()));
                    }




            game.Camera.Position = new Vector3(0.ToFix(), 60.ToFix(), 90.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Static Group"; }
        }
    }
}