using System;
using System.Collections.Generic;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {

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
