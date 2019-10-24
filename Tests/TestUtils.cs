using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SharpAlgos;
using NUnit.Framework;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable MemberCanBePrivate.Global

namespace SharpAlgosTests
{
    [TestFixture]
    public partial class TestUtils
    {
        [Test]
        public void TestParse()
        {
            Assert.AreEqual(1.5, double.Parse("1.5"), 1e-8);
            Assert.Throws<FormatException>(() => double.Parse("1,5"));
            Assert.AreEqual(123, int.Parse("123"));
            Assert.Throws<FormatException>(() => int.Parse("123,0"));
            Assert.Throws<FormatException>(() => int.Parse("123.0"));
        }

        [Test]
        public void TestStableMariageMatching()
        {
            var menPreference = CreatePreferences("abe abi eve cath ivy jan dee fay bea hope gay" + Environment.NewLine +
                                        "bob cath hope abi dee eve fay bea jan ivy gay" + Environment.NewLine +
                                        "col hope eve abi dee bea fay ivy gay cath jan" + Environment.NewLine +
                                        "dan ivy fay dee gay hope eve jan bea cath abi" + Environment.NewLine +
                                        "ed jan dee bea cath fay eve abi ivy hope gay" + Environment.NewLine +
                                        "fred bea abi dee gay eve ivy cath jan hope fay" + Environment.NewLine +
                                        "gav gay eve ivy bea cath abi dee hope jan fay" + Environment.NewLine +
                                        "hal abi eve hope fay ivy cath jan bea gay dee" + Environment.NewLine +
                                        "ian hope cath dee gay bea abi fay ivy jan eve" + Environment.NewLine +
                                        "jon abi fay jan gay eve bea dee cath ivy hope");
            var womenPreference = CreatePreferences("abi bob fred jon gav ian abe dan ed col hal" + Environment.NewLine +
                                          "bea bob abe col fred gav dan ian ed jon hal" + Environment.NewLine +
                                          "cath fred bob ed gav hal col ian abe dan jon" + Environment.NewLine +
                                          "dee fred jon col abe ian hal gav dan bob ed" + Environment.NewLine +
                                          "eve jon hal fred dan abe gav col ed ian bob" + Environment.NewLine +
                                          "fay bob abe ed ian jon dan fred gav col hal" + Environment.NewLine +
                                          "gay jon gav hal fred bob abe col ed dan ian" + Environment.NewLine +
                                          "hope gav jon bob abe ian dan hal ed col fred" + Environment.NewLine +
                                          "ivy ian col hal gav fred bob abe ed jon dan" + Environment.NewLine +
                                          "jan ed hal gav abe bob jon col ian fred dan");
            var res = Utils.StableMatching(menPreference, womenPreference).ToList();
            var observed = string.Join(" ",res.OrderBy(x => x.Key).Select(x=>x.Key+"+"+x.Value));
            Assert.AreEqual("abi+jon bea+fred cath+bob dee+col eve+hal fay+dan gay+gav hope+ian ivy+abe jan+ed", observed);
        }

