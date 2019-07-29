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
        public void TestMaximumSubsequenceSumWithNoAdjacentElement()
        {
            Assert.AreEqual(26, Utils.MaximumSubsequenceSumWithNoAdjacentElement(new[] { 1, 2, 9, 4, 5, 0, 4, 11, 6 }));
        }

        [TestCase(new[] { 2, 1, 5, 6, 2, 3 }, 10,2,3)]
        [TestCase(new[] { 1, 2, 3, 4, 5 }, 9,2,4)]
        public void LargestRectangleArea(int[] heights, int expectedResult, int expectedStartIndex, int expectedEndIndex)
        {
            int observedStartIndex;
            int observedEndIndex;
            Assert.AreEqual(expectedResult, Utils.LargestRectangleArea(heights, out observedStartIndex, out observedEndIndex));
        }


        [Test]
        public void TestMinimumSumPartition()
        {
            Assert.AreEqual(5, Utils.MinimumSumPartition(new[] { 10, 20, 15, 5, 25 }));
        }


        [Test]
        public void TestMaximizeValueOfTheExpression()
        {
            Assert.AreEqual(46, Utils.MaximizeValueOfTheExpression(new[] { 3, 9, 10, 1, 30, 40 }));
        }

        [Test]
        public void TestCanBeDividedInToSubsetWithEqualSum()
        {
            Assert.AreEqual(true, Utils.CanBeDividedInto_2_SubsetWithEqualSum(new[] {3, 1, 1, 2, 2, 1}));
            Assert.AreEqual(false, Utils.CanBeDividedInto_2_SubsetWithEqualSum(new[] {3, 2}));
        }

        [Test]
        public void TestCanBeDividedInto_K_SubsetWithEqualSum()
        {
            List<int>[] valuesForEachSubset;
            Assert.AreEqual(true, Utils.CanBeDividedInto_K_SubsetWithEqualSum(new[] { 3, 1, 1, 2, 2, 1 }, 1, out valuesForEachSubset));
            Assert.AreEqual(true, Utils.CanBeDividedInto_K_SubsetWithEqualSum(new[] { 3, 1, 1, 2, 2, 1 }, 2, out valuesForEachSubset));
            Assert.AreEqual(false, Utils.CanBeDividedInto_K_SubsetWithEqualSum(new[] { 3, 2 },2, out valuesForEachSubset));

            Assert.AreEqual(true, Utils.CanBeDividedInto_K_SubsetWithEqualSum(new[] { 7, 3, 2, 1, 5, 4, 8 }, 1, out valuesForEachSubset));
            Assert.AreEqual(true, Utils.CanBeDividedInto_K_SubsetWithEqualSum(new[] { 7, 3, 2, 1, 5, 4, 8 }, 3, out valuesForEachSubset));
            Assert.AreEqual(false, Utils.CanBeDividedInto_K_SubsetWithEqualSum(new[] { 7, 3, 2, 1, 5, 4, 9 }, 3, out valuesForEachSubset));

            Assert.AreEqual(true, Utils.CanBeDividedInto_K_SubsetWithEqualSum(new[] { 4, 3, 2, 3, 5, 2, 1 }, 4, out valuesForEachSubset));
            Assert.AreEqual(true, Utils.CanBeDividedInto_K_SubsetWithEqualSum(new[] { 780, 935, 2439, 444, 513, 1603, 504, 2162, 432, 110, 1856, 575, 172, 367, 288, 316 }, 4, out valuesForEachSubset));
        }

        [Test]
        public void TestCanExtractSubsetEqualToSum()
        {
            Assert.AreEqual(true, Utils.CanExtractSubsetEqualToSum(new[] {7, 3, 2, 5, 13}, 14));
            Assert.AreEqual(false, Utils.CanExtractSubsetEqualToSum(new[] {6, 3, 2, 13}, 14));
            Assert.AreEqual(false, Utils.CanExtractSubsetEqualToSum(new[] {3, 2}, 4));
        }

        [Test]
        public void TestLongestAlternatingSubsequence()
        {
            Assert.AreEqual(6, Utils.LongestAlternatingSubsequence(new[] {8, 9, 6, 4, 5, 7, 3, 2, 4}));
            Assert.AreEqual(6, Utils.LongestAlternatingSubsequence(new[] {10, 22, 9, 33, 49, 50, 31, 60}));

        }

        [TestCase(6, new[] {1, 3, 6, 7, 9, 4, 10, 5, 6})]
        [TestCase(6, new[] {0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15})]
        [TestCase(4, new[] {10, 9, 2, 5, 3, 7, 101, 18})]
        [TestCase(3, new[] {3, 1, 2, 0, 5, 2})]
        [TestCase(2, new[] {3, 2, 1, 0, -1, -5, -2})]
        [TestCase(1, new[] {1, 1})]
        [TestCase(1, new[] {2})]
        [TestCase(0, new int[] { })]
        public void TestLengthOfLongestIncreasingSubsequence(int expected, int[] data)
        {
            Assert.AreEqual(expected, Utils.LengthOfLongestIncreasingSubsequence(data));
        }

        [TestCase(new[] {1, 2, 3, 4, 5}, new[] {1, 4, 2, 3, 4, 5}, false)]
        [TestCase(new[] {1, 2, 5}, new[] {3, 1, 2, 0, 5, 2}, false)]
        [TestCase(new[] {1}, new[] {1, 1}, false)]
        [TestCase(new int[] { }, new int[] { }, false)]
        [TestCase(new[] {-5, -2}, new[] {3, 2, 1, 0, -1, -5, -2}, false)]
        [TestCase(new[] {1, 2, 3, 4, 5}, new[] {1, 4, 2, 3, 4, 5}, true)]
        [TestCase(new[] {1, 2, 2, 5, 5}, new[] {3, 1, 2, 0, 2, 5, 2, 5}, true)]
        [TestCase(new[] {1, 1}, new[] {1, 1}, true)]
        public void TestLongestIncreasingSubsequence(int[] expected, int[] data, bool allowEquality)
        {
            Assert.IsTrue(expected.SequenceEqual(Utils.LongestIncreasingSubsequence(data, allowEquality)));
        }

        [TestCase(new int[] { }, new int[] { })]
        [TestCase(new[] {1}, new[] {1})]
        [TestCase(new[] {1}, new[] {1, 1})]
        [TestCase(new[] {1, 2, 5, 4, 3}, new[] {1, 2, 5, 2, 4, 3})]
        public void TestLongestBitonicSubsequence(int[] expected, int[] data)
        {
            Assert.IsTrue(expected.SequenceEqual(Utils.LongestBitonicSubsequence(data)));
        }

        [TestCase(new int[] { }, new int[] { })]
        [TestCase(new[] {1}, new[] {1, 1})]
        [TestCase(new[] {1, 2, 3, 4, 5}, new[] {1, 4, 2, 3, 4, 5})]
        [TestCase(new[] {99}, new[] {1, 99, 2, 3, 4, 5})]
        [TestCase(new[] {8, 12, 14}, new[] {0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11})]
        public void TestMaximumSumIncreasingSubsequence(int[] expected, int[] data)
        {
            Assert.IsTrue(expected.SequenceEqual(Utils.MaximumSumIncreasingSubsequence(data)));
        }

        [TestCase(new[] { 0, 1, 0, 2, 1, 0, 1, 3, 2, 1, 2, 1 }, 6)]
        [TestCase(new[] { 2, 0, 2 }, 2)]
        public void TestTrap(int[] nums, int expectedResult)
        {
            Assert.AreEqual(expectedResult, Utils.TrapRainWater2D(nums));
        }

        [TestCase(new[] { 12,25,6}, "62512")]
        [TestCase(new[] { 2, 0, 2 }, "220")]
        [TestCase(new[] { 327, 321, 6, 32 }, "632732321")]
        public void TestBuildBiggestNumberUsingAllPartsTrap(int[] parts, string expectedResult)
        {
            Assert.AreEqual(expectedResult, Utils.BuildBiggestNumberUsingAllParts(parts));
        }


        [TestCase(new[] { 1, 3, 5, 2, 6 }, 0, "")]
        [TestCase(new int[] { }, 0, "")]
        [TestCase(new int[] { }, 1, "")]
        [TestCase(new[] { 1, 3, 5, 2, 6 }, 1, "6")]
        [TestCase(new[] { 1, 3, 5, 2, 6 }, 2, "56")]
        [TestCase(new[] { 1, 3, 5, 2, 6 }, 3, "526")]
        [TestCase(new[] { 1, 3, 5, 2, 6 }, 4, "3526")]
        [TestCase(new[] { 1, 3, 5, 2, 6 }, 5, "13526")]
        [TestCase(new[] { 1, 3, 5, 2, 6 }, 6, "13526")]
        [TestCase(new[] { 1, 3, 5, 2, 6 }, 1000000000, "13526")]
        [TestCase(new[] { 4, 3, 2, 1 }, 1, "4")]
        [TestCase(new[] { 4, 3, 2, 1 }, 2, "43")]
        [TestCase(new[] { 4, 3, 2, 1 }, 3, "432")]
        [TestCase(new[] { 4, 3, 2, 1 }, 4, "4321")]
        public void TestBuildBiggestNumberUsingSubsequenceOfKElements(int[] digits, int K, string expectedResult)
        {
            Assert.AreEqual(expectedResult, string.Join("",Utils.BuildBiggestNumberUsingSubsequenceOfKElements(digits, K)));
        }




    }
}
