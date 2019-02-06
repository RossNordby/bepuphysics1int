using System;
using System.Collections.Generic;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.UpdateableSystems;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Blocks floating in a viscous fluid.
    /// </summary>
    public class BuoyancyDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public BuoyancyDemo(DemosGame game)
            : base(game)
        {
            var tris = new List<Vector3[]>();
            Fix32 basinWidth = 100.ToFix();
            Fix32 basinLength = 100.ToFix();
            Fix32 basinHeight = 16.ToFix();
            Fix32 waterHeight = 15.ToFix();

            //Remember, the triangles composing the surface need to be coplanar with the surface.  In this case, this means they have the same height.
            tris.Add(new[]
                         {
                             new Vector3(basinWidth .Neg().Div(2.ToFix()), waterHeight, basinLength .Neg().Div(2.ToFix())), new Vector3(basinWidth.Div(2.ToFix()), waterHeight, basinLength .Neg().Div(2.ToFix())),
                             new Vector3(basinWidth .Neg().Div(2.ToFix()), waterHeight, basinLength.Div(2.ToFix()))
                         });
            tris.Add(new[]
                         {
                             new Vector3(basinWidth .Neg().Div(2.ToFix()), waterHeight, basinLength.Div(2.ToFix())), new Vector3(basinWidth.Div(2.ToFix()), waterHeight, basinLength .Neg().Div(2.ToFix())),
                             new Vector3(basinWidth.Div(2.ToFix()), waterHeight, basinLength.Div(2.ToFix()))
                         });
            var fluid = new FluidVolume(Vector3.Up, (-9.81m).ToFix(), tris, waterHeight, .8m.ToFix(), .8m.ToFix(), .7m.ToFix());
            Space.ForceUpdater.Gravity = new Vector3(0.ToFix(), (-9.81m).ToFix(), 0.ToFix());


            //fluid.FlowDirection = Vector3.Right;
            //fluid.FlowForce = 80;
            //fluid.MaxFlowSpeed = 50;
            Space.Add(fluid);
            game.ModelDrawer.Add(fluid);
            //Create the container.
            Space.Add(new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), basinWidth, 1.ToFix(), basinLength));
            Space.Add(new Box(new Vector3((basinWidth.Neg().Div(2.ToFix())).Sub(.5m.ToFix()), (basinHeight.Div(2.ToFix())).Sub(.5m.ToFix()), 0.ToFix()), 1.ToFix(), basinHeight, basinLength));
            Space.Add(new Box(new Vector3((basinWidth.Div(2.ToFix())).Add(.5m.ToFix()), (basinHeight.Div(2.ToFix())).Sub(.5m.ToFix()), 0.ToFix()), 1.ToFix(), basinHeight, basinLength));
            Space.Add(new Box(new Vector3(0.ToFix(), (basinHeight.Div(2.ToFix())).Sub(.5m.ToFix()), (basinLength.Neg().Div(2.ToFix())).Sub(.5m.ToFix())), basinWidth.Add(2.ToFix()), basinHeight, 1.ToFix()));
            Space.Add(new Box(new Vector3(0.ToFix(), (basinHeight.Div(2.ToFix())).Sub(.5m.ToFix()), (basinLength.Div(2.ToFix())).Add(.5m.ToFix())), basinWidth.Add(2.ToFix()), basinHeight, 1.ToFix()));


            //Create a tiled floor.
            Entity toAdd;
            Fix32 boxWidth = 10.ToFix();
            int numBoxesWide = 8;
            for (int i = 0; i < numBoxesWide; i++)
            {
                for (int k = 0; k < numBoxesWide; k++)
                {
                    toAdd = new Box(new Vector3(
((boxWidth.Neg().Mul(numBoxesWide.ToFix())).Div(2.ToFix())).Add((boxWidth.Add(.1m.ToFix())).Mul(i.ToFix())),
15.ToFix(),
((boxWidth.Neg().Mul(numBoxesWide.ToFix())).Div(2.ToFix())).Add((boxWidth.Add(.1m.ToFix())).Mul(k.ToFix()))),
                        boxWidth, 5.ToFix(), boxWidth, 300.ToFix());

                    Space.Add(toAdd);
                }
            }


            //Create a bunch o' spheres and dump them into the water.
            //Random random = new Random();
            //for (int k = 0; k < 80; k++)
            //{
            //    var toAddSphere = new Sphere(new Vector3(-48 + k * 1f, 12 + 4 * k, random.Next(-15, 15)), 2, 27);
            //    Space.Add(toAddSphere);
            //}


            game.Camera.Position = new Vector3(0.ToFix(), waterHeight.Add(5.ToFix()), 35.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Buoyancy"; }
        }
    }
}