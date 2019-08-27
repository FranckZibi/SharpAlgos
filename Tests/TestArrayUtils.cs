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
            //return allDuplicatesIndexes.Max();
            return true;
        }


    }
}
