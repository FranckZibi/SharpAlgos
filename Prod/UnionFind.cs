using System.Collections.Generic;

namespace SharpAlgos
{
    public class UnionFind<T>
    {
        #region  private fields
        private readonly IDictionary<T, int> ranks = new Dictionary<T, int>();
        private readonly IDictionary<T, T> childToParent = new Dictionary<T, T>();
        public int Count { get { return childToParent.Count; } }
        public int DistinctFamilyCount {get; private set;}
        #endregion

        /// <summary>
        /// join the set containing 'a' with the one containing 'b' in o(1) time. 
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
            var deltaRank = ranks[rootA] - ranks[rootB];
            if (deltaRank >= 0)
            {
                childToParent[rootB] = rootA;
                if (deltaRank == 0)
                {
                    ++ranks[rootA];
                }
            }
            else
            {
                childToParent[rootA] = rootB;
            }
            return true; //newly connected
        }

        /// <summary>
        /// finds the root of the set containing 't' in o(1) time
        /// (so it will be 't' itself if 't' is alone in the set)
        /// </summary>
        /// <param name="t">the item we want to extract the root</param>
        /// <returns>the root of 't'</returns>
        public T Find(T t) 
        {
            if (Add(t)) //new node
            {
                return t;
            }
            var currentParent = childToParent[t];
            if (Equals(t, currentParent))
            {
                return currentParent; //'t' is already a root node
            }
            //we find the root node of 't' and change its direct parent to be the root of the node
            childToParent[t] = Find(currentParent);
            return childToParent[t];
        }

        /// <summary>
        /// Add element 't' in the structure
        /// </summary>
        /// <param name="t">the element to add</param>
        /// <returns>true if the element was successfully added
        /// false if it was already present</returns>
        public bool Add(T t)
        {
            if (childToParent.ContainsKey(t))
            {
                return false; //already present
            }
            ranks[t] = 0;
            childToParent[t] = t;
            ++DistinctFamilyCount;
            return true;
        }
    }
}
