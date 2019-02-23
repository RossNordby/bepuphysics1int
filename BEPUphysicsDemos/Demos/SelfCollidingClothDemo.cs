using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.NarrowPhaseSystems;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A basic lattice of constraints acting like cloth.
    /// </summary>
    public class SelfCollidingClothDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public SelfCollidingClothDemo(DemosGame game)
            : base(game)
        {
            //Joints can also act like springs by modifying their springSettings.
            //Though using a bunch of DistanceJoint objects can be slower than just applying direct spring forces,
            //it is significantly more stable and allows rigid structures.
            //The extra stability can make it useful for cloth-like simulations.
            Entity latticePiece;
            BallSocketJoint joint;

            NarrowPhaseHelper.Factories.BoxBox.Count = 4000;
            NarrowPhaseHelper.Factories.BoxSphere.Count = 1000;
            
            int numColumns = 40;
            int numRows = 40;
            Fix xSpacing = 1.0m.ToFix();
            Fix zSpacing = 1.0m.ToFix();
            var lattice = new Entity[numRows, numColumns];
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                {
                    latticePiece = new Box(
                        new Vector3(
(xSpacing.Mul(i.ToFix())).Sub((((numRows - 1).ToFix()).Mul(xSpacing)).Div(2.ToFix())),
15.58m.ToFix(),
(2.ToFix().Add(zSpacing.Mul(j.ToFix()))).Sub((((numColumns - 1).ToFix()).Mul(zSpacing)).Div(2.ToFix()))),
                        xSpacing, .2m.ToFix(), zSpacing, 10.ToFix());

                    lattice[i, j] = latticePiece;

                    Space.Add(latticePiece);
                }
            //The joints composing the cloth can have their max iterations set independently from the solver iterations.
            //More iterations (up to the solver's own max) will increase the quality at the cost of speed.
            int clothIterations = 3;
            //So while the above clamps joint iterations, setting the solver's iteration limit can lower the
            //rest of the solving load (collisions).
            Space.Solver.IterationLimit = 10;

            Fix damping = 20000.ToFix(), stiffness = 20000.ToFix();
            Fix starchDamping = 5000.ToFix(), starchStiffness = 500.ToFix();

            //Loop through the grid and set up the joints.
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                {
                    if (i == 0 && j + 1 < numColumns)
                    {
                        //Add in column connections for left edge.
                        joint = new BallSocketJoint(lattice[0, j], lattice[0, j + 1], lattice[0, j].Position + new Vector3(xSpacing.Neg().Div(2.ToFix()), 0.ToFix(), zSpacing.Div(2.ToFix())));
                        joint.SpringSettings.Damping = damping; joint.SpringSettings.Stiffness = stiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);
                    }
                    if (i == numRows - 1 && j + 1 < numColumns)
                    {
                        //Add in column connections for right edge.
                        joint = new BallSocketJoint(lattice[numRows - 1, j], lattice[numRows - 1, j + 1], lattice[numRows - 1, j].Position + new Vector3(xSpacing.Div(2.ToFix()), 0.ToFix(), zSpacing.Div(2.ToFix())));
                        joint.SpringSettings.Damping = damping; joint.SpringSettings.Stiffness = stiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);
                    }
                    if (i + 1 < numRows && j == 0)
                    {
                        //Add in row connections for top edge.
                        joint = new BallSocketJoint(lattice[i, 0], lattice[i + 1, 0], lattice[i, 0].Position + new Vector3(xSpacing.Div(2.ToFix()), 0.ToFix(), zSpacing.Neg().Div(2.ToFix())));
                        joint.SpringSettings.Damping = damping; joint.SpringSettings.Stiffness = stiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);
                    }
                    if (i + 1 < numRows && j == numColumns - 1)
                    {
                        //Add in row connections for bottom edge.
                        joint = new BallSocketJoint(lattice[i, numColumns - 1], lattice[i + 1, numColumns - 1], lattice[i, numColumns - 1].Position + new Vector3(xSpacing.Div(2.ToFix()), 0.ToFix(), zSpacing.Div(2.ToFix())));
                        joint.SpringSettings.Damping = damping; joint.SpringSettings.Stiffness = stiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);

                    }


                    if (i + 1 < numRows && j + 1 < numColumns)
                    {
                        //Add in interior connections.
                        joint = new BallSocketJoint(lattice[i, j], lattice[i + 1, j], lattice[i, j].Position + new Vector3(xSpacing.Div(2.ToFix()), 0.ToFix(), zSpacing.Div(2.ToFix())));
                        joint.SpringSettings.Damping = damping; joint.SpringSettings.Stiffness = stiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);

                        joint = new BallSocketJoint(lattice[i, j], lattice[i, j + 1], lattice[i, j].Position + new Vector3(xSpacing.Div(2.ToFix()), 0.ToFix(), zSpacing.Div(2.ToFix())));
                        joint.SpringSettings.Damping = damping; joint.SpringSettings.Stiffness = stiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);

                        joint = new BallSocketJoint(lattice[i, j], lattice[i + 1, j + 1], lattice[i, j].Position + new Vector3(xSpacing.Div(2.ToFix()), 0.ToFix(), zSpacing.Div(2.ToFix())));
                        joint.SpringSettings.Damping = damping; joint.SpringSettings.Stiffness = stiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);
                    }

                    if (i + 2 < numRows && j + 2 < numColumns)
                    {
                        //Add in skipping 'starch' connections.
                        joint = new BallSocketJoint(lattice[i, j], lattice[i + 2, j], lattice[i, j].Position + new Vector3(xSpacing, 0.ToFix(), zSpacing));
                        joint.SpringSettings.Damping = starchDamping; joint.SpringSettings.Stiffness = starchStiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);

                        joint = new BallSocketJoint(lattice[i, j], lattice[i, j + 2], lattice[i, j].Position + new Vector3(xSpacing, 0.ToFix(), zSpacing));
                        joint.SpringSettings.Damping = starchDamping; joint.SpringSettings.Stiffness = starchStiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);

                        joint = new BallSocketJoint(lattice[i, j], lattice[i + 2, j + 2], lattice[i, j].Position + new Vector3(xSpacing, 0.ToFix(), zSpacing));
                        joint.SpringSettings.Damping = starchDamping; joint.SpringSettings.Stiffness = starchStiffness;
                        joint.SolverSettings.MaximumIterationCount = clothIterations;
                        Space.Add(joint);
                    }

                    //Add in collision rules.
                    if (j - 1 >= 0)
                    {
                        if (i - 1 >= 0) CollisionRules.AddRule(lattice[i, j], lattice[i - 1, j - 1], CollisionRule.NoBroadPhase);
                        CollisionRules.AddRule(lattice[i, j], lattice[i, j - 1], CollisionRule.NoBroadPhase);
                        if (i + 1 < numRows) CollisionRules.AddRule(lattice[i, j], lattice[i + 1, j - 1], CollisionRule.NoBroadPhase);
                    }

                    if (i + 1 < numRows) CollisionRules.AddRule(lattice[i, j], lattice[i + 1, j], CollisionRule.NoBroadPhase);
                }




            //Add some ground.
            var sphere = new Sphere(new Vector3(7.ToFix(), 0.ToFix(), 0.ToFix()), 10.ToFix());
            sphere.Material.KineticFriction = .2m.ToFix();
            Space.Add(sphere);
            Space.Add(new Box(new Vector3(0.ToFix(), (-20.5m).ToFix(), 0.ToFix()), 100.ToFix(), 10.ToFix(), 100.ToFix()));

            game.Camera.Position = new Vector3(0.ToFix(), 5.ToFix(), 25.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Self-Colliding Burlap"; }
        }
    }
}