        private static Dictionary<string, List<string>> CreatePreferences(string str)
        {
            var result = new Dictionary<string, List<string>>();
            foreach (var l in str.Split(new []{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                var list = l.Split().ToList();
                var key = list[0];
                list.RemoveAt(0);
                result[key] = list;
            }
            return result;
        }

        [Test]
        public void TestUnionFind()
        {
            var uf = new UnionFind<int>();
            for (int i = 0; i <= 10; ++i)
            {
                Assert.AreEqual(i, uf.Find(i));
                Assert.AreEqual(i + 1, uf.Count);
                Assert.AreEqual(i + 1, uf.DistinctFamilyCount);
            }
            for (int i = 0; i <= 10; ++i)
            {
                Assert.AreEqual(false, uf.Union(i, i));
            }

            //we join all even values
            for (int i = 2; i <= 10; i += 2)
            {
                Assert.AreEqual(true, uf.Union(0, i));
                Assert.AreEqual(11, uf.Count);
                Assert.AreEqual(uf.Find(0), uf.Find(i));
            }
            for (int i = 0; i <= 10; i += 2)
            {
                Assert.AreEqual(false, uf.Union(0, i));
            }

            Assert.AreEqual(1 + 5, uf.DistinctFamilyCount);

            //we join all odd values
            for (int i = 3; i <= 9; i += 2)
            {
                Assert.AreEqual(true, uf.Union(1, i));
                Assert.AreEqual(11, uf.Count);
                Assert.AreEqual(uf.Find(1), uf.Find(i));
            }
            for (int i = 1; i <= 9; i += 2)
            {
                Assert.AreEqual(false, uf.Union(1, i));
            }

            Assert.AreEqual(2, uf.DistinctFamilyCount);

            //we join all values
            Assert.AreEqual(true, uf.Union(9, 10));
            Assert.AreEqual(11, uf.Count);
            Assert.AreEqual(1, uf.DistinctFamilyCount);
            for (int i = 0; i <= 10; ++i)
            {
                for (int j = 0; j <= 10; ++j)
                {
                    Assert.AreEqual(false, uf.Union(i, j));
                    Assert.AreEqual(uf.Find(i), uf.Find(j));
                }
            }

            uf = new UnionFind<int>();
            Assert.IsTrue(uf.Add(1));
            Assert.IsTrue(uf.Add(2));
            Assert.IsTrue(uf.Add(3));
            Assert.IsFalse(uf.Add(2));
            Assert.AreEqual(3, uf.Count);
            Assert.AreEqual(3, uf.DistinctFamilyCount);
            uf.Union(1, 2);
            Assert.AreEqual(3, uf.Count);
            Assert.AreEqual(2, uf.DistinctFamilyCount);
        }

        [Test]
        public void TestMatrixChainMultiplication()
        {
            var matrixDimensions = new List<KeyValuePair<int, int>>();
            matrixDimensions.Add(new KeyValuePair<int, int>(10, 30));
            matrixDimensions.Add(new KeyValuePair<int, int>(30, 5));
            matrixDimensions.Add(new KeyValuePair<int, int>(5, 60));
            Assert.AreEqual(4500, Utils.MatrixChainMultiplicationMinimalCost(matrixDimensions));
            Assert.AreEqual("([10,30]*[30,5])*[5,60]", Utils.MatrixChainMultiplicationParenthesis(matrixDimensions));
            
            matrixDimensions = new List<KeyValuePair<int, int>>();
            matrixDimensions.Add(new KeyValuePair<int, int>(5, 10));
            matrixDimensions.Add(new KeyValuePair<int, int>(10, 6));
            matrixDimensions.Add(new KeyValuePair<int, int>(6, 30));
            matrixDimensions.Add(new KeyValuePair<int, int>(30, 4));
            matrixDimensions.Add(new KeyValuePair<int, int>(4, 12));
            matrixDimensions.Add(new KeyValuePair<int, int>(12, 16));
            Assert.AreEqual(2228, Utils.MatrixChainMultiplicationMinimalCost(matrixDimensions));
            Assert.AreEqual("(([5,10]*[10,6])*([6,30]*[30,4]))*([4,12]*[12,16])", Utils.MatrixChainMultiplicationParenthesis(matrixDimensions));
        }

        public static int NbTotalCombinationsInKeyboard(int length)
        {
            int[,] keyboard = {{ 1,2,3},{ 4,5,6},{ 7,8,9},{ -1,0,-1}};
            int?[,] cache = new int?[10,1+length];
            int result = 0;
            foreach (var p in Utils.AllPoints(keyboard))
            {
                result += NBTotalCombinationsInKeyboard_Helper(length, p, keyboard, cache);
            }

            return result;
        }

        private static int NBTotalCombinationsInKeyboard_Helper(int length, Point start, int[,] keyboard, int?[,]cache)
        {
            var keyValue = keyboard[start.X, start.Y];
            if (keyValue < 0)
            {
                return 0;
            }
            if (length <= 0)
            {
                return 0;
            }
            if (length == 1)
            {
                return 1;
            }
            if (cache[keyValue, length].HasValue)
            {
                return cache[keyValue, length].Value;
            }

            int result = 0;
            foreach (var p in Utils.AllPointsHorizontalVertical(keyboard, start.X, start.Y))
            {
                result += NBTotalCombinationsInKeyboard_Helper(length - 1, p, keyboard, cache);
            }

            result += NBTotalCombinationsInKeyboard_Helper(length - 1, start, keyboard, cache);
            cache[keyValue, length] = result;
            return result;
        }
        [Test]
        public void TestNbTotalCombinationsInKeyboard()
        {
            Assert.AreEqual(36, NbTotalCombinationsInKeyboard(2));
        }


        public static List<List<string>> WordBreakListProblem(string word, string[] dico)
        {
            var result = new List<List<string>>();
            WordBreakListProblem_Helper(word, dico, new List<string>(), result);
            return result;
        }

        public static void WordBreakListProblem_Helper(string word, string[] dico, List<string> path, List<List<string>> result)
        {
            if (word.Length == 0)
            {
                result.Add(new List<string>(path));
                return;
            }
            for (var index = 0; index < dico.Length; index++)
            {
                var d = dico[index];
                if (word.StartsWith(d))
                {
                    path.Add(d);
                    WordBreakListProblem_Helper(word.Substring(d.Length), dico, path,result);
                    path.RemoveAt(path.Count - 1);
                }
            }
        }
        [Test]
        public void TestWordBreakListProblem()
        {
            Assert.AreEqual(8, WordBreakListProblem("wordbreakproblem", new []{"this","th","is","famous", "word", "break", "b", "r","e","a","k","br","bre","break", "ak", "problem"}).Count);
        }

        public static bool WordBreakProblem(string word, string[] dico)
        {
            var cache = new bool?[1+word.Length];
            return WordBreakProblem_Helper(word, dico, cache);
        }

        public static bool WordBreakProblem_Helper(string word, string[] dico, bool?[] cache)
        {
            if (word.Length == 0)
            {
                return true;
            }

            if (cache[word.Length].HasValue)
            {
                return cache[word.Length].Value;
            }

            foreach (var d in dico)
            {
                if (word.StartsWith(d)&& WordBreakProblem_Helper(word.Substring(d.Length), dico, cache))
                {
                    cache[word.Length] = true;
                    return true;
                }
            }
            cache[word.Length] = false;
            return false;
        }
        [Test]
        public void TestWordBreakProblem()
        {
            Assert.AreEqual(true, WordBreakProblem("wordbreakproblem", new[] { "this", "th", "is", "famous", "word", "break", "b", "r", "e", "a", "k", "br", "bre", "break", "ak", "problem" }));
            Assert.AreEqual(false, WordBreakProblem("wordbreaKproblem", new[] { "this", "th", "is", "famous", "word", "break", "b", "r", "e", "a", "k", "br", "bre", "break", "ak", "problem" }));
        }


        public static bool WildcardPatternMatching(string word, string pattern)
        {
            return WildcardPatternMatching_Helper(word, pattern, new bool?[1 + word.Length, 1+pattern.Length]);
        }

        public static bool WildcardPatternMatching_Helper(string word, string pattern, bool?[,] cache)
        {
            if (pattern.Length == 0)
            {
                return word.Length == 0;
            }

            if (cache[word.Length, pattern.Length].HasValue)
            {
                return cache[word.Length, pattern.Length].Value;
            }

            if (pattern[0] != '*')
            {
                if ((word.Length == 0) || (pattern[0] != '?' && pattern[0] != word[0]) )
                {
                    return false;
                }

                cache[word.Length, pattern.Length] = WildcardPatternMatching_Helper(word.Substring(1), pattern.Substring(1), cache);
                return cache[word.Length, pattern.Length].Value;
            }
            for (int i = 0; i <= word.Length; ++i)
            {
                if (WildcardPatternMatching_Helper(word.Substring(i), pattern.Substring(1), cache))
                {
                    cache[word.Length, pattern.Length] = true;
                    return true;
                }

            }
            cache[word.Length, pattern.Length] = false;
            return false;
        }

        [Test]
        public void TestWildcardPatternMatching()
        {
            Assert.AreEqual(true, WildcardPatternMatching("xyxzzxy", "x***y"));
            Assert.AreEqual(false, WildcardPatternMatching("xyxzzxy", "x***x"));
            Assert.AreEqual(true, WildcardPatternMatching("xyxzzxy", "x***x?"));
            Assert.AreEqual(true, WildcardPatternMatching("xyxzzxy", "*"));
        }

        public static double ProbabilityAliveAfterTakingNStepsIsland(int matrixWidth, int nbSteps, int startRow, int startCol)
        {
            return ProbabilityAliveAfterTakingNStepsIsland_Helper(matrixWidth, nbSteps, startRow, startCol, new double?[matrixWidth, matrixWidth,1+nbSteps]);
        }

        private static double ProbabilityAliveAfterTakingNStepsIsland_Helper(int matrixWidth, int nbSteps, int row, int col, double?[,,] cache)
        {
            if (row < 0 || col < 0 || row >= matrixWidth || col >= matrixWidth)
            {
                return 0;
            }

            if (nbSteps <= 0)
            {
                return 1;
            }

            if (!cache[row, col, nbSteps].HasValue)
            {
                cache[row, col, nbSteps] = (ProbabilityAliveAfterTakingNStepsIsland_Helper(matrixWidth, nbSteps - 1, row - 1, col, cache)
                                            + ProbabilityAliveAfterTakingNStepsIsland_Helper(matrixWidth, nbSteps - 1, row, col-1, cache)
                                            + ProbabilityAliveAfterTakingNStepsIsland_Helper(matrixWidth, nbSteps - 1, row + 1, col, cache)
                                            + ProbabilityAliveAfterTakingNStepsIsland_Helper(matrixWidth, nbSteps - 1, row, col+1, cache)) / 4.0;
            }

            return cache[row, col, nbSteps].Value;
        }

        [Test]
        public void TestProbabilityAliveAfterTakingNStepsIsland()
        {
            Assert.AreEqual(0.5, ProbabilityAliveAfterTakingNStepsIsland(2, 1, 0, 0), 1e-8);
            Assert.AreEqual(1.0, ProbabilityAliveAfterTakingNStepsIsland(3, 1, 1, 1), 1e-8);
            Assert.AreEqual(0.25, ProbabilityAliveAfterTakingNStepsIsland(3, 3, 0, 0), 1e-8);
            Assert.AreEqual(0.57519701756973518, ProbabilityAliveAfterTakingNStepsIsland(10, 20, 3, 3), 1e-8);
        }

      
     

        [Test]
        public void TestAllCombinations()
        {
            var input = new List<List<string>>();
            var result = new List<string>(from t in Utils.AllCombinations(input) select string.Join("|", t.ToArray()));
            Assert.AreEqual(0, result.Count);
            input.Add(new List<string> { "1A" });
            result = new List<string>(from t in Utils.AllCombinations(input) select string.Join("|", t.ToArray()));
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("1A", result[0]);
            input.Add(new List<string> { "2A" });
            input.Add(new List<string> { "3A" });
            result = new List<string>(from t in Utils.AllCombinations(input) select string.Join("|", t.ToArray()));
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("1A|2A|3A", result[0]);
            input[0].Add("1B");
            input[2].Add("3B");
            result = new List<string>(from t in Utils.AllCombinations(input) select string.Join("|", t.ToArray()));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains("1A|2A|3A"));
            Assert.IsTrue(result.Contains("1B|2A|3A"));
            Assert.IsTrue(result.Contains("1A|2A|3B"));
            Assert.IsTrue(result.Contains("1B|2A|3B"));
            input.Add(new List<string> { "4A" });
            result = new List<string>(from t in Utils.AllCombinations(input) select string.Join("|", t.ToArray()));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains("1A|2A|3A|4A"));
            Assert.IsTrue(result.Contains("1B|2A|3A|4A"));
            Assert.IsTrue(result.Contains("1A|2A|3B|4A"));
            Assert.IsTrue(result.Contains("1B|2A|3B|4A"));
        }

