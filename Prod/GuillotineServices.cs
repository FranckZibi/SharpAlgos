using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace SharpAlgos
{
	public static class GuillotineServices
    {
        public static List<Rectangle> ExtractSlicesToMaximizeScore(int pizzaWidth, int pizzaHeight, Func<int, int, int, int, int> scoreFunction)
        {
            int pizzaSize = pizzaWidth * pizzaHeight;
            int cacheLength = pizzaSize * pizzaSize;
            var cache = new int[cacheLength];
            for (int sliceSize = 1; sliceSize <= pizzaSize; ++sliceSize)
            {
                ComputeScoreForAllSlicesOfSliceSize(new[] { 0, pizzaWidth - 1 }, pizzaWidth, pizzaHeight, scoreFunction, sliceSize, cache);
                if ((sliceSize%1000) == 1)
                {
                    Console.WriteLine("Done:"+ (100*sliceSize/pizzaSize) + "%");
                }
            }
            var slices = new List<Rectangle>();
            RetrieveAllSlicesForBestScore(ToSliceId(0, 0, pizzaWidth, pizzaHeight, pizzaWidth, pizzaHeight), pizzaWidth, pizzaHeight, scoreFunction, cache, slices);
            return slices;
        }
        private static int ToSliceId(int x, int y, int width, int height, int pizzaWidth, int pizzaHeight)
        {
            int id1 = x * pizzaHeight +y;
            int id2 = (x+width-1) * pizzaHeight + (y+height-1);
            return id2 * pizzaHeight * pizzaWidth + id1;
        }
        private static Rectangle SliceIdToRectangle(int sliceId, int pizzaWidth, int pizzaHeight)
        {
            int id1 = sliceId % (pizzaHeight * pizzaWidth);
            int x = id1 / pizzaHeight;
            int y = id1 % pizzaHeight;
            int id2 = sliceId / (pizzaHeight * pizzaWidth);
            int xEnd = id2 / pizzaHeight;
            int yEnd = id2 % pizzaHeight;
            return new Rectangle(x, y, xEnd - x + 1, yEnd - y + 1);
        }

        //we have already computed score for all slices of size < sliceSize
        //We will compute score for all slices of size exactly = 'sliceSize'
	    // ReSharper disable once UnusedMethodReturnValue.Local
        private static int ComputeScoreForAllSlicesOfSliceSize(int[] splittedX, int pizzaWidth, int pizzaHeight, Func<int,int,int,int, int> scoreFunction, int sliceSize, int[] cache)
        {
            var validWidths = new List<int>();
            for (int w = 1; w <= sliceSize; ++w)
            {
                if (sliceSize % w == 0)
                {
                    validWidths.Add(w);
                }
            }

            int bestScore = 0;
            for (int x = splittedX[0]; x <= splittedX[1]; ++x)
            {
                int maxWidth = Math.Min(pizzaWidth - x, sliceSize);
                foreach (var width in validWidths)
                {
                    if (width > maxWidth)
                    {
                        break;
                    }
                    int height = sliceSize / width;
                    for (int y = 0; y <= pizzaHeight- height; ++y)
                    {
                        Debug.Assert(width*height == sliceSize);
                        int scoreIfNoSubSplit = scoreFunction(x, y, width, height);
                        int scoreWithSubSplit = BestSubSplitScore(x, y, width, height, pizzaWidth, pizzaHeight, cache, out _, out _);
                        var score = Math.Max(scoreIfNoSubSplit, scoreWithSubSplit);
                        cache[ToSliceId(x, y, width, height, pizzaWidth, pizzaHeight)] = score;
                        bestScore = Math.Max(bestScore, score);
                    }
                }
            }
            return bestScore;
        }

        private static int BestSubSplitScore(int x, int y, int width, int height, int pizzaWidth, int pizzaHeight, int[] cache, out int bestLeftIndex, out int bestRightIndex)
        {
            int currentBestScore = 0;
            bestLeftIndex = bestRightIndex = -1;
            for (int leftWidth = 1; leftWidth < width; ++leftWidth)
            {
                int leftIndex = ToSliceId(x, y, leftWidth, height, pizzaWidth, pizzaHeight);
                int rightIndex = ToSliceId(x + leftWidth, y, width - leftWidth, height, pizzaWidth, pizzaHeight);
                if (cache[leftIndex] + cache[rightIndex] > currentBestScore)
                {
                    currentBestScore = cache[leftIndex] + cache[rightIndex];
                    bestLeftIndex = leftIndex;
                    bestRightIndex = rightIndex;
                }
            }
            for (int topHeight = 1; topHeight < height; ++topHeight)
            {
                int leftIndex = ToSliceId(x, y, width, topHeight, pizzaWidth, pizzaHeight);
                int rightIndex = ToSliceId(x, y + topHeight, width, height - topHeight, pizzaWidth, pizzaHeight);
                if (cache[leftIndex] + cache[rightIndex] > currentBestScore)
                {
                    currentBestScore = cache[leftIndex] + cache[rightIndex];
                    bestLeftIndex = leftIndex;
                    bestRightIndex = rightIndex;
                }
            }
            return currentBestScore;
        }

        //This method will be called after having computed scores for all possibles slices, 
        //It will store in 'splits' the best split for slice 'sliceId'
        private static void RetrieveAllSlicesForBestScore(int sliceId, int pizzaWidth, int pizzaHeight, Func<int, int, int, int, int> scoreFunction, int[] cache, List<Rectangle> splits)
        {
            int sliceIdScore = cache[sliceId];
            if (sliceIdScore == 0)
            {
                return;
            }
            Debug.Assert(sliceIdScore > 0);
            var slice = SliceIdToRectangle(sliceId, pizzaWidth, pizzaHeight);
            if (scoreFunction(slice.X, slice.Y, slice.Width, slice.Height) == sliceIdScore)
            {
                //no need to split 'sliceId' more : it is already the optimal split
                splits.Add(slice);
                return;
            }
            //We need to split the slice 'sliceId' more.
            //We retrieve the optimal way to split 'sliceId' into 2 sub splits : 'leftSliceId' and 'rightSliceId'
            int leftSliceId;
            int rightSliceId;
            BestSubSplitScore(slice.X, slice.Y, slice.Width, slice.Height, pizzaWidth, pizzaHeight, cache, out leftSliceId, out rightSliceId);
            //We retrieve the best way to sub split the 'leftSliceId' part of the original 'sliceId'
            RetrieveAllSlicesForBestScore(leftSliceId, pizzaWidth, pizzaHeight, scoreFunction, cache, splits);
            //We retrieve the best way to sub split the 'right' part of the original 'sliceId'
            RetrieveAllSlicesForBestScore(rightSliceId, pizzaWidth, pizzaHeight, scoreFunction, cache, splits);
        }
    }
}
