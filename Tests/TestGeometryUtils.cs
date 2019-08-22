using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {

        [Test]
        public void TestClockwiseOrAligned()
        {
            Assert.AreEqual(0, Utils.Direction(new Point(0, 0), new Point(1, 0), new Point(2, 0))); //aligned 
            Assert.AreEqual(0, Utils.Direction(new Point(0, 0), new Point(0, 0), new Point(0, 0))); //aligned
            Assert.AreEqual(0, Utils.Direction(new Point(0, 0), new Point(1, 0), new Point(1, 0))); //aligned
            Assert.AreEqual(1, Utils.Direction(new Point(0, 0), new Point(1, 0), new Point(1, 1))); //counter clockwise
            Assert.AreEqual(-1, Utils.Direction(new Point(0, 0), new Point(1, 0), new Point(1, -1))); //clockwise
        }

        [Test]
        public void TestDistancePoints()
        {
            Assert.AreEqual(1.0, Utils.Distance(new Point(0, 0), new Point(1, 0)), 1e-6);
            Assert.AreEqual(Math.Sqrt(8), Utils.Distance(new Point(-1, -1), new Point(1, 1)), 1e-6);
        }

        [Test]
        public void TestSmallestCircleIncludingAllPoints()
        {
            var r = new Random(0);
            var radius = 1.0;
            //var circle = Tuple.Create(radius, center);
            //in all test, the smallest circle is the circle centered at (0,0) with radius = 1

            //all points aligned in same line between (-1.0) and (1,0)
            var points = new List<Tuple<double, double>>();
            points.Add(Tuple.Create(-1.0, 0.0));
            points.Add(Tuple.Create(1.0, 0.0));
            while (points.Count < 50)
            {
                points.Add(Tuple.Create(2*r.NextDouble()-1, 0.0));
                var observedResult = Utils.SmallestCircleIncludingAllPoints(points);
                Assert.AreEqual(radius, observedResult.Item1, 1e-8);
                Assert.AreEqual(0.0, observedResult.Item2.Item1, 1e-8);
                Assert.AreEqual(0.0, observedResult.Item2.Item2, 1e-8);
            }

            //3 points in circle, all other inside circle
            for (int nbTests = 0; nbTests < 10; ++nbTests)
            {
                points.Clear();
                points.Add(Tuple.Create(-1.0, 0.0));
                points.Add(Tuple.Create(1.0, 0.0));
                var xInCircle = 2*r.NextDouble()-1;
                var yInCircle = Math.Sqrt(radius*radius - xInCircle * xInCircle);
                points.Add(Tuple.Create(xInCircle,yInCircle));
                while (points.Count < 50)
                {
                    var observedResult = Utils.SmallestCircleIncludingAllPoints(points);
                    Assert.AreEqual(radius, observedResult.Item1, 1e-8);
                    Assert.AreEqual(0.0, observedResult.Item2.Item1, 1e-8);
                    Assert.AreEqual(0.0, observedResult.Item2.Item2, 1e-8);
                    var tmpRadius = r.NextDouble();
                    var x = tmpRadius * r.NextDouble();
                    var y = Math.Sqrt(tmpRadius* tmpRadius - x * x);
                    points.Add(Tuple.Create(x, y));
                }
            }

            //all points in circle
            for (int nbTests = 0; nbTests < 10; ++nbTests)
            {
                points.Clear();
                points.Add(Tuple.Create(-1.0, 0.0));
                points.Add(Tuple.Create(1.0, 0.0));
                while (points.Count < 50)
                {
                    var xInCircle = 2 * r.NextDouble() - 1;
                    var yInCircle = Math.Sqrt(radius * radius - xInCircle * xInCircle);
                    points.Add(Tuple.Create(xInCircle, yInCircle));
                    var observedResult = Utils.SmallestCircleIncludingAllPoints(points);
                    Assert.AreEqual(radius, observedResult.Item1, 1e-8);
                    Assert.AreEqual(0.0, observedResult.Item2.Item1, 1e-8);
                    Assert.AreEqual(0.0, observedResult.Item2.Item2, 1e-8);
                }
            }


        }
        //        public static Tuple<double, Tuple<double, double>> SmallestCircleIncludingAllPoints(IEnumerable<Tuple<double, double>> p)



        [Test]
        public void TestConvexLowerAndUpperHull()
        {
            List<Point> lower, upper;
            var points = new List<Point>();
            for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 3; ++j)
            {
                points.Add(new Point(i, j));
                points.Add(new Point(i, j));
            }
            Utils.ConvexLowerAndUpperHull(points, out lower, out upper);
            Assert.IsTrue(lower.SequenceEqual(new[] {new Point(0, 0), new Point(2, 0), new Point(2, 2)}));
            Assert.IsTrue(upper.SequenceEqual(new[] {new Point(0, 0), new Point(0, 2), new Point(2, 2)}));
        }


        [Test]
        public void TestConvexHull()
        {
            Assert.IsTrue(new Point[] { }.SequenceEqual(Utils.ConvexHull(new Point[] { })));
            var a = new Point(0, 0);
            Assert.IsTrue(new[] {a}.SequenceEqual(Utils.ConvexHull(new[] {a})));
            var b = new Point(2, 0);
            Assert.IsTrue(new[] {a, b}.SequenceEqual(Utils.ConvexHull(new[] {b, a})));
            var c = new Point(3, 1);
            var d = new Point(2, 2);
            var e = new Point(0, 2);
            var f = new Point(1, 1);
            Assert.IsTrue(new[] {a, b, c, d, e}.SequenceEqual(Utils.ConvexHull(new[] {f, e, d, c, b, a})));
        }

        [Test]
        public void TestOverlappingSurfaceOfAllRectangles()
        {
            var r1 = Tuple.Create(0, 10, 10, 0);
            var r2 = Tuple.Create(5, 15, 15, 5);
            Assert.AreEqual(0, Utils.OverlappingSurfaceOfAllRectangles( new[] {r1}.ToList()));
            Assert.AreEqual(100, Utils.OverlappingSurfaceOfAllRectangles(new[] { r1, r1 }.ToList()));
            Assert.AreEqual(25, Utils.OverlappingSurfaceOfAllRectangles(new[] { r1, r2 }.ToList()));
            Assert.AreEqual(100, Utils.OverlappingSurfaceOfAllRectangles(new[] { r1, r1, r2 }.ToList()));
        }

        [Test]
        public void TestGetArea()
        {
            Assert.AreEqual(0.0, Utils.GetArea(new Point[] { }), 1e-9);
            var a = new Point(0, 0);
            Assert.AreEqual(0.0, Utils.GetArea(new[] {a}), 1e-9);
            var b = new Point(1, 0);
            Assert.AreEqual(0.0, Utils.GetArea(new[] {a, b}), 1e-9);
            var c = new Point(2, 0);
            var d = new Point(2, 2);
            var e = new Point(1, 1);
            var f = new Point(0, 2);
            Assert.AreEqual(1.0, Utils.GetArea(new[] {f, e, d}), 1e-9);
            Assert.AreEqual(1.0, Utils.GetArea(new[] {d, e, f}), 1e-9);
            Assert.AreEqual(3.0, Utils.GetArea(new[] {f, e, d, c, b, a}), 1e-9);
            Assert.AreEqual(3.0, Utils.GetArea(new[] {a, b, c, d, e, f}), 1e-9);
        }


        [Test]
        public void TestMinDistance()
        {
            Assert.AreEqual(0.0, Utils.MaxDistance(new[] { new Point(0, 0) }), 1e-9);
            Assert.AreEqual(0.0, Utils.MaxDistance(new[] { new Point(0, 0), new Point(0, 0) }), 1e-9);
            Assert.AreEqual(1.0, Utils.MaxDistance(new[] { new Point(0, 0), new Point(1, 0), new Point(0, 0), new Point(1, 0) }), 1e-9);
            Assert.AreEqual(2, Utils.MaxDistance(new[] { new Point(0, 0), new Point(1, -1), new Point(2, 0), new Point(1, 1), new Point(1, 1) }), 1e-9);
            var r = new Random(0);
            for (int testIndex = 0; testIndex < 20; ++testIndex)
            {
                int testLength = r.Next(0, 200); //size of each test is between 0 and 200 points
                var p = new List<Point>();
                while (p.Count < testLength)
                {
                    p.Add(new Point(r.Next(-100000, +100000), r.Next(-100000, +100000)));
                }

                var minDistance = Utils.MinDistance(p);
                var minDistanceBruteForce = MinDistanceBruteForce(p);
                Assert.AreEqual(minDistanceBruteForce, minDistance, 1e-9);
            }
        }

        private static double MinDistanceBruteForce(List<Point> points)
        {
            double minDistance = double.MaxValue;
            for (int i = 0; i < points.Count; ++i)
                for (int j = i + 1; j < points.Count; ++j)
                {
                    minDistance = Math.Min(minDistance, Utils.Distance(points[i], points[j]));
                }

            return minDistance;
        }

        [Test]
        public void TestMaxDistance()
        {
            Assert.AreEqual(0.0, Utils.MaxDistance(new[] { new Point(0, 0) }), 1e-9);
            Assert.AreEqual(0.0, Utils.MaxDistance(new[] { new Point(0, 0), new Point(0, 0) }), 1e-9);
            Assert.AreEqual(1.0, Utils.MaxDistance(new[] { new Point(0, 0), new Point(1, 0), new Point(0, 0), new Point(1, 0) }), 1e-9);
            Assert.AreEqual(2, Utils.MaxDistance(new[] { new Point(0, 0), new Point(1, -1), new Point(2, 0), new Point(1, 1), new Point(1, 1) }), 1e-9);
            var r = new Random(0);
            for (int testIndex = 0; testIndex < 20; ++testIndex)
            {
                int testLength = r.Next(0, 500); //size of each test is between 0 and 500 points
                var p = new List<Point>();
                while (p.Count < testLength)
                {
                    p.Add(new Point(r.Next(-100000, +100000), r.Next(-100000, +100000)));
                }

                var maxDistance = Utils.MaxDistance(p);
                var maxDistanceBruteForce = MaxDistanceBruteForce(p);
                Assert.AreEqual(maxDistanceBruteForce, maxDistance, 1e-9);
            }
        }

        private static double MaxDistanceBruteForce(List<Point> points)
        {
            double minDistance = 0;
            for (int i = 0; i < points.Count; ++i)
                for (int j = i + 1; j < points.Count; ++j)
                {
                    minDistance = Math.Max(minDistance, Utils.Distance(points[i], points[j]));
                }

            return minDistance;
        }


    }
}

