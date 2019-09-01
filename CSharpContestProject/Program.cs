using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Local


namespace CSharpContestProject
{
    static class Program
    {

        private static void Main(string[] args)
        {
            //int nbTests = ReadInt();
            int nbTests = 1;
            for (int i = 1; i <= nbTests; ++i)
            {
                Process();
            }
        }

        public static void Process()
        {
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
        private static Point ReadPoint() { var xy = ReadInts().ToArray(); return new Point(xy[0], xy[1]); }
        private static Point[] ReadPoints()
        {
            var ints = ReadInts().ToArray();
            var result = new Point[ints.Length / 2];
            for (var i = 0; i < result.Length; ++i)
            {
                result[i] = new Point(ints[2 * i], ints[2 * i + 1]);
            }
            return result;
        }
        private static Point[] ReadPointsColumn(int nbRows) { return ReadTColumn(nbRows, ReadPoint); }
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
                var line = Console.ReadLine()??"";
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
