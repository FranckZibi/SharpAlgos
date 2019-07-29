using System;
using System.Collections.Generic;
using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {
        [TestCase("", "")]
        [TestCase("a", "a")]
        [TestCase("aa", "aa")]
        [TestCase("aabaa", "acabaa")]
        [TestCase("45854", "4758546")]
        [TestCase("4554", "475546")]
        [TestCase("BCACB", "ABBDCACB")]
        public void TestLongestPalindromicSubsequence_and_LengthOfLongestPalindromicSubsequence(string expected, string a)
        {
            Assert.AreEqual(expected.Length, Utils.LengthOfLongestPalindromicSubsequence(a.ToList()));
            Assert.AreEqual(expected, new string(Utils.LongestPalindromicSubsequence(a.ToList()).ToArray()));
        }

        [TestCase("", "")]
        [TestCase("45", "457495")]
        [TestCase("4", "44")]
        [TestCase("ATCG", "ATACTCGGA")]
        public void TestLongestRepeatedSubsequence_and_LengthOfLongestRepeatedSubsequence(string expected, string a)
        {
            Assert.AreEqual(expected.Length, Utils.LengthOfLongestRepeatedSubsequence(a.ToList()));
            Assert.AreEqual(expected, new string(Utils.LongestRepeatedSubsequence(a.ToList()).ToArray()));
        }
    }
}
