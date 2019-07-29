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
