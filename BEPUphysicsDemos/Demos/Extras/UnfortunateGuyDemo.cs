using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.JointLimits;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using System.Collections.Generic;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras
{
    /// <summary>
    /// A one-armed guy flops on the ground.  Involves a detailed ragdoll-like limb with appropriate limits.
    /// </summary>
    public class UnfortunateGuyDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public UnfortunateGuyDemo(DemosGame game)
            : base(game)
        {
            Entity ground = new Box(Vector3.Zero, 10.ToFix(), 1.ToFix(), 10.ToFix());
            Space.Add(ground);

            //Rather than add basically redundant code for every limb, this demo
            //focuses on a single arm, showing some extra details and limits.

            //Make the torso.
            var bodies = new List<CompoundShapeEntry>()
            {
                new CompoundShapeEntry(new BoxShape(2.ToFix(), 1.5m.ToFix(), 1.ToFix()), new Vector3((-1).ToFix(), 3.ToFix(), 0.ToFix()), 50.ToFix()),
                new CompoundShapeEntry(new SphereShape(.45m.ToFix()), new Vector3(.4m.ToFix(), 3.ToFix(), 0.ToFix()), 1.ToFix()),
                new CompoundShapeEntry(new SphereShape(.25m.ToFix()), new Vector3((-1.9m).ToFix(), 3.5m.ToFix(), 0.ToFix()), 1.ToFix()),
                new CompoundShapeEntry(new SphereShape(.25m.ToFix()), new Vector3((-1.9m).ToFix(), 2.5m.ToFix(), 0.ToFix()), 1.ToFix()),
                new CompoundShapeEntry(new SphereShape(.25m.ToFix()), new Vector3((-.3m).ToFix(), 2.3m.ToFix(), 0.ToFix()), 1.ToFix())
            };

            var torso = new CompoundBody(bodies, 54.ToFix());
            Space.Add(torso);

            //Make the upper arm.
            Entity upperArm = new Box(torso.Position + new Vector3(1.ToFix(), 1.4m.ToFix(), 0.ToFix()), .4m.ToFix(), 1.2m.ToFix(), .4m.ToFix(), 8.ToFix());
            Space.Add(upperArm);


            //A ball socket joint will handle the linear degrees of freedom between the two entities.
            var ballSocketJoint = new BallSocketJoint(torso, upperArm, torso.Position + new Vector3(1.ToFix(), .7m.ToFix(), 0.ToFix()));
            Space.Add(ballSocketJoint);

            //Shoulders don't have a simple limit.  The EllipseSwingLimit allows angles within an ellipse, which is closer to how some joints function
            //than just flat planes (like two RevoluteLimits) or a single angle (like SwingLimits).
            var swingLimit = new EllipseSwingLimit(torso, upperArm, Vector3.Up, MathHelper.PiOver2, MathHelper.PiOver4.Mul(3.ToFix()));
            Space.Add(swingLimit);

            //Upper arms can't spin around forever.
            var twistLimit = new TwistLimit(torso, upperArm, Vector3.Up, Vector3.Up, MathHelper.PiOver4.Neg().Div(2.ToFix()), MathHelper.PiOver4);
            twistLimit.SpringSettings.Stiffness = 100.ToFix();
            twistLimit.SpringSettings.Damping = 100.ToFix();
            Space.Add(twistLimit);


            //Make the lower arm.
            Entity lowerArm = new Box(upperArm.Position + new Vector3(0.ToFix(), 1.4m.ToFix(), 0.ToFix()), .35m.ToFix(), 1.3m.ToFix(), .35m.ToFix(), 8.ToFix());
            Space.Add(lowerArm);


            var elbow = new SwivelHingeJoint(upperArm, lowerArm, upperArm.Position + new Vector3(0.ToFix(), .6m.ToFix(), 0.ToFix()), Vector3.Forward);
            //Forearm can twist a little.
            elbow.TwistLimit.IsActive = true;
            elbow.TwistLimit.MinimumAngle = (MathHelper.PiOver4.Neg().Div(2.ToFix()));
            elbow.TwistLimit.MaximumAngle = (MathHelper.PiOver4.Div(2.ToFix()));
            elbow.TwistLimit.SpringSettings.Damping = 100.ToFix();
            elbow.TwistLimit.SpringSettings.Stiffness = 100.ToFix();


            //The elbow is like a hinge, but it can't hyperflex.
            elbow.HingeLimit.IsActive = true;
            elbow.HingeLimit.MinimumAngle = 0.ToFix();
            elbow.HingeLimit.MaximumAngle = (MathHelper.Pi.Mul(.7m.ToFix()));
            Space.Add(elbow);

            Entity hand = new Box(lowerArm.Position + new Vector3(0.ToFix(), .9m.ToFix(), 0.ToFix()), .4m.ToFix(), .55m.ToFix(), .25m.ToFix(), 3.ToFix());
            Space.Add(hand);

            ballSocketJoint = new BallSocketJoint(lowerArm, hand, lowerArm.Position + new Vector3(0.ToFix(), .7m.ToFix(), 0.ToFix()));
            Space.Add(ballSocketJoint);

            //Wrists can use an ellipse limit too.
            swingLimit = new EllipseSwingLimit(lowerArm, hand, Vector3.Up, MathHelper.PiOver4, MathHelper.PiOver2);
            Space.Add(swingLimit);

            //Allow a little extra twist beyond the forearm.
            twistLimit = new TwistLimit(lowerArm, hand, Vector3.Up, Vector3.Up, MathHelper.PiOver4.Neg().Div(2.ToFix()), MathHelper.PiOver4.Div(2.ToFix()));
            twistLimit.SpringSettings.Stiffness = 100.ToFix();
            twistLimit.SpringSettings.Damping = 100.ToFix();
            Space.Add(twistLimit);

            //The hand is pretty floppy without some damping.
            var angularMotor = new AngularMotor(lowerArm, hand);
            angularMotor.Settings.VelocityMotor.Softness = .5m.ToFix();
            Space.Add(angularMotor);

            //Make sure the parts of the arm don't collide.
            CollisionRules.AddRule(torso, upperArm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(lowerArm, upperArm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(lowerArm, hand, CollisionRule.NoBroadPhase);

            game.Camera.Position = new Vector3(0.ToFix(), 4.ToFix(), 20.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Unfortunate One-armed Guy"; }
        }
    }
}