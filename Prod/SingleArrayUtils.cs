using System;
using System.Collections.Generic;

namespace SharpAlgos
{
    public static partial class Utils
    {
        /// <summary>
        /// return all longest intervals containing exactly 'k' distinct elements
        /// Complexity:         o( n )
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static List<Tuple<int, int>> LongestIntervalsContainingExactly_K_DistinctItems<T>(T[] data, int k)
        {
            var result = new List<Tuple<int, int>>();
            int i = 0;
            int j = 0;
            int dist = 0;
            var itemToCountInCurrentInterval = new Dictionary<T, int>();
            foreach (var item in data)
            {
                itemToCountInCurrentInterval[item] = 0;
            }
            while (j < data.Length)
            {
                while (dist == k)
                {
                    //we reduce the interval by the left side, removing element at i (data[i])
                    --itemToCountInCurrentInterval[data[i]];
                    if (itemToCountInCurrentInterval[data[i]] == 0)
                    {
                        --dist;
                    }
                    ++i;
                }
                while (j < data.Length && (dist < k || itemToCountInCurrentInterval[data[j]] != 0))
                {
                    //we increase the interval by the right side, adding element at 'j' (data[j])
                    if (itemToCountInCurrentInterval[data[j]] == 0)
                    {
                        ++dist;
                    }
                    ++itemToCountInCurrentInterval[data[j]];
                    ++j;
                }
                if (dist == k)
                {
                    result.Add(Tuple.Create(i, j - 1));
                }
            }
            return result;
        }

        #region Duplicate detection
        /// <summary>
        /// find if there is a duplicate in interval [i,j]
        /// Complexity:         o( n )
        /// Memory Complexity:  o( n )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns>true if there is a duplicate in interval</returns>
        public static bool HasDuplicateInInterval<T>(T[] data, int i, int j)
        {
            var cache = ComputeCacheForFindDuplicate(data);
            return HasDuplicateInInterval(cache, i, j);
        }
        /// <summary>
        /// find if there is a duplicate in interval [i,j]
        /// Complexity:         o( 1 )    (+ o( n ) preparation time )
        /// Memory Complexity:  o( n ) 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns>true if there is a duplicate in interval</returns>
        public static bool HasDuplicateInInterval(Tuple<int[], int[]> cache, int i, int j)
        {
            var idxToMaxIdxWithDuplicates = cache.Item2;

            if (idxToMaxIdxWithDuplicates[j] >= i)
            {
                return true;
            }
            return false; //no duplicate found in  [i,j]

        }
        private static Tuple<int[],int[]> ComputeCacheForFindDuplicate<T>(T[] data)
        {
            var idxToLastIdxWithSameValue = new int[data.Length];
            var idxToMaxIdxWithDuplicates = new int[data.Length];
            var itemToPreviousIndex =  new Dictionary<T, int>();
            for (var i = 0; i < data.Length; i++)
            {
                int lastIndex;
                if (itemToPreviousIndex.TryGetValue(data[i], out lastIndex))
                {
                    idxToLastIdxWithSameValue[i] = lastIndex;
                }
                else
                {
                    idxToLastIdxWithSameValue[i] = -1;
                }
                itemToPreviousIndex[data[i]] = i;
                idxToMaxIdxWithDuplicates[i] =(i == 0) ? -1 : Math.Max(idxToMaxIdxWithDuplicates[i - 1], idxToLastIdxWithSameValue[i]);
            }
            return Tuple.Create(idxToLastIdxWithSameValue, idxToMaxIdxWithDuplicates);
        }
        #endregion
    }
}
