using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Materials;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Earthquake simulator with a jenga stack test subject.
    /// </summary>
    public class EarthquakeDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public EarthquakeDemo(DemosGame game)
            : base(game)
        {
            Entity ground = new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix());
            Space.Add(ground);
            //Springs: Create a lattice of springs to hold the boxes together.
            var platform = new Box(new Vector3(0.ToFix(), 4.ToFix(), 0.ToFix()), 18.ToFix(), 1.ToFix(), 18.ToFix(), 400.ToFix());
            platform.Material.KineticFriction = .8m.ToFix();

            Space.Add(platform);

            //Create a big mess of DistanceJoints representing springs.

            var distanceJoint = new DistanceJoint(ground, platform, new Vector3(9.ToFix(), .5m.ToFix(), (-9).ToFix()), new Vector3(9.ToFix(), 3.5m.ToFix(), (-9).ToFix()));
            distanceJoint.SpringSettings.Stiffness = 3000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3(9.ToFix(), .5m.ToFix(), (-9).ToFix()), new Vector3((-9).ToFix(), 3.5m.ToFix(), (-9).ToFix()));
            distanceJoint.SpringSettings.Stiffness = 3000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3((-9).ToFix(), .5m.ToFix(), (-9).ToFix()), new Vector3(9.ToFix(), 3.5m.ToFix(), (-9).ToFix()));
            distanceJoint.SpringSettings.Stiffness = 3000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3((-9).ToFix(), .5m.ToFix(), (-9).ToFix()), new Vector3((-9).ToFix(), 3.5m.ToFix(), (-9).ToFix()));
            distanceJoint.SpringSettings.Stiffness = 3000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);

            distanceJoint = new DistanceJoint(ground, platform, new Vector3(9.ToFix(), .5m.ToFix(), 9.ToFix()), new Vector3(9.ToFix(), 3.5m.ToFix(), 9.ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3(9.ToFix(), .5m.ToFix(), 9.ToFix()), new Vector3((-9).ToFix(), 3.5m.ToFix(), 9.ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3((-9).ToFix(), .5m.ToFix(), 9.ToFix()), new Vector3(9.ToFix(), 3.5m.ToFix(), 9.ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3((-9).ToFix(), .5m.ToFix(), 9.ToFix()), new Vector3((-9).ToFix(), 3.5m.ToFix(), 9.ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);

            distanceJoint = new DistanceJoint(ground, platform, new Vector3(9.ToFix(), .5m.ToFix(), (-9).ToFix()), new Vector3(9.ToFix(), 3.5m.ToFix(), (-9).ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3(9.ToFix(), .5m.ToFix(), (-9).ToFix()), new Vector3(9.ToFix(), 3.5m.ToFix(), 9.ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3(9.ToFix(), .5m.ToFix(), 9.ToFix()), new Vector3(9.ToFix(), 3.5m.ToFix(), (-9).ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3(9.ToFix(), .5m.ToFix(), 9.ToFix()), new Vector3(9.ToFix(), 3.5m.ToFix(), 9.ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);

            distanceJoint = new DistanceJoint(ground, platform, new Vector3((-9).ToFix(), .5m.ToFix(), (-9).ToFix()), new Vector3((-9).ToFix(), 3.5m.ToFix(), (-9).ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3((-9).ToFix(), .5m.ToFix(), (-9).ToFix()), new Vector3((-9).ToFix(), 3.5m.ToFix(), 9.ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3((-9).ToFix(), .5m.ToFix(), 9.ToFix()), new Vector3((-9).ToFix(), 3.5m.ToFix(), (-9).ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);
            distanceJoint = new DistanceJoint(ground, platform, new Vector3((-9).ToFix(), .5m.ToFix(), 9.ToFix()), new Vector3((-9).ToFix(), 3.5m.ToFix(), 9.ToFix()));
            distanceJoint.SpringSettings.Stiffness = 6000.ToFix();
            distanceJoint.SpringSettings.Damping = 0.ToFix();
            Space.Add(distanceJoint);

            int numBlocksTall = 10; //How many 'stories' tall.
            Fix blockWidth = 4.ToFix(); //Total width/length of the tower.
            Fix blockHeight = 1.333m.ToFix();
            Entity toAdd;
            for (int i = 0; i < numBlocksTall; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        toAdd =
                            new Box(
                                new Vector3(
((j.ToFix().Mul(blockWidth)).Div(3.ToFix())).Sub(blockWidth.Div(3.ToFix())),
(5.ToFix().Add(blockHeight.Div(2.ToFix()))).Add(i.ToFix().Mul(blockHeight)),
                                    0.ToFix()),
blockWidth.Div(3.ToFix()), blockHeight, blockWidth, (3 * numBlocksTall + 1 - 2 * i).ToFix());
                        Space.Add(toAdd);
                        toAdd.Material = new Material(.8m.ToFix(), .8m.ToFix(), 0.ToFix());
                    }
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        toAdd =
                            new Box(
                                new Vector3(0.ToFix(),
(5.ToFix().Add(blockHeight.Div(2.ToFix()))).Add(i.ToFix().Mul(blockHeight)),
((j.ToFix().Mul(blockWidth)).Div(3.ToFix())).Sub(blockWidth.Div(3.ToFix()))),
                                blockWidth, blockHeight, blockWidth.Div(3.ToFix()), (3 * numBlocksTall + 1 - 2 * i).ToFix());
                        Space.Add(toAdd);
                        toAdd.Material = new Material(.8m.ToFix(), .8m.ToFix(), 0.ToFix());
                    }
                }
            }

            game.Camera.Position = new Vector3(0.ToFix(), 7.ToFix(), 30.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Earthquake!"; }
        }
    }
}