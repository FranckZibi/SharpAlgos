﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        /// <summary>
        /// Compute the max sub sum from 'T'
        /// Complexity:         o( T.Length ) 
        /// Memory Complexity:  o( 1 ) 
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
        /// Compute max subsequence without using adjacent item
        /// Complexity:         o( m.Length ) 
        /// Memory Complexity:  o( m.Length ) 
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
        /// Compute the largest rectangle inside an histogram
        /// the start & end indexes are stored in [startIndex,endIndex]
        /// Complexity:         o( heights.Length ) 
        /// Memory Complexity:  o( heights.Length ) 
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

        /// <summary>
        /// Compute max of 'm[i]-m[j]+m[k]-m[l]' with i>j>k>l
        /// Complexity:         o( m.Length ) 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int MaximizeValueOfTheExpression(int[] m)
        {
            var l1 = Enumerable.Repeat(int.MinValue, m.Length).ToArray(); //L1[t] : max of m[i] for i>=t
            l1[m.Length - 1] = m.Last();
            for (int i = m.Length - 2; i >= 0; --i)
            {
                l1[i] = Math.Max(l1[i + 1], m[i]);
            }
            var l2 = Enumerable.Repeat(int.MinValue, m.Length).ToArray(); //L2[t] : max of m[i]-m[j] for j>=t & i>j
            for (int i = m.Length - 2; i >= 0; --i)
            {
                l2[i] = Math.Max(l2[i + 1], l1[i + 1] - m[i]);
            }
            var l3 = Enumerable.Repeat(int.MinValue, m.Length).ToArray(); //L3[t] : max of m[i]-m[j]+m[k] for k>=t & i>j>k
            for (int i = m.Length - 3; i >= 0; --i)
            {
                l3[i] = Math.Max(l3[i + 1], l2[i + 1] + m[i]);
            }
            var l4 = Enumerable.Repeat(int.MinValue, m.Length).ToArray(); //L4[t] : max of m[i]-m[j]+m[k]-m[l] for l>=t & i>j>k>l
            for (int i = m.Length - 4; i >= 0; --i)
            {
                l4[i] = Math.Max(l4[i + 1], l3[i + 1] - m[i]);
            }
            return l4.First();
        }

        //try to 2 divide 'm' into 2 parts so the difference of the sum of elements in each part is minimum
        public static int MinimumSumPartition(int[] m)
        {
            return MinimumSumPartition_Helper(m, m.Length, 0, 0, new Dictionary<string, int>());
        }
        private static int MinimumSumPartition_Helper(int[] m, int nbRemaining, int s1, int s2, IDictionary<string, int> cache)
        {
            if (nbRemaining <= 0)
            {
                return Math.Abs(s1 - s2);
            }
            string key = nbRemaining + "|" + s1;
            if (!cache.ContainsKey(key))
            {
                var minimumSumIfAddedInS1 = MinimumSumPartition_Helper(m, nbRemaining - 1, s1 + m[nbRemaining - 1], s2, cache);
                var minimumSumIfAddedInS2 = MinimumSumPartition_Helper(m, nbRemaining - 1, s1, s2 + m[nbRemaining - 1], cache);
                cache[key] = Math.Min(minimumSumIfAddedInS1, minimumSumIfAddedInS2);
            }
            return cache[key];

        }

        /// <summary>
        /// check if we can extract a subset of 'm' with sum = 'sum'
        /// Complexity:         o( sum*m.Length )
        /// Memory Complexity:  o( sum*m.Length )
        /// </summary>
        /// <param name="m"></param>
        /// <param name="sum"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check if we can divide 'm' array into 'K' subset with equal sum
        /// Complexity:         o( N^(K+1) )
        /// </summary>
        /// <param name="m"></param>
        /// <param name="K"></param>
        /// <param name="valuesForEachSubset"></param>
        /// <returns></returns>
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

        /// <summary>
        /// compute 'Longest Increasing Sub Sequence' (LIS)
        /// Complexity:         o( N*Log(N) )
        /// Memory Complexity:  o( N )
        /// </summary>
        /// <param name="data"></param>
        /// <param name="allowEquality"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Compute length of 'Longest Increasing Sub Sequence'
        /// Complexity:         o( N*Log(N) )
        /// Memory Complexity:  o( N )
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Longest Bitonic Subsequence: Longest increasing then decreasing subsequence
        /// Complexity:         o( n^2 )
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Maximum Sum Increasing Subsequence: increasing subsequence with the maximum sum
        /// Complexity:         o( n^2 )
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// find the biggest number using all 'parts' in o(n log(n)) time
        /// for example: from 12 56 3 , the biggest number is 56312
        /// Complexity:         o(n log(n))
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string BuildBiggestNumberUsingAllParts(int[] parts)
        {
            var elements = parts.Select(x => x.ToString()).ToList();
            elements.Sort((x, y) => string.Compare((y+x), x+y, StringComparison.Ordinal));
            return string.Join("", elements);
        }

        /// <summary>
        /// find the biggest number using a subsequence of k elements from 'parts'
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="k"></param>
        /// <returns></returns>
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
