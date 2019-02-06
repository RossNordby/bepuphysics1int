
using System;
using System.Collections.Generic;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using ConversionHelper;
using FixMath.NET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Matrix = BEPUutilities.Matrix;
using Ray = BEPUutilities.Ray;
using Vector3 = BEPUutilities.Vector3;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Casts a ray against an object and visualizes the result.
    /// </summary>
    public class RayCastTestDemo : StandardDemo
    {
        private Vector3 origin;
        private Vector3 direction;
        private RayCastResult result;
        private bool hitAnything;

        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public RayCastTestDemo(DemosGame game)
            : base(game)
        {
            Space.Add(new Box(new Vector3(0.ToFix(), (-0.5m).ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix()));

            //Put whatever you'd like to ray cast here.
            var capsule = new Capsule(new Vector3(0.ToFix(), 1.2m.ToFix(), 0.ToFix()), 1.ToFix(), 0.6m.ToFix());
            capsule.AngularVelocity = new Vector3(1.ToFix(), 1.ToFix(), 1.ToFix());
            Space.Add(capsule);

            var cylinder = new Cylinder(new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 2.ToFix(), .5m.ToFix());
            cylinder.AngularVelocity = new Vector3(1.ToFix(), (-1).ToFix(), 1.ToFix());
            Space.Add(cylinder);

            var points = new List<Vector3>();

            var random = new Random(0);
            for (int k = 0; k < 40; k++)
            {
                points.Add(new Vector3(1.ToFix().Mul(random.NextDouble().ToFix()), 3.ToFix().Mul(random.NextDouble().ToFix()), 2.ToFix().Mul(random.NextDouble().ToFix())));
            }
            var convexHull = new ConvexHull(new Vector3(0.ToFix(), 10.ToFix(), 0.ToFix()), points);
            convexHull.AngularVelocity = new Vector3((-1).ToFix(), 1.ToFix(), 1.ToFix());
            Space.Add(convexHull);


            game.Camera.Position = new Vector3((-10).ToFix(), 5.ToFix(), 10.ToFix());
            game.Camera.Yaw(MathHelper.Pi.ToFix().Div((-4).ToFix()));
            game.Camera.Pitch(((-MathHelper.Pi).ToFix()).Div(9.ToFix()));

            //Starter ray.
            origin = new Vector3(10.ToFix(), 5.ToFix(), 0.ToFix());
            direction = new Vector3((-3).ToFix(), (-1).ToFix(), 0.ToFix());

        }



        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Ray Cast Test"; }
        }

        public override void Update(Fix64 dt)
        {
            base.Update(dt);

#if WINDOWS
            if (Game.MouseInput.RightButton == ButtonState.Pressed)
            {
                origin = Game.Camera.Position;
                direction = Game.Camera.WorldMatrix.Forward;
            }
#endif
            hitAnything = Space.RayCast(new Ray(origin, direction * 3.ToFix()), 10000.ToFix(), out result);

        }

        public override void Draw()
        {

            base.Draw();

            if (!hitAnything)
            {
                //If we didn't hit anything, just point out into something approximating infinity.
                result.HitData.Location = origin + direction * 10000.ToFix();
            }
            Game.LineDrawer.LightingEnabled = false;
            Game.LineDrawer.VertexColorEnabled = true;
            Game.LineDrawer.World = Microsoft.Xna.Framework.Matrix.Identity;
            Game.LineDrawer.View = MathConverter.Convert(Game.Camera.ViewMatrix);
            Game.LineDrawer.Projection = MathConverter.Convert(Game.Camera.ProjectionMatrix);
            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var pass in Game.LineDrawer.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                                       new[]
                                                               {
                                                                   new VertexPositionColor(MathConverter.Convert(origin), Color.Blue),
                                                                   new VertexPositionColor(MathConverter.Convert(result.HitData.Location), Color.Blue),
                                                                   new VertexPositionColor(MathConverter.Convert(result.HitData.Location), Color.Blue),
                                                                   new VertexPositionColor(MathConverter.Convert(result.HitData.Normal + result.HitData.Location), Color.Blue)
                                                               },
                                                       0, 2);
            }

        }



    }
}
