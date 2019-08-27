using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        #region Longest Palindrome

        /// <summary>
        /// Find longest palindrome in string 's' in o(Length) time (+o(Length) memory) using Manacher algo
        /// </summary>
        /// <param name="s">input string where we want to find the longest palindrome</param>
        /// <returns>
        /// Tuple.Item1: length of the longest palindrome
        /// Tuple.item2: start index of the longest palindrome
        /// </returns>
        public static Tuple<int, int> LongestPalindromeManacher(string s)
        {
            return LongestPalindromeManacher(s.ToArray(), '^', '#', '$');
        }

        private static Tuple<int, int> LongestPalindromeManacher<T>(IReadOnlyList<T> s, T start, T middle, T end)
        {
            var t = new T[2 + 2 * s.Count + 1];
            t[0] = start;
            t[1] = middle;
            for (var index = 0; index < s.Count; index++)
            {
                t[2+2*index] = s[index];
                t[2+2*index+1] = middle;
            }
            t[t.Length - 1] = end;

            //p[i] length of the longest palindrome centered on 'i'
            //It starts in 's' at index: (i-1-p[i])/2
            var p = new int[t.Length]; 
            int maxIndex = 1;
            int c = 0, d = 0;
            for (int i = 1; i < t.Length - 1; i++)
            {
                var mirror = c - (i - c);
                if (d > i)
                {
                    p[i] = Math.Min(d - i, p[mirror]);
                }
                while (Equals(t[i + 1 + p[i]], t[i - 1 - p[i]]))
                {
                    p[i]++;
                }
                if (p[i] > p[maxIndex])
                {
                    maxIndex = i;
                }
                if (i + p[i] > d)
                {
                    c = i;
                    d = i + p[i];
                }
            }

            var longestPalindromeLength = p[maxIndex];
            return Tuple.Create(longestPalindromeLength, (maxIndex - 1 - longestPalindromeLength) / 2);
        }

        /// <summary>
        /// Find longest palindrome in 's' in o (a.Length * Log(a.Length)  time
        /// </summary>
        /// <returns>
        /// Item1: length od the maximum palindrome
        /// Item2: index of the first palindrome with this length
        /// </returns>
        public static Tuple<int, int> LongestPalindromeWithHash(string s)
        {
            return ArrayHashComputer<char>.LongestPalindrome(s.ToList(), x => x);
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