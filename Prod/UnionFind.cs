
using System.Collections.Generic;

namespace SharpAlgos
{
    public class UnionFind<T>
    {
        #region  private fields
        private readonly IDictionary<T, int> _ranks = new Dictionary<T, int>();
        // _childToParent[t] == t means that 't' is a root
        private readonly IDictionary<T, T> _childToParent = new Dictionary<T, T>();
        public int Count { get { return _childToParent.Count; } }
        public int DistinctFamilyCount {get; private set;}
        #endregion

        /// <summary>
        /// join the set containing 'a' with the one containing 'b'
        /// Complexity:         o(1)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>false if 'a' and 'b' are already in the same set
        /// true if they were not </returns>
        public bool Union(T a, T b)
        {
            var rootA = Find(a);
            var rootB = Find(b);
            if (Equals(rootA, rootB))
            {
                return false; //already connected before
            }
            --DistinctFamilyCount;
            var deltaRank = _ranks[rootA] - _ranks[rootB];
            if (deltaRank >= 0)
            {
                _childToParent[rootB] = rootA;
                if (deltaRank == 0)
                {
                    ++_ranks[rootA];
                }
            }
            else
            {
                _childToParent[rootA] = rootB;
            }
            return true; //newly connected
        }

        /// <summary>
        /// finds the root of the set containing 't'
        /// (so it will be 't' itself if 't' is alone in the set)
        /// Complexity:         o(1)
        /// </summary>
        /// <param name="t">the item we want to extract the root</param>
        /// <returns>the root of 't'</returns>
        public T Find(T t) 
        {
            if (Add(t)) //new node
            {
                return t;
            }
            var currentParent = _childToParent[t];
            if (Equals(t, currentParent))
            {
                return currentParent; //'t' is already a root node
            }
            //we find the root node of 't' and change its direct parent to be the root of the node
            _childToParent[t] = Find(currentParent);
            return _childToParent[t];
        }

        /// <summary>
        /// Add element 't' in the structure
        /// Complexity:         o(1)
        /// </summary>
        /// <param name="t">the element to add</param>
        /// <returns>true if the element was successfully added
        /// false if it was already present</returns>
        public bool Add(T t)
        {
            if (_childToParent.ContainsKey(t))
            {
                return false; //already present
            }
            _ranks[t] = 0;
            _childToParent[t] = t;
            ++DistinctFamilyCount;
            return true;
        }
    }
}
