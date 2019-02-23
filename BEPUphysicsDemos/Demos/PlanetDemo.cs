using System;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.UpdateableSystems.ForceFields;
using BEPUphysicsDemos.SampleCode;
using BEPUphysics.NarrowPhaseSystems;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Boxes fall on a planetoid.
    /// </summary>
    public class PlanetDemo : StandardDemo
    {
        private Vector3 planetPosition;
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public PlanetDemo(DemosGame game)
            : base(game)
        {
            Space.ForceUpdater.Gravity = Vector3.Zero;

            //By pre-allocating a bunch of box-box pair handlers, the simulation will avoid having to allocate new ones at runtime.
            NarrowPhaseHelper.Factories.BoxBox.EnsureCount(1000);

            planetPosition = new Vector3(0.ToFix(), 0.ToFix(), 0.ToFix());
            var planet = new Sphere(planetPosition, 30.ToFix());
            Space.Add(planet);

            var field = new GravitationalField(new InfiniteForceFieldShape(), planet.Position, (66730 / 2).ToFix(), 100.ToFix());
            Space.Add(field);

            //Drop the "meteorites" on the planet.
            Entity toAdd;
            int numColumns = 10;
            int numRows = 10;
            int numHigh = 10;
            Fix separation = 5.ToFix();
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numColumns; j++)
                    for (int k = 0; k < numHigh; k++)
                    {
                        toAdd = new Box(new Vector3((separation.Mul(i.ToFix())).Sub((numRows.ToFix().Mul(separation)).Div(2.ToFix())), 40.ToFix().Add(k.ToFix().Mul(separation)), (separation.Mul(j.ToFix())).Sub((numColumns.ToFix().Mul(separation)).Div(2.ToFix()))), 1.ToFix(), 1.ToFix(), 1.ToFix(), 5.ToFix());
                        toAdd.LinearVelocity = new Vector3(30.ToFix(), 0.ToFix(), 0.ToFix());
                        toAdd.LinearDamping = 0.ToFix();
                        toAdd.AngularDamping = 0.ToFix();
                        Space.Add(toAdd);
                    }
            game.Camera.Position = new Vector3(0.ToFix(), 0.ToFix(), 150.ToFix());



        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Planet"; }
        }

        public override void Update(Fix dt)
        {
            //Orient the character and camera as needed.
            if (character.IsActive)
            {
                var down = planetPosition - character.CharacterController.Body.Position;
                character.CharacterController.Down = down;
                Game.Camera.LockedUp = -down;
            }
            else if (vehicle.IsActive)
            {
                Game.Camera.LockedUp = vehicle.Vehicle.Body.Position - planetPosition;
            }
            else
            {
                Game.Camera.LockedUp = Vector3.Up;
            }

            base.Update(dt);
        }


    }
}