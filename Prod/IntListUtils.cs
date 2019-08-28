using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable StringCompareToIsCultureSpecific
// ReSharper disable UnusedMember.Global

namespace SharpAlgos
{
    public static partial class Utils
    {
        /// <summary>
        /// Compute the max sub sum from 'T' in o(N) time and o(1) memory
        /// the sub array indexes are stored in [startIndexMasSubSum,endIndexMaxSubSum]
        /// </summary>
        /// <param name="T"></param>
        /// <param name="allowEmptyElement"></param>
        /// <param name="startIndexMasSubSum"></param>
        /// <param name="endIndexMaxSubSum"></param>
        /// <returns></returns>
        public static int MaxSubSum(int[] T, bool allowEmptyElement, out int startIndexMasSubSum, out int endIndexMaxSubSum)
        {
            endIndexMaxSubSum = 0;
            int previousMaxSum = T[0];
            int maxSubSum = previousMaxSum;
            for (int index = 1; index < T.Length; index++)
            {
                previousMaxSum = Math.Max(T[index], previousMaxSum + T[index]);
                if (previousMaxSum > maxSubSum)
                {
                    endIndexMaxSubSum = index;
                    maxSubSum = previousMaxSum;
                }
            }
            if ((maxSubSum < 0) && allowEmptyElement)
            {
                startIndexMasSubSum = endIndexMaxSubSum = 0;
                return 0;
            }
            startIndexMasSubSum = endIndexMaxSubSum;
            int tmpSum = T[endIndexMaxSubSum];
            while ((tmpSum < maxSubSum) && (startIndexMasSubSum >= 1))
            {
                --startIndexMasSubSum;
                tmpSum += T[startIndexMasSubSum];
            }
            return maxSubSum;
        }



        /// <summary>
        /// Compute max subsequence without using adjacent item in o(N) time (and o(N) memory)
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int MaximumSubsequenceSumWithNoAdjacentElement(int[] m)
        {
            var res = new int[m.Length];
            res[0] = m[0];
            res[1] = Math.Max(m[0], m[1]);
            for (int i = 2; i < m.Length; ++i)
            {
                res[i] = Math.Max(m[i], Math.Max(m[i] + res[i - 2], res[i - 1]));
            }
            return res.Last();
        }

