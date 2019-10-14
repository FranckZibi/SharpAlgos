using System;
using System.Collections.Generic;
using System.Drawing;

namespace SharpAlgos
{
    public static partial class Utils
    {
        /// <summary>
        /// return a matrix where matrix[row,col] = sum of all elements in m from top left to (row,col)
        /// in o(N*M) time (& memory)
        /// </summary>
        /// <param name="m"></param>
        /// <returns>the count matrix</returns>
        public static int[,] CreateCountMatrix(int[,] m)
        {
            var countMatrix = (int[,]) m.Clone();
            for (int row = 0; row < m.GetLength(0); ++row)
            {
                for (int col = 0; col < m.GetLength(1); ++col)
                {
                    countMatrix[row, col] += Default(countMatrix, row, col - 1, 0) +
                                             Default(countMatrix, row - 1, col, 0) -
                                             Default(countMatrix, row - 1, col - 1, 0);
                }
            }
            return countMatrix;
        }

        //create sum of element in a sub matrix in o(1) time (after pre processing that takes o(n*m) time)
        public static int SubMatrixSumInCountMatrix(int[,] countMatrix, int row0, int col0, int row1, int col1)
        {
            return countMatrix[row1, col1] - Default(countMatrix, row0 - 1, col1, 0) -
                   Default(countMatrix, row1, col0 - 1, 0) + Default(countMatrix, row0 - 1, col0 - 1, 0);
        }

        /// <summary>
        /// find the minimum cost to go from top left to bottom right on matrix in o(N*M) time (& memory)
        /// </summary>
        /// <param name="m">the matrix with the cost in each cell</param>
        /// <returns>the minimum cost</returns>
        public static int MinimumCostToGoFromTopLeftToBottomRightOfMatrix(int[,] m)
        {
            var cost = (int[,]) m.Clone();
            var nbRows = m.GetLength(0);
            var nbCols = m.GetLength(1);
            for (int row = 0; row < nbRows; ++row)
            {
                for (int col = 0; col < nbCols; ++col)
                {
                    if (row == 0 && col == 0)
                    {
                        continue;
                    }
                    var costLeft = row == 0 ? int.MaxValue : cost[row - 1, col];
                    var costTop = col == 0 ? int.MaxValue : cost[row, col - 1];
                    cost[row, col] += Math.Min(costLeft, costTop);
                }
            }
            return cost[nbRows - 1, nbCols - 1];
        }

        //Find the number of path (from top left to bottom right) with total cost equal 'givenCost' in o (m.Height * m.Width) time
        public static int NumberOfPathsToGoFromTopLeftToBottomRightOfMatrixWithGivenCost(int[,] m, int givenCost)
        {
            return NumberOfPathsToGoFromTopLeftToBottomRightOfMatrixWithGivenCost(m, new Dictionary<string, int>(), m.GetLength(0) - 1, m.GetLength(1) - 1, givenCost);
        }

        private static int NumberOfPathsToGoFromTopLeftToBottomRightOfMatrixWithGivenCost(int[,] m, IDictionary<string, int> cache, int row, int col, int remainingCost)
        {
            if (Math.Min(row, col) < 0)
            {
                return 0;
            }
            remainingCost -= m[row, col];
            if (row == 0 && col == 0)
            {
                return remainingCost == 0 ? 1 : 0;
            }
            string key = row + "|" + col + "|" + remainingCost;
            if (!cache.ContainsKey(key))
            { 
                cache[key] =
                     NumberOfPathsToGoFromTopLeftToBottomRightOfMatrixWithGivenCost(m, cache, row, col - 1,remainingCost) 
                    +NumberOfPathsToGoFromTopLeftToBottomRightOfMatrixWithGivenCost(m, cache, row - 1, col,remainingCost);
            }
            return cache[key];
        }

