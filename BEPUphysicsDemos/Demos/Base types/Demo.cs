
using System;
using System.Diagnostics;
using BEPUphysics;
using BEPUutilities;
using BEPUutilities.Threading;


namespace BEPUphysicsDemos.Demos
{
    /// <summary>
    /// Superclass of the sample simulations.
    /// </summary>
    public abstract class Demo
    {
        private int accumulatedPhysicsFrames;
        private double accumulatedPhysicsTime;
        private Fix32 previousTimeMeasurement;
        private ParallelLooper parallelLooper;

        protected Demo(DemosGame game)
        {
            Game = game;
            parallelLooper = new ParallelLooper();
            //This section lets the engine know that it can make use of multithreaded systems
            //by adding threads to its thread pool.

            Space = new Space(parallelLooper);


            game.Camera.LockedUp = Vector3.Up;
            game.Camera.ViewDirection = new Vector3(0.ToFix(), 0.ToFix(), (-1).ToFix());
            
        }

        /// <summary>
        /// Gets the average time spent per frame in the physics simulation.
        /// </summary>
        public double PhysicsTime { get; private set; }

        /// <summary>
        /// Gets the name of the demo.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the game this demo belongs to.
        /// </summary>
        public DemosGame Game { get; private set; }

        /// <summary>
        /// Gets or sets the space this simulation runs in.
        /// </summary>
        public Space Space { get; set; }

        /// <summary>
        /// Updates the game.
        /// </summary>
        /// <param name="dt">Game timestep.</param>
        public virtual void Update(Fix32 dt)
        {
            long startTime = Stopwatch.GetTimestamp();

            //Update the simulation.
            //Pass in dt to the function to use internal timestepping, if desired.
            //Using internal time stepping usually works best when the interpolation is also used.
            //Check out the asynchronous updating documentation for an example (though you don't have to use a separate thread to use interpolation).

#if WINDOWS
            if (Game.MouseInput.XButton1 == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                //Interpolation isn't used in the demos by default, so passing in a really short time adds a lot of time between discretely visible time steps.
                //Using a Space.TimeStepSettings.TimeStepDuration of 1/60f (the default), this will perform one time step every 20 frames (about three per second at the usual game update rate).
                //This can make it easier to examine behavior frame-by-frame.
                Space.Update(1.ToFix().Div(1200.ToFix())); 
            }
            else
#endif
                Space.Update();


            long endTime = Stopwatch.GetTimestamp();
			accumulatedPhysicsTime += (endTime - startTime) / (double) Stopwatch.Frequency;
            accumulatedPhysicsFrames++;
			previousTimeMeasurement = previousTimeMeasurement.Add(dt);
            if (previousTimeMeasurement > .3m.ToFix())
            {
				previousTimeMeasurement = previousTimeMeasurement.Sub(.3m.ToFix());
                PhysicsTime = accumulatedPhysicsTime / accumulatedPhysicsFrames;
                accumulatedPhysicsTime = 0;
                accumulatedPhysicsFrames = 0;
            }

        }

        /// <summary>
        /// Draws any extra UI components associated with this demo.
        /// </summary>
        public virtual void DrawUI()
        {
        }

        /// <summary>
        /// Performs arbitrary drawing associated with this demo.
        /// </summary>
        public virtual void Draw()
        {

        }

        /// <summary>
        /// Cleans up this simulation before moving to the next one.
        /// </summary>
        public virtual void CleanUp()
        {
            //Undo any in-demo configuration.
            ConfigurationHelper.ApplyDefaultSettings(Space);
            parallelLooper.Dispose();
        }
    }
}