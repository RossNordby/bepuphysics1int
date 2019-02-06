
using System;
using BEPUphysics.Constraints;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUphysics.Settings;
using BEPUutilities;
using FixMath.NET;
using Microsoft.Xna.Framework.Input;

namespace BEPUphysicsDemos.Demos.Extras
{
    /// <summary>
    /// Bunch of blocks arranged in a 3d pyramid, waiting to be blown up.
    /// </summary>
    public class SolidPyramidDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public SolidPyramidDemo(DemosGame game)
            : base(game)
        {
            Fix64 boxSize = 1.ToFix();
            int bottomBoxCount = 10;

            var ground = new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), 40.ToFix(), 1.ToFix(), 40.ToFix());
            Space.Add(ground);

            Fix64 spacing = 0.05m.ToFix();

            Fix64 offset = (-0.5m).ToFix().Mul((((bottomBoxCount - 1).ToFix()).Mul((boxSize.Add(spacing)))));
            var origin = new Vector3(offset, boxSize.Neg().Mul(0.5m.ToFix()), offset);
            for (int heightIndex = 0; heightIndex < bottomBoxCount - 2; ++heightIndex)
            {
                var levelWidth = bottomBoxCount - heightIndex;
                Fix64 perBoxWidth = boxSize.Add(spacing);
				//Move the origin for this level.
				origin.X =
origin.X.Add(perBoxWidth.Mul(0.5m.ToFix()));
				origin.Y = origin.Y.Add(boxSize);
				origin.Z = origin.Z.Add(perBoxWidth.Mul(0.5m.ToFix()));

                for (int i = 0; i < levelWidth; ++i)
                {
                    for (int j = 0; j < levelWidth; ++j)
                    {
                        var position = new Vector3(
origin.X.Add(i.ToFix().Mul(perBoxWidth)),
                            origin.Y,
origin.Z.Add(j.ToFix().Mul(perBoxWidth)));

                        var box = new Box(position, boxSize, boxSize, boxSize, 20.ToFix());

                        Space.Add(box);
                    }
                }
            }

            game.Camera.Position = new Vector3((-bottomBoxCount).ToFix().Mul(boxSize), 2.ToFix(), bottomBoxCount.ToFix().Mul(boxSize));
            game.Camera.Yaw(MathHelper.Pi.Div((-4).ToFix()));
            game.Camera.Pitch(MathHelper.Pi.Div(9.ToFix()));
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Solid Pyramid"; }
        }
    }
}