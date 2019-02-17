using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUutilities.DataStructures;
using BEPUutilities.ResourceManagement;
using BEPUutilities;
using BEPUutilities.Threading;


namespace BEPUphysics.BroadPhaseSystems.Hierarchies
{
    /// <summary>
    /// Broad phase that incrementally updates the internal tree acceleration structure.
    /// </summary>
    /// <remarks>
    /// This is a good all-around broad phase; its performance is consistent and all queries are supported and speedy.
    /// The memory usage is higher than simple one-axis sort and sweep, but a bit lower than the Grid2DSortAndSweep option.
    /// </remarks>
    public class DynamicHierarchy : BroadPhase
    {
        internal Node root;

        /// <summary>
        /// Constructs a new dynamic hierarchy broad phase.
        /// </summary>
        public DynamicHierarchy()
        {
            multithreadedRefit = MultithreadedRefit;
            multithreadedOverlap = MultithreadedOverlap;
            QueryAccelerator = new DynamicHierarchyQueryAccelerator(this);
        }

        /// <summary>
        /// Constructs a new dynamic hierarchy broad phase.
        /// </summary>
        /// <param name="parallelLooper">Parallel loop provider to use in the broad phase.</param>
        public DynamicHierarchy(IParallelLooper parallelLooper)
            : base(parallelLooper)
        {
            multithreadedRefit = MultithreadedRefit;
            multithreadedOverlap = MultithreadedOverlap;
            QueryAccelerator = new DynamicHierarchyQueryAccelerator(this);
        }

#if PROFILE
        /// <summary>
        /// Gets the time used in refitting the acceleration structure and making any necessary incremental improvements.
        /// </summary>
        public double RefitTime
        {
            get
            {
                return (endRefit - startRefit) / (double)Stopwatch.Frequency;
            }
        }
        /// <summary>
        /// Gets the time used in testing the tree against itself to find overlapping pairs. 
        /// </summary>
        public double OverlapTime
        {
            get
            {
                return (endOverlap - endRefit) / (double)Stopwatch.Frequency;
            }
        }
        long startRefit, endRefit;
        long endOverlap;


#endif

        #region Multithreading

        public void MultithreadedRefitPhase(int splitDepth)
        {
            if (splitDepth > 0)
            {
                root.CollectMultithreadingNodes(splitDepth, 1, multithreadingSourceNodes);
                //Go through every node and refit it.
                ParallelLooper.ForLoop(0, multithreadingSourceNodes.Count, multithreadedRefit);
                multithreadingSourceNodes.Clear();
                //Now that the subtrees belonging to the source nodes are refit, refit the top nodes.
                //Sometimes, this will go deeper than necessary because the refit process may require an extremely high level (nonmultithreaded) revalidation.
                //The waste cost is a matter of nanoseconds due to the simplicity of the operations involved.
                root.PostRefit(splitDepth, 1);
            }
            else
            {
                SingleThreadedRefitPhase();
            }
        }

        public void MultithreadedOverlapPhase(int splitDepth)
        {
            if (splitDepth > 0)
            {
                //The trees are now fully refit (and revalidated, if the refit process found it to be necessary).
                //The overlap traversal is conceptually similar to the multithreaded refit, but is a bit easier since there's no need to go back up the stack.
                if (!root.IsLeaf) //If the root is a leaf, it's alone- nothing to collide against! This test is required by the assumptions of the leaf-leaf test.
                {
                    root.GetMultithreadedOverlaps(root, splitDepth, 1, this, multithreadingSourceOverlaps);
                    ParallelLooper.ForLoop(0, multithreadingSourceOverlaps.Count, multithreadedOverlap);
                    multithreadingSourceOverlaps.Clear();
                }
            }
            else
            {
                SingleThreadedOverlapPhase();
            }
        }
		
        internal struct NodePair
        {
            internal Node a;
            internal Node b;
        }

        RawList<Node> multithreadingSourceNodes = new RawList<Node>(4);
        Action<int> multithreadedRefit;
        void MultithreadedRefit(int i)
        {
            multithreadingSourceNodes.Elements[i].Refit();
        }

        RawList<NodePair> multithreadingSourceOverlaps = new RawList<NodePair>(10);
        Action<int> multithreadedOverlap;
        void MultithreadedOverlap(int i)
        {
            var overlap = multithreadingSourceOverlaps.Elements[i];
            //Note: It's okay not to check to see if a and b are equal and leaf nodes, because the systems which added nodes to the list already did it.
            overlap.a.GetOverlaps(overlap.b, this);
        }

        #endregion

        public void SingleThreadedRefitPhase()
        {
            root.Refit();
        }

        public void SingleThreadedOverlapPhase()
        {
            if (!root.IsLeaf) //If the root is a leaf, it's alone- nothing to collide against! This test is required by the assumptions of the leaf-leaf test.
                root.GetOverlaps(root, this);
        }