        [Test]
        public void TestMaximumValidIndex()
        {
            Assert.AreEqual(150, Utils.MaximumValidIndex(100, 200, x => x <= 150));
            Assert.AreEqual(200, Utils.MaximumValidIndex(100, 200, x => x <= 250));
            Assert.AreEqual(100, Utils.MaximumValidIndex(100, 200, x => x <= 100));
            Assert.AreEqual(5, Utils.MaximumValidIndex(-100, +100, x => x <= 5));
            Assert.AreEqual(0, Utils.MaximumValidIndex(0, 3, x => x <= 0));
            Assert.AreEqual(1, Utils.MaximumValidIndex(0, 3, x => x <= 1));
            Assert.AreEqual(2, Utils.MaximumValidIndex(0, 3, x => x <= 2));
            Assert.AreEqual(3, Utils.MaximumValidIndex(0, 3, x => x <= 3));
            Assert.AreEqual(0, Utils.MaximumValidIndex(0, 0, x => true));
            Assert.AreEqual(1, Utils.MaximumValidIndex(0, 1, x => true));
            Assert.AreEqual(100, Utils.MaximumValidIndex(100, 100, x => true));
            Assert.AreEqual(100, Utils.MaximumValidIndex(0, 100, x => true));
        }

        [Test]
        public void TestAllPossibleSplit()
        {
            Assert.AreEqual(0, Utils.AllPossibleSplit(0, 0, false).Count);
            Assert.AreEqual(0, Utils.AllPossibleSplit(0, 0, true).Count);
            Assert.AreEqual(0, Utils.AllPossibleSplit(1000, 0, false).Count);
            Assert.AreEqual(0, Utils.AllPossibleSplit(1000, 0, true).Count);
            Assert.AreEqual(0, Utils.AllPossibleSplit(0, 1, false).Count);
            var result = ReformatAllPossibleSplit(Utils.AllPossibleSplit(0, 1, true));
            Assert.IsTrue(result.SequenceEqual(new[] { "0" }));
            result = ReformatAllPossibleSplit(Utils.AllPossibleSplit(1000, 1, false));
            Assert.IsTrue(result.SequenceEqual(new[] { "1000" }));
            result = ReformatAllPossibleSplit(Utils.AllPossibleSplit(1000, 1, true));
            Assert.IsTrue(result.SequenceEqual(new[] { "1000" }));

            result = ReformatAllPossibleSplit(Utils.AllPossibleSplit(3, 2, false));
            Assert.IsTrue(result.SequenceEqual(new[] { "1|2" , "2|1"}));
            result = ReformatAllPossibleSplit(Utils.AllPossibleSplit(3, 2, true));
            Assert.IsTrue(result.SequenceEqual(new[] { "0|3", "1|2", "2|1","3|0"}));

            Assert.AreEqual(0, Utils.AllPossibleSplit(1, 1000, false).Count);
            Assert.AreEqual(10, Utils.AllPossibleSplit(1, 10, true).Count);

            result = ReformatAllPossibleSplit(Utils.AllPossibleSplit(20, 5, false));
            Assert.AreEqual(Utils.Combination(20-1, 5-1), result.Count);
            result = ReformatAllPossibleSplit(Utils.AllPossibleSplit(20, 5, true));
            Assert.AreEqual(Utils.Combination(20+5-1, 20), result.Count);
        }
        private static List<string> ReformatAllPossibleSplit(List<List<int>> tmp)
        {
            var result = new List<string>();
            foreach(var e in tmp)
            {
                result.Add(string.Join("|", e.Select(x=>x.ToString()).ToArray()));
            }

            result.Sort();
            return result;
        }

