using System;
using BEPUphysics.DeactivationManagement;
using BEPUutilities;
using BEPUutilities.DataStructures;
using BEPUutilities.Threading;

namespace BEPUphysics.Constraints
{
    ///<summary>
    /// Iteratively solves solver updateables, converging to a solution for simulated joints and collision pair contact constraints.
    ///</summary>
    public class Solver : MultithreadedProcessingStage
    {
        RawList<SolverUpdateable> solverUpdateables = new RawList<SolverUpdateable>();
        internal int iterationLimit = 10;
        ///<summary>
        /// Gets or sets the maximum number of iterations the solver will attempt to use to solve the simulation's constraints.
        ///</summary>
        public int IterationLimit { get { return iterationLimit; } set { iterationLimit = Math.Max(value, 0); } }
        ///<summary>
        /// Gets the list of solver updateables in the solver.
        ///</summary>
        public ReadOnlyList<SolverUpdateable> SolverUpdateables
        {
            get
            {
                return new ReadOnlyList<SolverUpdateable>(solverUpdateables);
            }
        }
        protected internal TimeStepSettings timeStepSettings;
        ///<summary>
        /// Gets or sets the time step settings used by the solver.
        ///</summary>
        public TimeStepSettings TimeStepSettings
        {
            get
            {
                return timeStepSettings;
            }
            set
            {
                timeStepSettings = value;
            }
        }

        ///<summary>
        /// Gets or sets the deactivation manager used by the solver.
        /// When constraints are added and removed, the deactivation manager
        /// gains and loses simulation island connections that affect simulation islands
        /// and activity states.
        ///</summary>
        public DeactivationManager DeactivationManager { get; set; }

        ///<summary>
        /// Constructs a Solver.
        ///</summary>
        ///<param name="timeStepSettings">Time step settings used by the solver.</param>
        ///<param name="deactivationManager">Deactivation manager used by the solver.</param>
        public Solver(TimeStepSettings timeStepSettings, DeactivationManager deactivationManager)
        {
            TimeStepSettings = timeStepSettings;
            DeactivationManager = deactivationManager;
            Enabled = true;
        }
        ///<summary>
        /// Constructs a Solver.
        ///</summary>
        ///<param name="timeStepSettings">Time step settings used by the solver.</param>
        ///<param name="deactivationManager">Deactivation manager used by the solver.</param>
        /// <param name="parallelLooper">Parallel loop provider used by the solver.</param>
        public Solver(TimeStepSettings timeStepSettings, DeactivationManager deactivationManager, IParallelLooper parallelLooper)
            : this(timeStepSettings, deactivationManager)
        {
            ParallelLooper = parallelLooper;
            AllowMultithreading = true;
        }

        ///<summary>
        /// Adds a solver updateable to the solver.
        ///</summary>
        ///<param name="item">Updateable to add.</param>
        ///<exception cref="ArgumentException">Thrown when the item already belongs to a solver.</exception>
        public void Add(SolverUpdateable item)
        {
            if (item.Solver == null)
            {
                item.Solver = this;

                item.solverIndex = solverUpdateables.Count;
                solverUpdateables.Add(item);

                DeactivationManager.Add(item.simulationIslandConnection);
                item.OnAdditionToSolver(this);
            }
            else
                throw new ArgumentException("Solver updateable already belongs to something; it can't be added.", "item");
        }
        ///<summary>
        /// Removes a solver updateable from the solver.
        ///</summary>
        ///<param name="item">Updateable to remove.</param>
        ///<exception cref="ArgumentException">Thrown when the item does not belong to the solver.</exception>
        public void Remove(SolverUpdateable item)
        {

            if (item.Solver == this)
            {

                item.Solver = null;


                solverUpdateables.Count--;
                if (item.solverIndex < solverUpdateables.Count)
                {
                    //The solver updateable isn't the last element, so put the last element in its place.
                    solverUpdateables.Elements[item.solverIndex] = solverUpdateables.Elements[solverUpdateables.Count];
                    //Update the replacement's solver index to its new location.
                    solverUpdateables.Elements[item.solverIndex].solverIndex = item.solverIndex;
                }
                solverUpdateables.Elements[solverUpdateables.Count] = null;

                DeactivationManager.Remove(item.simulationIslandConnection);
                item.OnRemovalFromSolver(this);
            }

            else
                throw new ArgumentException("Solver updateable doesn't belong to this solver; it can't be removed.", "item");

        }
		
        protected override void UpdateSingleThreaded()
        {

            int totalUpdateableCount = solverUpdateables.Count;
            for (int i = 0; i < totalUpdateableCount; i++)
            {
                UnsafePrestep(solverUpdateables.Elements[i]);
            }

            //By performing all velocity modifications after the prestep, the prestep is free to read velocities consistently.
            //If the exclusive update was performed in the same call as the prestep, the velocities would enter inconsistent states based on update order.
            for (int i = 0; i < totalUpdateableCount; i++)
            {
                UnsafeExclusiveUpdate(solverUpdateables.Elements[i]);
            }
			
			for (int j = 0; j < iterationLimit; j++) {
				for (int i = 0; i < totalUpdateableCount; i++) {
					UnsafeSolveIteration(solverUpdateables.Elements[i]);
				}
			}
		}

        protected internal void UnsafePrestep(SolverUpdateable updateable)
        {
            updateable.UpdateSolverActivity();
            if (updateable.isActiveInSolver)
            {
                SolverSettings solverSettings = updateable.solverSettings;
                solverSettings.currentIterations = 0;
                solverSettings.iterationsAtZeroImpulse = 0;
                updateable.Update(timeStepSettings.TimeStepDuration);
            }
        }

        protected internal void UnsafeExclusiveUpdate(SolverUpdateable updateable)
        {
            if (updateable.isActiveInSolver)
            {
                updateable.ExclusiveUpdate();
            }
        }

        protected internal void UnsafeSolveIteration(SolverUpdateable updateable)
        {
            if (updateable.isActiveInSolver)
            {
                SolverSettings solverSettings = updateable.solverSettings;


                solverSettings.currentIterations++;
                if (solverSettings.currentIterations <= iterationLimit &&
                    solverSettings.currentIterations <= solverSettings.maximumIterationCount)
                {
                    if (updateable.SolveIteration() < solverSettings.minimumImpulse)
                    {
                        solverSettings.iterationsAtZeroImpulse++;
                        if (solverSettings.iterationsAtZeroImpulse > solverSettings.minimumIterationCount)
                            updateable.isActiveInSolver = false;

                    }
                    else
                    {
                        solverSettings.iterationsAtZeroImpulse = 0;
                    }
                }
                else
                {
                    updateable.isActiveInSolver = false;
                }

            }
        }


    }
}
