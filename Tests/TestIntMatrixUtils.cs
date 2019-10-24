using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {
        [Test]
        public void TestCreateCountMatrix()
        {
            var m = new[,] { { 0, 2, 5, 4, 1 }, { 4, 8, 2, 3, 7 }, { 6, 3, 4, 6, 2 }, { 7, 3, 1, 8, 3 }, { 1, 5, 7, 9, 4 } };
            var countMatrix = Utils.CreateCountMatrix(m);
            Assert.AreEqual(38, Utils.SubMatrixSumInCountMatrix(countMatrix, 1, 1, 3, 3));
        }

        [Test]
        public void TestMinimumCostToGoFromTopLeftToBottomRightOfMatrix()
        {
            var m = new[,] { { 4, 7, 8, 6, 4 }, { 6, 7, 3, 9, 2 }, { 3, 8, 1, 2, 4 }, { 7, 1, 7, 3, 7 }, { 2, 9, 8, 9, 3 } };
            Assert.AreEqual(36, Utils.MinimumCostToGoFromTopLeftToBottomRightOfMatrix(m));
        }

        [Test]
        public void TestNumberOfPathsWithGivenCost()
        {
            var m = new[,] { { 4, 7, 1, 6 }, { 5, 7, 3, 9 }, { 3, 2, 1, 2 }, { 7, 1, 6, 3 } };
            Assert.AreEqual(2, Utils.NumberOfPathsToGoFromTopLeftToBottomRightOfMatrixWithGivenCost(m, 25));
        }


        [Test]
        public void TestLongestSequenceSatisfyingConstraints()
        {
            var m = new[,] { { 10, 13, 14, 21, 23 }, { 11, 9, 22, 2, 3 }, { 12, 8, 1, 5, 4 }, { 15, 24, 7, 6, 20 }, { 16, 17, 18, 19, 25 } };
            Assert.IsTrue(new[] { 2, 3, 4, 5, 6, 7 }.SequenceEqual(Utils.LongestSequenceSatisfyingConstraints(m, (source, target) => source + 1 == target)));
        }

        [Test]
        public void TestMaximumPointsIn01MatrixSatisfyingGivenConstraints()
        {
            var m = new[,] { { 1, 1, -1, 1, 1 }, { 1, 0, 0, -1, 1 }, { 1, 1, 1, 1, -1 }, { -1, -1, 1, 1, 1 }, { 1, 1, -1, -1, 1 } };
            Assert.AreEqual(9, Utils.MaximumPointsIn01MatrixSatisfyingGivenConstraints(m));
            m = new[,] { { 1, 1, -1, 1, 1 }, { 1, 0, 0, -1, 1 }, { 1, 1, 1, 1, -1 }, { -1, -1, 1, 1, 1 }, { 1, 1, -1, -1, 1 }, { -1, -1, -1, -1, -1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 } };
            Assert.AreEqual(9, Utils.MaximumPointsIn01MatrixSatisfyingGivenConstraints(m));
        }

        [Test]
        public void TestTrapRainWater3D()
        {
            var heighMap = new[,] { { 14, 17, 18, 16, 14, 16 }, { 17, 3, 10, 2, 3, 8 }, { 11, 10, 4, 7, 1, 7 }, { 13, 7, 2, 9, 8, 10 }, { 13, 1, 3, 4, 8, 6 }, { 20, 3, 3, 9, 10, 8 } };
            Assert.AreEqual(25, Utils.TrapRainWater3D(heighMap));

            heighMap = new[,] { { 12, 13, 1, 12 }, { 13, 4, 13, 12 }, { 13, 8, 10, 12 }, { 12, 13, 12, 12 }, { 13, 13, 13, 13 } };
            Assert.AreEqual(14, Utils.TrapRainWater3D(heighMap));

            heighMap = new[,] { { 5, 5, 5, 1 }, { 5, 1, 1, 5 }, { 5, 1, 5, 5 }, { 5, 2, 5, 8 } };
            Assert.AreEqual(3, Utils.TrapRainWater3D(heighMap));
        }

        [Test]
        public void TestMaximumSubmatrixSum()
        {
            int[] coordinates;
            var matrix = new[,] { { -5, -6, 3, 1, 0}, { 9, 7, 8, 3, 7 }, { -6, -2, -1, 2, -4}, { -7, 5, 5, 2, -6},{3,2,-9,-5,1}};
            Assert.AreEqual(34, Utils.MaximumSubmatrixSum(matrix, out coordinates));
            Assert.IsTrue(coordinates.SequenceEqual(new []{1,0,1,4}));

            matrix = new[,] { { -5, -6, 3, 1, 0 }, { 9, -7, 8, 3, 7 }, { -6, -2, -1, 2, -4 }, { -7, 5, 5, 2, -6 }, { 3, 2, -9, -5, 1 } };
            Assert.AreEqual(23, Utils.MaximumSubmatrixSum(matrix, out coordinates));
            Assert.IsTrue(coordinates.SequenceEqual(new[] { 0, 2, 3, 3 }));

            matrix = new[,] { { -5, -6, 3, 1, 0 }, { 9, 7, 8, 3, 7 }, { -6, -2, -1, 2, -4 }, { -7, 5, 5, 2, -6 }, { 3, 2, 9, -5, 1 } };
            Assert.AreEqual(35, Utils.MaximumSubmatrixSum(matrix, out coordinates));
            Assert.IsTrue(coordinates.SequenceEqual(new[] { 1, 1, 4, 3 }));
        }
    }
}
