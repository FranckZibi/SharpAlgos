using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpAlgos;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {

        [Test]
        public void TestLongestIntervalsContainingExactly_K_DistinctItems()
        {
            var res = Utils.LongestIntervalsContainingExactly_K_DistinctItems(new[] { 1, 2, 9, 4, 5, 0, 4, 11, 6 }, 1);
            Assert.AreEqual(9, res.Count);
            for (int i = 0; i < res.Count; ++i)
            {
                Assert.AreEqual(i, res[i].Item1);
                Assert.AreEqual(i, res[i].Item2);
            }

            res = Utils.LongestIntervalsContainingExactly_K_DistinctItems(new[] { 1, 2, 1, 4, 5, 5, 1, 1, 1 }, 2);
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual(Tuple.Create(0, 2), res[0]);
            Assert.AreEqual(Tuple.Create(2, 3), res[1]);
            Assert.AreEqual(Tuple.Create(3, 5), res[2]);
            Assert.AreEqual(Tuple.Create(4, 8), res[3]);

            res = Utils.LongestIntervalsContainingExactly_K_DistinctItems(new[] { 1, 2, 1, 4, 5, 5, 1, 1, 1 }, 3);
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(Tuple.Create(0, 3), res[0]);
            Assert.AreEqual(Tuple.Create(2, 8), res[1]);

            res = Utils.LongestIntervalsContainingExactly_K_DistinctItems(new[] { 1, 2, 1, 4, 5, 5, 1, 1, 1 }, 5);
            Assert.AreEqual(0, res.Count);
        }



        [Test]
        public void TestHasDuplicateInInterval()
        {
            var rand = new Random(0);
            for (int length = 1; length <= 200; ++length)
            {
                for (int test = 0; test < 100; ++test)
                {
                    var i = rand.Next(length);
                    var j = rand.Next(length);
                    var start = Math.Min(i, j);
                    var end = Math.Max(i, j);
                    var s = RandomString(length, rand,10);
                    var expected = HasDuplicateInIntervalSlow(s.ToArray(), start, end);
                    var observed = Utils.HasDuplicateInInterval(s.ToArray(), start, end);
                    Assert.AreEqual(expected, observed);
                }
            }
        }

        private static bool HasDuplicateInIntervalSlow<T>(IReadOnlyList<T> data, int i, int j)
        {

            var itemToIndexes = new Dictionary<T, List<int>>();

            for (int idx = i; idx <= j; ++idx)
            {
                if (!itemToIndexes.ContainsKey(data[idx]))
                {
                    itemToIndexes[data[idx]] = new List<int>();
                }
                itemToIndexes[data[idx]].Add(idx);
            }
            var allDuplicatesIndexes = itemToIndexes.Where(x => x.Value.Count >= 2).Select(x => x.Value).SelectMany(x => x).ToList();
            if (allDuplicatesIndexes.Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}