        [Test]
        public void TestIndexOf()
        {
            int[,] m = {{1, 2, 3}, {4, 5, 6}};
            Assert.AreEqual(new Point(1, 2), m.IndexOf(6));
            Assert.IsFalse(m.IndexOf(7).HasValue);
        }

        [Test]
        public void TestRotate45DegreesClockwise()
        {
            int[,] m = {{1, 2, 3}, {4, 5, 6}};
            var rotated = m.Rotate45DegreesClockwise();
            Assert.AreEqual(rotated.GetLength(0), m.GetLength(1));
            Assert.AreEqual(rotated.GetLength(1), m.GetLength(0));
            int[,] rotatedExpected = {{4, 1}, {5, 2}, {6, 3}};
            for (int row = 0; row < rotatedExpected.GetLength(0); ++row)
                for (int col = 0; col < rotatedExpected.GetLength(1); ++col)
                {
                    Assert.AreEqual(rotatedExpected[row, col], rotated[row, col]);
                }
        }

        [Test]
        public void TestMaxInRow()
        {
            int[,] m = { { 1, 2, 3 }, { 4, 5, 6 } };
            Assert.AreEqual(3, m.MaxInRow(0));
            Assert.AreEqual(6, m.MaxInRow(1));
        }

        [Test]
        public void TestMaxInCol()
        {
            int[,] m = { { 1, 2, 3 }, { 4, 5, 6 } };
            Assert.AreEqual(4, m.MaxInCol(0));
            Assert.AreEqual(5, m.MaxInCol(1));
            Assert.AreEqual(6, m.MaxInCol(2));
        }

