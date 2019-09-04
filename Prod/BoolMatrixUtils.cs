using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        /// <summary>
        /// identify all connected components in a grid in o(N*M) time (& memory)
        /// </summary>
        /// <param name="isAccessible">'bool matrix' of available squares</param>
        /// <param name="segmentIdToDimension">segmentId => nb of available squares in the segment</param>
        /// <param name="segmentIdToCount">segmentId => nb of available squares in the segment</param>
        /// <returns>for each square the segmentId (>= 1) associated with the square,
        /// or 0 if the square is not accessible</returns>
        public static int[,] IdentifyAllSegments(bool[,] isAccessible, out IDictionary<int, Rectangle> segmentIdToDimension, out IDictionary<int, int> segmentIdToCount)
        {
            segmentIdToDimension = new Dictionary<int, Rectangle>();
            segmentIdToCount = new Dictionary<int, int>();

            var result = new int[isAccessible.GetLength(0), isAccessible.GetLength(1)];

            var previousSegmentId = 0;
            foreach (var p in AllPoints(isAccessible))
            {
                if (!isAccessible[p.X, p.Y])
                {
                    continue;
                }
                if (result[p.X, p.Y] != 0)
                {
                    continue;
                }
                ++previousSegmentId;
                var currentSegmentId = previousSegmentId; //we have found a new segment, which segmentId is 'currentSegmentId'
                var pointsWhereNeighborsNeedToBeProcessed = new Queue<Point>(); //we'll need to process the neighbors of the points in this queue
                pointsWhereNeighborsNeedToBeProcessed.Enqueue(p);
                result[p.X, p.Y] = currentSegmentId;
                segmentIdToCount[currentSegmentId] = 1;
                segmentIdToDimension[currentSegmentId] = new Rectangle(p.X, p.Y, 1, 1);
                while (pointsWhereNeighborsNeedToBeProcessed.Count != 0)
                {
                    var current = pointsWhereNeighborsNeedToBeProcessed.Dequeue();
                    foreach (var neighbor in AllPointsHorizontalVertical(isAccessible, current.X, current.Y))
                    //foreach (var neighbor in AllPointsAround(isAccessible, current.X, current.Y, 1))
                    {
                        if (!isAccessible[neighbor.X, neighbor.Y] || (result[neighbor.X, neighbor.Y] != 0))
                        {
                            continue;
                        }

                        result[neighbor.X, neighbor.Y] = currentSegmentId;
                        pointsWhereNeighborsNeedToBeProcessed.Enqueue(neighbor);

                        //we update 'segmentIdToCount'
                        ++segmentIdToCount[currentSegmentId];

                        //we update 'segmentIdToDimension'
                        var rect = segmentIdToDimension[currentSegmentId];
                        var minX = Math.Min(rect.X, neighbor.X);
                        var maxX = Math.Max(rect.X + rect.Width - 1, neighbor.X);
                        var minY = Math.Min(rect.Y, neighbor.Y);
                        var maxY = Math.Max(rect.Y + rect.Height - 1, neighbor.Y);
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
            int[] heights = new int[h];
            int maxArea = 0;
            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrix[x, y])
                    {
                        ++heights[y];
                    }
                    else
                    {
                        heights[y] = 0;
                    }
                }
                int startIndex;
                int endIndex;
                maxArea = Math.Max(maxArea, LargestRectangleArea(heights, out startIndex, out endIndex));
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