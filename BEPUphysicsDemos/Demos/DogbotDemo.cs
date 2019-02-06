using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.JointLimits;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Motorized walking robo-dog.
    /// </summary>
    public class DogbotDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public DogbotDemo(DemosGame game)
            : base(game)
        {
            Entity body = new Box(new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix()), 4.ToFix(), 2.ToFix(), 2.ToFix(), 20.ToFix());
            Space.Add(body);

            Entity head = new Cone(body.Position + new Vector3(3.2m.ToFix(), .3m.ToFix(), 0.ToFix()), 1.5m.ToFix(), .7m.ToFix(), 4.ToFix());
            head.OrientationMatrix = Matrix3x3.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2);
            Space.Add(head);

            //Attach the head to the body
            var universalJoint = new UniversalJoint(body, head, head.Position + new Vector3((-.8m).ToFix(), 0.ToFix(), 0.ToFix()));
            Space.Add(universalJoint);
            //Keep the head from swinging around too much.
            var angleLimit = new SwingLimit(body, head, Vector3.Right, Vector3.Right, MathHelper.PiOver4);
            Space.Add(angleLimit);

            var tail = new Box(body.Position + new Vector3((-3m).ToFix(), 1m.ToFix(), 0.ToFix()), 1.6m.ToFix(), .1m.ToFix(), .1m.ToFix(), 4.ToFix());
            Space.Add(tail);
            //Keep the tail from twisting itself off.
            universalJoint = new UniversalJoint(body, tail, tail.Position + new Vector3(.8m.ToFix(), 0.ToFix(), 0.ToFix()));
            Space.Add(universalJoint);

            //Give 'em some floppy ears.
            var ear = new Box(head.Position + new Vector3((-.2m).ToFix(), 0.ToFix(), (-.65m).ToFix()), .01m.ToFix(), .7m.ToFix(), .2m.ToFix(), 1.ToFix());
            Space.Add(ear);

            var ballSocketJoint = new BallSocketJoint(head, ear, head.Position + new Vector3((-.2m).ToFix(), .35m.ToFix(), (-.65m).ToFix()));
            Space.Add(ballSocketJoint);

            ear = new Box(head.Position + new Vector3((-.2m).ToFix(), 0.ToFix(), .65m.ToFix()), .01m.ToFix(), .7m.ToFix(), .3m.ToFix(), 1.ToFix());
            Space.Add(ear);

            ballSocketJoint = new BallSocketJoint(head, ear, head.Position + new Vector3((-.2m).ToFix(), .35m.ToFix(), .65m.ToFix()));
            Space.Add(ballSocketJoint);


            Box arm;
            Cylinder shoulder;
            PointOnLineJoint pointOnLineJoint;

            //*************  First Arm   *************//
            arm = new Box(body.Position + new Vector3((-1.8m).ToFix(), (-.5m).ToFix(), 1.5m.ToFix()), .5m.ToFix(), 3.ToFix(), .2m.ToFix(), 20.ToFix());
            Space.Add(arm);

            shoulder = new Cylinder(body.Position + new Vector3((-1.8m).ToFix(), .3m.ToFix(), 1.25m.ToFix()), .1m.ToFix(), .7m.ToFix(), 10.ToFix());
            shoulder.OrientationMatrix = Matrix3x3.CreateFromAxisAngle(Vector3.Right, MathHelper.PiOver2);
            Space.Add(shoulder);

            //Connect the shoulder to the body.
            var axisJoint = new RevoluteJoint(body, shoulder, shoulder.Position, Vector3.Forward);

            //Motorize the connection.
            axisJoint.Motor.IsActive = true;
            axisJoint.Motor.Settings.VelocityMotor.GoalVelocity = 1.ToFix();

            Space.Add(axisJoint);

            //Connect the arm to the shoulder.
            axisJoint = new RevoluteJoint(shoulder, arm, shoulder.Position + new Vector3(0.ToFix(), .6m.ToFix(), 0.ToFix()), Vector3.Forward);
            Space.Add(axisJoint);

            //Connect the arm to the body.
            pointOnLineJoint = new PointOnLineJoint(arm, body, arm.Position, Vector3.Up, arm.Position + new Vector3(0.ToFix(), (-.4m).ToFix(), 0.ToFix()));
            Space.Add(pointOnLineJoint);


            shoulder.OrientationMatrix *= Matrix3x3.CreateFromAxisAngle(Vector3.Forward, MathHelper.Pi); //Force the walker's legs out of phase.

            //*************  Second Arm   *************//
            arm = new Box(body.Position + new Vector3(1.8m.ToFix(), (-.5m).ToFix(), 1.5m.ToFix()), .5m.ToFix(), 3.ToFix(), .2m.ToFix(), 20.ToFix());
            Space.Add(arm);

            shoulder = new Cylinder(body.Position + new Vector3(1.8m.ToFix(), .3m.ToFix(), 1.25m.ToFix()), .1m.ToFix(), .7m.ToFix(), 10.ToFix());
            shoulder.OrientationMatrix = Matrix3x3.CreateFromAxisAngle(Vector3.Right, MathHelper.PiOver2);
            Space.Add(shoulder);

            //Connect the shoulder to the body.
            axisJoint = new RevoluteJoint(body, shoulder, shoulder.Position, Vector3.Forward);

            //Motorize the connection.
            axisJoint.Motor.IsActive = true;
            axisJoint.Motor.Settings.VelocityMotor.GoalVelocity = 1.ToFix();

            Space.Add(axisJoint);

            //Connect the arm to the shoulder.
            axisJoint = new RevoluteJoint(shoulder, arm, shoulder.Position + new Vector3(0.ToFix(), .6m.ToFix(), 0.ToFix()), Vector3.Forward);
            Space.Add(axisJoint);


            //Connect the arm to the body.
            pointOnLineJoint = new PointOnLineJoint(arm, body, arm.Position, Vector3.Up, arm.Position + new Vector3(0.ToFix(), (-.4m).ToFix(), 0.ToFix()));
            Space.Add(pointOnLineJoint);

            //*************  Third Arm   *************//
            arm = new Box(body.Position + new Vector3((-1.8m).ToFix(), (-.5m).ToFix(), (-1.5m).ToFix()), .5m.ToFix(), 3.ToFix(), .2m.ToFix(), 20.ToFix());
            Space.Add(arm);

            shoulder = new Cylinder(body.Position + new Vector3((-1.8m).ToFix(), .3m.ToFix(), (-1.25m).ToFix()), .1m.ToFix(), .7m.ToFix(), 10.ToFix());
            shoulder.OrientationMatrix = Matrix3x3.CreateFromAxisAngle(Vector3.Right, MathHelper.PiOver2);
            Space.Add(shoulder);

            //Connect the shoulder to the body.
            axisJoint = new RevoluteJoint(body, shoulder, shoulder.Position, Vector3.Forward);

            //Motorize the connection.
            axisJoint.Motor.IsActive = true;
            axisJoint.Motor.Settings.VelocityMotor.GoalVelocity = 1.ToFix();

            Space.Add(axisJoint);

            //Connect the arm to the shoulder.
            axisJoint = new RevoluteJoint(shoulder, arm, shoulder.Position + new Vector3(0.ToFix(), .6m.ToFix(), 0.ToFix()), Vector3.Forward);
            Space.Add(axisJoint);

            //Connect the arm to the body.
            pointOnLineJoint = new PointOnLineJoint(arm, body, arm.Position, Vector3.Up, arm.Position + new Vector3(0.ToFix(), (-.4m).ToFix(), 0.ToFix()));
            Space.Add(pointOnLineJoint);


            shoulder.OrientationMatrix *= Matrix3x3.CreateFromAxisAngle(Vector3.Forward, MathHelper.Pi); //Force the walker's legs out of phase.

            //*************  Fourth Arm   *************//
            arm = new Box(body.Position + new Vector3(1.8m.ToFix(), (-.5m).ToFix(), (-1.5m).ToFix()), .5m.ToFix(), 3.ToFix(), .2m.ToFix(), 20.ToFix());
            Space.Add(arm);

            shoulder = new Cylinder(body.Position + new Vector3(1.8m.ToFix(), .3m.ToFix(), (-1.25m).ToFix()), .1m.ToFix(), .7m.ToFix(), 10.ToFix());
            shoulder.OrientationMatrix = Matrix3x3.CreateFromAxisAngle(Vector3.Right, MathHelper.PiOver2);
            Space.Add(shoulder);

            //Connect the shoulder to the body.
            axisJoint = new RevoluteJoint(body, shoulder, shoulder.Position, Vector3.Forward);

            //Motorize the connection.
            axisJoint.Motor.IsActive = true;
            axisJoint.Motor.Settings.VelocityMotor.GoalVelocity = 1.ToFix();

            Space.Add(axisJoint);

            //Connect the arm to the shoulder.
            axisJoint = new RevoluteJoint(shoulder, arm, shoulder.Position + new Vector3(0.ToFix(), .6m.ToFix(), 0.ToFix()), Vector3.Forward);
            Space.Add(axisJoint);

            //Connect the arm to the body.
            pointOnLineJoint = new PointOnLineJoint(arm, body, arm.Position, Vector3.Up, arm.Position + new Vector3(0.ToFix(), (-.4m).ToFix(), 0.ToFix()));
            Space.Add(pointOnLineJoint);

            //Add some ground.
            Space.Add(new Box(new Vector3(0.ToFix(), (-3.5m).ToFix(), 0.ToFix()), 20m.ToFix(), 1.ToFix(), 20m.ToFix()));

            game.Camera.Position = new Vector3(0.ToFix(), 2.ToFix(), 20.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Dogbot"; }
        }
    }
}