
using System;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A spinning blade connected to a piston connected to a spinning box slices up some other boxes.
    /// </summary>
    public class SawContraptionDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public SawContraptionDemo(DemosGame game)
            : base(game)
        {
            //Add a kinematic entity that will form the base for the strange contraption.
            var pistonGroundAttachment = new Box(new Vector3(0.ToFix(), (-1).ToFix(), 0.ToFix()), 1.ToFix(), 2.ToFix(), .5m.ToFix());
            pistonGroundAttachment.AngularVelocity = new Vector3(0.ToFix(), .2m.ToFix(), 0.ToFix()); //Make it spin a little to rotate the whole thing.
            Space.Add(pistonGroundAttachment);

            var pistonBox1 = new Box(pistonGroundAttachment.Position + new Vector3(0.ToFix(), 0.ToFix(), 1.ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix(), 100.ToFix());
            var pistonBox2 = new Box(pistonBox1.Position + new Vector3(0.ToFix(), 2.ToFix(), 0.ToFix()), .5m.ToFix(), .8m.ToFix(), .5m.ToFix(), 10.ToFix());
            Space.Add(pistonBox1);
            Space.Add(pistonBox2);

            //Connect the piston entities to the base with a revolute joint that acts like an axis joint
            var axisJoint = new RevoluteJoint(pistonGroundAttachment, pistonBox1, (pistonGroundAttachment.Position + pistonBox1.Position) / 2.ToFix(), Vector3.Forward);

            //Keep the axis from rotating too far so that the saw blade won't just continually ram into the ground. 
            //The limit's 'basis' and test axis used to determine the position of the limits and the current angle of the constraint
            //are automatically initialized by the RevoluteJoint constructor.  The basis and testAxis can be set afterwards; 
            //the initialization is just a reasonable 'guess' at the kind of limit configuration desired.
            axisJoint.Limit.IsActive = true;
            axisJoint.Limit.MinimumAngle = MathHelper.PiOver2.Neg();
            axisJoint.Limit.MaximumAngle = MathHelper.PiOver2;
            Space.Add(axisJoint);

            var piston = new PrismaticJoint(pistonBox1, pistonBox2, pistonBox1.Position, Vector3.Up, pistonBox2.Position);
            //Set up the piston limits.
            piston.Limit.IsActive = true; //By default, the limit and motor are both inactive.
            piston.Limit.Minimum = 2.ToFix();
            piston.Limit.Maximum = 5.ToFix();

            //Set up the servo motor.
            piston.Motor.IsActive = true;
            piston.Motor.Settings.Mode = MotorMode.Servomechanism;
            //Distance from the anchor that the piston will try to reach.
            piston.Motor.Settings.Servo.Goal = 5.ToFix();
            //Set the maximum force the motor can use to reach its goal.
            piston.Motor.Settings.MaximumForce = 100.ToFix();
            //This piston, by default, moves at a constant speed, but...
            piston.Motor.Settings.Servo.BaseCorrectiveSpeed = 1.ToFix();
            //... if the stiffness constant is changed to a positive value, it can also act like a spring.
            piston.Motor.Settings.Servo.SpringSettings.Stiffness = 0.ToFix();
            //For a non-springy constraint like the piston, the dampingConstant can also be thought of as inverse 'softness.'
            piston.Motor.Settings.Servo.SpringSettings.Damping = 1000.ToFix();

            //Add the piston to the space.
            Space.Add(piston);

            //Create a saw bladey object on the end of the piston.
            var blade = new Box(pistonBox2.Position + new Vector3(0.ToFix(), 0.ToFix(), .5m.ToFix()), .3m.ToFix(), 2.5m.ToFix(), .1m.ToFix(), 5.ToFix());
            Space.Add(blade);

            //Connect the saw to the piston with a second axis joint.
            axisJoint = new RevoluteJoint(pistonBox2, blade, (pistonBox2.Position + blade.Position) / 2.ToFix(), Vector3.Forward);
            //Revolute joints can be used to make axis joints (as it is here), but you can also use them to make hinges.
            Space.Add(axisJoint);

            //Make the blade spin.
            axisJoint.Motor.IsActive = true;
            axisJoint.Motor.Settings.VelocityMotor.GoalVelocity = 30.ToFix();
            axisJoint.Motor.Settings.MaximumForce = 200.ToFix();

            //Add some ground.
            Space.Add(new Box(new Vector3(0.ToFix(), (-3).ToFix(), 0.ToFix()), 20.ToFix(), 1.ToFix(), 20.ToFix()));

            //Make some debris for the saw to chop.
            for (Fix64 k = 0.ToFix(); k < MathHelper.Pi.Mul(2.ToFix()); k = k.Add(MathHelper.Pi.Div(20.ToFix())))
            {
                Space.Add(new Box(new Vector3(Fix64Ext.Cos(k).Mul(4.ToFix()), (-2).ToFix(), Fix64Ext.Sin(k).Mul(6.5m.ToFix())), .5m.ToFix(), 1.ToFix(), .5m.ToFix(), 10.ToFix()));
            }


            game.Camera.Position = new Vector3(0.ToFix(), 2.ToFix(), 20.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Saw Contraption"; }
        }
    }
}