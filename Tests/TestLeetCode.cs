
// 23/8/2017 20h26 25
// 23/8/2017 22h09 29
// 24/8/2017 21h12 29
// 13/12/2017 20h40 121 / 711
// 13/12/2017 21h24 126 / 711
// 13/12/2017 21h55 129 / 711
// 13/12/2017 23h22 132 / 711
// 14/12/2017 19h23 132 / 711
// 14/12/2017 20h33 135 / 711
// 14/12/2017 22h40 142 / 711
// 15/12/2017 21h49 142 / 711
// 15/12/2017 22h19 147 / 711
// 16/12/2017 11h28 151 / 711
// 16/12/2017 12h02 155 / 711
// 16/12/2017 16h52 168 / 711
// 16/12/2017 17h55 171 / 711
// 16/12/2017 21h55 173 / 711
// 17/12/2017  8:52 173/711 Solved - Easy 101 Medium 53 Hard 19
// 17/12/2017 11:06 178/711 Solved - Easy 102 Medium 57 Hard 19
// 17/12/2017 11:34 180/711 Solved - Easy 102 Medium 59 Hard 19
// 17/12/2017 12:54 182/715 Solved - Easy 102 Medium 61 Hard 19

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpAlgos;

// ReSharper disable PossibleMultipleEnumeration

namespace SharpAlgosTests
{
    [TestFixture]
    public class TestLeetCode
    {



        [TestCase(new[] {2, 7, 11, 15}, 9, new[] {0, 1})]
        [TestCase(new[] {3, 3}, 6, new[] {0, 1})]
        [TestCase(new[] {3}, 6, new int[] {})]
        [TestCase(new int[] {}, 6, new int[] {})]
        [TestCase(null, 6, new int[] {})]
        [TestCase(new[] {2, 7, 11, 15}, 12, new int[] {})]
        public void TwoSum(int[] nums, int target, int[] expectedResult)
        {
            AssertAreEqual(expectedResult, LeetCodeUtils.TwoSum(nums, target));
        }
 


        [TestCase(new[] { 0, 0, 0, 0, 1, 2, 3 }, "abcabc")]
        [TestCase(new[] { 0, 0, 1, 2, 3}, "aaaa")]
        public void TestAllMaximumBorderLength(int[] expected, string data)
        {
            Assert.IsTrue(expected.SequenceEqual(LeetCodeUtils.AllMaximumBorderLength(data)));
        }



        [TestCase(3, "abcabc")]
        [TestCase(7, "abcabca")]
        [TestCase(1, "aaaa")]
        [TestCase(0, "")]
        [TestCase(1, "a")]
        public void TestDetectWordMinimumPeriodLength(int expected, string data)
        {
            Assert.AreEqual(expected, LeetCodeUtils.DetectWordMinimumPeriodLength(data));
        }

        [TestCase("abcabcbb", 3)]
        [TestCase("aaaaa", 1)]
        [TestCase("pwwkew", 3)]
        [TestCase("aabcde", 5)]
        [TestCase("abcdee", 5)]
        [TestCase("anviaj", 5)]
        [TestCase("a", 1)]
        [TestCase("", 0)]
        [TestCase(null, 0)]
        public void LengthOfLongestSubstring(string s, int expectedResult)
        {
            Assert.AreEqual(expectedResult, LeetCodeUtils.LengthOfLongestSubstring(s), s);
        }

        


        //[TestCase(new[] { 7, 7, 7, 7 }, new [] { "00", "01", "10", "11" })]
        [TestCase(new[] { 40, 5, 2, 1 }, new [] { "1", "01", "001", "000" })]
        public void CreateHuffmanCode(int[] frequencies, string[] expectedResult)
        {
            Assert.IsTrue(expectedResult.SequenceEqual(LeetCodeUtils.CreateHuffmanCode(frequencies)));
        }



        [TestCase(3,3)]
        [TestCase(1,256)]
        [TestCase(5, 22222)]
        [TestCase(1, 1000000000)]
        public void FindNthDigit(int expectedResult, int n)
        {
            Assert.AreEqual(expectedResult, LeetCodeUtils.FindNthDigit(n));
        }

        [TestCase(8, 0, 6, 4, 1.0)]
        public void KnightProbability(int N, int K, int r, int c, double expectedResult)
        {
            Assert.AreEqual(expectedResult, LeetCodeUtils.KnightProbability(N,K,r,c), 1e-5);
        }

        [TestCase("MCMXCVII", 1997)]
        [TestCase("LXXVIII", 78)]
        public void IntToRoman(string expectedResult, int num)
        {
            Assert.AreEqual(expectedResult, LeetCodeUtils.IntToRoman(num));
        }

        [TestCase(1997, "MCMXCVII")]
        [TestCase(78, "LXXVIII")]
        public void RomanToInt(int expectedResult, string num)
        {
            Assert.AreEqual(expectedResult, LeetCodeUtils.RomanToInt(num));
        }


        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(100, 1536)]
        public void NthUglyNumber(int n, int expectedResult)
        {
            Assert.AreEqual(expectedResult, LeetCodeUtils.NthUglyNumber(n));
        }


        private void AssertAreEqual<T>(IEnumerable<T> expectedResult, IEnumerable<T> observedResult)
        {
            if (expectedResult == null)
            {
                Assert.AreEqual(null, observedResult);
                return;
            }
            Assert.AreNotEqual(null, observedResult);
            Assert.IsTrue(expectedResult.SequenceEqual(observedResult), "expecting="+ string.Join(",", expectedResult)+ " / observed="+string.Join(",", observedResult));
        }
    }
}