        [Test]
        public void TestRow()
        {
            int[,] m = { { 1, 2, 3 }, { 4, 5, 6 } };
            Assert.IsTrue(m.Row(0).SequenceEqual(new[] { 1, 2, 3 }));
            Assert.IsTrue(m.Row(1).SequenceEqual(new[] { 4,5,6 }));
        }

        [Test]
        public void TestCol()
        {
            int[,] m = { { 1, 2, 3 }, { 4, 5, 6 } };
            Assert.IsTrue(m.Col(0).SequenceEqual(new[] { 1, 4 }));
            Assert.IsTrue(m.Col(1).SequenceEqual(new[] { 2, 5 }));
            Assert.IsTrue(m.Col(2).SequenceEqual(new[] { 3, 6 }));
        }

        [Test]
        public void TestMaximumProdRodCutting()
        {
            Assert.AreEqual(9, MaximumProdRodCutting(6));
            Assert.AreEqual(18, MaximumProdRodCutting(8));
            Assert.AreEqual(243, MaximumProdRodCutting(15));
        }

        public static int MaximumProdRodCutting(int rodLength)
        {
            if (rodLength <= 1)
            {
                return rodLength;
            }
            var result = new int[1+rodLength];
            result[0] = result[1] = 1;
            for (int i = 2; i <= rodLength; ++i)
            {
                result[i] = i;
                for (int j = 1; j < i; ++j)
                {
                    result[i] = Math.Max(result[i], result[j] * result[i - j]);
                }
            }
            return result[rodLength];
        }

