using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.BroadPhaseEntries.Events;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using ConversionHelper;


namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Piling up kinematic objects to test some corner cases related to dynamic->kinematic switching.
    /// </summary>
    public class AccumulationTestDemo : StandardDemo
    {
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public AccumulationTestDemo(DemosGame game)
            : base(game)
        {
            //x and y, in terms of heightmaps, refer to their local x and y coordinates.  In world space, they correspond to x and z.
            //Setup the heights of the terrain.
            int xLength = 256;
            int zLength = 256;

            Fix32 xSpacing = .5m.ToFix();
            Fix32 zSpacing = .5m.ToFix();
            var heights = new Fix32[xLength, zLength];
            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < zLength; j++)
                {
                    Fix32 x = (i - xLength / 2).ToFix();
                    Fix32 z = (j - zLength / 2).ToFix();
                    heights[i, j] = (10.ToFix().Mul((Fix32Ext.Sin(x.Div(8.ToFix())).Add(Fix32Ext.Sin(z.Div(8.ToFix()))))));
                }
            }
            //Create the terrain.
            var terrain = new Terrain(heights, new AffineTransform(
                    new Vector3(xSpacing, 1.ToFix(), zSpacing),
                    Quaternion.Identity,
                    new Vector3(((-xLength).ToFix().Mul(xSpacing)).Div(2.ToFix()), 0.ToFix(), ((-zLength).ToFix().Mul(zSpacing)).Div(2.ToFix()))));

            Space.Add(terrain);
            game.ModelDrawer.Add(terrain);




            eventHandler = (sender, other, pair) =>
            {

                var entityCollidable = other as EntityCollidable;
                if (entityCollidable == null || !entityCollidable.Entity.IsDynamic)
                {
                    sender.Events.RemoveAllEvents();
                    sender.Entity.LinearVelocity = new Vector3();
                    sender.Entity.AngularVelocity = new Vector3();
                    sender.Entity.BecomeKinematic();
                    sender.CollisionRules.Group = CollisionRules.DefaultDynamicCollisionGroup;
                }
            };

            game.Camera.Position = new Vector3(0.ToFix(), 30.ToFix(), 20.ToFix());

        }

        InitialCollisionDetectedEventHandler<EntityCollidable> eventHandler;

        void Launch()
        {
            Sphere sphere = new Sphere(Game.Camera.Position, 1.ToFix(), 10.ToFix());
            sphere.CollisionInformation.Events.InitialCollisionDetected += eventHandler;
            sphere.LinearVelocity = Game.Camera.WorldMatrix.Forward * 30.ToFix();
            Space.Add(sphere);
            Game.ModelDrawer.Add(sphere);
        }


        public override void Update(Fix32 dt)
        {
#if XBOX360
            if(Game.GamePadInput.Triggers.Left > .5m)
#else
            if (Game.MouseInput.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
#endif
                Launch();
            base.Update(dt);

        }


        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Accumulation Test"; }
        }
    }
}