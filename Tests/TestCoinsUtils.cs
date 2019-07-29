using System.Collections.Generic;
using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {

        [TestCase(new[] { 1, 2, 5 }, 11, 3)]
        [TestCase(new[] { 227, 99, 328, 299, 42, 322 }, 9847, 31)]
        [TestCase(new[] { 259, 78, 94, 130, 493, 4, 168, 149 }, 4769, 14)]
        [TestCase(new[] { 186, 419, 83, 408 }, 6249, 20)]
        public void CoinChangeTest(int[] coins, int amount, int expectedResult)
        {
            Assert.AreEqual(expectedResult, Utils.CoinChange(coins, amount));
        }

        [Test]
        public void MinimumCoinChangeMakingWithDuplicateCoinsTest()
        {
            var coins = new List<int>();
            coins.AddRange(Enumerable.Repeat(2, 18));
            coins.AddRange(Enumerable.Repeat(5, 26));
            coins.AddRange(Enumerable.Repeat(12, 5));
            coins.AddRange(Enumerable.Repeat(24, 1));
            Assert.AreEqual(17, Utils.MinimumCoinChangeMakingWithDuplicateCoins(coins.ToArray(), 133));

            coins.Clear();
            coins.AddRange(Enumerable.Repeat(4, 11));
            coins.AddRange(Enumerable.Repeat(10, 2));
            coins.AddRange(Enumerable.Repeat(14, 28));
            coins.AddRange(Enumerable.Repeat(15, 4));
            coins.AddRange(Enumerable.Repeat(21, 1));
            Assert.AreEqual(-1, Utils.MinimumCoinChangeMakingWithDuplicateCoins(coins.ToArray(), 1));
        }

        [TestCase(new[] { 1, 2, 5 }, 5, 4)]
        [TestCase(new[] { 11, 24, 37, 50, 63, 76, 89, 102 }, 5000, 992951208)]
        public void CountWaysToAchieveAmountTest(int[] coins, int amount, int expectedResult)
        {
            Assert.AreEqual(expectedResult, Utils.CountWaysToAchieveAmount(amount, coins));
        }



        [Test]
        public void TestMinimumCoinChangeMaking()
        {
            Assert.AreEqual(3, Utils.MinimumCoinChangeMaking(15, new[] { 1, 3, 5, 7 }));
            Assert.AreEqual(4, Utils.MinimumCoinChangeMaking(18, new[] { 1, 3, 5, 7 }));
            Assert.AreEqual(-1, Utils.MinimumCoinChangeMaking(4, new[] { 3, 5, 7 }));
        }



        [Test]
        public void TestTotalNumberOfWaysToGetTheDenominationOfCoins()
        {
            Assert.AreEqual(6, Utils.TotalNumberOfWaysToGetTheDenominationOfCoins(8, new[] { 1, 3, 5, 7 }));
            Assert.AreEqual(4, Utils.TotalNumberOfWaysToGetTheDenominationOfCoins(4, new[] { 1, 2, 3 }));
            Assert.AreEqual(0, Utils.TotalNumberOfWaysToGetTheDenominationOfCoins(4, new[] { 3, 5, 7 }));
        }






    }
}
