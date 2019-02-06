using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Paths;
using BEPUphysics.Paths.PathFollowing;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.CollisionShapes;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A playground for the character controller to frolic in.
    /// </summary>
    public class CharacterPlaygroundDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public CharacterPlaygroundDemo(DemosGame game)
            : base(game)
        {

            game.Camera.Position = new Vector3((-10).ToFix(), 7.ToFix(), 5.ToFix());
            game.Camera.ViewDirection = new Vector3(0.ToFix(), 0.ToFix(), 1.ToFix());
            //Since this is the character playground, turn on the character by default.
            character.Activate();
            //Having the character body visible would be a bit distracting.
            character.CharacterController.Body.Tag = "noDisplayObject";

            //Load in mesh data for the environment.
            Vector3[] staticTriangleVertices;
            int[] staticTriangleIndices;


            var playgroundModel = game.Content.Load<Model>("CharacterControllerTestTerrain");
            //This is a little convenience method used to extract vertices and indices from a model.
            //It doesn't do anything special; any approach that gets valid vertices and indices will work.
            ModelDataExtractor.GetVerticesAndIndicesFromModel(playgroundModel, out staticTriangleVertices, out staticTriangleIndices);
            var staticMesh = new StaticMesh(staticTriangleVertices, staticTriangleIndices, new AffineTransform(new Vector3(0.01m.ToFix(), 0.01m.ToFix(), 0.01m.ToFix()), Quaternion.Identity, new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix())));
            staticMesh.Sidedness = TriangleSidedness.Counterclockwise;

            Space.Add(staticMesh);
            game.ModelDrawer.Add(staticMesh);



            //Add a spinning blade for the character to ram itself into.
            var fanBase = new Cylinder(new Vector3((-13).ToFix(), .5m.ToFix(), 50.ToFix()), 1.1m.ToFix(), 1.ToFix());
            var fanBlade = new Box(fanBase.Position + new Vector3(0.ToFix(), .8m.ToFix(), 0.ToFix()), 5.ToFix(), .1m.ToFix(), 1m.ToFix(), 5.ToFix());
            var fanJoint = new RevoluteJoint(fanBase, fanBlade, (fanBase.Position + fanBlade.Position) * .5m.ToFix(), Vector3.Up);
            fanJoint.Motor.IsActive = true;
            fanJoint.Motor.Settings.VelocityMotor.GoalVelocity = 30.ToFix();
            fanJoint.Motor.Settings.MaximumForce = 300.ToFix();
            Space.Add(fanBase);
            Space.Add(fanBlade);
            Space.Add(fanJoint);

            //Add a bridge connecting the two towers.
            Vector3 startPosition = new Vector3((-19.3m).ToFix(), (10.5m - .25m).ToFix(), (23 - .85m).ToFix());
            var startPlatform = new Box(startPosition - new Vector3(0.ToFix(), 0.ToFix(), 2.2m.ToFix()), 4.ToFix(), .5m.ToFix(), 6.ToFix());
            Space.Add(startPlatform);
            Vector3 offset = new Vector3(0.ToFix(), 0.ToFix(), 1.7m.ToFix());
            Box previousLink = startPlatform;
            Vector3 position = new Vector3();
            for (int i = 1; i <= 7; i++)
            {
                position = startPosition + offset * i.ToFix();
                Box link = new Box(position, 3.ToFix(), .3m.ToFix(), 1.5m.ToFix(), 50.ToFix());
                Space.Add(link);
                Space.Add(new RevoluteJoint(previousLink, link, position - offset * .5m.ToFix(), Vector3.Right));

                previousLink = link;
            }
            var endPlatform = new Box(position - new Vector3(0.ToFix(), 0.ToFix(), (-3.8m).ToFix()), 4.ToFix(), .5m.ToFix(), 6.ToFix());
            Space.Add(endPlatform);

            Space.Add(new RevoluteJoint(previousLink, endPlatform, position + offset * .5m.ToFix(), Vector3.Right));


            //Add in a floating platform controlled by a curve to serve as an elevator.
            Entity movingEntity = new Box(new Vector3((-10).ToFix(), 0.ToFix(), (-10).ToFix()), 3.ToFix(), 1.ToFix(), 3.ToFix());

            var positionCurve = new CardinalSpline3D();

            positionCurve.PreLoop = CurveEndpointBehavior.Mirror;
            positionCurve.PostLoop = CurveEndpointBehavior.Mirror;

            positionCurve.ControlPoints.Add((-1).ToFix(), new Vector3((-19.3m).ToFix(), 0.ToFix(), 43.ToFix()));
            positionCurve.ControlPoints.Add(0.ToFix(), new Vector3((-19.3m).ToFix(), 0.ToFix(), 43.ToFix()));
            positionCurve.ControlPoints.Add(2.ToFix(), new Vector3((-19.3m).ToFix(), 0.ToFix(), 43.ToFix()));
            positionCurve.ControlPoints.Add(3.ToFix(), new Vector3((-19.3m).ToFix(), 0.ToFix(), 43.ToFix()));
            positionCurve.ControlPoints.Add(4.ToFix(), new Vector3((-19.3m).ToFix(), 5.ToFix(), 43.ToFix()));
            positionCurve.ControlPoints.Add(5.ToFix(), new Vector3((-19.3m).ToFix(), 10.ToFix(), 43.ToFix()));
            positionCurve.ControlPoints.Add(6.ToFix(), new Vector3((-19.3m).ToFix(), 10.ToFix(), 43.ToFix()));
            positionCurve.ControlPoints.Add(8.ToFix(), new Vector3((-19.3m).ToFix(), 10.ToFix(), 43.ToFix()));
            positionCurve.ControlPoints.Add(9.ToFix(), new Vector3((-19.3m).ToFix(), 10.ToFix(), 43.ToFix()));

            elevatorMover = new EntityMover(movingEntity);
            Space.Add(elevatorMover);
            Space.Add(movingEntity);

            elevatorPath = positionCurve;

            //Add in another floating platform controlled by a curve for horizontal transport.
            movingEntity = new Box(new Vector3((-10).ToFix(), 0.ToFix(), (-10).ToFix()), 2.5m.ToFix(), .5m.ToFix(), 2.5m.ToFix());

            var platformCurve = new LinearInterpolationCurve3D();

            platformCurve.PreLoop = CurveEndpointBehavior.Mirror;
            platformCurve.PostLoop = CurveEndpointBehavior.Mirror;

            platformCurve.ControlPoints.Add(0.ToFix(), new Vector3((-1.75m).ToFix(), 10.ToFix(), 21.5m.ToFix()));
            platformCurve.ControlPoints.Add(2.ToFix(), new Vector3((-1.75m).ToFix(), 10.ToFix(), 21.5m.ToFix()));
            platformCurve.ControlPoints.Add(5.ToFix(), new Vector3((-1.75m).ToFix(), 10.ToFix(), 15.5m.ToFix()));
            platformCurve.ControlPoints.Add(10.ToFix(), new Vector3((-19.3m).ToFix(), 10.ToFix(), 15.5m.ToFix()));
            platformCurve.ControlPoints.Add(12.ToFix(), new Vector3((-19.3m).ToFix(), 10.ToFix(), 15.5m.ToFix()));
            platformCurve.ControlPoints.Add(15.ToFix(), new Vector3((-25).ToFix(), 10.ToFix(), 15.5m.ToFix()));
            platformCurve.ControlPoints.Add(22.ToFix(), new Vector3((-25).ToFix(), 10.ToFix(), 38.ToFix()));
            platformCurve.ControlPoints.Add(23.ToFix(), new Vector3((-22.75m).ToFix(), 10.ToFix(), 38.ToFix()));
            platformCurve.ControlPoints.Add(25.ToFix(), new Vector3((-22.75m).ToFix(), 10.ToFix(), 38.ToFix()));

            //Make it spin too.  That'll be fun.  Or something.
            var platformRotationCurve = new QuaternionSlerpCurve();
            platformRotationCurve.PreLoop = CurveEndpointBehavior.Mirror;
            platformRotationCurve.PostLoop = CurveEndpointBehavior.Mirror;
            platformRotationCurve.ControlPoints.Add(0.ToFix(), Quaternion.Identity);
            platformRotationCurve.ControlPoints.Add(15.ToFix(), Quaternion.Identity);
            platformRotationCurve.ControlPoints.Add(22.ToFix(), Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2));
            platformRotationCurve.ControlPoints.Add(25.ToFix(), Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2));

            platformMover = new EntityMover(movingEntity);
            platformRotator = new EntityRotator(movingEntity);
            Space.Add(platformMover);
            Space.Add(platformRotator);
            Space.Add(movingEntity);

            platformPath = platformCurve;
            platformOrientationPath = platformRotationCurve;

            //Add in a diving board.

            Box divingBoardBase = new Box(new Vector3((-9).ToFix(), 10.ToFix(), 39.3m.ToFix()), 5.ToFix(), 1.ToFix(), 3.ToFix());
            Box divingBoard = new Box(divingBoardBase.Position + new Vector3((-2).ToFix(), 0.ToFix(), 3.5m.ToFix()), 1.ToFix(), .3m.ToFix(), 3.ToFix(), 5.ToFix());
            var divingBoardJoint = new RevoluteJoint(divingBoardBase, divingBoard, divingBoard.Position + new Vector3(0.ToFix(), 0.ToFix(), (-1.5m).ToFix()), Vector3.Right);
            divingBoardJoint.Motor.IsActive = true;
            divingBoardJoint.Motor.Settings.Mode = MotorMode.Servomechanism;
            divingBoardJoint.Motor.Settings.Servo.Goal = 0.ToFix();
            divingBoardJoint.Motor.Settings.Servo.SpringSettings.Stiffness = 5000.ToFix();
            divingBoardJoint.Motor.Settings.Servo.SpringSettings.Damping = 0.ToFix();

            Space.Add(divingBoardBase);
            Space.Add(divingBoard);
            Space.Add(divingBoardJoint);


            //Add a second diving board for comparison.

            Box divingBoard2 = new Box(divingBoardBase.Position + new Vector3(2.ToFix(), 0.ToFix(), 5m.ToFix()), 1.ToFix(), .3m.ToFix(), 6.ToFix(), 5.ToFix());
            var divingBoardJoint2 = new RevoluteJoint(divingBoardBase, divingBoard2, divingBoard2.Position + new Vector3(0.ToFix(), 0.ToFix(), (-3).ToFix()), Vector3.Right);
            divingBoardJoint2.Motor.IsActive = true;
            divingBoardJoint2.Motor.Settings.Mode = MotorMode.Servomechanism;
            divingBoardJoint2.Motor.Settings.Servo.Goal = 0.ToFix();
            divingBoardJoint2.Motor.Settings.Servo.SpringSettings.Stiffness = 10000.ToFix();
            divingBoardJoint2.Motor.Settings.Servo.SpringSettings.Damping = 0.ToFix();

            Space.Add(divingBoard2);
            Space.Add(divingBoardJoint2);

            //Add a seesaw for people to jump on.
            Box seesawBase = new Box(new Vector3((-7).ToFix(), .45m.ToFix(), 52.ToFix()), 1.ToFix(), .9m.ToFix(), .3m.ToFix());
            Box seesawPlank = new Box(seesawBase.Position + new Vector3(0.ToFix(), .65m.ToFix(), 0.ToFix()), 1.2m.ToFix(), .2m.ToFix(), 6.ToFix(), 3.ToFix());
            RevoluteJoint seesawJoint = new RevoluteJoint(seesawBase, seesawPlank, seesawPlank.Position, Vector3.Right);
            Space.Add(seesawJoint);
            Space.Add(seesawBase);
            Space.Add(seesawPlank);

            Space.Add(new Box(seesawPlank.Position + new Vector3(0.ToFix(), 1.3m.ToFix(), 2.ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix(), 5.ToFix()));


            //Add in some boxes to bump and jump on.
            int numColumns = 3;
            int numRows = 3;
            int numHigh = 3;
            Fix64 xSpacing = 1.01m.ToFix();
			Fix64 ySpacing = 1.01m.ToFix();
			Fix64 zSpacing = 1.01m.ToFix();
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        Space.Add(new Box(new Vector3(
(5.ToFix().Add(xSpacing.Mul(i.ToFix()))).Sub((((numRows - 1).ToFix()).Mul(xSpacing)).Div(2m.ToFix())),
1.58m.ToFix().Add(k.ToFix().Mul((ySpacing))),
(45.ToFix().Add(zSpacing.Mul(j.ToFix()))).Sub((((numColumns - 1).ToFix()).Mul(zSpacing)).Div(2m.ToFix()))),
.5m.ToFix(), .5m.ToFix(), .5m.ToFix(), 5.ToFix()));
                    }



            //Add a log to roll!
            //Make it a compound so some boxes can be added to let the player know it's actually spinning.
            CompoundBody log = new CompoundBody(new List<CompoundShapeEntry>()
            {
                new CompoundShapeEntry(new CylinderShape(4.ToFix(), 1.8m.ToFix()), Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2), 20.ToFix()),
                new CompoundShapeEntry(new BoxShape(.5m.ToFix(), .5m.ToFix(), 3.7m.ToFix()),  new Vector3(1.75m.ToFix(), 0.ToFix(),0.ToFix()), 0.ToFix()),
                new CompoundShapeEntry(new BoxShape(.5m.ToFix(), 3.7m.ToFix(), .5m.ToFix()), new Vector3(1.75m.ToFix(), 0.ToFix(),0.ToFix()), 0.ToFix()),
                new CompoundShapeEntry(new BoxShape(.5m.ToFix(), .5m.ToFix(), 3.7m.ToFix()),  new Vector3((-1.75m).ToFix(), 0.ToFix(),0.ToFix()), 0.ToFix()),
                new CompoundShapeEntry(new BoxShape(.5m.ToFix(), 3.7m.ToFix(), .5m.ToFix()), new Vector3((-1.75m).ToFix(), 0.ToFix(),0.ToFix()), 0.ToFix())
            }, 50.ToFix());
            log.Position = new Vector3((-14.5m).ToFix(), 10.ToFix(), 41.ToFix());
            log.AngularDamping = 0.ToFix();


            RevoluteJoint logJointA = new RevoluteJoint(divingBoardBase, log, log.Position + new Vector3(2.5m.ToFix(), 0.ToFix(), 0.ToFix()), Vector3.Right);
            RevoluteJoint logJointB = new RevoluteJoint(endPlatform, log, log.Position + new Vector3((-2.5m).ToFix(), 0.ToFix(), 0.ToFix()), Vector3.Right);
            Space.Add(logJointA);
            Space.Add(logJointB);

            Space.Add(log);


            //Put some planks to stand on that show various slopes.
            int numPads = 10;
            for (int i = 0; i < numPads; i++)
            {
                offset = new Vector3(0.ToFix(), 0.ToFix(), 4.ToFix());
                Box a = new Box(new Vector3((i * 1.5m + 3.5m).ToFix(), 10.ToFix(), 24.ToFix()), 1.5m.ToFix(), 1.ToFix(), 4.ToFix());
                Box b = new Box(new Vector3((i * 1.5m + 3.5m).ToFix(), 10.ToFix(), 24.ToFix()), 1.5m.ToFix(), 1.ToFix(), 4.ToFix());
                Fix64 angle = ((-i).ToFix().Mul(MathHelper.PiOver2)).Div(numPads.ToFix());
                b.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Right, angle);
                b.Position += offset * .5m.ToFix() + Quaternion.Transform(offset * .5m.ToFix(), b.Orientation);

                Space.Add(a);
                Space.Add(b);
            }

        }

        EntityMover elevatorMover;
        Path<Vector3> elevatorPath;
        EntityMover platformMover;
        EntityRotator platformRotator;
        Path<Vector3> platformPath;
        Path<Quaternion> platformOrientationPath;
        Fix64 pathTime;


        public override void Update(Fix64 dt)
        {
			//Increment the time.  Note that the space's timestep is used
			//instead of the method's dt.  This is because the demos, by
			//default, update the space once each game update.  Using the
			//space's update time keeps things synchronized.
			//If the engine is using internal time stepping,
			//the passed in dt should be used instead (or put this logic into
			//an updateable that runs with space updates).
			pathTime = pathTime.Add(Space.TimeStepSettings.TimeStepDuration);
            elevatorMover.TargetPosition = elevatorPath.Evaluate(pathTime);
            platformMover.TargetPosition = platformPath.Evaluate(pathTime);
            platformRotator.TargetOrientation = platformOrientationPath.Evaluate(pathTime);
            base.Update(dt);
        }

        public override void DrawUI()
        {
#if XBOX360
            Game.DataTextDrawer.Draw("Press \"A\" to toggle the character.", new Microsoft.Xna.Framework.(50, 50));
#else
            Game.DataTextDrawer.Draw("Press \"C\" to toggle the character.", new Microsoft.Xna.Framework.Vector2(50, 50));
#endif
            base.DrawUI();
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Character Playground"; }
        }
    }
}