        [Test]
        public void TestRodCutting()
        {
            Assert.AreEqual(10, RodCutting(4, new []{1,2,3,4,5,6,7,8}, new []{1,5,8,9,10,17,17,20}));
        }

        private static int RodCutting(int rodLength, int[] length, int[] price)
        {
            int[] maxPrices = new int[1+rodLength];
            for (int currentRodLength = 1; currentRodLength < maxPrices.Length; ++currentRodLength)
            {
                for (int j = 0; j < length.Length; ++j)
                {
                    if (length[j] > currentRodLength)
                    {
                        continue;
                    }

                    int jScore = price[j] + maxPrices[currentRodLength - length[j]];
                    if (jScore > maxPrices[currentRodLength])
                    {
                        maxPrices[currentRodLength] = jScore;
                    }
                }
            }
            return maxPrices.Max();
        }

        public static int NumberOfTimesPatternAppearsInStringSubsequence(string s, string pattern)
        {
            var cache = new int?[s.Length,pattern.Length];
            var result = NumberOfTimesPatternAppearsInStringSubsequence_Helper(s, pattern, 0, 0, cache);
            return result;
        }

        public static int NumberOfTimesPatternAppearsInStringSubsequence_Helper(string s, string pattern, int idxS, int idxPattern, int?[,] cache)
        {
            if (idxPattern >= pattern.Length)
            {
                return 1;
            }

            if (idxS >= s.Length)
            {
                return 0;
            }

            if (cache[idxS, idxPattern].HasValue)
            {
                return cache[idxS, idxPattern].Value;
            }

            int result = 0;
            for (int j = idxS; j < s.Length; ++j)
            {
                if (pattern[idxPattern] == s[j])
                {
                    result += NumberOfTimesPatternAppearsInStringSubsequence_Helper(s, pattern, j+1, idxPattern+1, cache);
                }
            }

            cache[idxS, idxPattern] = result;
            return result;
        }

