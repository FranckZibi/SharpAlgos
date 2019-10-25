using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly Func<int, int, int> _computeParent;
        //sparseTable[i,powerOf2] = precomputed value for interval starting at i and of length 2^powerOf2
        private readonly int[,] _sparseTable;
        //log2[n] : the maximum allowed power such as 2^log2[n] <= n
        private readonly int[] _log2;
        #endregion

        /// <summary>
        /// Return a SparseTable that will be used to compute min in interval
        public static SparseTable Min(int[] data) { return new SparseTable(data, Math.Min, idx => data[idx]); }
        /// <summary>
        /// Return a SparseTable that will be used to compute max in interval
        public static SparseTable Max(int[] data) { return new SparseTable(data, Math.Max, idx => data[idx]); }
        /// <summary>
        /// Return a SparseTable that will be used to compute the index of min in interval
        public static SparseTable IndexOfMin(int[] data) { return new SparseTable(data, (i, j) => data[i] <= data[j] ? i : j, idx => idx); }
        /// <summary>
        /// Return a SparseTable that will be used to compute the index of max in interval
        public static SparseTable IndexOfMax(int[] data) { return new SparseTable(data, (i, j) => data[i] >= data[j] ? i : j, idx => idx); }

        /// <summary>
        /// Min or Max Query for interval [start, end]
        /// We look at the 2 longest precomputed intervals (starting at 'start' for the 1st and ending at 'end' for the 2nd)
        /// Complexity:         o( 1 ) time  (+ o( N*log(N) ) preparation time paid once)
        /// Memory Complexity:  o( N*log(N) )
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public int Query(int start, int end)
        {
            int powerOf2 = _log2[end - start + 1];
            return _computeParent(_sparseTable[start, powerOf2], _sparseTable[end - (1 << powerOf2) + 1, powerOf2]);
        }

        private SparseTable(int[] data, Func<int, int, int> computeParent, Func<int, int> singleElementValue)
        {
            _computeParent = computeParent;
            var n = data.Length;
            _log2 = new int[1 + n]; // precompute all log values
            _log2[0] = -1;
            for (int i = 1; i <= n; i++)
            {
                _log2[i] = _log2[i / 2] + 1;
            }
            _sparseTable = new int[1 + n, 1 + _log2[n]];
            for (int i = 0; i < n; i++) //precompute results for all intervals of length 1 (= 2^0)
            {
                _sparseTable[i, 0] = singleElementValue(i); //data[i];
            }
            int powerOf2 = 1;
            int currentLength = 2;
            while (currentLength <= n)
            {
                //we compute results for all intervals of length 'currentLength' (we have already computed results for all interval of length: currentLength/2)
                for (int i = 0; i + currentLength <= n; ++i)
                {
                    _sparseTable[i, powerOf2] = computeParent(_sparseTable[i, powerOf2 - 1], _sparseTable[i + currentLength / 2, powerOf2 - 1]);
                }
                currentLength *= 2;
                ++powerOf2;
            }
        }
    }
    #endregion

    #region Cartesian Tree
    //a Min Cartesian Tree is a Min Heap, where:
    //  (*) the root id is the index of the minimum in the initial array
    //  (*) the left child of a node is the index of the minimum in the left part of the array (left to the current node)
    //  (*) the right child of a node is the index of the minimum in the right part of the array (right to the current node)
    public class CartesianTree
    {
        private int RootIndex { get; }
        private int[] LeftChild { get; } //leftChild[i] : vertex at left of vertex 'i' , or -1 if no such vertex exists
        private int[] RightChild { get; } //rightChild[i] : vertex at right of vertex 'i' , or -1 if no such vertex exists

        public CartesianTree(int[] data, bool isMinCartesianTree)
        {
            int n = data.Length;
            var parent = new int[n];
            LeftChild = new int[n];
            RightChild = new int[n];
            for (int i = 0; i < n; ++i)
            {
                parent[i] = LeftChild[i] = RightChild[i] = -1;
            }

            int root = 0;
            for (int i = 1; i <= n - 1; i++)
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
        private readonly bool _isSumQuery;
        //from the value of 2 consecutive segment, compute the value of the 2 segments merged
        private readonly Func<int, int, int> _computeParent; 
        private readonly int _dataLength;
        private readonly int[] _heap; // heap[0] : root value
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
            if (_valueToSetToEachElementInSegment == null)
            {
                _valueToSetToEachElementInSegment = new int?[_heap.Length];
            }
            SetValueInInterval(0, 0, _dataLength - 1, newValue, Math.Max(startIndex, 0), Math.Min(endIndex, _dataLength - 1));
        }
        private int?[] _valueToSetToEachElementInSegment;
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
            _heap[segmentId] = _computeParent(_heap[2 * segmentId + 1], _heap[2 * segmentId + 2]);
        }
        private void LazyPropagateSetValueInInterval(int segmentId, int segmentStartIndex, int segmentEndIndex)
        {
            if (_valueToSetToEachElementInSegment?[segmentId] == null)
            {
                return;
            }
            SetValueToSegmentAndLazyPropagateToChildren(segmentId, segmentStartIndex, segmentEndIndex, _valueToSetToEachElementInSegment[segmentId].Value);
            _valueToSetToEachElementInSegment[segmentId] = null;
        }
        private void SetValueToSegmentAndLazyPropagateToChildren(int segmentId, int segmentStartIndex, int segmentEndIndex, int newValueForEachElementOfSegment)
        {
            if (_isSumQuery)
            {
                _heap[segmentId] = (segmentEndIndex - segmentStartIndex + 1) * newValueForEachElementOfSegment;
            }
            else //min or max
            {
                _heap[segmentId] = newValueForEachElementOfSegment;
            }
            if (segmentStartIndex != segmentEndIndex) //not a leaf
            {
                _valueToSetToEachElementInSegment[2 * segmentId + 1] = newValueForEachElementOfSegment;
                _valueToSetToEachElementInSegment[2 * segmentId + 2] = newValueForEachElementOfSegment;
            }
        }
        #endregion

        #region adding a value in an entire range
        public void AddValueInInterval(int toAdd, int startIndex, int endIndex)
        {
            if (_valueToAddToEachElementInSegment == null)
            {
                _valueToAddToEachElementInSegment = new int[_heap.Length];
            }
            AddValueInInterval(0, 0, _dataLength - 1, toAdd, Math.Max(startIndex, 0), Math.Min(endIndex, _dataLength - 1));
        }
        private int[] _valueToAddToEachElementInSegment;
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
            _heap[segmentId] = _computeParent(_heap[2 * segmentId + 1], _heap[2 * segmentId + 2]);
        }
        private void LazyPropagateAddValueInInterval(int segmentId, int segmentStartIndex, int segmentEndIndex)
        {
            if (_valueToAddToEachElementInSegment == null || _valueToAddToEachElementInSegment[segmentId] == 0)
            {
                return;
            }
            AddValueToSegmentAndLazyPropagateToChildren(segmentId, segmentStartIndex, segmentEndIndex, _valueToAddToEachElementInSegment[segmentId]);
            _valueToAddToEachElementInSegment[segmentId] = 0;
        }
        private void AddValueToSegmentAndLazyPropagateToChildren(int segmentId, int segmentStartIndex, int segmentEndIndex, int toAdd)
        {
            if (_isSumQuery)
            {
                _heap[segmentId] += (segmentEndIndex - segmentStartIndex + 1) * toAdd;
            }
            else //min or max
            {
                _heap[segmentId] += toAdd;
            }
            if (segmentStartIndex != segmentEndIndex) //not a leaf
            {
                _valueToAddToEachElementInSegment[2 * segmentId + 1] += toAdd;
                _valueToAddToEachElementInSegment[2 * segmentId + 2] += toAdd;
            }
        }
        #endregion

        /// <summary>
        /// Query an interval [startIndex, endIndex] (for min/max/sum)  in o(log(N)) time 
        /// </summary>
        /// <param name="startIndex">start index of the interval</param>
        /// <param name="endIndex">end index of the interval</param>
        /// <returns>the interval value (min/max/sum)</returns>
        public int Query(int startIndex, int endIndex) { return Query(0, 0, _dataLength - 1, Math.Max(0, startIndex), Math.Min(_dataLength - 1, endIndex)); }
        //return the contribution of segment 'segmentId' to compute the value for interval [startIndex, endIndex]
        private int Query(int segmentId, int segmentStartIndex, int segmentEndIndex, int startIndex, int endIndex)
        {
            LazyPropagateSetValueInInterval(segmentId, segmentStartIndex, segmentEndIndex);
            LazyPropagateAddValueInInterval(segmentId, segmentStartIndex, segmentEndIndex);
            if (segmentStartIndex >= startIndex && segmentEndIndex <= endIndex)
            {
                return _heap[segmentId]; // 'segmentId' is entirely in [startIndex, endIndex] : so it contributes at 100%
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
            return _computeParent(Query(2 * segmentId + 1, segmentStartIndex, mid, startIndex, endIndex), Query(2 * segmentId + 2, mid + 1, segmentEndIndex, startIndex, endIndex));
        }


        private SegmentTree(int[] data, Func<int, int, int> computeParent)
        {
            _computeParent = computeParent;
            _isSumQuery = computeParent(-5, 5) == 0;
            _dataLength = data.Length;
            int power = 1 + (int)Math.Ceiling(Math.Log(data.Length) / Math.Log(2));
            int heapLength = (int)Math.Pow(2, power);
            _heap = new int[1 + heapLength];
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
                _heap[segmentId] = data[segmentStartIndex];
            }
            else
            {
                var mid = (segmentStartIndex + segmentEndIndex) / 2;
                _heap[segmentId] = _computeParent(ComputeHeap(2 * segmentId + 1, segmentStartIndex, mid, data), ComputeHeap(2 * segmentId + 2, mid + 1, segmentEndIndex, data));
            }
            return _heap[segmentId];
        }
    }
    #endregion


    #region SegmentTreeCountInfToK : count number of elements in interval [startIndex, endIndex] <= K , in o(log^2(N)) time & o(n log(N)) memory
    public class SegmentTreeCountInferiorToK
    {
        #region private fields
        private readonly int _dataLength;
        private readonly List<int>[] _sortedElementsAtEachNode; // sortedElementsAtEachNode[0]: all sorted elements
        #endregion

        public SegmentTreeCountInferiorToK(int[] data)
        {
            _dataLength = data.Length;
            int power = 1 + (int)Math.Ceiling(Math.Log(data.Length) / Math.Log(2));
            int heapLength = (int)Math.Pow(2, power);
            _sortedElementsAtEachNode = new List<int>[1 + heapLength];
            ComputeHeapForNbInfToK(0, 0, data.Length - 1, data);
        }

        /// <summary>
        /// retrieve the number of elements in interval [startIndex, endIndex] less or equal to K
        /// Complexity:         o(log^2(N))
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public int Query(int startIndex, int endIndex, int k) { return Query(0, 0, _dataLength - 1, k, startIndex, endIndex); }


        /// <summary>
        /// retrieve the number of elements in interval [startIndex, endIndex] equals to K
        /// Complexity:         o( log^2(N) )
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="k"></param>
        /// <returns>the number of elements equals to K in interval</returns>
        public int QueryEqualsToK(int startIndex, int endIndex, int k)
        {
            return Query(startIndex, endIndex, k) - Query(startIndex, endIndex, k-1);
        }


        private List<int> ComputeHeapForNbInfToK(int segmentId, int segmentStartIndex, int segmentEndIndex, int[] data)
        {
            if (segmentStartIndex == segmentEndIndex)
            {
                _sortedElementsAtEachNode[segmentId] = new List<int> { data[segmentStartIndex] };
            }
            else
            {
                var mid = (segmentStartIndex + segmentEndIndex) / 2;
                _sortedElementsAtEachNode[segmentId] = MergeSort(ComputeHeapForNbInfToK(2 * segmentId + 1, segmentStartIndex, mid, data), ComputeHeapForNbInfToK(2 * segmentId + 2, mid + 1, segmentEndIndex, data));
            }
            return _sortedElementsAtEachNode[segmentId];
        }

        /// <summary>
        /// we have 2 lists in increasing order : ' a' & 'b', and we want to merge the 2 list to a single increasing list
        /// Complexity:         o(a.Length + b.Length)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
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
        private int Query(int segmentId, int segmentStartIndex, int segmentEndIndex, int k, int startIndex, int endIndex)
        {
            if (segmentStartIndex >= startIndex && segmentEndIndex <= endIndex)
            {
                var sortedElementsInSegment = _sortedElementsAtEachNode[segmentId];
                if (sortedElementsInSegment[0] > k)
                {
                    return 0;
                }
                return 1 + Utils.MaximumValidIndex(0, sortedElementsInSegment.Count - 1, x => sortedElementsInSegment[x] <= k);
            }
            int mid = (segmentStartIndex + segmentEndIndex) / 2;
            if (mid < startIndex) //only right part of 'segmentId' intersects [startIndex, endIndex] and contributes to the result
            {
                return Query(2 * segmentId + 2, mid + 1, segmentEndIndex, k, startIndex, endIndex);
            }
            if (mid >= endIndex) //only left part of 'segmentId' intersects [startIndex, endIndex] and contributes to the result
            {
                return Query(2 * segmentId + 1, segmentStartIndex, mid, k, startIndex, endIndex);
            }
            //both left part and right part of 'segmentId' contribute to the result
            return Query(2 * segmentId + 1, segmentStartIndex, mid, k, startIndex, endIndex) + Query(2 * segmentId + 2, mid + 1, segmentEndIndex, k, startIndex, endIndex);
        }
    }
    #endregion

    #region Euler Tour of binary tree
    public class EulerTourOnBinaryTree<T>
    {
        //list of vertices found during an Euler Tour: (starting&ending at root) and visiting all vertices (from left to right)
        private List<T> EulerTour { get; }
        //EulerTourHeight[i]: distance between root and vertex 'EulerTour[i]'  (= 0 if vertex is the root of the tree)
        private readonly List<int> _eulerTourHeight;
        //FirstIndexInEulerTour[v]: first time we met vertex 'v' in euler path
        private IDictionary<T, int> FirstIndexInEulerTour { get; }
        private SparseTable _lazySparseTableIndexOfMin; //only used to compute Lowest Common Ancestor 

        public EulerTourOnBinaryTree(Graph<T> tree, T rootId)
        {
            // get euler tour & indices of first occurence
            FirstIndexInEulerTour = new Dictionary<T, int>();
            EulerTour = new List<T>();
            _eulerTourHeight = new List<int>();

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
                _eulerTourHeight.Add(h);
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
            if (_lazySparseTableIndexOfMin == null)
            {
                _lazySparseTableIndexOfMin = SparseTable.IndexOfMin(_eulerTourHeight.ToArray());
            }
            var idxOfMinHeightInEulerPath = _lazySparseTableIndexOfMin.Query(Math.Min(id1, id2), Math.Max(id1, id2));
            return EulerTour[idxOfMinHeightInEulerPath];
        }
        public int NearestToRoot(int iInEulerTour, int jInEulerTour)
        {
            return _eulerTourHeight[iInEulerTour] < _eulerTourHeight[jInEulerTour] ? iInEulerTour : jInEulerTour;
        }
    }
    #endregion
}
