using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;

using Microsoft.Xna.Framework.Graphics;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Drops some objects onto a field of transformed mesh instances.
    /// Instanced meshes are a low-memory usag approach to creating
    /// lots of similar static collision objects.
    /// </summary>
    public class InstancedMeshDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public InstancedMeshDemo(DemosGame game)
            : base(game)
        {



            Vector3[] vertices;
            int[] indices;
            ModelDataExtractor.GetVerticesAndIndicesFromModel(game.Content.Load<Model>("guy"), out vertices, out indices);
            var meshShape = new InstancedMeshShape(vertices, indices);

            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    //Create a transform and the instance of the mesh.
                    var transform = new AffineTransform(
                        new Vector3((random.NextDouble().ToFix().Mul(6.ToFix())).Add(.5m.ToFix()), (random.NextDouble().ToFix().Mul(6.ToFix())).Add(.5m.ToFix()), (random.NextDouble().ToFix().Mul(6.ToFix())).Add(.5m.ToFix())), 
                         Quaternion.CreateFromAxisAngle(Vector3.Normalize(new Vector3(random.NextDouble().ToFix(), random.NextDouble().ToFix(), random.NextDouble().ToFix())), random.NextDouble().ToFix().Mul(100.ToFix())),
                        new Vector3((i * 2).ToFix(), 3.ToFix(), (j * 2).ToFix()));
                    var mesh = new InstancedMesh(meshShape, transform);
                    //Making the triangles one-sided makes collision detection a bit more robust, since the backsides of triangles won't try to collide with things
                    //and 'pull' them back into the mesh.
                    mesh.Sidedness = TriangleSidedness.Counterclockwise;
                    Space.Add(mesh);
                    game.ModelDrawer.Add(mesh);

                }
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    //Drop a box on the mesh.
                    Space.Add(new Box(new Vector3(((i + 1) * 4).ToFix(), 10.ToFix(), ((j + 1) * 4).ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix(), 10.ToFix()));
                }
            }

            Space.Add(new Box(new Vector3(10.ToFix(), 0.ToFix(), 10.ToFix()), 20.ToFix(), 1.ToFix(), 20.ToFix()));

            game.Camera.Position = new Vector3(10.ToFix(), 6.ToFix(), 30.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Instanced Meshes"; }
        }
    }
}