        protected override void UpdateSingleThreaded()
        {
            Overlaps.Clear();
            if (root != null)
            {
#if PROFILE
                startRefit = Stopwatch.GetTimestamp();
#endif
                SingleThreadedRefitPhase();
#if PROFILE
                endRefit = Stopwatch.GetTimestamp();
#endif
                SingleThreadedOverlapPhase();
#if PROFILE
                endOverlap = Stopwatch.GetTimestamp();
#endif
            }
        }

        UnsafeResourcePool<LeafNode> leafNodes = new UnsafeResourcePool<LeafNode>();

        /// <summary>
        /// Adds an entry to the hierarchy.
        /// </summary>
        /// <param name="entry">Entry to add.</param>
        public override void Add(BroadPhaseEntry entry)
        {
            base.Add(entry);
            //Entities do not set up their own bounding box before getting stuck in here.  If they're all zeroed out, the tree will be horrible.
            Vector3 offset;
            Vector3.Subtract(ref entry.boundingBox.Max, ref entry.boundingBox.Min, out offset);
            if (Fix32Ext.MulSafe(Fix32Ext.MulSafe(offset.X, offset.Y), offset.Z) == F64.C0)
                entry.UpdateBoundingBox();
            //Could buffer additions to get a better construction in the tree.
            var node = leafNodes.Take();
            node.Initialize(entry);
            if (root == null)
            {
                //Empty tree.  This is the first and only node.
                root = node;
            }
            else
            {
                if (root.IsLeaf) //Root is alone.
                    root.TryToInsert(node, out root);
                else
                {
                    BoundingBox.CreateMerged(ref node.BoundingBox, ref root.BoundingBox, out root.BoundingBox);
                    var internalNode = (InternalNode)root;
                    Vector3.Subtract(ref root.BoundingBox.Max, ref root.BoundingBox.Min, out offset);
                    internalNode.currentVolume = Fix32Ext.MulSafe(Fix32Ext.MulSafe(offset.X, offset.Y), offset.Z);
                    //internalNode.maximumVolume = internalNode.currentVolume * InternalNode.MaximumVolumeScale;
                    //The caller is responsible for the merge.
                    var treeNode = root;
                    while (!treeNode.TryToInsert(node, out treeNode)) ;//TryToInsert returns the next node, if any, and updates node bounding box.
                }
            }
        }
        /// <summary>
        /// Removes an entry from the hierarchy.
        /// </summary>
        /// <param name="entry">Entry to remove.</param>
        public override void Remove(BroadPhaseEntry entry)
        {
            if (root == null)
                throw new InvalidOperationException("Entry not present in the hierarchy.");
            //Attempt to search for the entry with a boundingbox lookup first.
            if (!RemoveFast(entry))
            {
                //Oof, could not locate it with the fast method; it must have been force-moved or something.
                //Fall back to a slow brute force approach.
                if (!RemoveBrute(entry))
                {
                    throw new InvalidOperationException("Entry not present in the hierarchy.");
                }
            }
        }

        internal bool RemoveFast(BroadPhaseEntry entry)
        {
            LeafNode leafNode;
            //Update the root with the replacement just in case the removal triggers a root change.
            if (root.RemoveFast(entry, out leafNode, out root))
            {
                leafNode.CleanUp();
                leafNodes.GiveBack(leafNode);
                base.Remove(entry);
                return true;
            }
            return false;
        }

        internal bool RemoveBrute(BroadPhaseEntry entry)
        {
            LeafNode leafNode;
            //Update the root with the replacement just in case the removal triggers a root change.
            if (root.Remove(entry, out leafNode, out root))
            {
                leafNode.CleanUp();
                leafNodes.GiveBack(leafNode);
                base.Remove(entry);
                return true;
            }
            return false;
        }

        #region Debug
        internal void Analyze(List<int> depths, out int nodeCount)
        {
            nodeCount = 0;
            root.Analyze(depths, 0, ref nodeCount);
        }

        /// <summary>
        /// Forces a full rebuild of the tree. Useful to return the tree to a decent level of quality if the tree has gotten horribly messed up.
        /// Watch out, this is a slow operation. Expect to drop frames.
        /// </summary>
        public void ForceRebuild()
        {
            if (root != null && !root.IsLeaf)
            {
                ((InternalNode)root).Revalidate();
            }
        }


        /// <summary>
        /// Measures the cost of the tree, based on the volume of the tree's nodes.
        /// Approximates the expected cost of volume-based queries against the tree. 
        /// Useful for comparing against other trees.
        /// </summary>
        /// <returns>Cost of the tree.</returns>
        public Fix32 MeasureCostMetric()
        {
            if (root != null)
            {
                var offset = root.BoundingBox.Max - root.BoundingBox.Min;
                var volume = Fix32Ext.MulSafe(Fix32Ext.MulSafe(offset.X, offset.Y), offset.Z);
                if (volume < F64.C1em9)
                    return F64.C0;
                return root.MeasureSubtreeCost().Div(volume);
            }
            return F64.C0;
        }
        #endregion
    }

}