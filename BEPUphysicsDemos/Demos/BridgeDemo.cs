using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Constraints.SolverGroups;
using BEPUutilities;
using ConversionHelper;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A long string of blocks connected by joints.
    /// </summary>
    public class BridgeDemo : StandardDemo
    {


        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public BridgeDemo(DemosGame game)
            : base(game)
        {
            //Form a long chain of planks connected by revolute joints.
            //The revolute joints control the three linear degrees of freedom and two angular degrees of freedom.
            //The third allowed angular degree of freedom allows the bridge to flex like a rope bridge.
            Vector3 startPosition = new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix());
            var startPlatform = new Box(startPosition - new Vector3(0.ToFix(), 0.ToFix(), 3.2m.ToFix()), 8.ToFix(), .5m.ToFix(), 8.ToFix());
            Space.Add(startPlatform);
            Vector3 offset = new Vector3(0.ToFix(), 0.ToFix(), 1.7m.ToFix());
            Box previousLink = startPlatform;
            Vector3 position = new Vector3();
            for (int i = 1; i <= 200; i++)
            {
                position = startPosition + offset * i.ToFix();
                Box link = new Box(position, 4.5m.ToFix(), .3m.ToFix(), 1.5m.ToFix(), 50.ToFix());
                Space.Add(link);
                Space.Add(new RevoluteJoint(previousLink, link, position - offset * .5m.ToFix(), Vector3.Right));

                previousLink = link;
            }
            var endPlatform = new Box(position - new Vector3(0.ToFix(), 0.ToFix(), (-4.8m).ToFix()), 8.ToFix(), .5m.ToFix(), 8.ToFix());
            Space.Add(endPlatform);

            Space.Add(new RevoluteJoint(previousLink, endPlatform, position + offset * .5m.ToFix(), Vector3.Right));


            game.Camera.Position = startPosition + new Vector3(0.ToFix(), 1.ToFix(), (offset.Z.Mul(200.ToFix())).Add(5.ToFix()));
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Bridge"; }
        }
    }
}