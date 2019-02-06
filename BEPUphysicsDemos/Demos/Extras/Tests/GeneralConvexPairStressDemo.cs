using BEPUphysics.Entities;
using System;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUutilities;
using FixMath.NET;

namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Demo showing a wall of blocks stacked up.
    /// </summary>
    public class GeneralConvexPairStressDemo : StandardDemo
    {


        Random random = new Random();
        Fix64 width = 45.ToFix();
        Fix64 height = 45.ToFix();
        Fix64 length = 45.ToFix();
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public GeneralConvexPairStressDemo(DemosGame game)
            : base(game)
        {
            Space.Remove(vehicle.Vehicle);
            //Enable simplex caching.
            ConfigurationHelper.ApplySuperSpeedySettings(Space);

            for (int i = 0; i < 2000; i++)
            {
                EntityShape shape;
                switch (i % 3)
                {
                    case 0:
                        shape = new CylinderShape(0.5m.ToFix().Add(random.NextDouble().ToFix().Mul(1.5m.ToFix())), 0.5m.ToFix().Add(random.NextDouble().ToFix().Mul(1.5m.ToFix())));
                        break;
                    case 1:
                        shape = new ConeShape(0.5m.ToFix().Add(random.NextDouble().ToFix().Mul(1.5m.ToFix())), 0.5m.ToFix().Add(random.NextDouble().ToFix().Mul(1.5m.ToFix())));
                        break;
                    default:
                        shape = new CapsuleShape(0.5m.ToFix().Add(random.NextDouble().ToFix().Mul(1.5m.ToFix())), 0.5m.ToFix().Add(random.NextDouble().ToFix().Mul(1.5m.ToFix())));
                        break;

                }

                var toAdd = new Entity(shape, 2.ToFix());
                //toAdd.LocalInertiaTensorInverse = new BEPUutilities.Matrix3x3();
                RandomizeEntityState(toAdd);
                Space.Add(toAdd);

            }
            Space.ForceUpdater.Gravity = new Vector3();

            game.Camera.Position = new Vector3(0.ToFix(), 6.ToFix(), 15.ToFix());
        }

        private void RandomizeEntityState(Entity entity)
        {
            entity.Position = new Vector3(
(random.NextDouble().ToFix()).Mul(width),
(random.NextDouble().ToFix()).Mul(height),
(random.NextDouble().ToFix()).Mul(length));
            entity.Orientation = Quaternion.CreateFromAxisAngle(
                Vector3.Normalize(new Vector3(
(random.NextDouble().ToFix().Mul(2.ToFix())).Sub(1.ToFix()),
(random.NextDouble().ToFix().Mul(2.ToFix())).Sub(1.ToFix()),
(random.NextDouble().ToFix().Mul(2.ToFix())).Sub(1.ToFix()))),
random.NextDouble().ToFix().Mul(50.ToFix()));
            Fix64 linearFactor = 0.5m.ToFix();
            entity.LinearVelocity = new Vector3(
(random.NextDouble().ToFix().Mul(2.ToFix()).Sub(1.ToFix())).Mul(linearFactor),
(random.NextDouble().ToFix().Mul(2.ToFix()).Sub(1.ToFix())).Mul(linearFactor),
((random.NextDouble().ToFix().Mul(2.ToFix())).Sub(1.ToFix())).Mul(linearFactor));
            Fix64 angularFactor = 0.5m.ToFix();
            entity.AngularVelocity = new Vector3(
(random.NextDouble().ToFix().Mul(2.ToFix()).Sub(1.ToFix())).Mul(angularFactor),
(random.NextDouble().ToFix().Mul(2.ToFix()).Sub(1.ToFix())).Mul(angularFactor),
((random.NextDouble().ToFix().Mul(2.ToFix())).Sub(1.ToFix())).Mul(angularFactor));
        }

        public override void Update(Fix64 dt)
        {
            for (int i = 0; i < 10; i++)
            {
                RandomizeEntityState(Space.Entities[random.Next(Space.Entities.Count)]);
            }
            base.Update(dt);
        }

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "General Convex Pair Stress Test"; }
        }
    }
}