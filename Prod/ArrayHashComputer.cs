using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpAlgos
{
    /// <summary>
    /// Computes the hash of a any sub array of 'array' 
    /// Complexity:         o(1) time (+ o(array.Length) preparation time)
    /// Memory complexity:  o(array.Length)
    /// </summary>
    public class ArrayHashComputer<T>
    {
        #region private fields
        private readonly Func<T, long> _toLong;
        private readonly long _multiplier;
        private readonly long _modulo;
        /// <summary>
        /// _powerModulo[i] = _multiplier^i % _modulo
        /// </summary>
        private readonly long[] _powerModulo;
        /// <summary>
        /// _hashEndingAtIndex[i] =  un normalized hash of the array starting at array[0] and ending at array[i] 
        /// To compute the normalized hash for an array starting at index 'i', we'll have to multiply it by '_powerModulo[_powerModulo.Length-1-i]'
        /// </summary>
        private readonly long[] _unnormalizedHashEndingAtIndex;
        #endregion

        public ArrayHashComputer(IList<T> array, Func<T, long> toLong, int maxPowerForNormalization = -1, long multiplier = 257, long modulo = 1000000007L /*3367900313L*/)
        {
            _toLong = toLong;
            _multiplier = multiplier;
            _modulo = modulo;
            if (maxPowerForNormalization == -1)
            {
                maxPowerForNormalization = array.Count;
            }
            _powerModulo = new long[1 + maxPowerForNormalization];
            for (int i = 0; i < _powerModulo.Length; ++i)
            {
                _powerModulo[i] =  i==0?1L: ((_powerModulo[i - 1] * multiplier) % _modulo);
            }
            _unnormalizedHashEndingAtIndex = new long[array.Count];
            for (int i = 0; i < _unnormalizedHashEndingAtIndex.Length; ++i)
            {
                _unnormalizedHashEndingAtIndex[i] = ((i==0?0L:_unnormalizedHashEndingAtIndex[i - 1])+_powerModulo[i]*toLong(array[i])) % _modulo;
            }
        }

        /// <summary>
        /// computes the hash of the sub array starting at 'startIndex' and ending at 'endIndexIncluded' in o(1) time
        /// </summary>
        /// <param name="startIndex">start index of the sub array</param>
        /// <param name="endIndexIncluded">end index of the sub array</param>
        /// <returns>the (normalized) hash of the sub array</returns>
        public long Hash(int startIndex, int endIndexIncluded)
        {
            var unormalizedHash = (_unnormalizedHashEndingAtIndex[endIndexIncluded] + _modulo - (startIndex == 0 ? 0 : _unnormalizedHashEndingAtIndex[startIndex - 1])) % _modulo;
            var normalizerMultiplier = _powerModulo[_powerModulo.Length -1 - startIndex];
            var normalizedHash =  (unormalizedHash * normalizerMultiplier) % _modulo;
            return normalizedHash >= 0 ? normalizedHash : normalizedHash + _modulo;
        }

        public static ArrayHashComputer<char> FromString(string s) { return new ArrayHashComputer<char>(s.ToCharArray(), x => x); }


        #region Longest Common Sub Array
        /// <summary>
        /// given 2 arrays, finds the longest sub array they have in common in o ( (a.Length * log(a.Length)  ) time using hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="toLong"></param>
        /// <returns>the longest sub array they have in common</returns>
        public static List<T> LongestCommonSubArrayWithHash(IList<T> a, IList<T> b, Func<T, long> toLong)
        {
            int maxPowerForNormalization = Math.Max(a.Count, b.Count);
            var hashA = new ArrayHashComputer<T>(a, toLong, maxPowerForNormalization);
            var hashB = new ArrayHashComputer<T>(b, toLong, maxPowerForNormalization);
            int minLength = 0;
            int maxLength = Math.Min(a.Count, b.Count);
            int firstIndex = -1;
            while (minLength < maxLength)
            {
                var length = (minLength + maxLength + 1) / 2;
                int firstValidIndex = hashA.FirstIndexOfSameSubArrayWithLength(hashB, length);
                if (firstValidIndex != -1)
                {
                    firstIndex = firstValidIndex;
                    minLength = length;
                }
                else
                {
                    maxLength = length - 1;
                }
            }
            //return minLength; //uncomment if we just want the length of the common sub string
            var result = new List<T>(minLength);
            for (int i = firstIndex; i < firstIndex + minLength; ++i)
            {
                result.Add(a[i]);
            }
            return result;
        }

        /// <summary>
        /// finds the index of first sub array of length 'subArrayLength' that can be found both in 'this and 'other' array in o (this.Length) time
        /// if 'subArrayLength == 0', it will return 0 (the first occurence of an empty string is at index 0)
        /// </summary>
        /// <param name="other"></param>
        /// <param name="subArrayLength">the exact length of the sub array to find</param>
        /// <returns>index of the sub array in the 'this' array, or -1 if no such array of length 'subArrayLength' exists</returns>
        public int FirstIndexOfSameSubArrayWithLength(ArrayHashComputer<T> other, int subArrayLength)
        {
            if (subArrayLength <= 0)
            {
                return 0;
            }
            var hashInOtherArray = new HashSet<long>();
            for (var startIndex = 0; startIndex <= other.Length-subArrayLength; ++startIndex)
            {
                hashInOtherArray.Add(other.Hash(startIndex, startIndex + subArrayLength - 1));
            }
            for (var startIndex = 0; startIndex <= Length - subArrayLength; ++startIndex)
            {
                if (hashInOtherArray.Contains(Hash(startIndex, startIndex + subArrayLength - 1)))
                {
                    return startIndex;
                }
            }
            return -1;  //there is no common array of length 'subArrayLength' in 'this' and 'other' array
        }
        #endregion

        /// <summary>
        /// recompute the hash of 'array' in o(array.Length) time
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public long RecomputeHash(IEnumerable<T> array)
        {
            var mult = 1L;
            var hash = 0L;
            foreach (var t in array)
            {
                hash = (hash + _toLong(t) * mult) % _modulo;
                mult = (mult * _multiplier) % _modulo;
            }
            return (hash*_powerModulo[_powerModulo.Length - 1]) % _modulo;
        }

        /// <summary>
        /// return all the indexes of 'pattern' in array in: o( Length ) time
        /// using Rabin-Karp Algorithm for string matching
        /// </summary>
        /// <param name="pattern">the pattern to find</param>
        /// <returns>all the indexes in the 'this' array where we can find the pattern( empty list if the pattern is not found)</returns>
        public List<int> AllIndexesOfPattern(IList<T> pattern)
        {
            var result = new List<int>();
            if (pattern.Count > Length)
            {
                return result;
            }
            var patternHash = RecomputeHash(pattern);
            for (var startIndex = 0; startIndex <= Length - pattern.Count; ++startIndex)
            {
                if (patternHash == Hash(startIndex, startIndex + pattern.Count - 1))
                {
                    result.Add(startIndex);
                }
            }
            return result;
        }

        /// <summary>
        /// count the number of distinct sub array (of non empty length) in o (Length^2) time
        /// </summary>
        /// <returns></returns>
        public int DistinctSubArrayCount()
        {
            int result = 0;
            for (int subArrayLength = 1; subArrayLength <= Length; ++subArrayLength)
            {
                var hashInOtherArray = new HashSet<long>();
                for (var startIndex = 0; startIndex <= Length - subArrayLength; ++startIndex)
                {
                    hashInOtherArray.Add(Hash(startIndex, startIndex + subArrayLength - 1));
                }
                result += hashInOtherArray.Count;
            }
            return result;
        }

        #region palindrome
        /// <summary>
        /// Find longest palindrome in 'a' in o (a.Length * Log(a.Length)  time
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="toLong"></param>
        /// <returns>
        /// Item1: length od the maximum palindrome
        /// Item2: index of the first palindrome with this length
        /// </returns>
        public static Tuple<int, int> LongestPalindrome(List<T> a, Func<T, long> toLong)
        {
            if (a.Count <= 1)
            {
                return Tuple.Create(a.Count, 0);
            }
            var aReverse = new List<T>(a);
            aReverse.Reverse();
            var hashA = new ArrayHashComputer<T>(a, toLong, a.Count);
            var hashReverse = new ArrayHashComputer<T>(aReverse, toLong, a.Count);
            Func<int, int> palindromeLengthToIndex = (l => IndexFirstPalindromeOfLength(l, hashA, hashReverse));
            int maxEvenLengthPalindrome = 2 * Utils.MaximumValidIndex(0, a.Count / 2, halfLength => palindromeLengthToIndex(2 * halfLength) != -1);
            int maxOddLengthPalindrome = 2 * Utils.MaximumValidIndex(0, (a.Count - 1) / 2, halfLength => palindromeLengthToIndex(2 * halfLength + 1) != -1) + 1;
            int maxPalindromeLength = Math.Max(maxEvenLengthPalindrome, maxOddLengthPalindrome);
            var maxIndex = palindromeLengthToIndex(maxPalindromeLength);
            return Tuple.Create(maxPalindromeLength, maxIndex);
        }
        private static int IndexFirstPalindromeOfLength(int l, ArrayHashComputer<T> a, ArrayHashComputer<T> aReversed)
        {
            Debug.Assert(a.Length == aReversed.Length);
            if (l <= 1)
            {
                return 0;
            }
            for (int start = 0; start < (a.Length - l + 1); ++start)
            {
                int end = start + l / 2 - 1;
                var aHash = a.Hash(start, end);
                int reverStart = a.Length - (start + l);
                var aReverseHash = aReversed.Hash(reverStart, reverStart + l / 2 - 1);
                if (aHash == aReverseHash)
                {
                    return start;
                }
            }
            return -1;
        }
        #endregion
        private int Length => _unnormalizedHashEndingAtIndex.Length;
    }
}
