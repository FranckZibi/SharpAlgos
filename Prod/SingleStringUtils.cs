using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        #region Longest Palindromic Subsequence using LCS(word, word.Reverse())
        //given a string, finds a smaller string that is the longest palindromic subsequence it contains in o (length(a) * length(a) ) time
        public static List<T> LongestPalindromicSubsequence<T>(List<T> a)
        {
            return LongestCommonSubsequence(a, Enumerable.Reverse(a).ToList());
        }
        public static int LengthOfLongestPalindromicSubsequence<T>(List<T> a)
        {
            return LengthOfLongestCommonSubsequence(a, Enumerable.Reverse(a).ToList());
        }
        #endregion

        #region Longest repeated subsequence
        //We compute LCS(a, a) where we consider that a[i] is always different to itself (same index)
        public static List<T> LongestRepeatedSubsequence<T>(List<T> a)
        {
            var m = new int[1 + a.Count, 1 + a.Count];
            for (var lengthV1 = 1; lengthV1 <= a.Count; ++lengthV1) //for each line of the matrix
                for (var lengthV2 = 1; lengthV2 <= a.Count; ++lengthV2)
                {
                    m[lengthV1, lengthV2] = ((lengthV1 != lengthV2) && Equals(a[lengthV1 - 1], a[lengthV2 - 1])) ? (1 + m[lengthV1 - 1, lengthV2 - 1]) : Math.Max(m[lengthV1 - 1, lengthV2], m[lengthV1, lengthV2 - 1]);
                }
            var x = a.Count;
            var y = a.Count;
            var result = new List<T>();
            while (Math.Min(x, y) >= 1)
            {
                if ((x != y) && Equals(a[x - 1], a[y - 1]))
                {
                    result.Add(a[x - 1]);
                    --x;
                    --y;
                }
                else if (m[x - 1, y] > m[x, y - 1])
                {
                    --x;
                }
                else
                {
                    --y;
                }
            }
            result.Reverse();
            return result;
        }

        public static int LengthOfLongestRepeatedSubsequence<T>(List<T> a)
        {
            var prevLine = new int[1 + a.Count];
            var currentLine = new int[1 + a.Count];
            for (var lengthV1 = 1; lengthV1 <= a.Count; ++lengthV1) //for each sub sequence of 'a' (starting from 1st to 'aLength' caracter)
            {
                var tmp = currentLine; currentLine = prevLine; prevLine = tmp; //We swap previous and current line
                for (var lengthV2 = 1; lengthV2 <= a.Count; ++lengthV2)
                {
                    currentLine[lengthV2] = ((lengthV1 != lengthV2) && Equals(a[lengthV1 - 1], a[lengthV2 - 1])) ? (1 + prevLine[lengthV2 - 1]) : Math.Max(prevLine[lengthV2], currentLine[lengthV2 - 1]);
                }
            }
            return currentLine.Max();
        }
        #endregion



    }
}