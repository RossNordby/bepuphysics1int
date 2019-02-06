using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Constraints.TwoEntity.JointLimits;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    class JointLimitTestDemo : StandardDemo
    {
        public JointLimitTestDemo(DemosGame game)
            : base(game)
        {
            Fix32 bounciness = 1.ToFix();
            Fix32 baseMass = 100.ToFix();
            Fix32 armMass = 10.ToFix();
            //DistanceLimit
            Box boxA = new Box(new Vector3((-21).ToFix(), 4.ToFix(), 0.ToFix()), 3.ToFix(), 3.ToFix(), 3.ToFix(), baseMass);
            Box boxB = new Box(boxA.Position + new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 1.ToFix(), 4.ToFix(), 1.ToFix(), armMass);
            CollisionRules.AddRule(boxA, boxB, CollisionRule.NoBroadPhase);
            boxB.ActivityInformation.IsAlwaysActive = true;

            var distanceLimit = new DistanceLimit(boxA, boxB, boxA.Position, boxB.Position - new Vector3(0.ToFix(), 2.ToFix(), 0.ToFix()), 1.ToFix(), 6.ToFix());
            distanceLimit.Bounciness = bounciness;

            Space.Add(boxA);
            Space.Add(boxB);
            Space.Add(distanceLimit);

            //EllipseSwingLimit
            boxA = new Box(new Vector3((-14).ToFix(), 4.ToFix(), 0.ToFix()), 3.ToFix(), 3.ToFix(), 3.ToFix(), baseMass);
            boxB = new Box(boxA.Position + new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 1.ToFix(), 4.ToFix(), 1.ToFix(), armMass);
            CollisionRules.AddRule(boxA, boxB, CollisionRule.NoBroadPhase);
            boxB.ActivityInformation.IsAlwaysActive = true;

            var ballSocketJoint = new BallSocketJoint(boxA, boxB, boxB.Position + new Vector3(0.ToFix(), (-2).ToFix(), 0.ToFix()));
            var ellipseSwingLimit = new EllipseSwingLimit(boxA, boxB, Vector3.Up, MathHelper.Pi.Div(1.5m.ToFix()), MathHelper.Pi.Div(3.ToFix()));
            ellipseSwingLimit.Bounciness = bounciness;

            Space.Add(boxA);
            Space.Add(boxB);
            Space.Add(ballSocketJoint);
            Space.Add(ellipseSwingLimit);

            //LinearAxisLimit
            boxA = new Box(new Vector3((-7).ToFix(), 4.ToFix(), 0.ToFix()), 3.ToFix(), 3.ToFix(), 3.ToFix(), baseMass);
            boxB = new Box(boxA.Position + new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 1.ToFix(), 4.ToFix(), 1.ToFix(), armMass);
            CollisionRules.AddRule(boxA, boxB, CollisionRule.NoBroadPhase);
            boxB.ActivityInformation.IsAlwaysActive = true;

            var pointOnLineJoint = new PointOnLineJoint(boxA, boxB, boxA.Position, Vector3.Up, boxB.Position + new Vector3(0.ToFix(), (-2).ToFix(), 0.ToFix()));
            var linearAxisLimit = new LinearAxisLimit(boxA, boxB, boxA.Position, boxB.Position + new Vector3(0.ToFix(), (-2).ToFix(), 0.ToFix()), Vector3.Up, 0.ToFix(), 4.ToFix());
            linearAxisLimit.Bounciness = bounciness;

            Space.Add(boxA);
            Space.Add(boxB);
            Space.Add(pointOnLineJoint);
            Space.Add(linearAxisLimit);

            //RevoluteLimit
            boxA = new Box(new Vector3(0.ToFix(), 4.ToFix(), 0.ToFix()), 3.ToFix(), 3.ToFix(), 3.ToFix(), baseMass);
            boxB = new Box(boxA.Position + new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 1.ToFix(), 4.ToFix(), 1.ToFix(), armMass);
            CollisionRules.AddRule(boxA, boxB, CollisionRule.NoBroadPhase);
            boxB.ActivityInformation.IsAlwaysActive = true;

            ballSocketJoint = new BallSocketJoint(boxA, boxB, boxB.Position + new Vector3(0.ToFix(), (-2).ToFix(), 0.ToFix()));
            var revoluteAngularJoint = new RevoluteAngularJoint(boxA, boxB, Vector3.Forward);
            var revoluteLimit = new RevoluteLimit(boxA, boxB, Vector3.Forward, Vector3.Up, MathHelper.PiOver4.Neg(), MathHelper.PiOver4);
            revoluteLimit.Bounciness = bounciness;

            Space.Add(boxA);
            Space.Add(boxB);
            Space.Add(ballSocketJoint);
            Space.Add(revoluteAngularJoint);
            Space.Add(revoluteLimit);

            //SwingLimit
            boxA = new Box(new Vector3(7.ToFix(), 4.ToFix(), 0.ToFix()), 3.ToFix(), 3.ToFix(), 3.ToFix(), baseMass);
            boxB = new Box(boxA.Position + new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 1.ToFix(), 4.ToFix(), 1.ToFix(), armMass);
            CollisionRules.AddRule(boxA, boxB, CollisionRule.NoBroadPhase);
            boxB.ActivityInformation.IsAlwaysActive = true;

            ballSocketJoint = new BallSocketJoint(boxA, boxB, boxB.Position + new Vector3(0.ToFix(), (-2).ToFix(), 0.ToFix()));
            var swingLimit = new SwingLimit(boxA, boxB, Vector3.Up, Vector3.Up, MathHelper.PiOver4);
            swingLimit.Bounciness = bounciness;

            Space.Add(boxA);
            Space.Add(boxB);
            Space.Add(ballSocketJoint);
            Space.Add(swingLimit);

            //TwistLimit
            boxA = new Box(new Vector3(14.ToFix(), 4.ToFix(), 0.ToFix()), 3.ToFix(), 3.ToFix(), 3.ToFix(), baseMass);
            boxB = new Box(boxA.Position + new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 1.ToFix(), 4.ToFix(), 1.ToFix(), armMass);
            CollisionRules.AddRule(boxA, boxB, CollisionRule.NoBroadPhase);
            boxB.ActivityInformation.IsAlwaysActive = true;

            ballSocketJoint = new BallSocketJoint(boxA, boxB, boxB.Position + new Vector3(0.ToFix(), (-2).ToFix(), 0.ToFix()));
            revoluteAngularJoint = new RevoluteAngularJoint(boxA, boxB, Vector3.Up);
            var twistLimit = new TwistLimit(boxA, boxB, Vector3.Up, Vector3.Up, MathHelper.PiOver4.Neg(), MathHelper.PiOver4);
            twistLimit.Bounciness = bounciness;

            Space.Add(boxA);
            Space.Add(boxB);
            Space.Add(ballSocketJoint);
            Space.Add(revoluteAngularJoint);
            Space.Add(twistLimit);

            Space.Add(new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 60.ToFix(), 1.ToFix(), 60.ToFix()));
            game.Camera.Position = new Vector3(0.ToFix(), 6.ToFix(), 15.ToFix());
        }

        public override string Name
        {
            get { return "Joint Limit Behavior Test"; }
        }
    }
}
