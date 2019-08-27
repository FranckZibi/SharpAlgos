using System;
using System.Collections.Generic;
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable UnusedMember.Global

namespace SharpAlgos
{
    public static partial class Utils
    {


        #region Duplicate detection
        //TODO: returns index of duplicates instead of true/false
        /// <summary>
        /// find if there is a duplicate in interval [i,j] in o(n) time (+o(n) memory)
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
        /// find if there is a duplicate in interval [i,j] in o(1) time (+o(n) preparation time & o(n) preparation memory)
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