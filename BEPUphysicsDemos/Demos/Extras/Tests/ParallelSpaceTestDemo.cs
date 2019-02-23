using System;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics;
using System.Collections.Generic;
using BEPUphysics.Entities;
using BEPUutilities;


namespace BEPUphysicsDemos.Demos.Extras.Tests
{
    /// <summary>
    /// Verifies the functionality of multiple spaces running simultaneously.
    /// </summary>
    public class ParallelSpaceTestDemo : StandardDemo
    {
        List<Space> spaces = new List<Space>();
        List<Entity> entities = new List<Entity>();
        /// <summary>
        /// Constructs a new demo.
        /// </summary>
        /// <param name="game">Game owning this demo.</param>
        public ParallelSpaceTestDemo(DemosGame game)
            : base(game)
        {
            for (int i = 0; i < 32; i++)
            {
                var space = new Space(null);
                space.ForceUpdater.Gravity = new Vector3(0.ToFix(), (-9.81m).ToFix(), 0.ToFix());
                var box = new Box(new Vector3((20 * i).ToFix(), 0.ToFix(), 0.ToFix()), 100.ToFix(), 1.ToFix(), 100.ToFix());
                space.Add(box);
                //game.ModelDrawer.Add(box);
                for (int j = 0; j < 30; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        box = new Box(new Vector3((20 * i).ToFix(), (2 + j * 1.1m).ToFix(), 0.ToFix()), 1.ToFix(), 1.ToFix(), 1.ToFix());
                        entities.Add(box);
                        space.Add(box);
                        //game.ModelDrawer.Add(box);
                    }
                }
                spaces.Add(space);
            }
            game.Camera.Position = new Vector3(20.ToFix(), 10.ToFix(), 70.ToFix());

        }

        Random random = new Random();
        public override void Update(Fix dt)
        {
            base.Update(dt);
            for (int i = 0; i < entities.Count; i++)
            {
                var impulse = dt * new Vector3((random.NextDouble() * 30 - 15).ToFix(), (random.NextDouble() * 30 - 15).ToFix(), (random.NextDouble() * 30 - 15).ToFix());
                entities[i].ApplyLinearImpulse(ref impulse);
                impulse = dt * new Vector3((random.NextDouble() * 10 - 5).ToFix(), (random.NextDouble() * 10 - 5).ToFix(), (random.NextDouble() * 10 - 5).ToFix());
                entities[i].ApplyAngularImpulse(ref impulse);
            }

            //for (int i = 0; i < numberOfSpaces; i++)
            //{
            //    spaces[i].Update();
            //}
            //The "real" space has a convenient thread pool we can use.
            Space.ParallelLooper.ForLoop(0, spaces.Count, i =>
            {
                spaces[i].Update();
            });

			timeSinceLastReset =
timeSinceLastReset.Add(dt);
            if (timeSinceLastReset > 10.ToFix())
            {
                Console.WriteLine("Resetting.  Number of resets performed: " + (++resets));
                Game.SwitchSimulation(1);
            }   
        }
        static int resets;
        Fix timeSinceLastReset;

        /// <summary>
        /// Gets the name of the simulation.
        /// </summary>
        public override string Name
        {
            get { return "Terrain"; }
        }
    }
}