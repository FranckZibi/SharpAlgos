using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {

        //Input:
        //  isAccessible 'bool matrix' of available squares
        //Output:
        //  segmentIdToCount:           segmentId => nb of available squares in the segment
        //  segmentIdToDimension:       segmentId => max dimension of the segment
        //  returned value:             for each square the segmentId associated with the square, or -1 if the square is not accessible
        public static int[,] IdentifyAllSegments(bool[,] isAccessible, out IDictionary<int, Rectangle> segmentIdToDimension, out IDictionary<int, int> segmentIdToCount)
        {
            segmentIdToDimension = new Dictionary<int, Rectangle>();
            segmentIdToCount = new Dictionary<int, int>();

            var result = new int[isAccessible.GetLength(0), isAccessible.GetLength(1)];
            result.SetAll(-1);

            var previousSegmentId = -1;
            foreach (var p in AllPoints(isAccessible))
            {
                if (!isAccessible[p.X, p.Y])
                {
                    continue;
                }
                if (result[p.X, p.Y] != -1)
                {
                    continue;
                }
                ++previousSegmentId;
                var currentSegmentId = previousSegmentId; //we have found a new segment, wich segmentId is 'currentSegmentId'
                var toProcess = new Queue<Point>();
                toProcess.Enqueue(p);
                result[p.X, p.Y] = currentSegmentId;
                segmentIdToCount[currentSegmentId] = 1;
                segmentIdToDimension[currentSegmentId] = new Rectangle(p.X, p.Y, 1, 1);
                while (toProcess.Count != 0)
                {
                    var current = toProcess.Dequeue();
                    foreach (var pAround in AllPointsHorizontalVertical(isAccessible, current.X, current.Y))
                    //foreach (var pAround in AllPointsAround(isAccessible, current.X, current.Y, 1))
                    {
                        if (!isAccessible[pAround.X, pAround.Y] || (result[pAround.X, pAround.Y] != -1))
                        {
                            continue;
                        }

                        result[pAround.X, pAround.Y] = currentSegmentId;
                        toProcess.Enqueue(pAround);

                        //we update 'segmentIdToCount'
                        ++segmentIdToCount[currentSegmentId];

                        //we update 'segmentIdToDimension'
                        var rect = segmentIdToDimension[currentSegmentId];
                        var minX = Math.Min(rect.X, pAround.X);
                        var maxX = Math.Max(rect.X + rect.Width - 1, pAround.X);
                        var minY = Math.Min(rect.Y, pAround.Y);
                        var maxY = Math.Max(rect.Y + rect.Height - 1, pAround.Y);
                        segmentIdToDimension[currentSegmentId] = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
                    }
                }
            }
            return result;
        }

        //returns the area (width x height) of the biggest rectangle in the matrix (with only true in it)
        public static int MaximalRectangleArea(bool[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);
            int[] heigts = new int[h];
            int maxArea = 0;
            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrix[x, y])
                    {
                        ++heigts[y];
                    }
                    else
                    {
                        heigts[y] = 0;
                    }
                }
                int startIndex;
                int endIndex;
                maxArea = Math.Max(maxArea, LargestRectangleArea(heigts, out startIndex, out endIndex));
            }
            return maxArea;
        }

        //returns the width of the side of the biggest square in the matrix (with only true in it)
        //so the area of the square is 'the return value' * 'the return value'
        public static int MaximalSquareWidth(bool[,] maze)
        {
            int maxixmalWidth = 0;
            var count = new int[maze.GetLength(0), maze.GetLength(1)]; //width of the biggest rectangle ending (bottom right) at (row,col)
            for (var row = 0; row < maze.GetLength(0); ++row)
                for (var col = 0; col < maze.GetLength(1); ++col)
                {
                    if (!maze[row, col])
                    {
                        continue;
                    }
                    count[row, col] = 1 + new[] { Default(count, row, col - 1, 0), Default(count, row - 1, col, 0), Default(count, row - 1, col - 1, 0) }.Min();
                    maxixmalWidth = Math.Max(maxixmalWidth, count[row, col]);
                }
            return maxixmalWidth;
        }


    }
}