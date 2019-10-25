
using NUnit.Framework;
using SharpAlgos;

// ReSharper disable PossibleMultipleEnumeration
namespace SharpAlgosTests
{
    [TestFixture]
    public class TestBoardUtils
    {
        [TestCase(8, 0, 6, 4, 1.0)]
        public void KnightProbability(int n, int k, int r, int c, double expectedResult)
        {
            Assert.AreEqual(expectedResult, Utils.KnightProbability(n, k, r, c), 1e-5);
        }
    }
}
