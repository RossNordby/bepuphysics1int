using BEPUphysics.Constraints.TwoEntity.JointLimits;
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
    public class TwistTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public TwistTestDemo(DemosGame game)
            : base(game)
        {
            var a = new Box(new Vector3((-2).ToFix(), 2.ToFix(), 0.ToFix()), 1.ToFix(), 2.ToFix(), 2.ToFix(), 5.ToFix());
            var b = new Box(new Vector3(2.ToFix(), 2.ToFix(), 0.ToFix()), 1.ToFix(), 2.ToFix(), 2.ToFix(), 5.ToFix());
            b.Orientation = Quaternion.CreateFromAxisAngle(new Vector3(0.ToFix(), 1.ToFix(), 0.ToFix()), MathHelper.PiOver4);
            Space.Add(a);
            Space.Add(b);

            var twistJoint = new TwistJoint(a, b, a.OrientationMatrix.Right, b.OrientationMatrix.Right);
            var twistMotor = new TwistMotor(a, b, a.OrientationMatrix.Right, b.OrientationMatrix.Right);
            twistMotor.Settings.Mode = MotorMode.Servomechanism;

            //Space.Add(twistJoint);
            Space.Add(twistMotor);

            var ballSocketJoint = new BallSocketJoint(a, b, (a.Position + b.Position) * 0.5m.ToFix());
            var swingLimit = new SwingLimit(a, b, a.OrientationMatrix.Right, a.OrientationMatrix.Right, MathHelper.PiOver2);

            Space.Add(ballSocketJoint);
            Space.Add(swingLimit);





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