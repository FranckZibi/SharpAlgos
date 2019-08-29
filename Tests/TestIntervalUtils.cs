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
            int minValue = -100;
            int maxValue = 100;
            var rand = new Random(0);
            for (int length = 1; length <= 100; ++length)
            {
                for (int testId = 0; testId < 100; ++testId)
                {
                    var intervals = GetRandomIntervals(length,minValue, maxValue, rand);
                    var x = rand.Next(minValue - 50, maxValue + 50);
                    foreach(bool endOfIntervalIsIncludedInInterval in new[]{true,false})
                    {
                        var observed = Utils.IntervalTree.ValueOf(intervals, endOfIntervalIsIncludedInInterval).AllIntervalsContaining(x);
                        observed = observed.OrderBy(i => i.Item1).ThenBy(i => i.Item2).ToList();
                        var expected = AllIntervalsContainingXSlow(intervals, x, endOfIntervalIsIncludedInInterval);
                        expected = expected.OrderBy(i => i.Item1).ThenBy(i => i.Item2).ToList();
                        Assert.AreEqual(expected, observed);

                        var observedCount = Utils.IntervalTree.ValueOf(intervals, endOfIntervalIsIncludedInInterval).CountIntervalsContaining(x);
                        Assert.AreEqual(expected.Count , observedCount);
                    }
                }
            }
        }

        public static List<Tuple<int,int>> GetRandomIntervals(int nbIntervals, int minValue, int maxValue, Random rand)
        {

            var result = new List<Tuple<int, int>>();
            while (result.Count < nbIntervals)
            {
                var r1 = rand.Next(minValue, maxValue);
                var r2 = rand.Next(minValue, maxValue);
                result.Add(Tuple.Create(Math.Min(r1,r2), Math.Max(r1, r2)));
            }
            return result;
        }


        public static List<Tuple<int, int>> AllIntervalsContainingXSlow(List<Tuple<int, int>> intervals, int x, bool endOfIntervalIsIncludedInInterval)
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

            int t = 104;
            var str_t = Convert.ToString(t, 2);

            int c_t = ~t;
            int neg_t = -t;
            var str_neg_t = Convert.ToString(neg_t, 2);

            int add = t & neg_t;
            var str_add =Convert.ToString(add, 2);



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
