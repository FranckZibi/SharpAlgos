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
        [Test]
        public void TestIntervalTreeSeveralElement()
        {
            int minValue = -30;
            int maxValue = 30;
            var rand = new Random(0);
            for (int testId = 0; testId < 20; ++testId)
            {
                for (int length = 1; length <= 25; ++length)
                {
                    var intervals = GetRandomIntervals(length, minValue, maxValue, rand);
                    for (int x = minValue-2; x<= maxValue+2; ++x)
                    {
                        foreach(bool endOfIntervalIsIncludedInInterval in new[]{true,false})
                        {
                            var intervalTree = Utils.IntervalTree.ValueOf(intervals, endOfIntervalIsIncludedInInterval);
                            var observed = intervalTree?.AllIntervalsContaining(x)??new List<Tuple<int, int>>();
                            observed = observed.OrderBy(i => i.Item1).ThenBy(i => i.Item2).ToList();
                            var expected = AllIntervalsContainingXSlow(intervals, x, endOfIntervalIsIncludedInInterval);
                            expected = expected.OrderBy(i => i.Item1).ThenBy(i => i.Item2).ToList();
                            Assert.AreEqual(expected, observed);

                            var observedCount = intervalTree?.CountIntervalsContaining(x)??0;
                            Assert.AreEqual(expected.Count , observedCount);
                        }
                    }
                }
            }
        }

        private static List<Tuple<int, int>> GetRandomIntervals(int nbIntervals, int minValue, int maxValue, Random rand)
        {
            var result = new List<Tuple<int, int>>();
            while (result.Count < nbIntervals)
            {
                var r1 = rand.Next(minValue, maxValue);
                var r2 = rand.Next(minValue, maxValue);
                result.Add(Tuple.Create(Math.Min(r1, r2), Math.Max(r1, r2)));
            }
            return result;
        }

        [Test]
        public void TestMinimumPointsToCoverAllIntervals()
        {
            var i1 = Tuple.Create(0, 10);
            var i2 = Tuple.Create(10, 20);
            var i3 = Tuple.Create(21, 30);

            var observed = Utils.MinimumPointsToCoverAllIntervals(new List<Tuple<int, int>> {i1});
            var expected = new List<int> {10};
            Assert.AreEqual(expected, observed);

            observed = Utils.MinimumPointsToCoverAllIntervals(new List<Tuple<int, int>> { i1,i2 });
            expected = new List<int> { 10 };
            Assert.AreEqual(expected, observed);

            observed = Utils.MinimumPointsToCoverAllIntervals(new List<Tuple<int, int>> { i2, i3 });
            expected = new List<int> { 20,30 };
            Assert.AreEqual(expected, observed);

            observed = Utils.MinimumPointsToCoverAllIntervals(new List<Tuple<int, int>> { i1, i2,i3 });
            expected = new List<int> { 10,30 };
            Assert.AreEqual(expected, observed);
        }

        [Test]
        public void TestIntervalsUnion()
        {
            var i1 = Tuple.Create(0, 10);
            var i2 = Tuple.Create(10, 20);
            var i3 = Tuple.Create(21, 30);
            var i4 = Tuple.Create(19, 30);

            var observed = Utils.IntervalsUnion(new List<Tuple<int, int>> {i1}, false);
            var expected = new List<Tuple<int, int>> {i1};
            Assert.AreEqual(expected, observed);
            observed = Utils.IntervalsUnion(new List<Tuple<int, int>> { i1 }, true);
            Assert.AreEqual(expected, observed);

            observed = Utils.IntervalsUnion(new List<Tuple<int, int>> { i1,i2 }, false);
            expected = new List<Tuple<int, int>> { Tuple.Create(0,20) };
            Assert.AreEqual(expected, observed);
            observed = Utils.IntervalsUnion(new List<Tuple<int, int>> { i1, i2 }, true);
            expected = new List<Tuple<int, int>> { i1,i2 };
            Assert.AreEqual(expected, observed);

            observed = Utils.IntervalsUnion(new List<Tuple<int, int>> { i1, i2,i3 }, false);
            expected = new List<Tuple<int, int>> { Tuple.Create(0, 20),i3 };
            Assert.AreEqual(expected, observed);
            observed = Utils.IntervalsUnion(new List<Tuple<int, int>> { i1, i2,i3 }, true);
            expected = new List<Tuple<int, int>> { i1, i2, i3 };
            Assert.AreEqual(expected, observed);

            observed = Utils.IntervalsUnion(new List<Tuple<int, int>> { i1, i2, i4 }, false);
            expected = new List<Tuple<int, int>> { Tuple.Create(0, 30)};
            Assert.AreEqual(expected, observed);
            observed = Utils.IntervalsUnion(new List<Tuple<int, int>> { i1, i2, i4 }, true);
            expected = new List<Tuple<int, int>> { i1, Tuple.Create(10, 30) };
            Assert.AreEqual(expected, observed);
        }

        private static List<Tuple<int, int>> AllIntervalsContainingXSlow(List<Tuple<int, int>> intervals, int x, bool endOfIntervalIsIncludedInInterval)
        {
            var result = new List<Tuple<int, int>>();
            foreach (var i in intervals)
            {
                if ((endOfIntervalIsIncludedInInterval && x >= i.Item1 && x <= i.Item2)
                    || (!endOfIntervalIsIncludedInInterval && x >= i.Item1 && x < i.Item2))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        [Test]
        public void TestIntervalTreeSingleElement()
        {
            var intervals = new List<Tuple<int, int>>();
            var i1 = Tuple.Create(0,10);
            intervals.Add(i1);

            var tree = Utils.IntervalTree.ValueOf(intervals, true);
            var observed = tree.AllIntervalsContaining(-1);
            Assert.AreEqual(0, observed.Count);
            for (int idx = 0; idx <= 10; ++idx)
            {
                observed = tree.AllIntervalsContaining(idx);
                Assert.AreEqual(1, observed.Count);
                Assert.AreEqual(i1, observed[0]);
            }
            observed = tree.AllIntervalsContaining(11);
            Assert.AreEqual(0, observed.Count);


            tree = Utils.IntervalTree.ValueOf(intervals, false);
            observed = tree.AllIntervalsContaining(-1);
            Assert.AreEqual(0, observed.Count);
            for (int idx = 0; idx <= 9; ++idx)
            {
                observed = tree.AllIntervalsContaining(idx);
                Assert.AreEqual(1, observed.Count);
                Assert.AreEqual(i1, observed[0]);
            }
            observed = tree.AllIntervalsContaining(10);
            Assert.AreEqual(0, observed.Count);
            observed = tree.AllIntervalsContaining(11);
            Assert.AreEqual(0, observed.Count);
        }

        [Test]
        public void TestElementInMostInterval()
        {
            var intervals = new List<Tuple<double, double>>();
            intervals.Add(Tuple.Create(-5.0, 2.0));
            intervals.Add(Tuple.Create(-2.0, 10.0));
            intervals.Add(Tuple.Create(-1.0, 1.0));
            intervals.Add(Tuple.Create(10.0, 15.0));

            var observedResult = Utils.ElementInMostInterval(intervals);
            Assert.AreEqual(-1.0, observedResult.Item1, 1e-6);
            Assert.AreEqual(3, observedResult.Item2);
        }
    }
}
