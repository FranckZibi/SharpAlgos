using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        //number of ways to place K Bishops on a NxN board
        // https://cp-algorithms.com/combinatorics/bishops-on-chessboard.html

        public static IList<IList<string>> SolveNQueens(int n)
        {
            var result = new List<IList<string>>();
            var visited = new bool[n];
            SolveNQueens_Helper(n, visited, new List<int>(), result);
            return result;
        }

        private static void SolveNQueens_Helper(int n, bool[] visited, List<int> path, List<IList<string>> result)
        {
            if (path.Count >= n)
            {
                var solution = new List<string>();
                foreach (var p in path)
                {
                    var line = Enumerable.Repeat('.', n).ToArray();
                    line[p] = 'Q';
                    solution.Add(new string(line));
                }
                result.Add(solution);
            }

            for (int i = 0; i < n; ++i)
            {
                if (visited[i])
                    continue;
                if (!SolveNQueens_Helper_IsValid(path, i))
                    continue;
                visited[i] = true;
                path.Add(i);
                SolveNQueens_Helper(n, visited, path, result);
                visited[i] = false;
                path.RemoveAt(path.Count - 1);
            }
        }

        private static bool SolveNQueens_Helper_IsValid(List<int> path, int newColumn)
        {
            for (int i = 0; i < path.Count; ++i)
                if (Math.Abs(path[i] - newColumn) == (path.Count - i))
                    return false;
            return true;
        }

        public static double KnightProbability(int n, int k, int r, int c)
        {
            var moves = new List<int[]>
            {
                new[] {1, 2},
                new[] {2, 1},
                new[] {1, -2},
                new[] {2, -1},
                new[] {-1, 2},
                new[] {-2, 1},
                new[] {-1, -2},
                new[] {-2, -1}
            };
            double totalOk = KnightProbabilityCount(n, k, r, c, moves, Enumerable.Repeat(-1.0, 1 + n * n * 100).ToArray());
            double total = Math.Pow(8, k);
            return totalOk / total;
        }

        private static double KnightProbabilityCount(int n, int k, int r, int c, List<int[]> moves, double[] cache)
        {
            if (Math.Min(r, c) < 0) return 0;
            if (Math.Max(r, c) >= n) return 0;
            if (k < 0) return 0;
            if (k == 0) return 1;
            var key = (n * r + c) * 100 + (k - 1);
            if (cache[key] >= 0)
                return cache[key];
            double result = 0;
            foreach (var m in moves)
                result += KnightProbabilityCount(n, k - 1, r + m[0], c + m[1], moves, cache);
            cache[key] = result;
            return result;
        }
    }
}
