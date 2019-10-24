using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        /// <summary>
        /// Compute the Edit Distance between 'source' and 'target' in o (source.Length*target.Length) time,
        /// taking into account the cost of insertion/deletion/substitution and transposition
        /// Complexity:         o( source.Length * target.Length )
        /// Memory Complexity:  o( source.Length * target.Length )
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="insertionCost">insertion cost</param>
        /// <param name="deletionCost">deletion cost</param>
        /// <param name="substitutionCost">substitution cost</param>
        /// <param name="transpositionCost">transposition cost</param>
        /// <returns>minimum edit distance between 'a' and 'b'</returns>
        public static int EditDistance(string source, string target, int insertionCost, int deletionCost, int substitutionCost, int transpositionCost)
        {
            var matrix = new int[1 + source.Length, 1 + target.Length];
            //matrix[x,y] : edit distance from the first 'x' characters of source to the first 'y' characters of target
            for (var targetLength = 0; targetLength <= target.Length; ++targetLength)
            {
                matrix[0, targetLength] = targetLength * insertionCost;
            }
            for (var sourceLength = 0; sourceLength <= source.Length; ++sourceLength)
            {
                matrix[sourceLength, 0] = sourceLength * deletionCost;
            }
            for (var sourceLength = 1; sourceLength <= source.Length; ++sourceLength)
            {
                for (var targetLength = 1; targetLength <= target.Length; ++targetLength)
                {

                    var costForDeletion = deletionCost + matrix[sourceLength - 1, targetLength]; //remove last character from source then take above distance ('source minus last character' to 'target')
                    var costForInsertion = matrix[sourceLength, targetLength - 1] + insertionCost; //take left distance ('source' to 'target minus last character') then insert last character from source
                    var sameCharacter = source[sourceLength - 1] == target[targetLength - 1];
                    var costForSubstitution = matrix[sourceLength - 1, targetLength - 1] + (sameCharacter ? 0 : substitutionCost);
                    var costForTransposition = ((targetLength >= 2) && (sourceLength >= 2) && (source[sourceLength - 1] == target[targetLength - 2]) && (source[sourceLength - 2] == target[targetLength - 1]))
                                                ? (matrix[sourceLength - 2, targetLength - 2] + (sameCharacter ? 0 : transpositionCost))
                                                : int.MaxValue;
                    matrix[sourceLength, targetLength] = Math.Min(Math.Min(costForSubstitution, costForInsertion), Math.Min(costForDeletion, costForTransposition));
                }
            }
            return matrix[source.Length, target.Length];
        }

        #region Longest Common Subsequence (LCS)
        /// <summary>
        /// given 2 strings,
        /// finds a smaller string that is the longest sub sequence of both strings
        /// Complexity: o (a.Length * b.Length )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<T> LongestCommonSubsequence<T>(List<T> a, List<T> b)
        {
            var m = new int[1 + a.Count + 1, 1 + b.Count];
            for (var aLength = 1; aLength <= a.Count; ++aLength) //for each line of the matrix
            { 
                for (var bLength = 1; bLength <= b.Count; ++bLength)
                {
                    m[aLength, bLength] = Equals(a[aLength - 1], b[bLength - 1]) ? (1 + m[aLength - 1, bLength - 1]) : Math.Max(m[aLength - 1, bLength], m[aLength, bLength - 1]);
                }
            }
            var x = a.Count;
            var y = b.Count;
            var result = new List<T>();
            while (Math.Min(x, y) >= 1)
            {
                if (Equals(a[x - 1], b[y - 1]))
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

        public static int LengthOfLongestCommonSubsequence<T>(List<T> a, List<T> b)
        {
            if (a.Count < b.Count)
            {
                return LengthOfLongestCommonSubsequence(b, a);
            }
            //a is the longest sequence
            var prevLine = new int[1 + b.Count];
            var currentLine = new int[1 + b.Count];
            for (var aLength = 1; aLength <= a.Count; ++aLength) //for each sub sequence of 'a' (starting from 1st to 'aLength' character)
            {
                var tmp = currentLine; currentLine = prevLine; prevLine = tmp; //We swap previous and current line
                for (var bLength = 1; bLength <= b.Count; ++bLength)
                {
                    currentLine[bLength] = Equals(a[aLength - 1], b[bLength - 1]) ? (1 + prevLine[bLength - 1]) : Math.Max(prevLine[bLength], currentLine[bLength - 1]);
                }
            }
            return currentLine.Max();
        }

        public static List<List<T>> AllLongestCommonSubsequence<T>(List<T> a, List<T> b)
        {
            if ((a == null) || (a.Count == 0) || (b == null) || (b.Count == 0))
            {
                return new List<List<T>>();
            }
            var m = new int[1 + a.Count + 1, 1 + b.Count];
            for (var x = 1; x <= a.Count; ++x) //for each line of the matrix
                for (var y = 1; y <= b.Count; ++y)
                {
                    m[x,y] = Equals(a[x - 1], b[y - 1]) ? (1 + m[x - 1,y - 1]) : Math.Max(m[x - 1,y], m[x,y - 1]);
                }
            return AllLongestCommonSubsequence_Helper(m, a, b, a.Count, b.Count, new List<T>(), new List<List<T>>());
        }

        private static List<List<T>> AllLongestCommonSubsequence_Helper<T>(int[,] m, List<T> a, List<T> b, int x, int y, List<T> current, List<List<T>> result)
        {
            if (Math.Min(x, y) <= 0)
            {
                if (current.Count != 0)
                {
                    result.Add(current);
                }
                return result;
            }
            if (Equals(a[x - 1], b[y - 1]))
            {
                current.Insert(0, a[x - 1]);
                return AllLongestCommonSubsequence_Helper(m, a, b, x - 1, y - 1, current, result);
            }
            if (m[x - 1,y] == m[x,y-1])
            {
                AllLongestCommonSubsequence_Helper(m, a, b, x - 1, y, current.ToList(), result);
                return AllLongestCommonSubsequence_Helper(m, a, b, x, y - 1, current, result);
            }
            return m[x-1,y] > m[x,y-1] ? AllLongestCommonSubsequence_Helper(m, a, b, x - 1, y, current, result) : AllLongestCommonSubsequence_Helper(m, a, b, x, y - 1, current, result);
        }
        #endregion

        #region Longest Common Substring
        /// <summary>
        /// given 2 strings, finds a smaller string that is the longest substring they have in common (using hash)
        /// Complexity: o( a.Length * log(a.Length)) )
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string LongestCommonSubstring_With_Hash(string a, string b)
        {
            return new string(ArrayHashComputer<char>.LongestCommonSubArrayWithHash(a.ToList(), b.ToList(), x => x).ToArray());
        }

        /// <summary>
        /// given 2 strings, finds a smaller string that is the longest substring they have in common (using Dynamic Programming)
        /// Complexity: o ( a.Length * b.Length )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<T> LongestCommonSubstring_With_DP<T>(List<T> a, List<T> b)
        {
            var m = new int[1 + a.Count, 1 + b.Count];
            var xMax = 0;
            var max = 0;
            for (var x = 1; x <= a.Count; ++x) //for each line of the matrix
            {
                for (var y = 1; y <= b.Count; ++y)
                {
                    m[x, y] = Equals(a[x - 1], b[y - 1]) ? (1 + m[x - 1, y - 1]) : 0;
                    if (m[x, y] > max)
                    {
                        max = m[x, y];
                        xMax = x;
                    }
                }
            }
            return a.GetRange(xMax - max, max);
        }
        public static int LengthOfLongestCommonSubstring<T>(List<T> a, List<T> b)
        {
            if (a.Count < b.Count)
            {
                return LengthOfLongestCommonSubstring(b, a);
            }
            var prevLine = new int[1 + b.Count];
            var currentLine = new int[1 + b.Count];
            var result = 0;
            for (var x = 1; x <= a.Count; ++x) //for each sub sequence of 'a' (starting from 1st to 'x' caracter)
            {
                var tmp = currentLine; currentLine = prevLine; prevLine = tmp; //We swap previous and current line
                for (var y = 1; y <= b.Count; ++y)
                {
                    currentLine[y] = Equals(a[x - 1], b[y - 1]) ? (1 + prevLine[y - 1]) : 0;
                }
                result = Math.Max(result, currentLine.Max());
            }
            return result;
        }
        #endregion

        #region Shortest Common Supersequence (SCS)
        /// <summary>
        /// given 2 strings 'a'&'b', find (the smallest) bigger string 'scs'
        /// so that 'a'&'b' are subsequence of 'scs'
        /// Complexity: o(a.Length * b.Length )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<T> ShortestCommonSupersequence<T>(List<T> a, List<T> b)
        {
            return LCS_to_SCS(a, b, LongestCommonSubsequence(a, b));
        }
        private static List<T> LCS_to_SCS<T>(List<T> a, List<T> b, List<T> lcs)
        {
            var scs = new List<T>();
            var aNextIndex = 0;
            var bNextIndex = 0;
            var lcsNextIndex = 0;
            while (lcsNextIndex < lcs.Count)
            {
                while (!Equals(a[aNextIndex], lcs[lcsNextIndex]))
                {
                    scs.Add(a[aNextIndex++]);
                }
                ++aNextIndex;
                while (!Equals(b[bNextIndex], lcs[lcsNextIndex]))
                {
                    scs.Add(b[bNextIndex++]);
                }
                ++bNextIndex;
                scs.Add(lcs[lcsNextIndex++]);
            }
            while (bNextIndex < b.Count)
            {
                scs.Add(b[bNextIndex++]);
            }
            while (aNextIndex < a.Count)
            {
                scs.Add(a[aNextIndex++]);
            }
            return scs;
        }
        public static int LengthOfShortestCommonSupersequence<T>(List<T> a, List<T> b)
        {
            return a.Count + b.Count - LengthOfLongestCommonSubsequence(a, b);
        }
        #endregion
    }
}