        [Test]
        public void TestNumberOfTimesPatternAppearsInStringSubsequence()
        {
            Assert.AreEqual(7, NumberOfTimesPatternAppearsInStringSubsequence("subsequence", "sue"));
        }

        public static int CountNDigitsWithoutConsecutiveOne(int nbDigits)
        {
            return CountNDigitsWithoutConsecutiveOne_Helper(nbDigits, false, new int?[1+nbDigits,2]);
        }
        // cache[nbDigits,0] = number of distinct binary numbers with no consecutive 1 if allowed to start with 0 or 1
        // cache[nbDigits,1] = number of distinct binary numbers with no consecutive 1 if mandatory to start with 0
        private static int CountNDigitsWithoutConsecutiveOne_Helper(int nbDigits, bool mustStartWith0, int?[,] cache)
        {
            if (nbDigits == 0)
            {
                return 0;
            }

            if (nbDigits == 1)
            {
                return mustStartWith0?1:2;
            }

            if (cache[nbDigits, mustStartWith0 ? 1 : 0].HasValue)
            {
                // ReSharper disable once PossibleInvalidOperationException
                return cache[nbDigits, mustStartWith0 ? 1 : 0].Value;
            }

            int count = CountNDigitsWithoutConsecutiveOne_Helper(nbDigits - 1, false, cache);
            if (!mustStartWith0)
            {
                count += CountNDigitsWithoutConsecutiveOne_Helper(nbDigits - 1, true, cache);
            }

            cache[nbDigits, mustStartWith0 ? 1 : 0] = count;
            return count;
        }

        [Test]
        public void TestCountNDigitsWithoutConsecutiveOne()
        {
            Assert.AreEqual(13, CountNDigitsWithoutConsecutiveOne(5));
        }

