using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {
        [Test]
        public void TestEditDistance()
        {
            Assert.AreEqual(0, Utils.EditDistance("toto", "toto", 1, 1, 1, 1));
            Assert.AreEqual(1, Utils.EditDistance("toto", "toto2", 1, 1, 1, 1));
            Assert.AreEqual(3, Utils.EditDistance("toto", "toto2", 3, 4, 4, 4));
            Assert.AreEqual(3, Utils.EditDistance("toto", "tot", 4, 3, 4, 4));
            Assert.AreEqual(5, Utils.EditDistance("niche", "chiens", 1, 1, 1, 1));
            Assert.AreEqual(1, Utils.EditDistance("niceh", "niche", 1, 1, 1, 1));
            Assert.AreEqual(2, Utils.EditDistance("niceh", "niche", 1, 1, 1, 1000));
            Assert.AreEqual(2, Utils.EditDistance("abcd", "badc", 1, 1, 1, 1));
            Assert.AreEqual(3, Utils.EditDistance("abcd", "badc", 1, 1, 1, 1000));
        }


        [TestCase("", "", "")]
        [TestCase("", "4545", "")]
        [TestCase("B", "AB", "CB")]
        [TestCase("GA", "GAH", "AGCAT")]
        [TestCase("MJAU", "XMJYAUZ", "MZJAWXU")]
        [TestCase("ADH", "ABCDGH", "AEDFHR")]
        [TestCase("GTAB", "AGGTAB", "GXTXAYB")]
        public void TestLongestCommonSubsequence_and_LengthOfLongestCommonSubsequence(string expected, string a, string b)
        {
            Assert.AreEqual(expected, new string(Utils.LongestCommonSubsequence(a.ToList(), b.ToList()).ToArray()));
            Assert.AreEqual(expected, new string(Utils.LongestCommonSubsequence(b.ToList(), a.ToList()).ToArray()));
            Assert.AreEqual(expected.Length, Utils.LengthOfLongestCommonSubsequence(a.ToList(), b.ToList()));
            Assert.AreEqual(expected.Length, Utils.LengthOfLongestCommonSubsequence(b.ToList(), a.ToList()));
        }

        [Test]
        public void TestAllLongestCommonSubsequence()
        {
            var r1 = Utils.AllLongestCommonSubsequence("XMJYAUZ".ToList(), "MZJAWXU".ToList()).Select(x => new string(x.ToArray())).ToList();
            r1.Sort();
            Assert.IsTrue(r1.SequenceEqual(new[] { "MJAU" }));
            var r2 = Utils.AllLongestCommonSubsequence("ABCBDAB".ToList(), "BDCABA".ToList()).Select(x => new string(x.ToArray())).ToList();
            r2.Sort();
            Assert.IsTrue(r2.SequenceEqual(new[] { "BCAB", "BCBA", "BDAB" }));
        }

        [TestCase("", "", "")]
        [TestCase("", "", "4545")]
        [TestCase("GCA", "GCAH", "AGCAT")]
        [TestCase("ABA", "ABADC", "DABAC")]
        [TestCase("bon", "bonjour", "abonnes")]
        [TestCase("jjcjcjccjcjjc", "ccjcjccjcccccccjcjjjcjcjjccccjcjccjjcccccjccjjjccjjcjjccjcjcjjcjcjcjjcjjccjjcjjcccjccjjcjcjjcjcjccjcjjcccjjccjjcccccjcjjjjcjccccjjjjccjjcjjccjc", "cjjcjjcccjcccjjcjjjccjcjcjccjcjccjccjcccjcjcjjcjjjjcccjccccjjjjcjjjjcjcjccjcjjcjjccccccjccjcccjcjjjccjjcjcjjccjjcjjjccjjjcjccjjccjjcjjjccccjcjjjjj")]
        public void TestLongestCommonSubstring_and_LengthOfLongestCommonSubstring(string expected, string a, string b)
        {
            Assert.AreEqual(expected.Length, Utils.LengthOfLongestCommonSubstring(a.ToList(), b.ToList()));
            Assert.AreEqual(expected.Length, Utils.LengthOfLongestCommonSubstring(b.ToList(), a.ToList()));
            Assert.AreEqual(expected, new string(Utils.LongestCommonSubstring(a.ToList(), b.ToList()).ToArray()));
            Assert.AreEqual(expected, new string(Utils.LongestCommonSubstring(b.ToList(), a.ToList()).ToArray()));
        }



        [TestCase("", "", "")]
        [TestCase("", "", "4545")]
        [TestCase("GCA", "GCAH", "AGCAT")]
        [TestCase("ABA", "ABADC", "DABAC")]
        [TestCase("bon", "bonjour", "abonnes")]
        [TestCase("abonnes", "abonnes", "abonnes")]
        [TestCase("jjcjcjccjcjjc", "ccjcjccjcccccccjcjjjcjcjjccccjcjccjjcccccjccjjjccjjcjjccjcjcjjcjcjcjjcjjccjjcjjcccjccjjcjcjjcjcjccjcjjcccjjccjjcccccjcjjjjcjccccjjjjccjjcjjccjc", "cjjcjjcccjcccjjcjjjccjcjcjccjcjccjccjcccjcjcjjcjjjjcccjccccjjjjcjjjjcjcjccjcjjcjjccccccjccjcccjcjjjccjjcjcjjccjjcjjjccjjjcjccjjccjjcjjjccccjcjjjjj")]
        public void TestLongestCommonSubstringWithHash(string expected, string a, string b)
        {
            Assert.AreEqual(expected, Utils.LongestCommonSubstringWithHash(a, b));
            Assert.AreEqual(expected, Utils.LongestCommonSubstringWithHash(b, a));
        }

        [TestCase(new[] { "" }, "", "")]
        [TestCase(new[] { "ABC" }, "AB", "BC")]
        [TestCase(new[] { "ABC" }, "", "ABC")]
        [TestCase(new[] { "ABC", "CAB", "ACB" }, "AB", "C")]
        [TestCase(new[] { "ABC", "CAB", "ACB" }, "AB", "C")]
        [TestCase(new[] { "ABCBDCABA", "ABDCABDAB", "ABDCBDABA" }, "ABCBDAB", "BDCABA")]
        public void TestShortestCommonSupersequence_and_LengthOfShortestCommonSupersequence(string[] expecteds, string a, string b)
        {
            Assert.AreEqual(expecteds[0].Length, Utils.LengthOfShortestCommonSupersequence(a.ToList(), b.ToList()));
            Assert.AreEqual(expecteds[0].Length, Utils.LengthOfShortestCommonSupersequence(b.ToList(), a.ToList()));
            Assert.IsTrue(expecteds.ToList().Contains(new string(Utils.ShortestCommonSupersequence(a.ToList(), b.ToList()).ToArray())));
            Assert.IsTrue(expecteds.ToList().Contains(new string(Utils.ShortestCommonSupersequence(b.ToList(), a.ToList()).ToArray())));
        }

    }
}
