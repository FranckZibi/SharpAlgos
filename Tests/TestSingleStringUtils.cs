using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        [Test]
        public void TestLongestPalindrome()
        {
            var rand = new Random(0);
            for (int length = 0; length <= 20; ++length)
            {
                for (int test = 0; test < 10; ++test)
                {
                    var s = RandomString(length, rand,3);
                    var expected = MaxPalindrome(s);
                    var observed = Utils.LongestPalindromeWithHash(s);
                    var observedManacher = Utils.LongestPalindromeManacher(s);
                    //expected length of longest palindrome
                    Assert.AreEqual(expected.Item1, observed.Item1);
                    Assert.AreEqual(expected.Item1, observedManacher.Item1);
                    //expected start index of longest palindrome
                    Assert.AreEqual(expected.Item2, observed.Item2);
                    Assert.AreEqual(expected.Item2, observedManacher.Item2);
                }
            }
        }


        private static string RandomString(int length, Random rand, int nbDistinctCharacters)
        {
            var sb = new StringBuilder();
            while (sb.Length < length)
            {
                sb.Append((char) ('A' + rand.Next(nbDistinctCharacters)));
            }
            return sb.ToString();
        }


        private static bool SlowIsPalindrome(int startIndex, int length, string s)
        {
            var endIndex = startIndex + length - 1;
            while (startIndex < endIndex)
            {
                if (s[startIndex++] != s[endIndex--])
                {
                    return false;
                }
            }
            return true;
        }
        private static Tuple<int,int> MaxPalindrome(string s)
        {
            for (int length = s.Length; length>=1;--length)
            {
                for (int i = 0; i < (s.Length - length + 1);++i)
                {
                    if (SlowIsPalindrome(i, length, s))
                    {
                        return Tuple.Create(length, i);
                    }
                }
            }
            return Tuple.Create(0,0);
        }



    }
}
