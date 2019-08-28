using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace SharpAlgos
{

    /*
     * 
     *Range Min/Max Query
     *              ;Cpu/query  ;Memory     ;Cpu Preparation;Cpu/Update
     * Brute Force  ;o(n)       ;o(1)       ;o(1)           ;o(N)
     * Segment Tree ;o(log(N))  ;o(N)       ;o(N)           ;o(log(N))
     * Sparse Table ;o(1)       ;o(N*log(N) ;o(N*log(N)     ;not available
     * 
     *Range Sum Query
     *              ;Cpu/query  ;Memory     ;Cpu Preparation;Cpu/Update
     * Brute Force  ;o(n)       ;o(1)       ;o(1)           ;o(1)
     * Segment Tree ;o(log(N))  ;o(N)       ;o(N)           ;o(log(N))
     * 
     */

    #region SparseTable : compute min/max in interval in o(1) time & o(N*log(N)) memory (+ o(N*log(N)) preparation time)
    public class SparseTable
    {
        #region private fields
        private readonly Func<int, int, int> computeParent;
        //sparseTable[i,powerOf2] = precomputed value for interval starting at i and of length 2^powerOf2
        private readonly int[,] sparseTable;
        //log2[n] : the maximum allowed power such as 2^log2[n] <= n
        private readonly int[] log2;
        #endregion
        //Compute min in interval of length N in o(1) time  (and o(N*log(N)) memory and o(N*log(N)) preparation time)
        public static SparseTable Min(int[] data) { return new SparseTable(data, Math.Min, idx => data[idx]); }
        //Compute max in interval of length N in o(1) time  (and o(N*log(N)) memory and o(N*log(N)) preparation time)
        public static SparseTable Max(int[] data) { return new SparseTable(data, Math.Max, idx => data[idx]); }
        //Compute index of min in interval of length N in o(1) time  (and o(N*log(N)) memory and o(N*log(N)) preparation time)
        public static SparseTable IndexOfMin(int[] data) { return new SparseTable(data, (i, j) => data[i] <= data[j] ? i : j, idx => idx); }
        //Compute index of max in interval of length N in o(1) time  (and o(N*log(N)) memory and o(N*log(N)) preparation time)
        public static SparseTable IndexOfMax(int[] data) { return new SparseTable(data, (i, j) => data[i] >= data[j] ? i : j, idx => idx); }
        public SparseTable(int[] data, Func<int, int, int> computeParent, Func<int, int> singleElementValue)
        {
            this.computeParent = computeParent;
            var N = data.Length;
            log2 = new int[1 + N]; // precompute all log values
            log2[0] = -1;
            for (int i = 1; i <= N; i++)
            {
                log2[i] = log2[i / 2] + 1;
            }
            sparseTable = new int[1 + N, 1 + log2[N]];
            for (int i = 0; i < N; i++) //precompute results for all intervals of length 1 (= 2^0)
            {
                sparseTable[i, 0] = singleElementValue(i); //data[i];
            }
            int powerOf2 = 1;
            int currentLength = 2;
            while (currentLength <= N)
            {
                //we compute results for all intervals of length 'currentLength' (we have already computed results for all interval of length: currentLength/2)
                for (int i = 0; i + currentLength <= N; ++i)
                {
                    sparseTable[i, powerOf2] = computeParent(sparseTable[i, powerOf2 - 1], sparseTable[i + currentLength / 2, powerOf2 - 1]);
                }
                currentLength *= 2;
                ++powerOf2;
            }
        }
        //we look at the 2 longest precomputed intervals (starting at 'start' for the 1st and ending at 'end' for the 2nd)
        public int Query(int start, int end)
        {
            int powerOf2 = log2[end - start + 1];
            return computeParent(sparseTable[start, powerOf2], sparseTable[end - (1 << powerOf2) + 1, powerOf2]);
        }
    }
    #endregion

    #region Cartesian Tree
    //a Min Cartesion Tree is a Min Heap, where:
    //  (*) the root id is the index of the minimum in the initial array
    //  (*) the left child of a node is the index of the minimum in the left part of the array (left to the current node)
    //  (*) the right child of a node is the index of the minimum in the right part of the array (right to the current node)
    public class CartesianTree
    {
        public int RootIndex { get; }
        public int[] LeftChild { get; } //leftChild[i] : vertex at left of vertex 'i' , or -1 if no such vertex exists
        public int[] RightChild { get; } //rightChild[i] : vertex at right of vertex 'i' , or -1 if no such vertex exists

        public CartesianTree(int[] data, bool isMinCartesianTree)
        {
            int N = data.Length;
            var parent = new int[N];
            LeftChild = new int[N];
            RightChild = new int[N];
            for (int i = 0; i < N; ++i)
            {
                parent[i] = LeftChild[i] = RightChild[i] = -1;
            }

            int root = 0;
            for (int i = 1; i <= N - 1; i++)
            {
                var last = i - 1;
                RightChild[i] = -1;
                while (IsBetterOrEqualThen(data[last], data[i], isMinCartesianTree) && last != root)
                {
                    last = parent[last];
                }

                if (IsBetterOrEqualThen(data[last], data[i], isMinCartesianTree))
                {
                    parent[root] = i;
                    LeftChild[i] = root;
                    root = i;
                }
                else if (RightChild[last] == -1)
                {
                    RightChild[last] = i;
                    parent[i] = last;
                    LeftChild[i] = -1;
                }
                else
                {
                    parent[RightChild[last]] = i;
                    LeftChild[i] = RightChild[last];
                    RightChild[last] = i;
                    parent[i] = last;
                }
            }
            RootIndex = root;
            parent[RootIndex] = -1;
        }
        private bool IsBetterOrEqualThen(int a, int b, bool isMinCartesianTree) { return isMinCartesianTree ? (a >= b) : (a <= b); }
    }
    #endregion

    #region Segment Tree: Compute min/max/sum in an interval in o(log(N)) time & o(N) memory (+ o(N) preparation time)
    //can set/add a value to any segment in o (log(N)) time
    public class SegmentTree
    {
        #region private fields
        private readonly bool isSumQuery;
        //from the value of 2 consecutive segment, compute the value of the 2 segments merged
        private readonly Func<int, int, int> computeParent; 
        private readonly int dataLength;
        private readonly int[] heap; // heap[0] : root value
        #endregion

        //Compute the min in an interval of length N in o(log(N)) time  (and o(N) memory and o(N) preparation time)
        public static SegmentTree Min(int[] data) { return new SegmentTree(data, Math.Min); }
        //Compute the max in an interval of length N in o(log(N)) time  (and o(N) memory and o(N) preparation time)
        public static SegmentTree Max(int[] data) { return new SegmentTree(data, Math.Max); }
        //Compute the sum in an interval of length N in o(log(N)) time  (and o(N) memory and o(N) preparation time)
        public static SegmentTree Sum(int[] data) { return new SegmentTree(data, (left, right) => left + right); }

        #region setting a value to an entire range
        public void SetValueInInterval(int newValue, int startIndex, int endIndex)
        {
            if (value_to_set_to_each_element_in_segment == null)
            {
                value_to_set_to_each_element_in_segment = new int?[heap.Length];
            }
            SetValueInInterval(0, 0, dataLength - 1, newValue, Math.Max(startIndex, 0), Math.Min(endIndex, dataLength - 1));
        }
        private int?[] value_to_set_to_each_element_in_segment;
        private void SetValueInInterval(int segmentId, int segmentStartIndex, int segmentEndIndex, int newValue, int startIndex, int endIndex)
        {
            LazyPropagateSetValueInInterval(segmentId, segmentStartIndex, segmentEndIndex);
            if (segmentEndIndex < startIndex || segmentStartIndex > endIndex)
            {
                return;
            }
            if (segmentStartIndex >= startIndex && segmentEndIndex <= endIndex)
            {
                SetValueToSegmentAndLazyPropagateToChildren(segmentId, segmentStartIndex, segmentEndIndex, newValue);
                return;
            }
            int mid = (segmentStartIndex + segmentEndIndex) / 2;
            SetValueInInterval(2 * segmentId + 1, segmentStartIndex, mid, newValue, startIndex, endIndex);
            SetValueInInterval(2 * segmentId + 2, mid + 1, segmentEndIndex, newValue, startIndex, endIndex);
            heap[segmentId] = computeParent(heap[2 * segmentId + 1], heap[2 * segmentId + 2]);
        }
        private void LazyPropagateSetValueInInterval(int segmentId, int segmentStartIndex, int segmentEndIndex)
        {
            if (value_to_set_to_each_element_in_segment?[segmentId] == null)
            {
                return;
            }
            SetValueToSegmentAndLazyPropagateToChildren(segmentId, segmentStartIndex, segmentEndIndex, value_to_set_to_each_element_in_segment[segmentId].Value);
            value_to_set_to_each_element_in_segment[segmentId] = null;
        }
        private void SetValueToSegmentAndLazyPropagateToChildren(int segmentId, int segmentStartIndex, int segmentEndIndex, int newValueForEachElementOfSegment)
        {
            if (isSumQuery)
            {
                heap[segmentId] = (segmentEndIndex - segmentStartIndex + 1) * newValueForEachElementOfSegment;
            }
            else //min or max
            {
                heap[segmentId] = newValueForEachElementOfSegment;
            }
            if (segmentStartIndex != segmentEndIndex) //not a leaf
            {
                value_to_set_to_each_element_in_segment[2 * segmentId + 1] = newValueForEachElementOfSegment;
                value_to_set_to_each_element_in_segment[2 * segmentId + 2] = newValueForEachElementOfSegment;
            }
        }
        #endregion

        #region adding a value in an entire range
        public void AddValueInInterval(int toAdd, int startIndex, int endIndex)
        {
            if (value_to_add_to_each_element_in_segment == null)
            {
                value_to_add_to_each_element_in_segment = new int[heap.Length];
            }
            AddValueInInterval(0, 0, dataLength - 1, toAdd, Math.Max(startIndex, 0), Math.Min(endIndex, dataLength - 1));
        }
        private int[] value_to_add_to_each_element_in_segment;
        private void AddValueInInterval(int segmentId, int segmentStartIndex, int segmentEndIndex, int toAdd, int startIndex, int endIndex)
        {
            LazyPropagateAddValueInInterval(segmentId, segmentStartIndex, segmentEndIndex);
            if (segmentEndIndex < startIndex || segmentStartIndex > endIndex)
            {
                return;
            }
            if (segmentStartIndex >= startIndex && segmentEndIndex <= endIndex)
            {
                AddValueToSegmentAndLazyPropagateToChildren(segmentId, segmentStartIndex, segmentEndIndex, toAdd);
                return;
            }
            int mid = (segmentStartIndex + segmentEndIndex) / 2;
            AddValueInInterval(2 * segmentId + 1, segmentStartIndex, mid, toAdd, startIndex, endIndex);
            AddValueInInterval(2 * segmentId + 2, mid + 1, segmentEndIndex, toAdd, startIndex, endIndex);
            heap[segmentId] = computeParent(heap[2 * segmentId + 1], heap[2 * segmentId + 2]);
        }
        private void LazyPropagateAddValueInInterval(int segmentId, int segmentStartIndex, int segmentEndIndex)
        {
            if (value_to_add_to_each_element_in_segment == null || value_to_add_to_each_element_in_segment[segmentId] == 0)
            {
                return;
            }
            AddValueToSegmentAndLazyPropagateToChildren(segmentId, segmentStartIndex, segmentEndIndex, value_to_add_to_each_element_in_segment[segmentId]);
            value_to_add_to_each_element_in_segment[segmentId] = 0;
        }
        private void AddValueToSegmentAndLazyPropagateToChildren(int segmentId, int segmentStartIndex, int segmentEndIndex, int toAdd)
        {
            if (isSumQuery)
            {
                heap[segmentId] += (segmentEndIndex - segmentStartIndex + 1) * toAdd;
            }
            else //min or max
            {
                heap[segmentId] += toAdd;
            }
            if (segmentStartIndex != segmentEndIndex) //not a leaf
            {
                value_to_add_to_each_element_in_segment[2 * segmentId + 1] += toAdd;
                value_to_add_to_each_element_in_segment[2 * segmentId + 2] += toAdd;
            }
        }
        #endregion

        /// <summary>
        /// Query an interval [startIndex, endIndex] (for min/max/sum)  in o(log(N)) time 
        /// </summary>
        /// <param name="startIndex">start index of the interval</param>
        /// <param name="endIndex">end index of the interval</param>
        /// <returns>the interval value (min/max/sum)</returns>
        public int Query(int startIndex, int endIndex) { return Query(0, 0, dataLength - 1, Math.Max(0, startIndex), Math.Min(dataLength - 1, endIndex)); }
        //return the contribution of segment 'segmentId' to compute the value for interval [startIndex, endIndex]
        private int Query(int segmentId, int segmentStartIndex, int segmentEndIndex, int startIndex, int endIndex)
        {
            LazyPropagateSetValueInInterval(segmentId, segmentStartIndex, segmentEndIndex);
            LazyPropagateAddValueInInterval(segmentId, segmentStartIndex, segmentEndIndex);
            if (segmentStartIndex >= startIndex && segmentEndIndex <= endIndex)
            {
                return heap[segmentId]; // 'segmentId' is entirely in [startIndex, endIndex] : so it contributes at 100%
            }
            int mid = (segmentStartIndex + segmentEndIndex) / 2;
            if (mid < startIndex) //only right part of 'segmentId' intersects [startIndex, endIndex] and contributes to the result
            {
                return Query(2 * segmentId + 2, mid + 1, segmentEndIndex, startIndex, endIndex);
            }
            if (mid >= endIndex) //only left part of 'segmentId' intersects [startIndex, endIndex] and contributes to the result
            {
                return Query(2 * segmentId + 1, segmentStartIndex, mid, startIndex, endIndex);
            }
            //both left part and right part of 'segmentId' contribute to the result
            return computeParent(Query(2 * segmentId + 1, segmentStartIndex, mid, startIndex, endIndex), Query(2 * segmentId + 2, mid + 1, segmentEndIndex, startIndex, endIndex));
        }


        private SegmentTree(int[] data, Func<int, int, int> computeParent)
        {
            this.computeParent = computeParent;
            isSumQuery = computeParent(-5, 5) == 0;
            dataLength = data.Length;
            int power = 1 + (int)Math.Ceiling(Math.Log(data.Length) / Math.Log(2));
            int heapLength = (int)Math.Pow(2, power);
            heap = new int[1 + heapLength];
            ComputeHeap(0, 0, data.Length - 1, data);
        }

        private int ComputeHeap(int segmentId, int segmentStartIndex, int segmentEndIndex, int[] data)
        {
            if (data.Length == 0)
            {
                return 0;
            }
            if (segmentStartIndex == segmentEndIndex)
            {
                heap[segmentId] = data[segmentStartIndex];
            }
            else
            {
                var mid = (segmentStartIndex + segmentEndIndex) / 2;
                heap[segmentId] = computeParent(ComputeHeap(2 * segmentId + 1, segmentStartIndex, mid, data), ComputeHeap(2 * segmentId + 2, mid + 1, segmentEndIndex, data));
            }
            return heap[segmentId];
        }
    }
    #endregion


    #region SegmentTreeCountInfToK : count number of elements in interval [startIndex, endIndex] <= K , in o(log^2(N)) time & o(n log(N)) memory
    public class SegmentTreeCountInferiorToK
    {
        #region private fields
        private readonly int dataLength;
        private readonly List<int>[] sortedElementsAtEachNode; // sortedElementsAtEachNode[0]: all sorted elements
        #endregion

        public SegmentTreeCountInferiorToK(int[] data)
        {
            dataLength = data.Length;
            int power = 1 + (int)Math.Ceiling(Math.Log(data.Length) / Math.Log(2));
            int heapLength = (int)Math.Pow(2, power);
            sortedElementsAtEachNode = new List<int>[1 + heapLength];
            ComputeHeapForNbInfToK(0, 0, data.Length - 1, data);
        }

        /// <summary>
        /// retrieve the number of elements in interval [startIndex, endIndex] less or equal to  K in o(log^2(N)) time
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="K"></param>
        /// <returns></returns>
        public int Query(int startIndex, int endIndex, int K) { return Query(0, 0, dataLength - 1, K, startIndex, endIndex); }


        /// <summary>
        /// retrieve the number of elements in interval [startIndex, endIndex] equals to  K in o(log^2(N)) time
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="K"></param>
        /// <returns>the number of elements equals to K in interval</returns>
        public int QueryEqualsToK(int startIndex, int endIndex, int K)
        {
            return Query(startIndex, endIndex, K) - Query(startIndex, endIndex, K-1);
        }


        private List<int> ComputeHeapForNbInfToK(int segmentId, int segmentStartIndex, int segmentEndIndex, int[] data)
        {
            if (segmentStartIndex == segmentEndIndex)
            {
                sortedElementsAtEachNode[segmentId] = new List<int> { data[segmentStartIndex] };
            }
            else
            {
                var mid = (segmentStartIndex + segmentEndIndex) / 2;
                sortedElementsAtEachNode[segmentId] = MergeSort(ComputeHeapForNbInfToK(2 * segmentId + 1, segmentStartIndex, mid, data), ComputeHeapForNbInfToK(2 * segmentId + 2, mid + 1, segmentEndIndex, data));
            }
            return sortedElementsAtEachNode[segmentId];
        }
        //we have 2 lists in increasing order : ' a' & 'b', and we want to merge the 2 list to a single increasing list
        private static List<int> MergeSort(List<int> a, List<int> b)
        {
            int newIndexInA = 0;
            int newIndexInB = 0;
            var result = new List<int>(a.Count + b.Count);
            for (; ; )
            {
                if (newIndexInA >= a.Count)
                {
                    if (newIndexInB < b.Count)
                    {
                        result.AddRange(b.Skip(newIndexInB));
                    }
                    return result;
                }
                if (newIndexInB >= b.Count)
                {
                    result.AddRange(a.Skip(newIndexInA));
                    return result;
                }
                if (a[newIndexInA] <= b[newIndexInB])
                {
                    result.Add(a[newIndexInA++]);
                }
                else
                {
                    result.Add(b[newIndexInB++]);
                }
            }
        }
        //return the contribution of segment 'segmentId' to compute the value for interval [startIndex, endIndex]
        private int Query(int segmentId, int segmentStartIndex, int segmentEndIndex, int K, int startIndex, int endIndex)
        {
            if (segmentStartIndex >= startIndex && segmentEndIndex <= endIndex)
            {
                var sortedElementsInSegment = sortedElementsAtEachNode[segmentId];
                if (sortedElementsInSegment[0] > K)
                {
                    return 0;
                }
                return 1 + Utils.MaximumValidIndex(0, sortedElementsInSegment.Count - 1, x => sortedElementsInSegment[x] <= K);
            }
            int mid = (segmentStartIndex + segmentEndIndex) / 2;
            if (mid < startIndex) //only right part of 'segmentId' intersects [startIndex, endIndex] and contributes to the result
            {
                return Query(2 * segmentId + 2, mid + 1, segmentEndIndex, K, startIndex, endIndex);
            }
            if (mid >= endIndex) //only left part of 'segmentId' intersects [startIndex, endIndex] and contributes to the result
            {
                return Query(2 * segmentId + 1, segmentStartIndex, mid, K, startIndex, endIndex);
            }
            //both left part and right part of 'segmentId' contribute to the result
            return Query(2 * segmentId + 1, segmentStartIndex, mid, K, startIndex, endIndex) + Query(2 * segmentId + 2, mid + 1, segmentEndIndex, K, startIndex, endIndex);
        }
    }
    #endregion

    #region Euler Tour of binary tree
    public class EulerTourOnBinaryTree<T>
    {
        //list of vertices found during an Euler Tour: (starting&ending at root) and visiting all vertices (from left to right)
        public List<T> EulerTour { get; }
        //EulerTourHeight[i]: distance between root and vertex 'EulerTour[i]'  (= 0 if vertex is the root of the tree)
        public List<int> EulerTourHeight;
        //FirstIndexInEulerTour[v]: first time we met vertex 'v' in euler path
        public IDictionary<T, int> FirstIndexInEulerTour { get; }
        private SparseTable lazySparseTableIndexOfMin; //only used to compute Lowest Common Ancestor 

        public EulerTourOnBinaryTree(Graph<T> tree, T rootId)
        {
            // get euler tour & indices of first occurences
            FirstIndexInEulerTour = new Dictionary<T, int>();
            EulerTour = new List<T>();
            EulerTourHeight = new List<int>();

            var toVisit = new Stack<T>();
            var visited = new HashSet<T>();
            var heightQueue = new Stack<int>();
            toVisit.Push(rootId);
            visited.Add(rootId);
            heightQueue.Push(0);
            while (toVisit.Count != 0)
            {
                var vertexId = toVisit.Pop();
                var h = heightQueue.Pop();
                EulerTour.Add(vertexId);
                EulerTourHeight.Add(h);
                if (FirstIndexInEulerTour.ContainsKey(vertexId))
                {
                    continue;
                }
                FirstIndexInEulerTour[vertexId] = EulerTour.Count - 1;
                foreach(var child in tree.Children(vertexId))
                {
                    if (!visited.Add(child))
                    {
                        continue; //already visited
                    }
                    toVisit.Push(vertexId);
                    heightQueue.Push(h);
                    toVisit.Push(child);
                    heightQueue.Push(h + 1);
                }
            }
        }

        //retrieve lowest common ancestor of 'vertexId1' & 'vertexId2' in o(1) time and o(N*Log(N)) memory (+ o(N*Log(N)) preparation time)
        public T LowestCommonAncestor(T vertexId1, T vertexId2)
        {
            int id1;
            if (!FirstIndexInEulerTour.TryGetValue(vertexId1, out id1))
            {
                return default(T);
            }
            int id2;
            if (!FirstIndexInEulerTour.TryGetValue(vertexId2, out id2))
            {
                return default(T);
            }
            if (lazySparseTableIndexOfMin == null)
            {
                lazySparseTableIndexOfMin = SparseTable.IndexOfMin(EulerTourHeight.ToArray());
            }
            var idxOfMinHeightInEulerPath = lazySparseTableIndexOfMin.Query(Math.Min(id1, id2), Math.Max(id1, id2));
            return EulerTour[idxOfMinHeightInEulerPath];
        }
        public int NearestToRoot(int iInEulerTour, int jInEuleurTour)
        {
            return EulerTourHeight[iInEulerTour] < EulerTourHeight[jInEuleurTour] ? iInEulerTour : jInEuleurTour;
        }
    }
    #endregion
}