        [Test]
        public void TestMaxKnapsackValue()
        {
            List<int> keptIndexes;
            Assert.AreEqual(2, Utils.MaxKnapsackValue(10, new[] { 10, 8, 3, 7 }, new[] { 1, 1, 1, 1 }, 0, out keptIndexes));
            Assert.AreEqual(new List<int> { 2, 3 }, keptIndexes);
            Assert.AreEqual(4, Utils.MaxKnapsackValue(11, new[] { 10, 9, 3, 7 }, new[] { 3, 3, 2, 2 }, 0, out keptIndexes));
            Assert.AreEqual(new List<int> { 2, 3 }, keptIndexes);
            Assert.AreEqual(90, Utils.MaxKnapsackValue(10, new[] { 5, 4, 6, 3 }, new[] { 10, 40, 30, 50 }, 0, out keptIndexes));
            Assert.AreEqual(new List<int> { 1, 3 }, keptIndexes);
            Assert.AreEqual(60, Utils.MaxKnapsackValue(10, new[] { 1, 2, 3, 8,7,4 }, new[] { 20, 5, 10, 40, 15, 25 }, 0, out keptIndexes));
            Assert.AreEqual(new List<int> { 0, 3 }, keptIndexes);
        }
        
       [Test]
        public void TestAllPermutations()
        {
            var observed = Utils.AllPermutations(new List<char>()).Select(x => new string(x.ToArray())).ToList();
            Assert.IsTrue(observed.Count == 0);
            observed = Utils.AllPermutations(new List<char> { 'A' }).Select(x => new string(x.ToArray())).ToList();
            observed.Sort();
            Assert.IsTrue(observed.SequenceEqual(new[] { "A" }));
            observed = Utils.AllPermutations(new List<char> { 'A', 'B', 'C' }).Select(x => new string(x.ToArray())).ToList();
            observed.Sort();
            Assert.IsTrue(observed.SequenceEqual(new[] { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" }));
        }

        [Test]
        public void TestAllCombinationAsBitArray()
        {
            var r = Utils.AllCombinationMaskAsBitArray(3, 1).ToList();
            r.Sort();
            Assert.IsTrue(r.SequenceEqual(new long[] {1, 2, 4}));
            r = Utils.AllCombinationMaskAsBitArray(3, 2).ToList();
            r.Sort();
            Assert.IsTrue(r.SequenceEqual(new long[] {3, 5, 6}));
            r = Utils.AllCombinationMaskAsBitArray(3, 0).ToList();
            r.Sort();
            Assert.IsTrue(r.SequenceEqual(new long[] {0}));
            r = Utils.AllCombinationMaskAsBitArray(4, 2).ToList();
            r.Sort();
            Assert.IsTrue(r.SequenceEqual(new long[] {3, 5, 6, 9, 10, 12}));
        }

        [Test]
        public void TestTowerOfHanoi()
        {
            var moves = new List<Tuple<int, string, string>>();
            Utils.TowerOfHanoi(3, "A","C","B", moves);
            Assert.IsTrue(moves.Select(x => x.Item1 + x.Item2 + x.Item3) .SequenceEqual(new[] {"1AC", "2AB", "1CB", "3AC", "1BA", "2BC", "1AC"}));
        }

        [Test]
        public void TestLeCompteEstBon()
        {
            var result = Utils.LeCompteEstBon(new[] {3, 4, 50, 2, 6, 9}, 328);
            Assert.AreEqual(328, result.Item1);

            result = Utils.LeCompteEstBon(new[] { 1, 2, 6, 5, 75, 9 }, 461);
            Assert.AreEqual(461, result.Item1);

            result = Utils.LeCompteEstBon(new[] { 7, 4, 3, 9, 25, 1 }, 911);
            Assert.AreEqual(911, result.Item1);

            result = Utils.LeCompteEstBon(new[] { 7, 8, 10, 25, 50, 100 }, 909);
            Assert.AreEqual(1, Math.Abs(result.Item1-909));

            result = Utils.LeCompteEstBon(new[] { 4, 5, 6, 7, 9, 10 }, 998);
            Assert.AreEqual(1, Math.Abs(result.Item1 - 998));
            result = Utils.LeCompteEstBon(new[] { 4, 5, 6, 7, 9, 10 }, 666);
            Assert.AreEqual(666, result.Item1);

            result = Utils.LeCompteEstBon(new[] { 2, 4, 5, 10 }, 444);
            Assert.AreEqual(400, result.Item1);
        }
    }
}
