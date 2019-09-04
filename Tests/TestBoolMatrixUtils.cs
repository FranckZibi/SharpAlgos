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
        public void TestMaximalRectangle()
        {
            var matrix = new bool[10, 10];
            matrix[5, 5] = matrix[5, 6] = matrix[6, 5] = matrix[6, 6] = true;
            Assert.AreEqual(4, Utils.MaximalRectangleArea(matrix));
        }



        [Test]
        public void TestIdentifyAllSegments()
        {

            var data = new[,] {{true, true, false}, { true, false, false }, {false, true, true } };
            var observed = Utils.IdentifyAllSegments(data, out var _, out var segmentIdToCount);
            var expected = new[,] { { 1, 1, 0 }, { 1, 0, 0}, { 0, 2, 2} };
            Assert.AreEqual(expected.ToList(), observed.ToList());
            Assert.AreEqual(2, segmentIdToCount.Count);
            Assert.AreEqual(3, segmentIdToCount[1]);
            Assert.AreEqual(2, segmentIdToCount[2]);
        }

        [Test]
        public void TestMaximalSquareSideSize()
        {
            var matrix = new bool[10, 10];
            Assert.AreEqual(0, Utils.MaximalSquareWidth(matrix));
            matrix[0, 0] = true;
            Assert.AreEqual(1, Utils.MaximalSquareWidth(matrix));
            matrix[1, 0] = true;
            Assert.AreEqual(1, Utils.MaximalSquareWidth(matrix));
            matrix[0, 1] = true;
            Assert.AreEqual(1, Utils.MaximalSquareWidth(matrix));
            matrix[1, 1] = true;
            Assert.AreEqual(2, Utils.MaximalSquareWidth(matrix));
        }
        

    }
}
