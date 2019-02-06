using System.Collections.Generic;
using BEPUphysics;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Constraints;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.JointLimits;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUutilities.DataStructures;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Poseable humanoid with joint friction.
    /// </summary>
    public class RagdollDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public RagdollDemo(DemosGame game)
            : base(game)
        {
            int numRows = 8;
            int numColumns = 3;
            Fix64 xSpacing = 5.ToFix();
            Fix64 zSpacing = 5.ToFix();
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    Ragdoll ragdoll = new Ragdoll();
                    //Transform and add every bone.
                    foreach (var bone in ragdoll.Bones)
                    {
                        bone.WorldTransform *= Matrix.CreateTranslation(new Vector3(
(i.ToFix().Mul(xSpacing)).Sub((((numRows - 1).ToFix()).Mul(xSpacing)).Div(2.ToFix())),
(i - 1.5m).ToFix(),
j.ToFix().Mul(zSpacing).Sub((((numColumns - 1).ToFix()).Mul(zSpacing)).Div(2.ToFix()))));
                        Space.Add(bone);
                    }

                    //Add every constraint.
                    foreach (var joint in ragdoll.Joints)
                    {
                        Space.Add(joint);
                    }
                }
            }

            //Add some ground.
            Space.Add(new Box(new Vector3(0.ToFix(), (-3.5m).ToFix(), 0.ToFix()), 50.ToFix(), 1.ToFix(), 50.ToFix()));

            game.Camera.Position = new Vector3(0.ToFix(), 5.ToFix(), (-35).ToFix());
            game.Camera.ViewDirection = new Vector3(0.ToFix(), 0.ToFix(), 1.ToFix());
        }

        public override void DrawUI()
        {
            Game.DataTextDrawer.Draw("Press J to toggle joint visualization.", new Microsoft.Xna.Framework.Vector2(40, 40));
            base.DrawUI();
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Ragdolls"; }
        }
    }

    /// <summary>
    /// Basic humanoid ragdoll.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The following creates the physical parts of a ragdoll (bones and joints).
    /// This is only one possible setup for a ragdoll.
    /// </para>
    /// <para>
    /// This example uses AngularMotors to simulate damping between ragdoll bones.
    /// A cheaper, though technically less 'realistic' approach, would be to simply
    /// change the angular damping property of the involved entities.
    /// </para>
    /// <para>
    /// Because of the large number of constraints and the focus on functional realism,
    /// this type of ragdoll should only be used sparingly.  If you need a dozen or more
    /// ragdolls going at once, particularly on the Xbox360, it may be a good idea to
    /// cut down the realism in favor of performance.
    /// </para>
    /// <para>
    /// The following example does not include a possible optimization for multithreaded
    /// simulations.  Since the solver cannot update a entity from more than one constraint
    /// at a time (to avoid data corruption), many overlapping constraints will make the solver
    /// slow down a bit.  A simulation composed of just a bunch of these ragdolls will incur some 
    /// multithreading overhead due to the overlaps.
    /// </para>
    /// <para>
    /// By batching the constraints between any two bones in the ragdoll into a single "SolverGroup"
    /// object, the solver only sees a single constraint instead of 3-5 different ones.  This can improve
    /// multithreaded performance, but was excluded here for the sake of simplicity.
    /// </para>
    /// <para>
    /// Some of the constraints, like SwivelHingeJoint, are in fact SolverGroups themselves.
    /// They contain a variety of configurable sub-constraints.   
    /// </para>
    /// </remarks>
    public class Ragdoll
    {
        //List format of ragdoll components for easy iteration.
        List<Entity> bones = new List<Entity>();
        List<SolverUpdateable> joints = new List<SolverUpdateable>();

        /// <summary>
        /// Gets the bones of the ragdoll.
        /// </summary>
        public ReadOnlyList<Entity> Bones
        {
            get { return new ReadOnlyList<Entity>(bones); }
        }

        /// <summary>
        /// Gets the joints of the ragdoll.
        /// </summary>
        public ReadOnlyList<SolverUpdateable> Joints
        {
            get { return new ReadOnlyList<SolverUpdateable>(joints); }
        }


        public Ragdoll()
        {

            #region Ragdoll Entities
            //Create the ragdoll's bones.
            var pelvis = new Box(Vector3.Zero, .5m.ToFix(), .28m.ToFix(), .33m.ToFix(), 20.ToFix());
            var torsoBottom = new Box(pelvis.Position + new Vector3(0.ToFix(), .3m.ToFix(), 0.ToFix()), .42m.ToFix(), .48m.ToFix(), .3m.ToFix(), 15.ToFix());
            var torsoTop = new Box(torsoBottom.Position + new Vector3(0.ToFix(), .3m.ToFix(), 0.ToFix()), .5m.ToFix(), .38m.ToFix(), .32m.ToFix(), 20.ToFix());

            var neck = new Box(torsoTop.Position + new Vector3(0.ToFix(), .2m.ToFix(), .04m.ToFix()), .19m.ToFix(), .24m.ToFix(), .2m.ToFix(), 5.ToFix());
            var head = new Sphere(neck.Position + new Vector3(0.ToFix(), .22m.ToFix(), (-.04m).ToFix()), .19m.ToFix(), 7.ToFix());

            var leftUpperArm = new Box(torsoTop.Position + new Vector3((-.46m).ToFix(), .1m.ToFix(), 0.ToFix()), .52m.ToFix(), .19m.ToFix(), .19m.ToFix(), 6.ToFix());
            var leftForearm = new Box(leftUpperArm.Position + new Vector3((-.5m).ToFix(), 0.ToFix(), 0.ToFix()), .52m.ToFix(), .18m.ToFix(), .18m.ToFix(), 5.ToFix());
            var leftHand = new Box(leftForearm.Position + new Vector3((-.35m).ToFix(), 0.ToFix(), 0.ToFix()), .28m.ToFix(), .13m.ToFix(), .22m.ToFix(), 4.ToFix());

            var rightUpperArm = new Box(torsoTop.Position + new Vector3(.46m.ToFix(), .1m.ToFix(), 0.ToFix()), .52m.ToFix(), .19m.ToFix(), .19m.ToFix(), 6.ToFix());
            var rightForearm = new Box(rightUpperArm.Position + new Vector3(.5m.ToFix(), 0.ToFix(), 0.ToFix()), .52m.ToFix(), .18m.ToFix(), .18m.ToFix(), 5.ToFix());
            var rightHand = new Box(rightForearm.Position + new Vector3(.35m.ToFix(), 0.ToFix(), 0.ToFix()), .28m.ToFix(), .13m.ToFix(), .22m.ToFix(), 4.ToFix());

            var leftThigh = new Box(pelvis.Position + new Vector3((-.15m).ToFix(), (-.4m).ToFix(), 0.ToFix()), .23m.ToFix(), .63m.ToFix(), .23m.ToFix(), 10.ToFix());
            var leftShin = new Box(leftThigh.Position + new Vector3(0.ToFix(), (-.6m).ToFix(), 0.ToFix()), .21m.ToFix(), .63m.ToFix(), .21m.ToFix(), 7.ToFix());
            var leftFoot = new Box(leftShin.Position + new Vector3(0.ToFix(), (-.35m).ToFix(), (-.1m).ToFix()), .23m.ToFix(), .15m.ToFix(), .43m.ToFix(), 5.ToFix());

            var rightThigh = new Box(pelvis.Position + new Vector3(.15m.ToFix(), (-.4m).ToFix(), 0.ToFix()), .23m.ToFix(), .63m.ToFix(), .23m.ToFix(), 10.ToFix());
            var rightShin = new Box(rightThigh.Position + new Vector3(0.ToFix(), (-.6m).ToFix(), 0.ToFix()), .21m.ToFix(), .63m.ToFix(), .21m.ToFix(), 7.ToFix());
            var rightFoot = new Box(rightShin.Position + new Vector3(0.ToFix(), (-.35m).ToFix(), (-.1m).ToFix()), .23m.ToFix(), .15m.ToFix(), .43m.ToFix(), 5.ToFix());
            #endregion

            #region Bone List
            //Make a convenient list of all of the bones.
            bones.Add(pelvis);
            bones.Add(torsoBottom);
            bones.Add(torsoTop);
            bones.Add(neck);
            bones.Add(head);
            bones.Add(leftUpperArm);
            bones.Add(leftForearm);
            bones.Add(leftHand);
            bones.Add(rightUpperArm);
            bones.Add(rightForearm);
            bones.Add(rightHand);
            bones.Add(leftThigh);
            bones.Add(leftShin);
            bones.Add(leftFoot);
            bones.Add(rightThigh);
            bones.Add(rightShin);
            bones.Add(rightFoot);
            #endregion

            #region Collision Rules
            //Prevent adjacent limbs from colliding.
            CollisionRules.AddRule(pelvis, torsoBottom, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(torsoBottom, torsoTop, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(torsoTop, neck, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(neck, head, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(head, torsoTop, CollisionRule.NoBroadPhase);

            CollisionRules.AddRule(torsoTop, leftUpperArm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(leftUpperArm, leftForearm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(leftForearm, leftHand, CollisionRule.NoBroadPhase);

            CollisionRules.AddRule(torsoTop, rightUpperArm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(rightUpperArm, rightForearm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(rightForearm, rightHand, CollisionRule.NoBroadPhase);

            CollisionRules.AddRule(pelvis, leftThigh, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(leftThigh, leftShin, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(leftThigh, torsoBottom, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(leftShin, leftFoot, CollisionRule.NoBroadPhase);

            CollisionRules.AddRule(pelvis, rightThigh, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(rightThigh, rightShin, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(rightThigh, torsoBottom, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(rightShin, rightFoot, CollisionRule.NoBroadPhase);
            #endregion

            //Create the constraints between the bones.
            #region Pelvis up to Head Constraints
            var pelvisToTorsoBottomBallSocketJoint = new BallSocketJoint(pelvis, torsoBottom, pelvis.Position + new Vector3(0.ToFix(), .1m.ToFix(), 0.ToFix()));
            var pelvisToTorsoBottomTwistLimit = new TwistLimit(pelvis, torsoBottom, Vector3.Up, Vector3.Up, MathHelper.Pi.Neg().Div(6.ToFix()), MathHelper.Pi.Div(6.ToFix()));
            var pelvisToTorsoBottomSwingLimit = new SwingLimit(pelvis, torsoBottom, Vector3.Up, Vector3.Up, MathHelper.Pi.Div(6.ToFix()));
            var pelvisToTorsoBottomMotor = new AngularMotor(pelvis, torsoBottom);
            pelvisToTorsoBottomMotor.Settings.VelocityMotor.Softness = .05m.ToFix();

            var torsoBottomToTorsoTopBallSocketJoint = new BallSocketJoint(torsoBottom, torsoTop, torsoBottom.Position + new Vector3(0.ToFix(), .25m.ToFix(), 0.ToFix()));
            var torsoBottomToTorsoTopSwingLimit = new SwingLimit(torsoBottom, torsoTop, Vector3.Up, Vector3.Up, MathHelper.Pi.Div(6.ToFix()));
            var torsoBottomToTorsoTopTwistLimit = new TwistLimit(torsoBottom, torsoTop, Vector3.Up, Vector3.Up, MathHelper.Pi.Neg().Div(6.ToFix()), MathHelper.Pi.Div(6.ToFix()));
            var torsoBottomToTorsoTopMotor = new AngularMotor(torsoBottom, torsoTop);
            torsoBottomToTorsoTopMotor.Settings.VelocityMotor.Softness = .05m.ToFix();

            var torsoTopToNeckBallSocketJoint = new BallSocketJoint(torsoTop, neck, torsoTop.Position + new Vector3(0.ToFix(), .15m.ToFix(), .05m.ToFix()));
            var torsoTopToNeckSwingLimit = new SwingLimit(torsoTop, neck, Vector3.Up, Vector3.Up, MathHelper.Pi.Div(6.ToFix()));
            var torsoTopToNeckTwistLimit = new TwistLimit(torsoTop, neck, Vector3.Up, Vector3.Up, MathHelper.Pi.Neg().Div(8.ToFix()), MathHelper.Pi.Div(8.ToFix()));
            var torsoTopToNeckMotor = new AngularMotor(torsoTop, neck);
            torsoTopToNeckMotor.Settings.VelocityMotor.Softness = .1m.ToFix();

            var neckToHeadBallSocketJoint = new BallSocketJoint(neck, head, neck.Position + new Vector3(0.ToFix(), .1m.ToFix(), .05m.ToFix()));
            var neckToHeadTwistLimit = new TwistLimit(neck, head, Vector3.Up, Vector3.Up, MathHelper.Pi.Neg().Div(8.ToFix()), MathHelper.Pi.Div(8.ToFix()));
            var neckToHeadSwingLimit = new SwingLimit(neck, head, Vector3.Up, Vector3.Up, MathHelper.Pi.Div(6.ToFix()));
            var neckToHeadMotor = new AngularMotor(neck, head);
            neckToHeadMotor.Settings.VelocityMotor.Softness = .1m.ToFix();
            #endregion

            #region Left Arm
            var torsoTopToLeftArmBallSocketJoint = new BallSocketJoint(torsoTop, leftUpperArm, torsoTop.Position + new Vector3((-.3m).ToFix(), .1m.ToFix(), 0.ToFix()));
            var torsoTopToLeftArmEllipseLimit = new EllipseSwingLimit(torsoTop, leftUpperArm, Vector3.Left, MathHelper.Pi.Mul(.75m.ToFix()), MathHelper.PiOver2);
            var torsoTopToLeftArmTwistLimit = new TwistLimit(torsoTop, leftUpperArm, Vector3.Left, Vector3.Left, MathHelper.PiOver2.Neg(), MathHelper.PiOver2);
            var torsoTopToLeftArmMotor = new AngularMotor(torsoTop, leftUpperArm);
            torsoTopToLeftArmMotor.Settings.VelocityMotor.Softness = .2m.ToFix();

            var leftUpperArmToLeftForearmSwivelHingeJoint = new SwivelHingeJoint(leftUpperArm, leftForearm, leftUpperArm.Position + new Vector3((-.28m).ToFix(), 0.ToFix(), 0.ToFix()), Vector3.Up);
            leftUpperArmToLeftForearmSwivelHingeJoint.HingeLimit.IsActive = true;
            leftUpperArmToLeftForearmSwivelHingeJoint.TwistLimit.IsActive = true;
            leftUpperArmToLeftForearmSwivelHingeJoint.TwistLimit.MinimumAngle = MathHelper.Pi.Neg().Div(8.ToFix());
            leftUpperArmToLeftForearmSwivelHingeJoint.TwistLimit.MaximumAngle = MathHelper.Pi.Div(8.ToFix());
            leftUpperArmToLeftForearmSwivelHingeJoint.HingeLimit.MinimumAngle = MathHelper.Pi.Neg().Mul(.8m.ToFix());
            leftUpperArmToLeftForearmSwivelHingeJoint.HingeLimit.MaximumAngle = 0.ToFix();
            //The SwivelHingeJoint has motors, but they are separately defined for twist/bending.
            //The AngularMotor covers all degrees of freedom.
            var leftUpperArmToLeftForearmMotor = new AngularMotor(leftUpperArm, leftForearm);
            leftUpperArmToLeftForearmMotor.Settings.VelocityMotor.Softness = .3m.ToFix();

            var leftForearmToLeftHandBallSocketJoint = new BallSocketJoint(leftForearm, leftHand, leftForearm.Position + new Vector3((-.2m).ToFix(), 0.ToFix(), 0.ToFix()));
            var leftForearmToLeftHandEllipseSwingLimit = new EllipseSwingLimit(leftForearm, leftHand, Vector3.Left, MathHelper.PiOver2, MathHelper.Pi.Div(6.ToFix()));
            var leftForearmToLeftHandTwistLimit = new TwistLimit(leftForearm, leftHand, Vector3.Left, Vector3.Left, MathHelper.Pi.Neg().Div(6.ToFix()), MathHelper.Pi.Div(6.ToFix()));
            var leftForearmToLeftHandMotor = new AngularMotor(leftForearm, leftHand);
            leftForearmToLeftHandMotor.Settings.VelocityMotor.Softness = .4m.ToFix();
            #endregion

            #region Right Arm
            var torsoTopToRightArmBallSocketJoint = new BallSocketJoint(torsoTop, rightUpperArm, torsoTop.Position + new Vector3(.3m.ToFix(), .1m.ToFix(), 0.ToFix()));
            var torsoTopToRightArmEllipseLimit = new EllipseSwingLimit(torsoTop, rightUpperArm, Vector3.Right, MathHelper.Pi.Mul(.75m.ToFix()), MathHelper.PiOver2);
            var torsoTopToRightArmTwistLimit = new TwistLimit(torsoTop, rightUpperArm, Vector3.Right, Vector3.Right, MathHelper.PiOver2.Neg(), MathHelper.PiOver2);
            var torsoTopToRightArmMotor = new AngularMotor(torsoTop, rightUpperArm);
            torsoTopToRightArmMotor.Settings.VelocityMotor.Softness = .2m.ToFix();

            var rightUpperArmToRightForearmSwivelHingeJoint = new SwivelHingeJoint(rightUpperArm, rightForearm, rightUpperArm.Position + new Vector3(.28m.ToFix(), 0.ToFix(), 0.ToFix()), Vector3.Up);
            rightUpperArmToRightForearmSwivelHingeJoint.HingeLimit.IsActive = true;
            rightUpperArmToRightForearmSwivelHingeJoint.TwistLimit.IsActive = true;
            rightUpperArmToRightForearmSwivelHingeJoint.TwistLimit.MinimumAngle = MathHelper.Pi.Neg().Div(8.ToFix());
            rightUpperArmToRightForearmSwivelHingeJoint.TwistLimit.MaximumAngle = MathHelper.Pi.Div(8.ToFix());
            rightUpperArmToRightForearmSwivelHingeJoint.HingeLimit.MinimumAngle = 0.ToFix();
            rightUpperArmToRightForearmSwivelHingeJoint.HingeLimit.MaximumAngle = MathHelper.Pi.Mul(.8m.ToFix());
            //The SwivelHingeJoint has motors, but they are separately defined for twist/bending.
            //The AngularMotor covers all degrees of freedom.
            var rightUpperArmToRightForearmMotor = new AngularMotor(rightUpperArm, rightForearm);
            rightUpperArmToRightForearmMotor.Settings.VelocityMotor.Softness = .3m.ToFix();

            var rightForearmToRightHandBallSocketJoint = new BallSocketJoint(rightForearm, rightHand, rightForearm.Position + new Vector3(.2m.ToFix(), 0.ToFix(), 0.ToFix()));
            var rightForearmToRightHandEllipseSwingLimit = new EllipseSwingLimit(rightForearm, rightHand, Vector3.Right, MathHelper.PiOver2, MathHelper.Pi.Div(6.ToFix()));
            var rightForearmToRightHandTwistLimit = new TwistLimit(rightForearm, rightHand, Vector3.Right, Vector3.Right, MathHelper.Pi.Neg().Div(6.ToFix()), MathHelper.Pi.Div(6.ToFix()));
            var rightForearmToRightHandMotor = new AngularMotor(rightForearm, rightHand);
            rightForearmToRightHandMotor.Settings.VelocityMotor.Softness = .4m.ToFix();
            #endregion

            #region Left Leg
            var pelvisToLeftThighBallSocketJoint = new BallSocketJoint(pelvis, leftThigh, pelvis.Position + new Vector3((-.15m).ToFix(), (-.1m).ToFix(), 0.ToFix()));
            var pelvisToLeftThighEllipseSwingLimit = new EllipseSwingLimit(pelvis, leftThigh, Vector3.Normalize(new Vector3((-.2m).ToFix(), (-1).ToFix(), (-.6m).ToFix())), MathHelper.Pi.Mul(.7m.ToFix()), MathHelper.PiOver4);
            pelvisToLeftThighEllipseSwingLimit.LocalTwistAxisB = Vector3.Down;
            var pelvisToLeftThighTwistLimit = new TwistLimit(pelvis, leftThigh, Vector3.Down, Vector3.Down, MathHelper.Pi.Neg().Div(6.ToFix()), MathHelper.Pi.Div(6.ToFix()));
            var pelvisToLeftThighMotor = new AngularMotor(pelvis, leftThigh);
            pelvisToLeftThighMotor.Settings.VelocityMotor.Softness = .1m.ToFix();

            var leftThighToLeftShinRevoluteJoint = new RevoluteJoint(leftThigh, leftShin, leftThigh.Position + new Vector3(0.ToFix(), (-.3m).ToFix(), 0.ToFix()), Vector3.Right);
            leftThighToLeftShinRevoluteJoint.Limit.IsActive = true;
            leftThighToLeftShinRevoluteJoint.Limit.MinimumAngle = MathHelper.Pi.Neg().Mul(.8m.ToFix());
            leftThighToLeftShinRevoluteJoint.Limit.MaximumAngle = 0.ToFix();
            leftThighToLeftShinRevoluteJoint.Motor.IsActive = true;
            leftThighToLeftShinRevoluteJoint.Motor.Settings.VelocityMotor.Softness = .2m.ToFix();

            var leftShinToLeftFootBallSocketJoint = new BallSocketJoint(leftShin, leftFoot, leftShin.Position + new Vector3(0.ToFix(), (-.3m).ToFix(), 0.ToFix()));
            var leftShinToLeftFootSwingLimit = new SwingLimit(leftShin, leftFoot, Vector3.Forward, Vector3.Forward, MathHelper.Pi.Div(8.ToFix()));
            var leftShinToLeftFootTwistLimit = new TwistLimit(leftShin, leftFoot, Vector3.Down, Vector3.Forward, MathHelper.Pi.Neg().Div(8.ToFix()), MathHelper.Pi.Div(8.ToFix()));
            var leftShinToLeftFootMotor = new AngularMotor(leftShin, leftFoot);
            leftShinToLeftFootMotor.Settings.VelocityMotor.Softness = .2m.ToFix();

            #endregion

            #region Right Leg
            var pelvisToRightThighBallSocketJoint = new BallSocketJoint(pelvis, rightThigh, pelvis.Position + new Vector3(.15m.ToFix(), (-.1m).ToFix(), 0.ToFix()));
            var pelvisToRightThighEllipseSwingLimit = new EllipseSwingLimit(pelvis, rightThigh, Vector3.Normalize(new Vector3(.2m.ToFix(), (-1).ToFix(), (-.6m).ToFix())), MathHelper.Pi.Mul(.7m.ToFix()), MathHelper.PiOver4);
            pelvisToRightThighEllipseSwingLimit.LocalTwistAxisB = Vector3.Down;
            var pelvisToRightThighTwistLimit = new TwistLimit(pelvis, rightThigh, Vector3.Down, Vector3.Down, MathHelper.Pi.Neg().Div(6.ToFix()), MathHelper.Pi.Div(6.ToFix()));
            var pelvisToRightThighMotor = new AngularMotor(pelvis, rightThigh);
            pelvisToRightThighMotor.Settings.VelocityMotor.Softness = .1m.ToFix();

            var rightThighToRightShinRevoluteJoint = new RevoluteJoint(rightThigh, rightShin, rightThigh.Position + new Vector3(0.ToFix(), (-.3m).ToFix(), 0.ToFix()), Vector3.Right);
            rightThighToRightShinRevoluteJoint.Limit.IsActive = true;
            rightThighToRightShinRevoluteJoint.Limit.MinimumAngle = MathHelper.Pi.Neg().Mul(.8m.ToFix());
            rightThighToRightShinRevoluteJoint.Limit.MaximumAngle = 0.ToFix();
            rightThighToRightShinRevoluteJoint.Motor.IsActive = true;
            rightThighToRightShinRevoluteJoint.Motor.Settings.VelocityMotor.Softness = .2m.ToFix();

            var rightShinToRightFootBallSocketJoint = new BallSocketJoint(rightShin, rightFoot, rightShin.Position + new Vector3(0.ToFix(), (-.3m).ToFix(), 0.ToFix()));
            var rightShinToRightFootSwingLimit = new SwingLimit(rightShin, rightFoot, Vector3.Forward, Vector3.Forward, MathHelper.Pi.Div(8.ToFix()));
            var rightShinToRightFootTwistLimit = new TwistLimit(rightShin, rightFoot, Vector3.Down, Vector3.Forward, MathHelper.Pi.Neg().Div(8.ToFix()), MathHelper.Pi.Div(8.ToFix()));
            var rightShinToRightFootMotor = new AngularMotor(rightShin, rightFoot);
            rightShinToRightFootMotor.Settings.VelocityMotor.Softness = .2m.ToFix();

            #endregion

            #region Joint List
            //Collect the joints.
            joints.Add(pelvisToTorsoBottomBallSocketJoint);
            joints.Add(pelvisToTorsoBottomTwistLimit);
            joints.Add(pelvisToTorsoBottomSwingLimit);
            joints.Add(pelvisToTorsoBottomMotor);

            joints.Add(torsoBottomToTorsoTopBallSocketJoint);
            joints.Add(torsoBottomToTorsoTopTwistLimit);
            joints.Add(torsoBottomToTorsoTopSwingLimit);
            joints.Add(torsoBottomToTorsoTopMotor);

            joints.Add(torsoTopToNeckBallSocketJoint);
            joints.Add(torsoTopToNeckTwistLimit);
            joints.Add(torsoTopToNeckSwingLimit);
            joints.Add(torsoTopToNeckMotor);

            joints.Add(neckToHeadBallSocketJoint);
            joints.Add(neckToHeadTwistLimit);
            joints.Add(neckToHeadSwingLimit);
            joints.Add(neckToHeadMotor);

            joints.Add(torsoTopToLeftArmBallSocketJoint);
            joints.Add(torsoTopToLeftArmEllipseLimit);
            joints.Add(torsoTopToLeftArmTwistLimit);
            joints.Add(torsoTopToLeftArmMotor);

            joints.Add(leftUpperArmToLeftForearmSwivelHingeJoint);
            joints.Add(leftUpperArmToLeftForearmMotor);

            joints.Add(leftForearmToLeftHandBallSocketJoint);
            joints.Add(leftForearmToLeftHandEllipseSwingLimit);
            joints.Add(leftForearmToLeftHandTwistLimit);
            joints.Add(leftForearmToLeftHandMotor);

            joints.Add(torsoTopToRightArmBallSocketJoint);
            joints.Add(torsoTopToRightArmEllipseLimit);
            joints.Add(torsoTopToRightArmTwistLimit);
            joints.Add(torsoTopToRightArmMotor);

            joints.Add(rightUpperArmToRightForearmSwivelHingeJoint);
            joints.Add(rightUpperArmToRightForearmMotor);

            joints.Add(rightForearmToRightHandBallSocketJoint);
            joints.Add(rightForearmToRightHandEllipseSwingLimit);
            joints.Add(rightForearmToRightHandTwistLimit);
            joints.Add(rightForearmToRightHandMotor);

            joints.Add(pelvisToLeftThighBallSocketJoint);
            joints.Add(pelvisToLeftThighEllipseSwingLimit);
            joints.Add(pelvisToLeftThighTwistLimit);
            joints.Add(pelvisToLeftThighMotor);

            joints.Add(leftThighToLeftShinRevoluteJoint);

            joints.Add(leftShinToLeftFootBallSocketJoint);
            joints.Add(leftShinToLeftFootSwingLimit);
            joints.Add(leftShinToLeftFootTwistLimit);
            joints.Add(leftShinToLeftFootMotor);

            joints.Add(pelvisToRightThighBallSocketJoint);
            joints.Add(pelvisToRightThighEllipseSwingLimit);
            joints.Add(pelvisToRightThighTwistLimit);
            joints.Add(pelvisToRightThighMotor);

            joints.Add(rightThighToRightShinRevoluteJoint);

            joints.Add(rightShinToRightFootBallSocketJoint);
            joints.Add(rightShinToRightFootSwingLimit);
            joints.Add(rightShinToRightFootTwistLimit);
            joints.Add(rightShinToRightFootMotor);
            #endregion


        }
    }
}