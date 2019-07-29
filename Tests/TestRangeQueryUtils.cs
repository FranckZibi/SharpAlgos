using System;
using System.Linq;
using SharpAlgos;
using NUnit.Framework;

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {
        [Test]
        public void TestTrieNode()
        {
            var root = new TrieNode();
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(0, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            root.Add("");
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(1, root.WordCount);
            Assert.IsTrue(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            root.Add("ab");
            Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(2, root.WordCount);
            Assert.IsTrue(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            Assert.IsFalse(root.Contains("ba"));
            Assert.IsTrue(root.Contains("ab"));
            root.Add("ac");
            Assert.AreEqual(4, root.NodeCount);
            Assert.AreEqual(3, root.WordCount);
            Assert.IsTrue(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            Assert.IsTrue(root.Contains("ab"));
            Assert.IsTrue(root.Contains("ac"));
            root.Delete("ab");
            Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(2, root.WordCount);
            Assert.IsTrue(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            Assert.IsFalse(root.Contains("ab"));
            Assert.IsTrue(root.Contains("ac"));
            root.Delete("ac");
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(1, root.WordCount);
            root.Delete("");
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(0, root.WordCount);
        }

        [Test]
        public void TestTernarySearchTreeNode()
        {
            var root = new TernarySearchTreeNode();
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(0, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsFalse(root.Contains("a"));
            root.Add("a");
            Assert.AreEqual(1, root.NodeCount);
            Assert.AreEqual(1, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsTrue(root.Contains("a"));
            root.Add("ab");
            Assert.AreEqual(2, root.NodeCount);
            Assert.AreEqual(2, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsTrue(root.Contains("a"));
            Assert.IsFalse(root.Contains("ba"));
            Assert.IsTrue(root.Contains("ab"));
            root.Add("ac");
            Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(3, root.WordCount);
            Assert.IsFalse(root.Contains(""));
            Assert.IsTrue(root.Contains("a"));
            Assert.IsTrue(root.Contains("ab"));
            Assert.IsTrue(root.Contains("ac"));
            root.Add("fgh");
            Assert.AreEqual(6, root.NodeCount);
            Assert.AreEqual(4, root.WordCount);
            Assert.IsTrue(root.Contains("a"));
            Assert.IsTrue(root.Contains("ab"));
            Assert.IsTrue(root.Contains("fgh"));
            root.Delete("fgh");
            Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(3, root.WordCount);
            root.Delete("ab");
            //Assert.AreEqual(2, root.NodeCount);
            Assert.AreEqual(2, root.WordCount);
            root.Delete("a");
            //Assert.AreEqual(3, root.NodeCount);
            Assert.AreEqual(1, root.WordCount);
        }


        private static int[] CreateRandomIntArray(int length, int minValue, int maxValue, Random r)
        {
            var result = new int[length];
            for (int i = 0; i < length; ++i)
                result[i] = r.Next(minValue, maxValue + 1);
            return result;
        }


        [TestCase(false, false)]    // No updates at all
        [TestCase(true, false)]      // SetValueInInterval
        [TestCase(false, true)]      // AddValueInInterval
        public void TestSegmentTree(bool useSetValue, bool useAddValue)
        {
            var r = new Random(0);

            //We ensure that empty interval do not throw exception
            SegmentTree.Min(new int[0]);
            SegmentTree.Max(new int[0]);
            SegmentTree.Sum(new int[0]);

            foreach (var length in new[]{1,2,7,8,9,31})
            {
                var ints = CreateRandomIntArray(length, -1000, +1000, r);
                var min = SegmentTree.Min((int[])ints.Clone());
                var max = SegmentTree.Max((int[])ints.Clone());
                //var minIndex = SegmentTree.IndexOfMin((int[])ints.Clone());
                //var maxIndex = SegmentTree.IndexOfMax((int[])ints.Clone());
                var sum = SegmentTree.Sum((int[])ints.Clone());
                for (int nbLoops = 0; nbLoops < 30; ++nbLoops)
                {
                    //we check the segment tree results for several intervals in the array (brute force vs segment tree)
                    for (int nbIntervalTests = 0; nbIntervalTests < 100; ++nbIntervalTests)
                    {
                        int idx3 = r.Next(length);
                        int idx4 = r.Next(length);
                        int startIndex = Math.Min(idx3, idx4);
                        int endIndex = Math.Max(idx3, idx4);
                        Assert.AreEqual(SlowMinInInterval(ints, startIndex, endIndex), min.Query(startIndex, endIndex));
                        Assert.AreEqual(SlowMaxInInterval(ints, startIndex, endIndex), max.Query(startIndex, endIndex));
                        Assert.AreEqual(SlowSumInInterval(ints, startIndex, endIndex), sum.Query(startIndex, endIndex));
                        //Assert.AreEqual(ints[SlowMinIndexInInterval(ints, startIndex, endIndex)], ints[minIndex.Query(startIndex, endIndex)]);
                        //Assert.AreEqual(ints[SlowMaxIndexInInterval(ints, startIndex, endIndex)], ints[maxIndex.Query(startIndex, endIndex)]);
                    }

                    //we modify some intervals in the array
                    //if useSetValue is true
                    //      we'll set the interval to a specific value
                    //else
                    //      we'll add a specific value to each element of the interval
                    for (int nbUpdates = 0; nbUpdates < 20; ++nbUpdates)
                    {
                        int idx1 = r.Next(length);
                        int idx2 = r.Next(length);
                        int start = Math.Min(idx1, idx2);
                        int end = Math.Max(idx1, idx2);
                        int newValue = r.Next(-1000, +1001);
                        if (useSetValue)
                        {
                            for (int i = start; i <= end; ++i)
                                ints[i] = newValue;
                            min.SetValueInInterval(newValue, start, end);
                            max.SetValueInInterval(newValue, start, end);
                            sum.SetValueInInterval(newValue, start, end);
                            //minIndex.SetValueInInterval(newValue, start, end);
                            //maxIndex.SetValueInInterval(newValue, start, end);
                        }
                        if (useAddValue)
                        {
                            for (int i = start; i <= end; ++i)
                                ints[i] += newValue;
                            min.AddValueInInterval(newValue, start, end);
                            max.AddValueInInterval(newValue, start, end);
                            sum.AddValueInInterval(newValue, start, end);
                            //minIndex.AddValueInInterval(newValue, start, end);
                            //maxIndex.AddValueInInterval(newValue, start, end);
                        }
                    }
                }
            }
        }


        [Test]
        public void TestSegmentTreeCountInferiorToK()
        {
            var r = new Random(0);
            foreach (var length in new[] { 1, 2, 7, 8, 9, 31, 32, 33, 64, 127, 128 })
            {
                var ints = CreateRandomIntArray(length, -1000, +1000, r);
                var st = new SegmentTreeCountInferiorToK((int[])ints.Clone());
                for (int startIndex = 0; startIndex < ints.Length; ++startIndex)
                    for (int endIndex = startIndex; endIndex < ints.Length; ++endIndex)
                        foreach (var K in new[] { r.Next(-2000, 2000), ints[startIndex], ints[endIndex], ints[(startIndex + endIndex) / 2], ints[startIndex] - 1, ints[endIndex] - 1, ints[startIndex] + 1, ints[endIndex] + 1 })
                            Assert.AreEqual(SlowCountInferiorToKInInterval(ints, startIndex, endIndex, K), st.Query(startIndex, endIndex, K));
            }
        }

        [Test]
        public void TestSparseTable()
        {
            var r = new Random(0);
            foreach (var length in new[] { 1, 2, 7, 8, 9, 31, 64, 127, 128, 129 })
            {
                var data = CreateRandomIntArray(length, -100000, +100000, r);
                var min = SparseTable.Min(data);
                var max = SparseTable.Max(data);
                var minIndex = SparseTable.IndexOfMin(data);
                var maxIndex = SparseTable.IndexOfMax(data);
                for (int startIndex = 0; startIndex < data.Length; ++startIndex)
                    for (int endIndex = startIndex; endIndex < data.Length; ++endIndex)
                    {
                        Assert.AreEqual(SlowMinInInterval(data, startIndex, endIndex), min.Query(startIndex, endIndex));
                        Assert.AreEqual(SlowMaxInInterval(data, startIndex, endIndex), max.Query(startIndex, endIndex));
                        Assert.AreEqual(data[SlowMinIndexInInterval(data, startIndex, endIndex)], data[minIndex.Query(startIndex, endIndex)]);
                        Assert.AreEqual(data[SlowMaxIndexInInterval(data, startIndex, endIndex)], data[maxIndex.Query(startIndex, endIndex)]);
                    }
            }
        }
        private static int SlowMinIndexInInterval(int[] data, int startIndex, int endIndex)
        {
            int result = startIndex;
            for (int i = startIndex; i <= endIndex; ++i)
                if (data[i] < data[result])
                    result = i;
            return result;
        }
        private static int SlowMaxIndexInInterval(int[] data, int startIndex, int endIndex)
        {
            int result = startIndex;
            for (int i = startIndex; i <= endIndex; ++i)
                if (data[i] > data[result])
                    result = i;
            return result;
        }
        private static int SlowMinInInterval(int[] data, int startIndex, int endIndex)
        {
            return data.Skip(startIndex).Take(endIndex - startIndex + 1).Min();
        }
        private static int SlowMaxInInterval(int[] data, int startIndex, int endIndex)
        {
            return data.Skip(startIndex).Take(endIndex - startIndex + 1).Max();
        }
        private static int SlowSumInInterval(int[] data, int startIndex, int endIndex)
        {
            return data.Skip(startIndex).Take(endIndex - startIndex + 1).Sum();
        }
        private static int SlowCountInferiorToKInInterval(int[] data, int startIndex, int endIndex, int K)
        {
            return data.Skip(startIndex).Take(endIndex - startIndex + 1).Count(i => i <= K);
        }
    }
}
