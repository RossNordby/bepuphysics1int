
using System;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysicsDemos.SampleCode;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using System.Collections.Generic;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// A spaceship blasts off into the sky (void).
    /// </summary>
    public class SpaceshipDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public SpaceshipDemo(DemosGame game)
            : base(game)
        {
            //Build the ship
            var shipFuselage = new CompoundShapeEntry(new CylinderShape(3.ToFix(), .7m.ToFix()), new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 4.ToFix());
            var shipNose = new CompoundShapeEntry(new ConeShape(2.ToFix(), .7m.ToFix()), new Vector3(0.ToFix(), 7.ToFix(), 0.ToFix()), 2.ToFix());
            var shipWing = new CompoundShapeEntry(new BoxShape(5.ToFix(), 2.ToFix(), .2m.ToFix()), new Vector3(0.ToFix(), 5.ToFix(), 0.ToFix()), 3.ToFix());
            var shipThrusters = new CompoundShapeEntry(new ConeShape(1.ToFix(), .5m.ToFix()), new Vector3(0.ToFix(), 3.25m.ToFix(), 0.ToFix()), 1.ToFix());

            var bodies = new List<CompoundShapeEntry>();
            bodies.Add(shipFuselage);
            bodies.Add(shipNose);
            bodies.Add(shipWing);
            bodies.Add(shipThrusters);

            var ship = new CompoundBody(bodies, 10.ToFix());

            //Setup the launch pad and ramp
            Entity toAdd = new Box(new Vector3(10.ToFix(), 4.ToFix(), 0.ToFix()), 26.ToFix(), 1.ToFix(), 6.ToFix());
            Space.Add(toAdd);
            toAdd = new Box(new Vector3(32.ToFix(), 7.8m.ToFix(), 0.ToFix()), 20.ToFix(), 1.ToFix(), 6.ToFix());
            toAdd.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.Pi.Neg().Div(8.ToFix()));
            Space.Add(toAdd);
            toAdd = new Box(new Vector3(32.ToFix(), 8.8m.ToFix(), (-3.5m).ToFix()), 20.ToFix(), 1.ToFix(), 1.ToFix());
            toAdd.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.Pi.Neg().Div(8.ToFix()));
            Space.Add(toAdd);
            toAdd = new Box(new Vector3(32.ToFix(), 8.8m.ToFix(), 3.5m.ToFix()), 20.ToFix(), 1.ToFix(), 1.ToFix());
            toAdd.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Forward, (MathHelper.Pi.Neg()).Div(8.ToFix()));
            Space.Add(toAdd);
            toAdd = new Box(new Vector3((-2.75m).ToFix(), 5.5m.ToFix(), 0.ToFix()), .5m.ToFix(), 2.ToFix(), 3.ToFix());
            Space.Add(toAdd);

            //Blast-off!
            ship.AngularDamping = .4m.ToFix(); //Helps keep the rocket on track for a little while longer :D
            var thruster = new Thruster(ship, new Vector3(0.ToFix(), (-2).ToFix(), 0.ToFix()), new Vector3(0.ToFix(), 300.ToFix(), 0.ToFix()), 0.ToFix());
            Space.Add(thruster);
            ship.Orientation = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.Pi.Div(2.ToFix())) * Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.Pi.Div(2.ToFix()));
            Space.Add(ship);


            game.Camera.Position = new Vector3((-14).ToFix(), 12.ToFix(), 25.ToFix());
            game.Camera.Yaw(MathHelper.Pi.Div((-4).ToFix()));
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Spaceship"; }
        }
    }
}