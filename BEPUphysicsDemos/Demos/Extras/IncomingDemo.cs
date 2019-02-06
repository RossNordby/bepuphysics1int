using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras
{
    /// <summary>
    /// Demo showing a tower of blocks being smashed by a sphere.
    /// </summary>
    public class IncomingDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public IncomingDemo(DemosGame game)
            : base(game)
        {
            Entity toAdd;
            //Build the stack...
            for (int k = 1; k <= 12; k++)
            {
                if (k % 2 == 1)
                {
                    toAdd = new Box(new Vector3((-3).ToFix(), k.ToFix(), 0.ToFix()), 1.ToFix(), 1.ToFix(), 7.ToFix(), 10.ToFix());
                    Space.Add(toAdd);
                    toAdd = new Box(new Vector3(3.ToFix(), k.ToFix(), 0.ToFix()), 1.ToFix(), 1.ToFix(), 7.ToFix(), 10.ToFix());
                    Space.Add(toAdd);
                }
                else
                {
                    toAdd = new Box(new Vector3(0.ToFix(), k.ToFix(), (-3).ToFix()), 7.ToFix(), 1.ToFix(), 1.ToFix(), 10.ToFix());
                    Space.Add(toAdd);
                    toAdd = new Box(new Vector3(0.ToFix(), k.ToFix(), 3.ToFix()), 7.ToFix(), 1.ToFix(), 1.ToFix(), 10.ToFix());
                    Space.Add(toAdd);
                }
            }
            //And then smash it!
            toAdd = new Sphere(new Vector3(0.ToFix(), 150.ToFix(), 0.ToFix()), 3.ToFix(), 100.ToFix());

            Space.Add(toAdd);
            Space.Add(new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 10.ToFix(), 1m.ToFix(), 10.ToFix()));
            game.Camera.Position = new Vector3(0.ToFix(), 6.ToFix(), 30.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Incoming!"; }
        }
    }
}