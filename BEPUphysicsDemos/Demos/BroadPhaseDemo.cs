using System;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A large bunch of cubes suspended in space.
    /// </summary>
    public class BroadPhaseDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public BroadPhaseDemo(DemosGame game)
            : base(game)
        {
            //Make a fatter kapow sphere.
            Space.Remove(kapow);
            kapow = new Sphere(new Vector3(11000.ToFix(), 0.ToFix(), 0.ToFix()), 1.5m.ToFix(), 1000.ToFix());
            Space.Add(kapow);
            Space.Solver.IterationLimit = 1; //Essentially no sustained contacts, so don't need to worry about accuracy.
            Space.ForceUpdater.Gravity = Vector3.Zero;

            int numColumns = 15;
            int numRows = 15;
            int numHigh = 15;
			Fix separation = 3.ToFix();

            Entity toAdd;

            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        toAdd = new Box(new Vector3(separation.Mul(i.ToFix()), k.ToFix().Mul(separation), separation.Mul(j.ToFix())), 1.ToFix(), 1.ToFix(), 1.ToFix(), 1.ToFix());
                        toAdd.Material.Bounciness = 1.ToFix(); //Superbouncy boxes help propagate shock waves.
                        toAdd.LinearDamping = 0.ToFix();
                        toAdd.AngularDamping = 0.ToFix();
                        Space.Add(toAdd);
                    }

            game.Camera.Position = new Vector3(0.ToFix(), 3.ToFix(), (-10).ToFix());
            game.Camera.ViewDirection = new Vector3(0.ToFix(), 0.ToFix(), 1.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Broad phase Stress Test"; }
        }
    }
}