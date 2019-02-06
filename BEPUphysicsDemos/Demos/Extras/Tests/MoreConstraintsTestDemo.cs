using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo showing a wall of blocks stacked up.
    /// </summary>
    public class MoreConstraintsTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public MoreConstraintsTestDemo(DemosGame game)
            : base(game)
        {

            Box boxA = new Box(new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 1.ToFix(), 2.ToFix(), 1.ToFix(), 10.ToFix());
            Box boxB = new Box(new Vector3(0.ToFix(), 8.ToFix(), 0.ToFix()), 1.ToFix(), 2.ToFix(), 1.ToFix(), 10.ToFix());
            boxA.Orientation = Quaternion.CreateFromYawPitchRoll(0.ToFix(), MathHelper.PiOver4, 0.ToFix());
            boxB.Orientation = Quaternion.CreateFromYawPitchRoll(MathHelper.PiOver4, 0.ToFix(), 0.ToFix());

            WeldJoint weld = new WeldJoint(boxA, boxB);

            Space.Add(boxA);
            Space.Add(boxB);
            Space.Add(weld);


            boxA = new Box(new Vector3(3.ToFix(), 5.ToFix(), 0.ToFix()), 1.ToFix(), 2.ToFix(), 1.ToFix(), 10.ToFix());
            boxB = new Box(new Vector3(3.ToFix(), 8.ToFix(), 0.ToFix()), 1.ToFix(), 2.ToFix(), 1.ToFix(), 10.ToFix());
            boxA.Orientation = Quaternion.CreateFromYawPitchRoll(0.ToFix(), MathHelper.PiOver4, 0.ToFix());
            boxB.Orientation = Quaternion.CreateFromYawPitchRoll(MathHelper.PiOver4, 0.ToFix(), 0.ToFix());

            BallSocketJoint ballSocket = new BallSocketJoint(boxA, boxB, (boxA.Position + boxB.Position) / 2.ToFix());
            AngularMotor angularMotor = new AngularMotor(boxA, boxB);
            angularMotor.Settings.Mode = MotorMode.Servomechanism;


            Space.Add(boxA);
            Space.Add(boxB);
            Space.Add(ballSocket);
            Space.Add(angularMotor);


            Box ground = new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 10.ToFix(), 1.ToFix(), 10.ToFix());

            Space.Add(ground);

            game.Camera.Position = new Vector3(0.ToFix(), 6.ToFix(), 15.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "More Constraint Testin'"; }
        }
    }
}