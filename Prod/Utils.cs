using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        public static void Swap<T>(IList<T> list, int i, int j)
        {
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        /// <summary>
        /// compute all combinations of 'IList<List<T>> allItems' containing exactly one element of each sub list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allItems"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> AllCombinations<T>(IList<List<T>> allItems)
        {
            var result = new List<List<T>>();
            if ((allItems == null) || (allItems.Count == 0) || allItems.Any(x => (x == null) || (x.Count == 0)))
            {
                return result;
            }
            AllCombinationsHelper(allItems, new List<T>(), result);
            return result;
        }
        private static void AllCombinationsHelper<T>(IList<List<T>> allItems, List<T> currentSolutionInProgress, List<List<T>> allSolutionSoFar)
        {
            if (currentSolutionInProgress.Count == allItems.Count)
            {
                allSolutionSoFar.Add(currentSolutionInProgress); // a new combination has been found
                return;
            }
            foreach (var t in allItems[currentSolutionInProgress.Count])
            {
                AllCombinationsHelper(allItems, new List<T>(currentSolutionInProgress) { t }, allSolutionSoFar);
            }
        }

        public static IList<IList<T>> AllPermutations<T>(List<T> data)
        {
            var result = new List<IList<T>>();
            AllPermutationsHelper(data, 0, result);
            return result;
        }
        private static void AllPermutationsHelper<T>(List<T> data, int i, IList<IList<T>> result)
        {
            if (i == data.Count - 1)
            {
                result.Add(new List<T>(data));
                return;
            }
            //var alreadyUsed = new HashSet<T>(); //to discard duplicate solutions
            for (var j = i; j < data.Count; ++j)
            {
                //if (!alreadyUsed.Add(data[j])) continue; //to discard duplicate solutions
                var tmp = data[i];
                data[i] = data[j];
                data[j] = tmp;
                AllPermutationsHelper(data, i + 1, result);
                tmp = data[i];
                data[i] = data[j];
                data[j] = tmp;
            }
        }



        #region Matrix Chain Multiplication

        //compute the optimal cost to multiply 'n' matrix together in o(n^3) time and o(n^2) memory
        // For 2 consecutive matrices (row1,col1) & (row2,col2) we always have col1=row2
        public static int MatrixChainMultiplicationMinimalCost(IList<KeyValuePair<int, int>> matrixDimensions)
        {
            return MatrixChainMultiplicationMinimalCost_Helper(matrixDimensions, new int?[matrixDimensions.Count, matrixDimensions.Count], null, 0, matrixDimensions.Count-1);
        }
        private static int MatrixChainMultiplicationMinimalCost_Helper(IList<KeyValuePair<int, int>> matrices, int?[,] m, int[,] whereToSplit, int i, int j)
        {
            if (i >= j)
            {
                return 0;
            }
            if (m[i, j].HasValue)
            {
                return m[i, j].Value;
            }
            var currentResult = int.MaxValue;
            for (var k = i; k < j; ++k)
            {
                //we compute the cost of splitting matrices multiplication between [i,k] and [k+1,j]
                var costIfSplitAtK = MatrixChainMultiplicationMinimalCost_Helper(matrices, m, whereToSplit, i, k) + MatrixChainMultiplicationMinimalCost_Helper(matrices, m, whereToSplit, k + 1, j);
                costIfSplitAtK += matrices[i].Key * matrices[k].Value * matrices[j].Value;
                if (costIfSplitAtK < currentResult)
                {
                    currentResult = costIfSplitAtK;
                    if (whereToSplit != null)
                    {
                        whereToSplit[i, j] = k;
                    }
                }
            }
            m[i, j] = currentResult;
            return currentResult;
        }
        //same as above but return a string with the right parenthesis
        public static string MatrixChainMultiplicationParenthesis(IList<KeyValuePair<int, int>> matrixDimensions)
        {
            var whereToSplit = new int[matrixDimensions.Count, matrixDimensions.Count];
            MatrixChainMultiplicationMinimalCost_Helper(matrixDimensions, new int?[matrixDimensions.Count, matrixDimensions.Count], whereToSplit, 0, matrixDimensions.Count - 1);
            return MatrixChainMultiplicationParenthesis_Helper(matrixDimensions.Select(x => "["+x.Key+","+x.Value+"]").ToList(), whereToSplit, 0, matrixDimensions.Count - 1);
        }
        private static string MatrixChainMultiplicationParenthesis_Helper(IList<string> matrices, int[,] whereToSplit, int i, int j)
        {
            if (i == j)
            {
                return matrices[i];
            }
            var k = whereToSplit[i, j];
            var result = MatrixChainMultiplicationParenthesis_Helper(matrices, whereToSplit, i, k) + "*" + MatrixChainMultiplicationParenthesis_Helper(matrices, whereToSplit, k + 1, j);
            if (i == 0 && j == matrices.Count - 1)
            {
                return result;
            }
            return "(" +result + ")";
        }
        #endregion


        //returns all possible ways to split the amount 'totalAmountToSplit' between 'nbRecipients' persons
        //if allowZeroAmount is true
        //  will allow some recipients to receive 0
        //  Number of combinations =  C(totalAmountToSplit+nbRecipients-1,totalAmountToSplit) => C(n,p) = n! / [p! (n-p)!]
        //else 
        //  all recipients must receive at least '1' 
        //  Number of combinations =  C(totalAmountToSplit-1, nbRecipients-1) => C(n,p) = n! / [p! (n-p)!]
        public static List<List<int>> AllPossibleSplit(int totalAmountToSplit, int nbRecipients, bool allowZeroAmount)
        {
            var result = new List<List<int>>();
            AllPossibleSplitHelper(totalAmountToSplit, nbRecipients, allowZeroAmount, new List<int>(), result);
            return result;
        }
        private static void AllPossibleSplitHelper(int remainingAmountToSplit, int nbRecipients, bool allowZeroAmount, List<int> currentSolutionInProgress, List<List<int>> allSolutionSoFar)
        {
            if (currentSolutionInProgress.Count == nbRecipients)
            {
                if ((remainingAmountToSplit == 0)&&(currentSolutionInProgress.Count!=0))
                {
                    allSolutionSoFar.Add(currentSolutionInProgress); // a new combination has been found
                }
                return;
            }
            for(var amountForNextRecipient = allowZeroAmount?0:1; amountForNextRecipient<= remainingAmountToSplit;++amountForNextRecipient)
            {
                AllPossibleSplitHelper(remainingAmountToSplit-amountForNextRecipient, nbRecipients, allowZeroAmount, new List<int>(currentSolutionInProgress) { amountForNextRecipient }, allSolutionSoFar);
            }
        }



        //find the max index for which IsValid is true in o(log(N)) time (using dichotomy search)
        //hypothesis: IsValid[min] is true and will always be true for an interval [min, y] then always false after y
        public static int MaximumValidIndex(int minLength, int maxLength, Func<int, bool> IsValid)
        {
            while (minLength < maxLength)
            {
                var middle = (minLength + maxLength + 1) / 2;
                if (IsValid(middle))
                {
                    minLength = middle;
                }
                else
                {
                    maxLength = middle - 1;
                }
            }
            return minLength;
        }

        /// <summary>
        /// Stable Mariage Matching using Gale-Shapley algo (in o (n^2)
        /// </summary>
        /// <param name="mPreference">for each m, the preferred w</param>
        /// <param name="wPreference">foreach w, the preferred m</param>
        /// <returns>a dictionary with, for each each w (.Key), the preferred m (.Value) </returns>
        public static Dictionary<T, T> StableMatching<T>(Dictionary<T, List<T>> mPreference, Dictionary<T, List<T>> wPreference)
        {
            var currentMatching = new Dictionary<T, T>();
            var mWithNotMatching = mPreference.Keys.ToList();
            var mToNextPreferredW = new Dictionary<T,int>();
            foreach (var m in mPreference.Keys)
            {
                mToNextPreferredW[m] = 0;
            }

            while (mWithNotMatching.Count != 0)
            {
                var m = mWithNotMatching.Last();
                var w = mPreference[m][mToNextPreferredW[m]];
                ++mToNextPreferredW[m];
                //both 'm' and 'w' are available
                if (!currentMatching.ContainsKey(w))
                {
                    mWithNotMatching.RemoveAt(mWithNotMatching.Count - 1);
                    currentMatching[w] = m;
                    continue;
                }
                var otherM = currentMatching[w];
                if (wPreference[w].IndexOf(m) < wPreference[w].IndexOf(otherM))
                {
                    //we update the current matching of 'w' from 'otherM' to 'm'
                    currentMatching[w] = m;
                    mWithNotMatching.RemoveAt(mWithNotMatching.Count - 1);
                    mWithNotMatching.Add(otherM);
                }
            }
            return currentMatching;
        }




        public static int MaxKnapsackValue(int knapSackCapacity, int[] weights, int[] values, int penaltyForNotFillingBag, out List<int> takenIndexes)
        {
            var nbItems = weights.Length;
            var computedValues = new int?[nbItems, knapSackCapacity+1];
            var takeIt = new bool[nbItems, knapSackCapacity+1];
            var result = Knapsack(nbItems - 1, knapSackCapacity, weights, values, penaltyForNotFillingBag, computedValues, takeIt);

            //We build 'takenIndexes'
            takenIndexes = new List<int>();
            var remainingCapacity = knapSackCapacity;
            for (var index = (nbItems - 1); index >= 0; --index)
            {
                if (takeIt[index,remainingCapacity])
                {
                    takenIndexes.Add(index);
                    remainingCapacity -= weights[index];
                }
            }
            takenIndexes.Sort();

            return result;
        }
        private static int Knapsack(int maxIndexOfItemToCheck, int remainingCapacity, int[] weights, int[] values, int penaltyForNotFillingBag, int?[,] computedValues, bool[,] takeIt)
        {
            if (computedValues[maxIndexOfItemToCheck,remainingCapacity].HasValue)
            {
                return computedValues[maxIndexOfItemToCheck,remainingCapacity].Value;
            }

            var mustFillBagExactly = (penaltyForNotFillingBag == int.MinValue) || (penaltyForNotFillingBag == int.MaxValue);
            int dontTakeValue;
            int? takeValue = null;

            if (maxIndexOfItemToCheck == 0)
            {
                if (mustFillBagExactly && remainingCapacity > 0)
                {
                    dontTakeValue = int.MinValue;
                }
                else
                {
                    dontTakeValue = penaltyForNotFillingBag * remainingCapacity;
                }
                if (weights[maxIndexOfItemToCheck] <= remainingCapacity)
                {
                    takeValue = values[maxIndexOfItemToCheck];
                    if (weights[maxIndexOfItemToCheck] < remainingCapacity)
                    {
                        if (mustFillBagExactly)
                        {
                            takeValue = int.MinValue;
                        }
                        else
                        {
                            takeValue += penaltyForNotFillingBag * (remainingCapacity - weights[maxIndexOfItemToCheck]);
                        }
                    }
                }
            }
            else
            {
                dontTakeValue = Knapsack(maxIndexOfItemToCheck - 1, remainingCapacity, weights, values, penaltyForNotFillingBag, computedValues, takeIt);
                if (weights[maxIndexOfItemToCheck] <= remainingCapacity)
                {
                    var knapsackIfTake = Knapsack(maxIndexOfItemToCheck - 1, remainingCapacity - weights[maxIndexOfItemToCheck], weights, values, penaltyForNotFillingBag, computedValues, takeIt);
                    if (mustFillBagExactly && (knapsackIfTake == int.MinValue))
                    {
                        takeValue = int.MinValue;
                    }
                    else
                    {
                        takeValue = values[maxIndexOfItemToCheck] + knapsackIfTake;
                    }
                }
            }
            if (takeValue.HasValue && (takeValue.Value > dontTakeValue))
            {
                computedValues[maxIndexOfItemToCheck,remainingCapacity] = takeValue;
                takeIt[maxIndexOfItemToCheck,remainingCapacity] = true;
            }
            else
            {
                computedValues[maxIndexOfItemToCheck,remainingCapacity] = dontTakeValue;
            }
            return computedValues[maxIndexOfItemToCheck,remainingCapacity].Value;
        }

        

        //Compute all ways to select 'p' elements among 'n'
        //Each combination is stored in a 64 bit array (a long) where exactly 'p' bits are set to 1
        public static IEnumerable<long> AllCombinationMaskAsBitArray(int n, int p)
        {
            if (p == 0)
            {
                yield return 0L;
                yield break;
            }
            for (int i = p - 1; i < n; ++i)
            {
                foreach (var e in AllCombinationMaskAsBitArray(i, p - 1))
                {
                    yield return e | (1L << i);
                }
            }
        }


        //Solve tower of Hanoi problem in o(2^nbDisks) time
        //We have 'n' disks in 'source' , ordered from the smallest one (id == 1) at top, to the biggest one (id == n) at bottom.
        //We want to move this entire tower (in the same order) to 'destination' moving one disk at a time, 
        //and we are not allowed to put a bigger disk on top of a smaller one
        //The number of moves required is exactly 2^n -1  ( move.Item1: disk to move / move.Item2:source / move.Item3:target)
        public static void TowerOfHanoi(int n, string source, string destination, string notUsed, List<Tuple<int,string,string>> moves)
        {
            if (n < 1)
            {
                return;
            }
            TowerOfHanoi(n - 1, source, notUsed, destination, moves);
            moves.Add(Tuple.Create(n, source, destination));
            TowerOfHanoi(n - 1, notUsed, destination, source, moves);
        }

        //solve 'le compte est bon' in o(3^N) time (N = length of allowed numbers)
        // return a Tuple where Item1 is the best achievable result (nearest to target) & Item2 is a description to go there
        public static Tuple<long,string> LeCompteEstBon(int[] numbers, int target)
        {
            // combinationMask => all achievable results using all elements of this combinationMask (+ a description string)
            var combinationToAchievableResultsWithDescription = new Dictionary<long, Dictionary<long,string>>();
            //we compute all achievable results using exactly 1 number (result = the number itself)
            for (int i = 0; i < numbers.Length; ++i)
            {
                combinationToAchievableResultsWithDescription[1L << i] = new Dictionary<long, string>();
                combinationToAchievableResultsWithDescription[1L << i][numbers[i]] = numbers[i].ToString();
            }
            for (int nbUsedNumbers = 2; nbUsedNumbers <= numbers.Length; ++nbUsedNumbers)
            {
                //for each combination using exactly 'nbUsedNumbers' numbers
                foreach (var combination in AllCombinationMaskAsBitArray(numbers.Length, nbUsedNumbers))
                {
                    //we compute all achievable results for this combination
                    var allCombinationResultsWithDescription = new Dictionary<long, string>();
                    //we split 'combinationMask' into 2 non empty sub set : leftCombination && rightCombination
                    for (long leftCombination= (combination - 1) & combination; leftCombination!=0; leftCombination = (leftCombination - 1) & combination)
                    {
                        var rightCombination = combination - leftCombination;
                        foreach (var singleResultWithDescriptionLeft in combinationToAchievableResultsWithDescription[leftCombination])
                            foreach (var singleResultWithDescriptionRight in combinationToAchievableResultsWithDescription[rightCombination])
                            {
                                //we compute all achievable results using leftResult & rightResult
                                long leftResult = singleResultWithDescriptionLeft.Key;
                                var leftDescription = singleResultWithDescriptionLeft.Value;
                                long rightResult = singleResultWithDescriptionRight.Key;
                                var rightDescription = singleResultWithDescriptionRight.Value;
                                allCombinationResultsWithDescription[leftResult + rightResult] = "(" + leftDescription + "+" + rightDescription + ")";
                                allCombinationResultsWithDescription[leftResult - rightResult] = "(" + leftDescription + "-" + rightDescription + ")";
                                allCombinationResultsWithDescription[leftResult * rightResult] = "" + leftDescription + "*" + rightDescription + "";
                                if (rightResult!=0&&leftResult % rightResult==0)
                                {
                                    allCombinationResultsWithDescription[leftResult / rightResult] = "(" + leftDescription + ")/(" + rightDescription + ")";
                                }
                            }
                    }
                    combinationToAchievableResultsWithDescription[combination] = allCombinationResultsWithDescription;
                }
            }
            
            //we look for the better result among all achievable results
            Tuple<long, string> finalResult = null;
            foreach (var achievableResultsWithDescription in combinationToAchievableResultsWithDescription)
                foreach (var singleResultWithDescription in achievableResultsWithDescription.Value)
                {
                    if (finalResult == null || Math.Abs(singleResultWithDescription.Key - target) < Math.Abs(finalResult.Item1 - target))
                    {
                        finalResult = Tuple.Create(singleResultWithDescription.Key, singleResultWithDescription.Value);
                    }
                }
            return finalResult;
        }

        #region reading input
        private static List<string> ReadAllLines()
        {
            var result = new List<string>();
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                result.Add(line);
            }
            return result;
        }
        private static int ReadInt() { return ReadSingleT(int.Parse); }
        private static IEnumerable<int> ReadInts() { return ReadAllTInLine(int.Parse); }
        private static int[] ReadIntsColumn(int nbRows) { return ReadTColumn(nbRows, ReadInt); }
        private static int[,] ReadIntMatrix(int nbRows) { return ReadTMatrix(nbRows, ReadInts); }
        private static long ReadLong() { return ReadSingleT(long.Parse); }
        private static IEnumerable<long> ReadLongs() { return ReadAllTInLine(long.Parse); }
        private static long[] ReadLongsColumn(int nbRows) { return ReadTColumn(nbRows, ReadLong); }
        private static long[,] ReadLongMatrix(int nbRows) { return ReadTMatrix(nbRows, ReadLongs); }
        private static double ReadDouble() { return ReadSingleT(double.Parse); }
        private static IEnumerable<double> ReadDoubles() { return ReadAllTInLine(double.Parse); }
        private static double[] ReadDoublesColumn(int nbRows) { return ReadTColumn(nbRows, ReadDouble); }
        private static double[,] ReadDoubleMatrix(int nbRows) { return ReadTMatrix(nbRows, ReadDoubles); }
        private static char[,] ReadCharMatrix(int nbRows) { return ReadTMatrix(nbRows, Console.ReadLine); }
        private static string[] ReadStringsColumn(int nbRows) { return ReadTColumn(nbRows, Console.ReadLine); }
        private static System.Drawing.Point ReadPoint() { var xy = ReadInts().ToArray(); return new System.Drawing.Point(xy[0], xy[1]); }
        private static System.Drawing.Point[] ReadPoints()
        {
            var ints = ReadInts().ToArray();
            var result = new System.Drawing.Point[ints.Length / 2];
            for (var i = 0; i < result.Length; ++i)
            {
                result[i] = new Point(ints[2 * i], ints[2 * i + 1]);
            }
            return result;
        }
        private static System.Drawing.Point[] ReadPointsColumn(int nbRows) { return ReadTColumn(nbRows, ReadPoint); }
        //Tuple<left,top,right,bottom> with right>left && top>bottom
        private static Tuple<int, int, int, int> ReadRectangle()
        {
            var x1y1x2y2 = ReadInts().ToArray();
            var left = Math.Min(x1y1x2y2[0], x1y1x2y2[2]);
            var top = Math.Max(x1y1x2y2[1], x1y1x2y2[3]);
            var right = Math.Max(x1y1x2y2[0], x1y1x2y2[2]);
            var bottom = Math.Min(x1y1x2y2[1], x1y1x2y2[3]);
            return Tuple.Create(left, top, right, bottom);
        }
        private static Tuple<int, int, int, int>[] ReadRectanglesColumn(int nbRows) { return ReadTColumn(nbRows, ReadRectangle); }
        private static char[,] ReadMaze(int nbRows)
        {
            char[,] maze = null;
            for (int row = 0; row < nbRows; row++)
            {
                var line = Console.ReadLine();
                if (maze == null)
                {
                    maze = new char[nbRows, line.Length];
                }
                for (int col = 0; col < line.Length; ++col)
                {
                    maze[row, col] = line[col];
                }
            }
            return maze;
        }
        private static T ReadSingleT<T>(Func<string, T> parseT) { return parseT(Console.ReadLine() ?? ""); }
        private static IEnumerable<T> ReadAllTInLine<T>(Func<string, T> parseT) { return (Console.ReadLine() ?? "").Trim().Split(' ', ';', ',', '\t').Select(parseT); }
        private static T[] ReadTColumn<T>(int nbRows, Func<T> readT)
        {
            var result = new T[nbRows];
            for (var i = 0; i < nbRows; ++i)
            {
                result[i] = readT();
            }
            return result;
        }
        private static T[,] ReadTMatrix<T>(int nbRows, Func<IEnumerable<T>> readTs)
        {
            T[,] result = null;
            for (var row = 0; row < nbRows; ++row)
            {
                var line = readTs().ToArray();
                if (result == null)
                {
                    result = new T[nbRows, line.Length];
                }
                for (int col = 0; col < line.Length; ++col)
                {
                    result[row, col] = line[col];
                }
            }
            return result;
        }
        #endregion

        #region matrix tools
        public static TY[,] Select<TX, TY>(this TX[,] data, Func<TX, TY> to)
        {
            var result = new TY[data.GetLength(0), data.GetLength(1)];
            for (var row = 0; row < data.GetLength(0); ++row)
                for (var col = 0; col < data.GetLength(1); ++col)
                {
                    result[row, col] = to(data[row, col]);
                }
            return result;
        }
        public static void SetAll<T>(this T[,] data, T newValue)
        {
            for (var row = 0; row < data.GetLength(0); ++row)
                for (var col = 0; col < data.GetLength(1); ++col)
                {
                    data[row, col] = newValue;
                }
        }
        public static Point? IndexOf<T>(this T[,] data, T toFind)
        {
            for (var row = 0; row < data.GetLength(0); ++row)
                for (var col = 0; col < data.GetLength(1); ++col)
                {
                    if (Equals(toFind, data[row, col]))
                    {
                        return new Point(row, col);
                    }
                }
            return null;
        }
        public static T[,] Rotate45DegreesClockwise<T>(this T[,] data)
        {
            var result = new T[data.GetLength(1), data.GetLength(0)];
            for (var row = 0; row < data.GetLength(0); ++row)
                for (var col = 0; col < data.GetLength(1); ++col)
                {
                    result[col, data.GetLength(0) - 1 - row] = data[row, col];
                }
            return result;
        }
        public static void Display<T>(this T[,] data, string separator) { data.Display(separator, x => x.ToString()); }
        public static void Display<T>(this T[,] data, string separator, Func<T, string> toString)
        {
            for (var row = 0; row < data.GetLength(0); ++row)
            {
                string line = "";
                for (var col = 0; col < data.GetLength(1); ++col)
                {
                    if (col != 0)
                    {
                        line += separator;
                    }
                    line += toString(data[row, col]);
                }
                Console.WriteLine(line);
            }
        }
        public static T[] ToArray<T>(this T[,] data)
        {
            var result = new T[data.Length];
            int idx = 0;
            for (var row = 0; row < data.GetLength(0); ++row)
                for (var col = 0; col < data.GetLength(1); ++col)
                {
                    result[idx++] = data[row, col];
                }
            return result;
        }
        public static T[,] Resize<T>(this T[,] data, int nbRows, int nbCols, T defaultValue)
        {
            var result = new T[nbRows, nbCols];
            for (var row = 0; row < nbRows; ++row)
                for (var col = 0; col < nbCols; ++col)
                {
                    result[row, col] = Default(data, row, col, defaultValue);
                }
            return result;
        }
        public static List<T> ToList<T>(this T[,] data) { return data.ToArray().ToList(); }
        public static IEnumerable<Point> AllPoints<T>(T[,] data)
        {
            for (var row = 0; row < data.GetLength(0); ++row)
                for (var col = 0; col < data.GetLength(1); ++col)
                {
                    yield return new Point(row, col);
                }
        }
        public static IEnumerable<Point> AllPointsHorizontalVertical<T>(T[,] data, Point p) { return AllPointsHorizontalVertical(data, p.X, p.Y); }
        public static IEnumerable<Point> AllPointsHorizontalVertical<T>(T[,] data, int row, int col)
        {
            if (IsValidCoordinate(data, row - 1, col))
            {
                yield return new Point(row - 1, col);
            }
            if (IsValidCoordinate(data, row + 1, col))
            {
                yield return new Point(row + 1, col);
            }
            if (IsValidCoordinate(data, row, col - 1))
            {
                yield return new Point(row, col - 1);
            }
            if (IsValidCoordinate(data, row, col + 1))
            {
                yield return new Point(row, col + 1);
            }
        }
        public static IEnumerable<Point> AllPointsAround<T>(T[,] data, Point p, int distance) { return AllPointsAround(data, p.X, p.Y, distance); }
        public static IEnumerable<Point> AllPointsAround<T>(T[,] data, int row, int col, int distance)
        {
            for (var x = row - distance; x <= row + distance; ++x)
                for (var y = col - distance; y <= col + distance; ++y)
                {
                    if ((x == row) && (y == col))
                    {
                        continue;
                    }
                    if (IsValidCoordinate(data, x, y))
                    {
                        yield return new Point(x, y);
                    }
                }
        }
        public static bool IsValidCoordinate<T>(T[,] data, int row, int col)
        {
            return row >= 0 && col >= 0 && row < data.GetLength(0) && col < data.GetLength(1);
        }
        public static T Default<T>(T[,] data, int row, int col, T defaultIfInvalid)
        {
            return IsValidCoordinate(data, row, col) ? data[row, col] : defaultIfInvalid;
        }
        public static int MaxInRow(this int[,] data, int row)
        {
            int result = int.MinValue;
            for (int col = 0; col < data.GetLength(1); ++col)
            {
                result = Math.Max(result, data[row, col]);
            }
            return result;
        }
        public static int MaxInCol(this int[,] data, int col)
        {
            int result = int.MinValue;
            for (int row = 0; row < data.GetLength(0); ++row)
            {
                result = Math.Max(result, data[row, col]);
            }
            return result;
        }
        public static T[] Row<T>(this T[,] data, int row)
        {
            var result = new T[data.GetLength(1)];
            for (int col = 0; col < result.Length; ++col)
            {
                result[col] = data[row, col];
            }
            return result;
        }
        public static T[] Col<T>(this T[,] data, int col)
        {
            var result = new T[data.GetLength(0)];
            for (int row = 0; row < result.Length; ++row)
            {
                result[row] = data[row, col];
            }
            return result;
        }
        #endregion 
    }
}

/*
public List<T> AllReachableFrom(T start)
{
    var visited = new HashSet<T> { start }; //var visited = new Dictionary<T,int>();visited[start] = 0;
    var toProcess = new Queue<T>();
    toProcess.Enqueue(start);
    while (toProcess.Count != 0)
    {
        var current = toProcess.Dequeue();
        foreach (var child in Children(current))
        {
            if (visited.Contains(child)) //if (visited.ContainsKey(child))
                continue;
            //visited[child] = visited[current] + 1;
            visited.Add(child);
            toProcess.Enqueue(child);
        }
    }
    return visited.ToList();
}
*/

