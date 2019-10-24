using System.Collections.Generic;
using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {
        [Test]
        public void MinimumCoinChangeMakingWithDuplicateCoinsTest()
        {
            var coins = new List<int>();
            coins.AddRange(Enumerable.Repeat(2, 18));
            coins.AddRange(Enumerable.Repeat(5, 26));
            coins.AddRange(Enumerable.Repeat(12, 5));
            coins.AddRange(Enumerable.Repeat(24, 1));
            Assert.AreEqual(17, Utils.MinimumCoinChangeMakingWithDuplicateCoins(133, coins.ToArray()));

            coins.Clear();
            coins.AddRange(Enumerable.Repeat(4, 11));
            coins.AddRange(Enumerable.Repeat(10, 2));
            coins.AddRange(Enumerable.Repeat(14, 28));
            coins.AddRange(Enumerable.Repeat(15, 4));
            coins.AddRange(Enumerable.Repeat(21, 1));
            Assert.AreEqual(-1, Utils.MinimumCoinChangeMakingWithDuplicateCoins(1, coins.ToArray()));
        }

        [TestCase(new[] { 1, 2, 5 }, 11, 3)]
        [TestCase(new[] { 227, 99, 328, 299, 42, 322 }, 9847, 31)]
        [TestCase(new[] { 259, 78, 94, 130, 493, 4, 168, 149 }, 4769, 14)]
        [TestCase(new[] { 186, 419, 83, 408 }, 6249, 20)]
        [TestCase(new[] { 1, 3, 5, 7 }, 15, 3)]
        [TestCase(new[] { 1, 3, 5, 7 }, 18, 4)]
        [TestCase(new[] { 3, 5, 7 }, 4, -1)]
        public void MinimumCoinChangeMakingTest(int[] coins, int amount, int expectedResult)
        {
            Assert.AreEqual(expectedResult, Utils.MinimumCoinChangeMaking(amount, coins));
        }

        [TestCase(new[] { 1, 2, 5 }, 5, 4)]
        [TestCase(new[] { 11, 24, 37, 50, 63, 76, 89, 102 }, 5000, 992951208)]
        [TestCase(new[] { 1, 3, 5, 7 }, 8, 6)]
        [TestCase(new[] { 1, 2, 3}, 4, 4)]
        [TestCase(new[] { 3, 5, 7}, 4, 0)]
        public void CountWaysToAchieveAmountTest(int[] coins, int amount, int expectedResult)
        {
            Assert.AreEqual(expectedResult, Utils.CountWaysToAchieveAmount(amount, coins));
        }
    }
}
