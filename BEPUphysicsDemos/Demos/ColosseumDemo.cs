using System;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Ring-shaped structure made of blocks.
    /// </summary>
    public class ColosseumDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public ColosseumDemo(DemosGame game)
            : base(game)
        {
            Fix angle;
            int numBoxesPerRing = 12;
            Fix blockWidth = 2.ToFix();
            Fix blockHeight = 2.ToFix();
            Fix blockLength = 6.ToFix();
            Fix radius = 15.ToFix();
            Entity toAdd;
            Space.Add(new Box(new Vector3(0.ToFix(), (blockHeight.Neg().Div(2.ToFix())).Sub(1.ToFix()), 0.ToFix()), 100.ToFix(), 2.ToFix(), 100.ToFix()));
            Fix increment = MathHelper.TwoPi.Div(numBoxesPerRing.ToFix());
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < numBoxesPerRing; k++)
                {
                    if (i % 2 == 0)
                    {
                        angle = k.ToFix().Mul(increment);
                        toAdd = new Box(new Vector3(Fix32Ext.Cos(angle).Neg().Mul(radius), i.ToFix().Mul(blockHeight), Fix32Ext.Sin(angle).Mul(radius)), blockWidth, blockHeight, blockLength, 20.ToFix());
                        toAdd.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
                        Space.Add(toAdd);
                    }
                    else
                    {
                        angle = ((k + .5m).ToFix()).Mul(increment);
                        toAdd = new Box(new Vector3(Fix32Ext.Cos(angle).Neg().Mul(radius), i.ToFix().Mul(blockHeight), Fix32Ext.Sin(angle).Mul(radius)), blockWidth, blockHeight, blockLength, 20.ToFix());
                        toAdd.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
                        Space.Add(toAdd);
                    }
                }
            }
            game.Camera.Position = new Vector3(0.ToFix(), 2.ToFix(), 2.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Colosseum"; }
        }
    }
}