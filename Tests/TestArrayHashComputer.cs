using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public class TestArrayHashComputer
    {
        [Test]
        public void ArrayHashComputerHashTest()
        {
            var test = "abcdeabc";
            var hashComputer = ArrayHashComputer<char>.FromString(test);

            Assert.AreEqual(hashComputer.Hash(0, 2), hashComputer.Hash(5, 7));
            for (int i = 1; i < 5; ++i)
            {
                Assert.AreNotEqual(hashComputer.Hash(0, 2), hashComputer.Hash(i, i + 2));
            }
            for (int i = 0; i < test.Length; ++i)
            {
                for (int j = 0; j < test.Length; ++j)
                {
                    if (test[i] == test[j])
                    {
                        Assert.AreEqual(hashComputer.Hash(i, i), hashComputer.Hash(j, j));
                    }
                    else
                    {
                        Assert.AreNotEqual(hashComputer.Hash(i, i), hashComputer.Hash(j, j));
                    }
                }
            }
        }

        [TestCase("abcdabc")]
        [TestCase("eqfvuokjvnlkyrgdnzem")]
        public void RecomputeHashTest(string str)
        {
            var hashA = ArrayHashComputer<char>.FromString(str);
            for(int i=0;i<str.Length;++i)
            {
                for (int j = i; j < str.Length; ++j)
                {
                    var subString = str.Substring(i, j - i + 1);
                    Assert.AreEqual(hashA.RecomputeHash(subString.ToList()), hashA.Hash(i,j));
                }
            }
        }

        [Test]
        public void AllIndexesOfPatternTest()
        {
            var hashA = ArrayHashComputer<char>.FromString("abcdabca");
            Assert.IsTrue(new[] { 0 }.SequenceEqual(hashA.AllIndexesOfPattern("abcdabca".ToList())));
            Assert.IsTrue(new [] {0, 4}.SequenceEqual(hashA.AllIndexesOfPattern("ab".ToList())));
            Assert.IsTrue(new [] {0, 4, 7}.SequenceEqual(hashA.AllIndexesOfPattern("a".ToList())));
            Assert.IsTrue(new int[] {}.SequenceEqual(hashA.AllIndexesOfPattern("f".ToList())));
        }

        [Test]
        public void DistinctSubArrayCountTest()
        {
            var hashA = ArrayHashComputer<char>.FromString("abc");
            Assert.AreEqual(6, hashA.DistinctSubArrayCount());
            hashA = ArrayHashComputer<char>.FromString("aba");
            Assert.AreEqual(5, hashA.DistinctSubArrayCount());
        }

        [Test]
        public void FirstIndexOfSameSubArrayWithLengthTest()
        {
            var hashA = ArrayHashComputer<char>.FromString("abcdeabc");
            var hashB = ArrayHashComputer<char>.FromString("abcdeabc");
            Assert.AreEqual(0, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 0));
            Assert.AreEqual(0, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 8));
            Assert.AreEqual(-1, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 9));

            hashA = ArrayHashComputer<char>.FromString("123456");
            hashB = ArrayHashComputer<char>.FromString("456789");
            Assert.AreEqual(0, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 0));
            Assert.AreEqual(3, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 1));
            Assert.AreEqual(3, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 2));
            Assert.AreEqual(3, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 3));
            Assert.AreEqual(-1, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 4));
            Assert.AreEqual(0, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 0));
            Assert.AreEqual(0, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 1));
            Assert.AreEqual(0, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 2));
            Assert.AreEqual(0, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 3));
            Assert.AreEqual(-1, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 4));

            hashA = ArrayHashComputer<char>.FromString("123");
            hashB = ArrayHashComputer<char>.FromString("456");
            Assert.AreEqual(0, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 0));
            Assert.AreEqual(-1, hashA.FirstIndexOfSameSubArrayWithLength(hashB, 1));
            Assert.AreEqual(-1, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 1));
            Assert.AreEqual(-1, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 2));
            Assert.AreEqual(-1, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 3));
            Assert.AreEqual(-1, hashB.FirstIndexOfSameSubArrayWithLength(hashA, 4));
        }
    }
}
