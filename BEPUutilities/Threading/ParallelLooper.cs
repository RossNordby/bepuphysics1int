using System;

namespace BEPUutilities.Threading
{
    /// <summary>
    /// Manages parallel for loops.
    /// Cannot handle general task-based parallelism.
    /// </summary>
    public class ParallelLooper : IParallelLooper, IDisposable
    {
        private int workerCount;

        //internal List<ParallelLoopWorker> workers = new List<ParallelLoopWorker>();

        internal int currentBeginIndex, currentEndIndex;
        internal Action<int> currentLoopBody;
        internal int iterationsPerSteal;
		
        /// <summary>
        /// Iterates over the interval.
        /// </summary>
        /// <param name="beginIndex">Starting index of the iteration.</param>
        /// <param name="endIndex">Ending index of the iteration.</param>
        /// <param name="loopBody">Function to call on each iteration.</param>
        public void ForLoop(int beginIndex, int endIndex, Action<int> loopBody)
        {
            for (int i = beginIndex; i < endIndex; i++) {
				loopBody(i);
			}
        }
		
        private bool disposed;

        /// <summary>
        /// Releases resources used by the object.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Releases resources used by the object.
        /// </summary>
        ~ParallelLooper()
        {
            Dispose();
        }
    }
}