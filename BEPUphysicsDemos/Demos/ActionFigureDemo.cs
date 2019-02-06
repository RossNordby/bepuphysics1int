using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Poseable humanoid with joint friction.
    /// </summary>
    public class ActionFigureDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public ActionFigureDemo(DemosGame game)
            : base(game)
        {
            //Make a simple, poseable action figure.  This isn't a full featured 'ragdoll' really;
            //ragdolls usually have specific joint limits and appropriate kinds of joints rather than all
            //ball socket joints.  This demo could be modified into a 'proper' ragdoll.
            Entity body = new Box(new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 1.5m.ToFix(), 2.ToFix(), 1.ToFix(), 10.ToFix());
            Space.Add(body);

            Entity head = new Sphere(body.Position + new Vector3(0.ToFix(), 2.ToFix(), 0.ToFix()), .5m.ToFix(), 5.ToFix());
            Space.Add(head);

            //Connect the head to the body.
            Space.Add(new BallSocketJoint(body, head, head.Position + new Vector3(0.ToFix(), (-.9m).ToFix(), 0.ToFix())));
            //Angular motors can be used to simulate friction when their goal velocity is 0.
            var angularMotor = new AngularMotor(body, head);
            angularMotor.Settings.MaximumForce = 150.ToFix(); //The maximum force of 'friction' in this joint.
            Space.Add(angularMotor);

            //Make the first arm.
            var upperLimb = new Box(body.Position + new Vector3((-1.6m).ToFix(), .8m.ToFix(), 0.ToFix()), 1.ToFix(), .5m.ToFix(), .5m.ToFix(), 5.ToFix());
            Space.Add(upperLimb);

            var lowerLimb = new Box(upperLimb.Position + new Vector3((-1.4m).ToFix(), 0.ToFix(), 0.ToFix()), 1.ToFix(), .5m.ToFix(), .5m.ToFix(), 5.ToFix());
            Space.Add(lowerLimb);

            //Connect the body to the upper arm.
            Space.Add(new BallSocketJoint(body, upperLimb, upperLimb.Position + new Vector3(.7m.ToFix(), 0.ToFix(), 0.ToFix())));
            angularMotor = new AngularMotor(body, upperLimb);
            angularMotor.Settings.MaximumForce = 250.ToFix();
            Space.Add(angularMotor);


            //Connect the upper arm to the lower arm.
            Space.Add(new BallSocketJoint(upperLimb, lowerLimb, upperLimb.Position + new Vector3((-.7m).ToFix(), 0.ToFix(), 0.ToFix())));
            angularMotor = new AngularMotor(upperLimb, lowerLimb);
            angularMotor.Settings.MaximumForce = 150.ToFix();
            Space.Add(angularMotor);

            //Make the second arm.
            upperLimb = new Box(body.Position + new Vector3(1.6m.ToFix(), .8m.ToFix(), 0.ToFix()), 1.ToFix(), .5m.ToFix(), .5m.ToFix(), 5.ToFix());
            Space.Add(upperLimb);

            lowerLimb = new Box(upperLimb.Position + new Vector3(1.4m.ToFix(), 0.ToFix(), 0.ToFix()), 1.ToFix(), .5m.ToFix(), .5m.ToFix(), 5.ToFix());
            Space.Add(lowerLimb);

            //Connect the body to the upper arm.
            Space.Add(new BallSocketJoint(body, upperLimb, upperLimb.Position + new Vector3((-.7m).ToFix(), 0.ToFix(), 0.ToFix())));
            //Angular motors can be used to simulate friction when their goal velocity is 0.
            angularMotor = new AngularMotor(body, upperLimb);
            angularMotor.Settings.MaximumForce = 250.ToFix(); //The maximum force of 'friction' in this joint.
            Space.Add(angularMotor);


            //Connect the upper arm to the lower arm.
            Space.Add(new BallSocketJoint(upperLimb, lowerLimb, upperLimb.Position + new Vector3(.7m.ToFix(), 0.ToFix(), 0.ToFix())));
            angularMotor = new AngularMotor(upperLimb, lowerLimb);
            angularMotor.Settings.MaximumForce = 150.ToFix();
            Space.Add(angularMotor);

            //Make the first leg.
            upperLimb = new Box(body.Position + new Vector3((-.6m).ToFix(), (-2.1m).ToFix(), 0.ToFix()), .5m.ToFix(), 1.3m.ToFix(), .5m.ToFix(), 8.ToFix());
            Space.Add(upperLimb);

            lowerLimb = new Box(upperLimb.Position + new Vector3(0.ToFix(), (-1.7m).ToFix(), 0.ToFix()), .5m.ToFix(), 1.3m.ToFix(), .5m.ToFix(), 8.ToFix());
            Space.Add(lowerLimb);

            //Connect the body to the upper leg.
            Space.Add(new BallSocketJoint(body, upperLimb, upperLimb.Position + new Vector3(0.ToFix(), .9m.ToFix(), 0.ToFix())));
            //Angular motors can be used to simulate friction when their goal velocity is 0.
            angularMotor = new AngularMotor(body, upperLimb);
            angularMotor.Settings.MaximumForce = 350.ToFix(); //The maximum force of 'friction' in this joint.
            Space.Add(angularMotor);


            //Connect the upper leg to the lower leg.
            Space.Add(new BallSocketJoint(upperLimb, lowerLimb, upperLimb.Position + new Vector3(0.ToFix(), (-.9m).ToFix(), 0.ToFix())));
            angularMotor = new AngularMotor(upperLimb, lowerLimb);
            angularMotor.Settings.MaximumForce = 250.ToFix();
            Space.Add(angularMotor);

            //Make the second leg.
            upperLimb = new Box(body.Position + new Vector3(.6m.ToFix(), (-2.1m).ToFix(), 0.ToFix()), .5m.ToFix(), 1.3m.ToFix(), .5m.ToFix(), 8.ToFix());
            Space.Add(upperLimb);

            lowerLimb = new Box(upperLimb.Position + new Vector3(0.ToFix(), (-1.7m).ToFix(), 0.ToFix()), .5m.ToFix(), 1.3m.ToFix(), .5m.ToFix(), 8.ToFix());
            Space.Add(lowerLimb);

            //Connect the body to the upper leg.
            Space.Add(new BallSocketJoint(body, upperLimb, upperLimb.Position + new Vector3(0.ToFix(), .9m.ToFix(), 0.ToFix())));
            //Angular motors can be used to simulate friction when their goal velocity is 0.
            angularMotor = new AngularMotor(body, upperLimb);
            angularMotor.Settings.MaximumForce = 350.ToFix(); //The maximum force of 'friction' in this joint.
            Space.Add(angularMotor);


            //Connect the upper leg to the lower leg.
            Space.Add(new BallSocketJoint(upperLimb, lowerLimb, upperLimb.Position + new Vector3(0.ToFix(), (-.9m).ToFix(), 0.ToFix())));
            angularMotor = new AngularMotor(upperLimb, lowerLimb);
            angularMotor.Settings.MaximumForce = 250.ToFix();
            Space.Add(angularMotor);

            //Add some ground.
            Space.Add(new Box(new Vector3(0.ToFix(), (-3.5m).ToFix(), 0.ToFix()), 40.ToFix(), 1.ToFix(), 40.ToFix()));

            game.Camera.Position = new Vector3(0.ToFix(), 5.ToFix(), 25.ToFix());
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Action Figure"; }
        }
    }
}