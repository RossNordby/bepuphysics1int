using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Demo showing a wall of blocks stacked up.
    /// </summary>
    public class WallDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public WallDemo(DemosGame game)
            : base(game)
        {
            int width = 10;
            int height = 10;
			Fix32 blockWidth = 2.ToFix();
			Fix32 blockHeight = 1.ToFix();
			Fix32 blockLength = 1.ToFix();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var toAdd =
                        new Box(
                            new Vector3(
((i.ToFix().Mul(blockWidth)).Add((.5m.ToFix().Mul(blockWidth)).Mul(((j % 2).ToFix())))).Sub((width.ToFix().Mul(blockWidth)).Mul(.5m.ToFix())),
(blockHeight.Mul(.5m.ToFix())).Add(j.ToFix().Mul((blockHeight))),
0.ToFix()),
                            blockWidth, blockHeight, blockLength, 10.ToFix());
                    Space.Add(toAdd);
                }
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
            get { return "Wall"; }
        }
    }
}