        //Find the longest sequence satisfying a given constraint in o(M*N) time (& memory)
        public static List<int> LongestSequenceSatisfyingConstraints(int[,] m, Func<int, int, bool> isOkToGoFromSourceValueToTargetValue)
        {
            var score = new int[m.GetLength(0), m.GetLength(1)];
            //uncomment line if the goal is only to compute length of longest path
            //Point[,] prevPoint= null; 
            var prevPoint = new Point[m.GetLength(0), m.GetLength(1)];
            int rowMax = 0;
            int colMax = 0;
            for (int row = 0; row < m.GetLength(0); ++row)
            {
                for (int col = 0; col < m.GetLength(1); ++col)
                {
                    LongestSequenceSatisfyingConstraints_Helper(m, score, prevPoint, isOkToGoFromSourceValueToTargetValue, row, col);
                    if (score[row, col] > score[rowMax, colMax])
                    {
                        rowMax = row;
                        colMax = col;
                    }
                }
            }

            //uncomment line to return length of longest path
            //return score[rowMax, colMax]; 
            var bestPath = new List<int>();
            var prev = new Point(rowMax, colMax);
            while (bestPath.Count < score[rowMax, colMax])
            {
                bestPath.Add(m[prev.X, prev.Y]);
                prev = prevPoint[prev.X, prev.Y];
            }
            bestPath.Reverse();
            return bestPath;
        }

        private static int LongestSequenceSatisfyingConstraints_Helper(int[,] m, int[,] score, Point[,] prevPoint, Func<int, int, bool> isOkToGoFromSourceValueToTargetValue, int row, int col)
        {
            if (score[row, col] != 0)
            {
                return score[row, col];
            }
            score[row, col] = 1;
            foreach (var p in AllPointsHorizontalVertical(m, row, col))
            {
                if (isOkToGoFromSourceValueToTargetValue(m[p.X, p.Y], m[row, col]))
                {
                    var currentResult = 1 + LongestSequenceSatisfyingConstraints_Helper(m, score, prevPoint, isOkToGoFromSourceValueToTargetValue, p.X, p.Y);
                    if (currentResult > score[row, col])
                    {
                        score[row, col] = currentResult;
                        if (prevPoint != null)
                        {
                            prevPoint[row, col] = p;
                        }
                    }
                }
            }
            return score[row, col];
        }

        public static int MaximumPointsIn01MatrixSatisfyingGivenConstraints(int[,] m)
        {
            var score = (int[,]) m.Clone();
            int colStart = 0;
            int delta = 1;
            for (int row = 0; row < m.GetLength(0); ++row)
            {
                for (int col = colStart; col >= 0 && col < m.GetLength(1); col += delta)
                {
                    if ((row == 0 && col == 0) || (m[row, col] == -1))
                    {
                        continue;
                    }
                    var newCol = col - delta;
                    var colScore = (newCol >= 0 && newCol < m.GetLength(1)) ? score[row, newCol] : -1;
                    var topScore = (row == 0) ? -1 : score[row - 1, col];
                    var neighbourScore = Math.Max(colScore, topScore);
                    if (neighbourScore == -1)
                    {
                        score[row, col] = -1;
                        continue;
                    }
                    score[row, col] += neighbourScore;
                }
                colStart = m.GetLength(1) - 1 - colStart;
                delta *= -1;
            }
            int res = 0;
            foreach (int e in score)
            {
                res = Math.Max(e, res);
            }
            return res;
        }


