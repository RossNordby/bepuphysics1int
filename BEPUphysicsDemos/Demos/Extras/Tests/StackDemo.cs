using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo showing a wall of blocks stacked up.
    /// </summary>
    public class StackDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public StackDemo(DemosGame game)
            : base(game)
        {
            kapow.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
            int height = 50;
            Fix64 blockWidth = 3.ToFix();
            Fix64 blockHeight = 1.ToFix();
            Fix64 blockLength = 3.ToFix();

            for (int i = 0; i < height; i++)
            {
                    var toAdd =
                        new Box(
                            new Vector3(
0.ToFix(),
(blockHeight.Mul(.5m.ToFix())).Add(i.ToFix().Mul((blockHeight))),
0.ToFix()),
                            blockWidth, blockHeight, blockLength, 10.ToFix());
                    Space.Add(toAdd);
            }

            Box ground = new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix());
            Space.Add(ground);

            game.Camera.Position = new Vector3(0.ToFix(), 6.ToFix(), 15.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Stack"; }
        }
    }
}