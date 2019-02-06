using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A cube of stacked spheres sits and waits to be knocked over.
    /// </summary>
    public class LotsOSpheresDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public LotsOSpheresDemo(DemosGame game)
            : base(game)
        {
            game.Camera.Position = new Vector3(0.ToFix(), 8.ToFix(), 25.ToFix());
            Space.Add(new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 120.ToFix(), 1.ToFix(), 120.ToFix()));

            int numColumns = 5;
            int numRows = 5;
            int numHigh = 5;
            Fix32 xSpacing = 2.09m.ToFix();
            Fix32 ySpacing = 2.08m.ToFix();
            Fix32 zSpacing = 2.09m.ToFix();
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        Space.Add(new Sphere(new Vector3(
(xSpacing.Mul(i.ToFix())).Sub((((numRows - 1).ToFix()).Mul(xSpacing)).Div(2.ToFix())),
1.58m.ToFix().Add(k.ToFix().Mul((ySpacing))),
(2.ToFix().Add(zSpacing.Mul(j.ToFix()))).Sub((((numColumns - 1).ToFix()).Mul(zSpacing)).Div(2.ToFix()))),
1.ToFix(), 1.ToFix()));
                    }
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Lots o' Spheres"; }
        }
    }
}