        public static long TrapRainWater3D(int[,] heightMap)
        {
            if (heightMap.Length == 0)
            {
                return 0;
            }
            int w = heightMap.GetLength(0);
            int h = heightMap.GetLength(1);
            var visited = new bool[w, h];
            var priority = new PriorityQueue<Point>(true);

            //we are building the wall around the waterpool
            foreach (var p in AllPoints(heightMap))
            {
                if ((p.X == 0) || (p.Y == 0) || (p.X == w - 1) || (p.Y == h - 1))
                {
                    priority.Enqueue(p, heightMap[p.X,p.Y]);
                    visited[p.X,p.Y] = true;
                }
            }

            int result = 0;
            while (priority.Count != 0)
            {
                var lowestInWall = priority.Dequeue();
                foreach (var neighbour in AllPointsHorizontalVertical(heightMap, lowestInWall.X, lowestInWall.Y))
                {
                    if (visited[neighbour.X,neighbour.Y])
                    {
                        continue;
                    }
                    visited[neighbour.X,neighbour.Y] = true;
                    if (heightMap[neighbour.X,neighbour.Y] < heightMap[lowestInWall.X,lowestInWall.Y])
                    {
                        //the neighbour will be filled with water, and concataned to the waterpool wall
                        result += heightMap[lowestInWall.X,lowestInWall.Y] - heightMap[neighbour.X,neighbour.Y];
                        heightMap[neighbour.X,neighbour.Y] = heightMap[lowestInWall.X,lowestInWall.Y];
                    }
                    priority.Enqueue(neighbour, heightMap[neighbour.X,neighbour.Y]);
                }
            }
            return result;
        }

        /// <summary>
        ///  compute the max sum from a sub matrix of a N*M 'matrix' in o(N^2*M) time
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="coordinates">4 integers coordinates where the max sum is located:  rowStart, colStart, rowEnd, colEnd </param>
        /// <returns>the max sum</returns>
        public static int MaximumSubmatrixSum(int[,] matrix, out int[] coordinates)
        {
            int nbRows = matrix.GetLength(0);
            int nbCols = matrix.GetLength(1);

            coordinates = new int[4];
            var countMatrix = CreateCountMatrix(matrix);
            int maxSum = int.MinValue;
            var subSum = new int[nbCols];
            for (int rowStart = 0; rowStart < nbRows; ++rowStart)
            {
                for (int rowEnd = rowStart; rowEnd < nbRows; ++rowEnd)
                {
                    for (int col = 0; col < nbCols; ++col)
                    {
                        subSum[col] = SubMatrixSumInCountMatrix(countMatrix, rowStart, col, rowEnd, col);
                    }
                    int colStart;
                    int colEnd;
                    int curMaxSum = MaxSubSum(subSum, false, out colStart, out colEnd);
                    if (curMaxSum > maxSum)
                    {
                        maxSum = curMaxSum;
                        coordinates[0] = rowStart;
                        coordinates[1] = colStart;
                        coordinates[2] = rowEnd;
                        coordinates[3] = colEnd;
                    }
                }
            }
            return maxSum;
        }

        /// <summary>
        /// compute the product of 2 matrices 'a' (N,M) and 'b' (M,K) in o(N*M*K) time
        /// each cell of the resulting matrix will be computed '% modulo'
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public static long[,] ProductModulo(long[,] a, long[,] b, long modulo)
        {
            var result = new long[a.GetLength(0), b.GetLength(1)];
            for (int row = 0; row < result.GetLength(0); row++)
            {
                for (int col = 0; col < result.GetLength(1); col++)
                {
                    long sum = 0;
                    for (int i = 0; i < a.GetLength(1); i++)
                    {
                        sum = (sum + a[row, i] * b[i, col]) % modulo;
                    }
                    result[row, col] = (sum + modulo) % modulo;
                }
            }
            return result;
        }

        /// <summary>
        /// Compute a square matrix of size (N*N) to power 'exp' in O (N^3 * log(exp) ) time
        /// each cell of the matrix will be computed '% modulo'
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="exp"></param>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public static long[,] PowerModulo(long[,] mat, int exp, long modulo)
        {
            if (exp == 1)
            {
                return mat;
            }
            var sq = ProductModulo(mat, mat, modulo);
            if (exp % 2 == 0)
            {
                return PowerModulo(sq, exp / 2, modulo);
            }
            else // >= 3 and is odd
            {
                return ProductModulo(mat, PowerModulo(sq, exp / 2, modulo), modulo);
            }
        }

    }
}