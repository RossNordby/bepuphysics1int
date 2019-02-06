using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.PositionUpdating;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A set of jenga blocks.
    /// </summary>
    public class JengaDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public JengaDemo(DemosGame game)
            : base(game)
        {

            Space.Remove(kapow);
            //Have to shrink the ball a little to make it fit between jenga tower blocks.
            kapow = new Sphere(new Vector3((-11000).ToFix(), 0.ToFix(), 0.ToFix()), .2m.ToFix(), 20.ToFix());
            kapow.PositionUpdateMode = PositionUpdateMode.Continuous; //The ball's really tiny! It will work better if it's handled continuously.
            Space.Add(kapow);
            int numBlocksTall = 18; //How many 'stories' tall.
            Fix32 blockWidth = 3.ToFix(); //Total width/length of the tower.
            Fix32 blockHeight = 1.ToFix().Div(2.ToFix());
            Entity toAdd;


            for (int i = 0; i < numBlocksTall; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        toAdd = new Box(new Vector3(
(j.ToFix().Mul((blockWidth.Div(3.ToFix())))).Sub(blockWidth.Div(3.ToFix())),
(blockHeight.Div(2.ToFix())).Add(i.ToFix().Mul((blockHeight))),
0.ToFix()),
blockWidth.Div(3.ToFix()), blockHeight, blockWidth, 10.ToFix());
                        Space.Add(toAdd);
                    }
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        toAdd = new Box(new Vector3(
0.ToFix(),
(blockHeight.Div(2.ToFix())).Add((i.ToFix()).Mul((blockHeight))),
(j.ToFix().Mul((blockWidth.Div(3.ToFix())))).Sub(blockWidth.Div(3.ToFix()))),
                                        blockWidth, blockHeight, blockWidth.Div(3.ToFix()), 10.ToFix());
                        Space.Add(toAdd);

                    }
                }
            }
            Space.Add(new Box(new Vector3(0.ToFix(), (-.5m).ToFix(), 0.ToFix()), 40.ToFix(), 1.ToFix(), 40.ToFix()));
            game.Camera.Position = new Vector3(0.ToFix(), 5.ToFix(), 15.ToFix());

        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Jenga"; }
        }
    }
}