        /// <summary>
        /// return all longest intervals containing exactly 'k' distinct elements in o(n) time
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static List<Tuple<int, int>> LongestIntervalsContainingExactly_K_DistinctItems(int[] data, int k)
        {
            var result = new List<Tuple<int, int>>();
            int i = 0;
            int j = 0;
            int dist = 0;
            var itemToCountInCurrentInterval = new Dictionary<int, int>();
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
                    result.Add(Tuple.Create(i, j-1));
                }
            }
            return result;
        }

        /// <summary>
        /// Compute the largest rectangle inside an histogram in o(N) time (and o(N) memory)
        /// the start & end indexes are stored in [startIndex,endIndex]
        /// </summary>
        /// <param name="heights">list height in the histogram</param>
        /// <param name="startIndex">start index of the largest rectangle</param>
        /// <param name="endIndex">end index of the largest rectangle</param>
        /// <returns></returns>
        public static int LargestRectangleArea(int[] heights, out int startIndex, out int endIndex)
        {
            startIndex = endIndex = 0;
            var h = new List<int> { 0 };
            h.AddRange(heights);
            h.Add(0);

            var indexes = new List<int>();
            indexes.Add(0);
            int maxArea = 0;
            for (int i = 1; i < h.Count; ++i)
            {
                while (h[i] < h[indexes.Last()])
                {
                    int h0 = h[indexes.Last()];
                    indexes.RemoveAt(indexes.Count - 1);
                    var currentArea = h0 * (i - indexes.Last() - 1);
                    if (currentArea > maxArea)
                    {
                        endIndex = i;
                        startIndex = indexes.Last();
                        maxArea = Math.Max(maxArea, currentArea);
                    }
                }
                indexes.Add(i);
            }
            return maxArea;
        }

        //compute max of 'm[i]-m[j]+m[k]-m[l]' with i>j>k>l in o(n) time
        public static int MaximizeValueOfTheExpression(int[] m)
        {
            var L1 = Enumerable.Repeat(int.MinValue, m.Length).ToArray(); //L1[t] : max of m[i] for i>=t
            L1[m.Length - 1] = m.Last();
            for (int i = m.Length - 2; i >= 0; --i)
            {
                L1[i] = Math.Max(L1[i + 1], m[i]);
            }
            var L2 = Enumerable.Repeat(int.MinValue, m.Length).ToArray(); //L2[t] : max of m[i]-m[j] for j>=t & i>j
            for (int i = m.Length - 2; i >= 0; --i)
            {
                L2[i] = Math.Max(L2[i + 1], L1[i + 1] - m[i]);
            }
            var L3 = Enumerable.Repeat(int.MinValue, m.Length).ToArray(); //L3[t] : max of m[i]-m[j]+m[k] for k>=t & i>j>k
            for (int i = m.Length - 3; i >= 0; --i)
            {
                L3[i] = Math.Max(L3[i + 1], L2[i + 1] + m[i]);
            }
            var L4 = Enumerable.Repeat(int.MinValue, m.Length).ToArray(); //L4[t] : max of m[i]-m[j]+m[k]-m[l] for l>=t & i>j>k>l
            for (int i = m.Length - 4; i >= 0; --i)
            {
                L4[i] = Math.Max(L4[i + 1], L3[i + 1] - m[i]);
            }
            return L4.First();
        }

        //try to 2 divide 'm' into 2 parts so the difference of the sum of elements in each part is minimum
        public static int MinimumSumPartition(int[] m)
        {
            return MinimumSumPartition_Helper(m, m.Length, 0, 0, new Dictionary<string, int>());
        }
        private static int MinimumSumPartition_Helper(int[] m, int nbRemaining, int S1, int S2, IDictionary<string, int> cache)
        {
            if (nbRemaining <= 0)
            {
                return Math.Abs(S1 - S2);
            }
            string key = nbRemaining + "|" + S1;
            if (!cache.ContainsKey(key))
            {
                var minimumSumIfAddedInS1 = MinimumSumPartition_Helper(m, nbRemaining - 1, S1 + m[nbRemaining - 1], S2, cache);
                var minimumSumIfAddedInS2 = MinimumSumPartition_Helper(m, nbRemaining - 1, S1, S2 + m[nbRemaining - 1], cache);
                cache[key] = Math.Min(minimumSumIfAddedInS1, minimumSumIfAddedInS2);
            }
            return cache[key];

        }

        //check if we can extract a subset of 'm' with sum = 'sum' in o(sum*m.Length) time (&memory)
        public static bool CanExtractSubsetEqualToSum(int[] m, int sum)
        {
            var cache = new bool[1 + m.Length, 1 + sum];
            for (int i = 0; i <= m.Length; ++i)
            {
                cache[i, 0] = true;
            }
            for (int i = 1; i <= m.Length; ++i)
                for (int amount = 1; amount <= sum; ++amount)
                {
                    cache[i, amount] = cache[i - 1, amount] || ((m[i - 1] <= amount) && cache[i - 1, amount - m[i - 1]]);
                }
            return cache[m.Length, sum];
        }

        public static bool CanBeDividedInto_2_SubsetWithEqualSum(int[] m)
        {
            return m.Sum() % 2 == 0 && CanBeDividedInto_2_SubsetWithEqualSum_Helper(m, new bool?[1 + m.Sum(), 1 + m.Length], m.Sum() / 2, m.Length);
        }
        private static bool CanBeDividedInto_2_SubsetWithEqualSum_Helper(int[] m, bool?[,] cache, int remainingSumInFirstSubset, int nbAuthorized)
        {
            if (nbAuthorized <= 0)
            {
                return remainingSumInFirstSubset == 0;
            }
            if (remainingSumInFirstSubset < 0)
            {
                return false;
            }
            if (cache[remainingSumInFirstSubset, nbAuthorized].HasValue)
            {
                return cache[remainingSumInFirstSubset, nbAuthorized].Value;
            }
            cache[remainingSumInFirstSubset, nbAuthorized] = CanBeDividedInto_2_SubsetWithEqualSum_Helper(m, cache, remainingSumInFirstSubset, nbAuthorized - 1) || CanBeDividedInto_2_SubsetWithEqualSum_Helper(m, cache, remainingSumInFirstSubset - m[nbAuthorized - 1], nbAuthorized - 1);
            return cache[remainingSumInFirstSubset, nbAuthorized].Value;
        }

        //check if we can divide 'm' array into 'K' subset with equal sum in o(N^(K+1)) time
        public static bool CanBeDividedInto_K_SubsetWithEqualSum(int[] m, int K, out List<int>[] valuesForEachSubset)
        {
            var sum = m.Sum();
            valuesForEachSubset = new List<int>[K];
            for(int k=0;k<K;++k)
            {
                valuesForEachSubset[k] = new List<int>();
            }
            return (sum % K == 0) && CanBeDividedInto_K_SubsetWithEqualSum_Helper(m, m.Length, Enumerable.Repeat(sum / K, K).ToArray(), valuesForEachSubset);
        }
        private static bool CanBeDividedInto_K_SubsetWithEqualSum_Helper(int[] m, int nbAuthorized, int[] remainingSum, List<int>[] valuesForEachSubset)
        {
            if (nbAuthorized <= 0)
            {
                return remainingSum.All(x => x == 0);
            }
            for(int k=0;k<remainingSum.Length;++k)
            {
                var toRemove = m[nbAuthorized - 1];
                if (remainingSum[k] < toRemove)
                {
                    continue;
                }
                //if (k >= 1 && remainingSum[k] == remainingSum[k - 1]) continue;
                remainingSum[k] -= toRemove;
                var result = CanBeDividedInto_K_SubsetWithEqualSum_Helper(m, nbAuthorized - 1, remainingSum, valuesForEachSubset);
                remainingSum[k] += toRemove;
                if (result)
                {
                    valuesForEachSubset[k].Add(toRemove);
                    return true;
                }
            }
            return false;
        }

        public static int LongestAlternatingSubsequence(int[] values)
        {
            var longestUp = new int[values.Length];
            var longestDown = new int[values.Length];
            for (int end = 0; end < values.Length; ++end)
            {
                longestUp[end] = longestDown[end] = 1;
                for (int j = 0; j < end; ++j)
                {
                    if (values[j] < values[end])
                    {
                        longestUp[end] = Math.Max(longestUp[end], 1 + longestDown[j]);
                    }
                    if (values[j] > values[end])
                    {
                        longestDown[end] = Math.Max(longestDown[end], 1 + longestUp[j]);
                    }
                }
            }
            return Math.Max(longestDown.Max(), longestUp.Max());
        }

        #region Longest Increasing Subsequence (LIS)
        //compute length of 'Longest Increasing Sub Sequence' in o( N*Log(N) ) (an o(N) memory)
        public static int LengthOfLongestIncreasingSubsequence(IEnumerable<int> data)
        {
            var minValueEndingAt = new List<int>();
            foreach (int t in data)
            {
                int idx = minValueEndingAt.BinarySearch(t);
                //uncomment to allow equality in sub sequence
                //if (idx == minValueEndingAt.Count - 1) ++idx; 
                if (idx < 0)
                {
                    idx = ~idx;
                }
                if (idx >= minValueEndingAt.Count)
                {
                    minValueEndingAt.Add(t);
                }
                else
                {
                    minValueEndingAt[idx] = t;
                }
            }
            return minValueEndingAt.Count;
        }


        //compute 'Longest Increasing Sub Sequence' in o( N*Log(N) ) (an o(N) memory)
        public static List<int> LongestIncreasingSubsequence(IList<int> data, bool allowEquality)
        {
            var prevIndexes = new List<int>();
            var indexMinValueEndingAt = new List<int>();
            // minValueEndingAt[l] , the minimal value for the last element of an increasing sub sequence of length 'l'
            var minValueEndingAt = new List<int>();
            if (data.Count <= 1)
            {
                return new List<int>(data);
            }
            for (int i = 0; i < data.Count; ++i)
            {
                int idx = minValueEndingAt.BinarySearch(data[i]);
                if (allowEquality && (idx == minValueEndingAt.Count - 1))
                {
                    ++idx;
                }
                if (idx < 0)
                {
                    idx = ~idx;
                }
                if (idx >= minValueEndingAt.Count)
                {
                    minValueEndingAt.Add(data[i]);
                    indexMinValueEndingAt.Add(i);
                }
                else
                {
                    minValueEndingAt[idx] = data[i];
                    indexMinValueEndingAt[idx] = i;
                }
                prevIndexes.Add(idx >= 1 ? indexMinValueEndingAt[idx - 1] : -1);
            }

            var result = new List<int>();
            int prevIndex = indexMinValueEndingAt.Last();
            while (prevIndex != -1)
            {
                result.Add(data[prevIndex]);
                prevIndex = prevIndexes[prevIndex];
            }

            result.Reverse();
            return result;
        }

        #endregion

        #region Longest Bitonic Subsequence: find the longest increasing then decreasing subsequence in o(n^2) time
        public static List<int> LongestBitonicSubsequence(int[] data)
        {
            if (data.Length == 0)
            {
                return new List<int>();
            }
            //maxLengthFromEachSide[0][i] : length of maximum increasing subsequence starting from left and ending at 'i'
            //maxLengthFromEachSide[1][i] : length of maximum increasing subsequence starting from right and ending at 'i'
            var maxLengthFromEachSide = new List<int[]>();
            for (int side = 0; side <= 1; ++side)
            {
                //maxLength[i] : length of maximum increasing subsequence ending at 'i'
                var maxLength = new int[data.Length];
                for (int end = 0; end < data.Length; ++end)
                {
                    maxLength[end] = 1;
                    for (int j = 0; j < end; ++j)
                    {
                        if (data[j] < data[end])
                        {
                            maxLength[end] = Math.Max(maxLength[end], 1 + maxLength[j]);
                        }
                    }
                }
                maxLengthFromEachSide.Add(maxLength);
                Array.Reverse(data);
            }

            //we look for the index where we reach the max length of 'increasing sequence from left'+'increasing sequence from right'
            int indexOfMiddleOfLongestBitonicSubsequence = 0;
            int lengthOfLongestBitonicSubsequence = 0;
            for (int i = 0; i < data.Length; ++i)
            {
                int currentLength = maxLengthFromEachSide[0][i] + maxLengthFromEachSide[1][data.Length - i - 1] - 1;
                if (currentLength > lengthOfLongestBitonicSubsequence)
                {
                    indexOfMiddleOfLongestBitonicSubsequence = i;
                    lengthOfLongestBitonicSubsequence = currentLength;
                }
            }
            //uncomment the following line to return the length of the Longest Bitonic Subsequence
            //return lengthOfLongestBitonicSubsequence;

            var result = new List<int>();
            result.Add(data[indexOfMiddleOfLongestBitonicSubsequence]);
            foreach (var maxLength in maxLengthFromEachSide)
            {
                result.Reverse();
                var indexLeft = indexOfMiddleOfLongestBitonicSubsequence;
                for (int i = indexLeft - 1; i >= 0; --i)
                {
                    if (data[i] < data[indexLeft] && maxLength[indexLeft] == maxLength[i] + 1)
                    {
                        result.Add(data[i]);
                        indexLeft = i;
                    }
                }
                Array.Reverse(data);
                indexOfMiddleOfLongestBitonicSubsequence = data.Length - indexOfMiddleOfLongestBitonicSubsequence - 1;
            }
            return result;
        }

        #endregion

        #region Maximum Sum Increasing Subsequence: find the increasing subsequence with the maximum sum in o(n^2) time
        public static List<int> MaximumSumIncreasingSubsequence(int[] data)
        {
            if (data.Length == 0)
            {
                return new List<int>();
            }
            var maxSumEndingAtIndex = new int[data.Length];
            var prevIndex = Enumerable.Repeat(-1, data.Length).ToArray();
            for (int end = 0; end < data.Length; ++end)
            {
                for (int j = 0; j < end; ++j)
                {
                    if (data[j] < data[end] && maxSumEndingAtIndex[j] > maxSumEndingAtIndex[end])
                    {
                        maxSumEndingAtIndex[end] = maxSumEndingAtIndex[j];
                        prevIndex[end] = j;
                    }
                }
                maxSumEndingAtIndex[end] += data[end];
            }
            //return maxSumEndingAtIndex.Max(); //uncomment this line to return the max sum

            var indexMaxSum = Array.IndexOf(maxSumEndingAtIndex, maxSumEndingAtIndex.Max());
            var result = new List<int>();
            while (indexMaxSum != -1)
            {
                result.Add(data[indexMaxSum]);
                indexMaxSum = prevIndex[indexMaxSum];
            }
            result.Reverse();
            return result;
        }
        #endregion

        public static int TrapRainWater2D(int[] height)
        {
            if (height.Length <= 2)
            {
                return 0;
            }
            var maxLeft = new int[height.Length];
            maxLeft[0] = height[0];
            for (int i = 1; i < height.Length; ++i)
            {
                maxLeft[i] = Math.Max(maxLeft[i - 1], height[i]);
            }
            var maxRight = new int[height.Length];
            maxRight[maxRight.Length - 1] = height.Last();
            for (int i = height.Length - 2; i >= 0; --i)
            {
                maxRight[i] = Math.Max(maxRight[i + 1], height[i]);
            }
            int result = 0;
            for (int i = 1; i < height.Length - 1; ++i)
            {
                int hLeft = maxLeft[i - 1];
                int hRight = maxRight[i + 1];
                int min = Math.Min(hLeft, hRight);
                int h = height[i];
                if (h < min)
                {
                    result += (min - h);
                }
            }
            return result;
        }

        public static bool CanJump(int[] nums)
        {
            int maxIndex = 0;
            for (int i = 0; i <= maxIndex; ++i)
            {
                maxIndex = Math.Max(maxIndex, i + nums[i]);
                if (maxIndex >= nums.Length - 1)
                {
                    return true;
                }
            }
            return false;
        }

        //find the biggest number using alls 'parts' in o(n log(n)) time
        // for exemple: from 12 56 3 , the biggest number is 56312
        public static string BuildBiggestNumberUsingAllParts(int[] parts)
        {
            var elements = parts.Select(x => x.ToString()).ToList();
            elements.Sort((x, y) => (y+x).CompareTo(x+y));
            return string.Join("", elements);
        }

        //find the biggest number using a subsequence of k elements from 'parts'  in o(?) time
        public static List<int> BuildBiggestNumberUsingSubsequenceOfKElements(int[] parts, int k)
        {
            var result = new List<int>();
            for (var i = 0; i < parts.Length; i++)
            {
                while (result.Count != 0 && result.Last() < parts[i] &&  ( result.Count + (parts.Length - i))>k )
                {
                    result.RemoveAt(result.Count - 1);
                }
                if (result.Count<k)
                {
                    result.Add(parts[i]);
                }
            }
            return result; //to return the biggest number: return string.Join("", result);
        }

    }
}