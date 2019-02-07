using System;
using BEPUphysics;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.Constraints;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.PositionUpdating;
using BEPUphysics.Settings;
using BEPUutilities;


namespace BEPUphysicsDemos
{
    /// <summary>
    /// This class contains a bunch of helper functions to set up the simulation for different goals.
    /// Try using some of the different profiles in the simulations to see the results.
    /// You can also play with the numbers in the profiles to see the results.
    /// 
    /// One configuration option that these functions do not take advantage of is changing the time step.
    /// By default, the Space.TimeStepSettings.TimeStepDuration is set to 1/60f.  This is pretty good for most simulations.
    /// However, sometimes, you may need to drop the rate down to 1/30f for performance reasons.  This harms the simulation quality quite a bit.
    /// On the other hand, the update rate can be increased to 1/120f or more, which vastly increases the simulation quality.
    /// 
    /// When using a non-60hz update rate it's a good idea to pass the elapsed gametime into the Space.Update method (you can find
    /// the demos's space update call in the Demo.cs Update function).  This will allow the engine to take as many timesteps are 
    /// necessary to keep up with passing time.  Just remember that if the simulation gets too hectic and the engine falls behind,
    /// performance will suffer a lot as it takes multiple expensive steps in a single frame trying to catch up.  In addition,
    /// since the number of time steps per frame isn't fixed when using internal time stepping, subtle unsmooth motion may creep in.
    /// This can be addressed by using the interpolation buffers.  Check out the Updating Asynchronously documentation for more information.
    /// [Asynchronous updating isn't needed to use internal time stepping, it's just a common use case.]
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Applies the default settings to the space.
        /// These values are what the engine starts with; they don't have to be applied unless you just want to get back to the defaults.
        /// This doesn't cover every single tunable field in the entire engine, just the main ones that this helper class is messing with.
        /// </summary>
        /// <param name="space">Space to configure.</param>
        public static void ApplyDefaultSettings(Space space)
        {
            MotionSettings.DefaultPositionUpdateMode = PositionUpdateMode.Discrete;
            SolverSettings.DefaultMinimumIterationCount = 1;
            space.Solver.IterationLimit = 10;
            GeneralConvexPairTester.UseSimplexCaching = false;
            MotionSettings.UseExtraExpansionForContinuousBoundingBoxes = false;

            //Set all the scaling settings back to their defaults.
            space.DeactivationManager.VelocityLowerLimit = 0.26m.ToFix();
            CollisionResponseSettings.MaximumPenetrationRecoverySpeed = 2.ToFix();
            CollisionResponseSettings.BouncinessVelocityThreshold = 1.ToFix();
            CollisionResponseSettings.StaticFrictionVelocityThreshold = .2m.ToFix();
            CollisionDetectionSettings.ContactInvalidationLength = .1m.ToFix();
            CollisionDetectionSettings.ContactMinimumSeparationDistance = .03m.ToFix();
            CollisionDetectionSettings.MaximumContactDistance = .1m.ToFix();
            CollisionDetectionSettings.DefaultMargin = .04m.ToFix();
            CollisionDetectionSettings.AllowedPenetration = .01m.ToFix();
            SolverSettings.DefaultMinimumImpulse = 0.001m.ToFix();

            //Adjust epsilons back to defaults.
            Toolbox.Epsilon = 1e-4m.ToFix();
            Toolbox.BigEpsilon = 1e-3m.ToFix();
            MPRToolbox.DepthRefinementEpsilon = 1e-3m.ToFix();
            MPRToolbox.RayCastSurfaceEpsilon = 1e-4m.ToFix();
            MPRToolbox.SurfaceEpsilon = 1e-4m.ToFix();
            PairSimplex.DistanceConvergenceEpsilon = 1e-4m.ToFix();
            PairSimplex.ProgressionEpsilon = 1e-4m.ToFix();

        }


        /// <summary>
        /// Applies slightly higher speed settings.
        /// The only change here is the default minimum iterations.
        /// In many simulations, having a minimum iteration count of 0 works just fine.
        /// It's a quick and still fairly robust way to get some extra performance.
        /// An example of where this might introduce some issues is sphere stacking.
        /// </summary>
        public static void ApplySemiSpeedySettings()
        {
            SolverSettings.DefaultMinimumIterationCount = 0;
        }

        /// <summary>
        /// Applies some low quality, high speed settings.
        /// The main benefit comes from the very low iteration cap.
        /// By enabling simplex caching, general convex collision detection
        /// gets a nice chunk faster, but some curved shapes lose collision detection robustness.
        /// </summary>
        /// <param name="space">Space to configure.</param>
        public static void ApplySuperSpeedySettings(Space space)
        {
            SolverSettings.DefaultMinimumIterationCount = 0;
            space.Solver.IterationLimit = 5;
            GeneralConvexPairTester.UseSimplexCaching = true;
        }

        /// <summary>
        /// Applies some higher quality settings.
        /// By using universal continuous collision detection, missed collisions
        /// will be much, much rarer.  This actually doesn't have a huge performance cost.
        /// The increased iterations put this as a midpoint between the normal and high stability settings.
        /// </summary>
        /// <param name="space">Space to configure.</param>
        public static void ApplyMediumHighStabilitySettings(Space space)
        {
            MotionSettings.DefaultPositionUpdateMode = PositionUpdateMode.Continuous;
            SolverSettings.DefaultMinimumIterationCount = 2;
            space.Solver.IterationLimit = 15;

        }

        /// <summary>
        /// Applies some high quality, low performance settings.
        /// By using universal continuous collision detection, missed collisions
        /// will be much, much rarer.  This actually doesn't have a huge performance cost.
        /// However, increasing the iteration limit and the minimum iterations to 5x the default
        /// will incur a pretty hefty overhead.
        /// On the upside, pretty much every simulation will be rock-solid.
        /// </summary>
        /// <param name="space">Space to configure.</param>
        public static void ApplyHighStabilitySettings(Space space)
        {
            MotionSettings.DefaultPositionUpdateMode = PositionUpdateMode.Continuous;
            MotionSettings.UseExtraExpansionForContinuousBoundingBoxes = true;
            SolverSettings.DefaultMinimumIterationCount = 5;
            space.Solver.IterationLimit = 50;
        }
    }
}
 