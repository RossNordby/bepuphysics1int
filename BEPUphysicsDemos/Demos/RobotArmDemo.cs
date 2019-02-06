using System;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework.Input;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Materials;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using System.Collections.Generic;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A clawbot spins, swivels, bends, and grabs.
    /// </summary>
    public class RobotArmDemo :
#if !WINDOWS
    //There's not enough buttons on the Xbox controller to have vehicles/characters and this demo's controls.
        Demo
#else
 StandardDemo
#endif
    {
        private readonly RevoluteJoint groundToBaseJoint;
        private readonly RevoluteJoint shoulder;
        private readonly SwivelHingeJoint elbow;
        private readonly RevoluteJoint clawHingeA;
        private readonly RevoluteJoint clawHingeB;

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Robotic Arm Thingamajig"; }
        }

        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public RobotArmDemo(DemosGame game)
            : base(game)
        {
            //Since this is not a "StandardDemo" derived class, we need to set our own gravity.
            Space.ForceUpdater.Gravity = new Vector3(0.ToFix(), (-9.81m).ToFix(), 0.ToFix());
            Entity ground = new Box(Vector3.Zero, 30.ToFix(), 1.ToFix(), 30.ToFix());
            Space.Add(ground);

            var armBase = new Cylinder(new Vector3(0.ToFix(), 2.ToFix(), 0.ToFix()), 2.5m.ToFix(), 1.ToFix(), 40.ToFix());
            Space.Add(armBase);

            //The arm base can rotate around the Y axis.
            //Rotation is controlled by user input.
            groundToBaseJoint = new RevoluteJoint(ground, armBase, Vector3.Zero, Vector3.Up);
            groundToBaseJoint.Motor.IsActive = true;
            groundToBaseJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
            groundToBaseJoint.Motor.Settings.MaximumForce = 3500.ToFix();
            Space.Add(groundToBaseJoint);

            Entity lowerArm = new Box(armBase.Position + new Vector3(0.ToFix(), 2.ToFix(), 0.ToFix()), 1.ToFix(), 3.ToFix(), .5m.ToFix(), 10.ToFix());
            Space.Add(lowerArm);

            shoulder = new RevoluteJoint(armBase, lowerArm, armBase.Position, Vector3.Forward);
            shoulder.Motor.IsActive = true;
            shoulder.Motor.Settings.Mode = MotorMode.Servomechanism;
            shoulder.Motor.Settings.MaximumForce = 2500.ToFix();

            //Don't want it to rotate too far; this keeps the whole contraption off the ground.
            shoulder.Limit.IsActive = true;
            shoulder.Limit.MinimumAngle = MathHelper.PiOver4.Neg();
            shoulder.Limit.MaximumAngle = MathHelper.PiOver4;
            Space.Add(shoulder);

            //Make the next piece of the arm.
            Entity upperArm = new Cylinder(lowerArm.Position + new Vector3(0.ToFix(), 3.ToFix(), 0.ToFix()), 3.ToFix(), .25m.ToFix(), 10.ToFix());
            Space.Add(upperArm);

            //Swivel hinges allow motion around two axes.  Imagine a tablet PC's monitor hinge.
            elbow = new SwivelHingeJoint(lowerArm, upperArm, lowerArm.Position + new Vector3(0.ToFix(), 1.5m.ToFix(), 0.ToFix()), Vector3.Forward);
            elbow.TwistMotor.IsActive = true;
            elbow.TwistMotor.Settings.Mode = MotorMode.Servomechanism;
            elbow.TwistMotor.Settings.MaximumForce = 1000.ToFix();

            elbow.HingeMotor.IsActive = true;
            elbow.HingeMotor.Settings.Mode = MotorMode.Servomechanism;
            elbow.HingeMotor.Settings.MaximumForce = 1250.ToFix();

            //Keep it from rotating too much.
            elbow.HingeLimit.IsActive = true;
            elbow.HingeLimit.MinimumAngle = MathHelper.PiOver2.Neg();
            elbow.HingeLimit.MaximumAngle = MathHelper.PiOver2;
            Space.Add(elbow);


            //Add a menacing claw at the end.
            var lowerPosition = upperArm.Position + new Vector3((-.65m).ToFix(), 1.6m.ToFix(), 0.ToFix());

            CollisionRules clawPart1ARules = new CollisionRules();
            var bodies = new List<CompoundChildData>()
            {
                new CompoundChildData() { Entry = new CompoundShapeEntry(new BoxShape(1.ToFix(), .25m.ToFix(), .25m.ToFix()), lowerPosition, 3.ToFix()), CollisionRules = clawPart1ARules },
                new CompoundChildData() { Entry = new CompoundShapeEntry(new ConeShape(1.ToFix(), .125m.ToFix()), lowerPosition + new Vector3((-.375m).ToFix(), .4m.ToFix(), 0.ToFix()), 3.ToFix()), Material = new Material(2.ToFix(),2.ToFix(),0.ToFix()) }
            };

            var claw = new CompoundBody(bodies, 6.ToFix());
            Space.Add(claw);

            clawHingeA = new RevoluteJoint(upperArm, claw, upperArm.Position + new Vector3(0.ToFix(), 1.5m.ToFix(), 0.ToFix()), Vector3.Forward);
            clawHingeA.Motor.IsActive = true;
            clawHingeA.Motor.Settings.Mode = MotorMode.Servomechanism;
            clawHingeA.Motor.Settings.Servo.Goal = MathHelper.PiOver2.Neg();
            //Weaken the claw to prevent it from crushing the boxes.
            clawHingeA.Motor.Settings.Servo.SpringSettings.Damping /= 100.ToFix();
            clawHingeA.Motor.Settings.Servo.SpringSettings.Stiffness /= 100.ToFix();

            clawHingeA.Limit.IsActive = true;
            clawHingeA.Limit.MinimumAngle = MathHelper.PiOver2.Neg();
            clawHingeA.Limit.MaximumAngle = MathHelper.Pi.Neg().Div(6.ToFix());
            Space.Add(clawHingeA);

            //Add one more claw to complete the contraption.
            lowerPosition = upperArm.Position + new Vector3(.65m.ToFix(), 1.6m.ToFix(), 0.ToFix());

            CollisionRules clawPart1BRules = new CollisionRules();
            bodies = new List<CompoundChildData>()
            {
                new CompoundChildData() { Entry = new CompoundShapeEntry(new BoxShape(1.ToFix(), .25m.ToFix(), .25m.ToFix()), lowerPosition, 3.ToFix()), CollisionRules = clawPart1BRules },
                new CompoundChildData() { Entry = new CompoundShapeEntry(new ConeShape(1.ToFix(), .125m.ToFix()), lowerPosition + new Vector3(.375m.ToFix(), .4m.ToFix(), 0.ToFix()), 3.ToFix()), Material = new Material(2.ToFix(),2.ToFix(),0.ToFix()) }
            };
            claw = new CompoundBody(bodies, 6.ToFix());
            Space.Add(claw);

            clawHingeB = new RevoluteJoint(upperArm, claw, upperArm.Position + new Vector3(0.ToFix(), 1.5m.ToFix(), 0.ToFix()), Vector3.Forward);
            clawHingeB.Motor.IsActive = true;
            clawHingeB.Motor.Settings.Mode = MotorMode.Servomechanism;
            clawHingeB.Motor.Settings.Servo.Goal = MathHelper.PiOver2;
            //Weaken the claw to prevent it from crushing the boxes.
            clawHingeB.Motor.Settings.Servo.SpringSettings.Damping /= 100.ToFix();
            clawHingeB.Motor.Settings.Servo.SpringSettings.Stiffness /= 100.ToFix();

            clawHingeB.Limit.IsActive = true;
            clawHingeB.Limit.MinimumAngle = MathHelper.Pi.Div(6.ToFix());
            clawHingeB.Limit.MaximumAngle = MathHelper.PiOver2;
            Space.Add(clawHingeB);

            //Keep the pieces of the robot from interacting with each other.
            //The CollisionRules.AddRule method is just a convenience method that adds items to the 'specific' dictionary.
            //Sometimes, it's a little unwieldy to reference the collision rules,
            //so the convenience method just takes the owners and hides the ugly syntax.
            CollisionRules.AddRule(armBase, lowerArm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(lowerArm, upperArm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(clawPart1ARules, upperArm, CollisionRule.NoBroadPhase);
            CollisionRules.AddRule(clawPart1BRules, upperArm, CollisionRule.NoBroadPhase);
            //Here's an example without a convenience method.  Since they are both direct CollisionRules references, it's pretty clean.
            clawPart1BRules.Specific.Add(clawPart1ARules, CollisionRule.NoBroadPhase);


            //Put some boxes on the ground to try to pick up.
            for (Fix64 k = 0.ToFix(); k < MathHelper.Pi.Mul(2.ToFix()); k += MathHelper.Pi.Div(6.ToFix()))
            {
                Space.Add(new Box(new Vector3(Fix64.Cos(k).Mul(5.5m.ToFix()), 2.ToFix(), Fix64.Sin(k).Mul(5.5m.ToFix())), 1.ToFix(), 1.ToFix(), 1.ToFix(), 10.ToFix()));
            }

            game.Camera.Position = new Vector3(0.ToFix(), 5.ToFix(), 13.ToFix());
        }


        public override void Update(Fix64 dt)
        {
#if !WINDOWS
            if (Game.GamePadInput.IsButtonDown(Buttons.LeftShoulder))
                groundToBaseJoint.Motor.Settings.Servo.Goal -= 1 * dt;
            if (Game.GamePadInput.IsButtonDown(Buttons.RightShoulder))
                groundToBaseJoint.Motor.Settings.Servo.Goal += 1 * dt;

            if (Game.GamePadInput.IsButtonDown(Buttons.A))
                shoulder.Motor.Settings.Servo.Goal = MathHelper.Min(shoulder.Motor.Settings.Servo.Goal + .5m * dt, shoulder.Limit.MaximumAngle);
            if (Game.GamePadInput.IsButtonDown(Buttons.B))
                shoulder.Motor.Settings.Servo.Goal = MathHelper.Max(shoulder.Motor.Settings.Servo.Goal - .5m * dt, shoulder.Limit.MinimumAngle);

            if (Game.GamePadInput.IsButtonDown(Buttons.X))
                elbow.HingeMotor.Settings.Servo.Goal = MathHelper.Min(elbow.HingeMotor.Settings.Servo.Goal + 1 * dt, elbow.HingeLimit.MaximumAngle);
            if (Game.GamePadInput.IsButtonDown(Buttons.Y))
                elbow.HingeMotor.Settings.Servo.Goal = MathHelper.Max(elbow.HingeMotor.Settings.Servo.Goal - 1 * dt, elbow.HingeLimit.MinimumAngle);

            if (Game.GamePadInput.IsButtonDown(Buttons.DPadUp))
                elbow.TwistMotor.Settings.Servo.Goal += 1f * dt;
            if (Game.GamePadInput.IsButtonDown(Buttons.DPadDown))
                elbow.TwistMotor.Settings.Servo.Goal -= 1f * dt;

            if (Game.GamePadInput.IsButtonDown(Buttons.LeftTrigger))
            {
                clawHingeA.Motor.Settings.Servo.Goal = MathHelper.Max(clawHingeA.Motor.Settings.Servo.Goal - Game.GamePadInput.Triggers.Left * 1.5m * dt, clawHingeA.Limit.MinimumAngle);
                clawHingeB.Motor.Settings.Servo.Goal = MathHelper.Min(clawHingeB.Motor.Settings.Servo.Goal + Game.GamePadInput.Triggers.Left * 1.5m * dt, clawHingeB.Limit.MaximumAngle);
            }
            if (Game.GamePadInput.IsButtonDown(Buttons.RightTrigger))
            {
                clawHingeA.Motor.Settings.Servo.Goal = MathHelper.Min(clawHingeA.Motor.Settings.Servo.Goal + Game.GamePadInput.Triggers.Right * 1.5m * dt, clawHingeA.Limit.MaximumAngle);
                clawHingeB.Motor.Settings.Servo.Goal = MathHelper.Max(clawHingeB.Motor.Settings.Servo.Goal - Game.GamePadInput.Triggers.Right * 1.5m * dt, clawHingeB.Limit.MinimumAngle);
            }

#else
            if (Game.KeyboardInput.IsKeyDown(Keys.N))
                groundToBaseJoint.Motor.Settings.Servo.Goal -= 1.ToFix().Mul(dt);
            if (Game.KeyboardInput.IsKeyDown(Keys.M))
                groundToBaseJoint.Motor.Settings.Servo.Goal += 1.ToFix().Mul(dt);

            if (Game.KeyboardInput.IsKeyDown(Keys.Q))
                shoulder.Motor.Settings.Servo.Goal = MathHelper.Min(shoulder.Motor.Settings.Servo.Goal.Add(.5m.ToFix().Mul(dt)), shoulder.Limit.MaximumAngle);
            if (Game.KeyboardInput.IsKeyDown(Keys.W))
                shoulder.Motor.Settings.Servo.Goal = MathHelper.Max(shoulder.Motor.Settings.Servo.Goal.Sub(.5m.ToFix().Mul(dt)), shoulder.Limit.MinimumAngle);

            if (Game.KeyboardInput.IsKeyDown(Keys.R))
                elbow.HingeMotor.Settings.Servo.Goal = MathHelper.Min(elbow.HingeMotor.Settings.Servo.Goal.Add(1.ToFix().Mul(dt)), elbow.HingeLimit.MaximumAngle);
            if (Game.KeyboardInput.IsKeyDown(Keys.T))
                elbow.HingeMotor.Settings.Servo.Goal = MathHelper.Max(elbow.HingeMotor.Settings.Servo.Goal.Sub(1.ToFix().Mul(dt)), elbow.HingeLimit.MinimumAngle);

            if (Game.KeyboardInput.IsKeyDown(Keys.O))
                elbow.TwistMotor.Settings.Servo.Goal += 1.ToFix().Mul(dt);
            if (Game.KeyboardInput.IsKeyDown(Keys.P))
                elbow.TwistMotor.Settings.Servo.Goal -= 1.ToFix().Mul(dt);

            if (Game.KeyboardInput.IsKeyDown(Keys.OemOpenBrackets))
            {
                clawHingeA.Motor.Settings.Servo.Goal = MathHelper.Max(clawHingeA.Motor.Settings.Servo.Goal.Sub(1.5m.ToFix().Mul(dt)), clawHingeA.Limit.MinimumAngle);
                clawHingeB.Motor.Settings.Servo.Goal = MathHelper.Min(clawHingeB.Motor.Settings.Servo.Goal.Add(1.5m.ToFix().Mul(dt)), clawHingeB.Limit.MaximumAngle);
            }
            if (Game.KeyboardInput.IsKeyDown(Keys.OemCloseBrackets))
            {
                clawHingeA.Motor.Settings.Servo.Goal = MathHelper.Min(clawHingeA.Motor.Settings.Servo.Goal.Add(1.5m.ToFix().Mul(dt)), clawHingeA.Limit.MaximumAngle);
                clawHingeB.Motor.Settings.Servo.Goal = MathHelper.Max(clawHingeB.Motor.Settings.Servo.Goal.Sub(1.5m.ToFix().Mul(dt)), clawHingeB.Limit.MinimumAngle);
            }

#endif
            base.Update(dt);
        }

        public override void DrawUI()
        {
            base.DrawUI();
            Game.DataTextDrawer.Draw("Arm controls:", new Microsoft.Xna.Framework.Vector2(50, 20));
#if !WINDOWS
            Game.TinyTextDrawer.Draw("Spin base: Left/Right Shoulder", new Vector2(50, 38));
            Game.TinyTextDrawer.Draw("Bend shoulder: A B", new Vector2(50, 53));
            Game.TinyTextDrawer.Draw("Bend elbow: X Y", new Vector2(50, 68));
            Game.TinyTextDrawer.Draw("Spin forearm: Up/Down Dpad", new Vector2(50, 83));
            Game.TinyTextDrawer.Draw("Open/close claw: Left/Right Trigger", new Vector2(50, 98));
#else
            Game.TinyTextDrawer.Draw("Spin base: N M", new Microsoft.Xna.Framework.Vector2(50, 38));
            Game.TinyTextDrawer.Draw("Bend shoulder: Q W", new Microsoft.Xna.Framework.Vector2(50, 53));
            Game.TinyTextDrawer.Draw("Bend elbow: R T", new Microsoft.Xna.Framework.Vector2(50, 68));
            Game.TinyTextDrawer.Draw("Spin forearm: O P", new Microsoft.Xna.Framework.Vector2(50, 83));
            Game.TinyTextDrawer.Draw("Open/close claw: [ ]", new Microsoft.Xna.Framework.Vector2(50, 98));
#endif
        }
    }
}