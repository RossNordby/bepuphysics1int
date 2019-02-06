using System;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using System.Diagnostics;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo designed to focus on box-box collision detection.
    /// </summary>
    public class BoxBoxTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public BoxBoxTestDemo(DemosGame game)
            : base(game)
        {
            //Fix64 blockWidth = 2;
            //Fix64 blockHeight = 2;
            //Fix64 blockLength = 6f;
            //Entity toAdd;


            //toAdd = new Box(new Vector3(0, 2,0), blockWidth, blockHeight, blockLength, 20);
            //toAdd.ActivityInformation.IsAlwaysActive = true;
            //toAdd.AllowStabilization = false;
            //Space.Add(toAdd);

            int numColumns = 3;
            int numRows = 3;
            int numHigh = 30;
            Fix64 xSpacing = 1.01m.ToFix();
            Fix64 ySpacing = 1.01m.ToFix();
            Fix64 zSpacing = 1.01m.ToFix();
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        Space.Add(new Box(new Vector3(
(xSpacing.Mul(i.ToFix())).Sub((((numRows - 1).ToFix()).Mul(xSpacing)).Div(2.ToFix())),
1.ToFix().Add(k.ToFix().Mul((ySpacing))),
(zSpacing.Mul(j.ToFix())).Sub((((numColumns - 1).ToFix()).Mul(zSpacing)).Div(2.ToFix()))),
.5m.ToFix(), .5m.ToFix(), .5m.ToFix(), 5.ToFix()));
                    }

            Space.Add(new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 20.ToFix(), 1.ToFix(), 20.ToFix()));
            game.Camera.Position = new Vector3(0.ToFix(), 3.ToFix(), 10.ToFix());
        }

        public override void Update(Fix64 dt)
        {
            if (Game.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                Debug.WriteLine("break.");
            base.Update(dt);
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Box-box test"; }
        }
    }
}