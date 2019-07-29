using System;
using System.Collections.Generic;

namespace SharpAlgos
{
    public static partial class Utils
    {

        //given 2 arrays, finds the longest sub array they have in common in o ( (a.Length * log(a.Length)  ) time using hash
        public static List<T> LongestCommonSubArrayWithHash<T>(IList<T> a, IList<T> b, Func<T, long> toLong)
        {
            int maxPowerForNormalization = Math.Max(a.Count, b.Count);
            var hashA = new ArrayHashComputer<T>(a, toLong, maxPowerForNormalization);
            var hashB = new ArrayHashComputer<T>(b, toLong, maxPowerForNormalization);
            int minLength = 0;
            int maxLength = Math.Min(a.Count, b.Count);
            int firstIndex = -1;
            while (minLength < maxLength)
            {
                var middle = (minLength + maxLength + 1) / 2;
                int firstValidIndex = hashA.FirstIndexOfSameSubArrayWithLength(hashB, middle);
                if (firstValidIndex != -1)
                {
                    firstIndex = firstValidIndex;
                    minLength = middle;
                }
                else
                {
                    maxLength = middle - 1;
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
        /// computes the hash of a any sub array of 'array' in o(1) time (+ o(array.Length) preparation time + o(array.Length) memory )
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
            /// _hashEndingAtIndex[i] =  unormalized hash of the array starting at array[0] and ending at array[i] 
            /// To compute the normalized hash for an array starting at index 'i', we'll have to multiply it by '_powerModulo[_powerModulo.Length-1-i]'
            /// </summary>
            private readonly long[] _unormalizedHashEndingAtIndex;
            #endregion

            private int Length => _unormalizedHashEndingAtIndex.Length;

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
                _unormalizedHashEndingAtIndex = new long[array.Count];
                for (int i = 0; i < _unormalizedHashEndingAtIndex.Length; ++i)
                {
                    _unormalizedHashEndingAtIndex[i] = ((i==0?0L:_unormalizedHashEndingAtIndex[i - 1])+_powerModulo[i]*toLong(array[i])) % _modulo;
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
                var unormalizedHash = (_unormalizedHashEndingAtIndex[endIndexIncluded] + _modulo - (startIndex == 0 ? 0 : _unormalizedHashEndingAtIndex[startIndex - 1])) % _modulo;
                var normalizerMultiplier = _powerModulo[_powerModulo.Length -1 - startIndex];
                var normalizedHash =  (unormalizedHash * normalizerMultiplier) % _modulo;
                return normalizedHash >= 0 ? normalizedHash : normalizedHash + _modulo;
            }

            public static ArrayHashComputer<char> FromString(string s) { return new ArrayHashComputer<char>(s.ToCharArray(), x => x); }

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
            /// return all the indexes of 'pattern' in array in o( Length ) time using Rabin-Karp Algorithm for string matching
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
            /// count the number of distinct sub array (of non empty length) in o (Length^2 ) time
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
        }
    }
}
