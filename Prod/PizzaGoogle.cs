using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace SharpAlgos
{

    //public class Slice
    //{
    //    #region fields
    //    public readonly int X;
    //    public readonly int Y;
    //    public readonly int Width;
    //    public readonly int Height;
    //    #endregion

    //    private bool Equals(Slice other)
    //    {
    //        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj)) return false;
    //        if (ReferenceEquals(this, obj)) return true;
    //        if (obj.GetType() != this.GetType()) return false;
    //        return Equals((Slice) obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        unchecked
    //        {
    //            var hashCode = X;
    //            hashCode = (hashCode*397) ^ Y;
    //            hashCode = (hashCode*397) ^ Width;
    //            hashCode = (hashCode*397) ^ Height;
    //            return hashCode;
    //        }
    //    }


      


    //    private int X_END => X + Width - 1;
    //    private int Y_END => Y + Height - 1;
    //    public int CountH => PizzaGoogle.CountH(X, Y, Width, Height);
    //    public int Count => Width*Height;

    //    public int ScoreForSorting
    //    {
    //        get
    //        {
    //            //!D return Count;
    //            return Count - CountH;
    //            /*
    //            double multCoeff = 1.0;
    //            if (!PizzaGoogle.usedPizza.ContentWithDefaultValue(X - 1, Y, false))
    //                multCoeff *= 0.1;
    //            if (!PizzaGoogle.usedPizza.ContentWithDefaultValue(X, Y - 1, false))
    //                multCoeff *= 0.1;
    //            return multCoeff * Count; */
    //        }
    //    }


    //    public static readonly Slice EMPTY = new Slice(0,0,0,0);

    //    public Slice(string data) : this(data.Split(',').Select(int.Parse).ToArray())
    //    {
    //    }

    //    private Slice(int[] data) : this(data[0], data[1], data[2], data[3])
    //    {
    //    }

    //    public Slice(int X, int Y, int Width, int Height)
    //    {
    //        this.X = X;
    //        this.Y = Y;
    //        this.Width = Width;
    //        this.Height = Height;
    //        //Debug.Assert(CountH >= PizzaGoogle.minH);
    //        //Debug.Assert(Count <= PizzaGoogle.maxSize);
    //        //Debug.Assert(CountInUsedPizza() == 0);
    //    }

    //    public void UpdateUsedPizza(ActualMatrix<bool> usedPizza)
    //    {
    //        for (int x = X; x < X + Width; ++x)
    //        {
    //            for (int y = Y; y < Y + Height; ++y)
    //            {
    //                Debug.Assert(usedPizza[x][y] == false);
    //                usedPizza[x][y] = true;
    //            }
    //        }
    //        Debug.Assert(CountInUsedPizza(usedPizza) == Count);
    //    }

    //    public void RevertUsedPizza(ActualMatrix<bool> usedPizza)
    //    {
    //        for (int x = X; x < X + Width; ++x)
    //        {
    //            for (int y = Y; y < Y + Height; ++y)
    //            {
    //                Debug.Assert(usedPizza[x][y] == true);
    //                usedPizza[x][y] = false;
    //            }
    //        }
    //        Debug.Assert(CountInUsedPizza(usedPizza) == 0);

    //    }

    //    private int CountInUsedPizza(AbstractConstMatrix<bool> usedPizza)
    //    {
    //        int result = 0;
    //        for (int x = X; x < X + Width; ++x)
    //        {
    //            for (int y = Y; y < Y + Height; ++y)
    //            {
    //                if (usedPizza[x][y])
    //                    ++result;
    //            }
    //        }
    //        return result;
    //    }

    //    public override string ToString()
    //    {
    //        return X + "," + Y + "," + Width + "," + Height;
    //    }



    //    public int CompareTo(Slice slice)
    //    {
    //        return ScoreForSorting == slice.ScoreForSorting
    //            ? slice.Count.CompareTo(Count)
    //            : slice.ScoreForSorting.CompareTo(ScoreForSorting);
    //    }

    //    private bool Intersect(Slice s)
    //    {
    //        return (X <= s.X_END) && (X_END >= s.X) && (Y <= s.Y_END) && (Y_END >= s.Y);
    //    }

    //    public bool IntersectAny(IEnumerable<Slice> legalSlices)
    //    {
    //        return legalSlices.Any(x => x.Intersect(this));
    //    }
    //}


    

    //public static class PizzaGoogle
    //{
    //    #region fields
    //    public static ActualMatrix<bool> pizza;
    //    private static ActualMatrix<int> countHMatrix;
    //    //public static ActualMatrix<bool> usedPizza;
    //    public static int minH;
    //    public static int maxSize;
    //    #endregion



    //    private static int ScoreFunction(int x, int y, int width, int height)
    //    {
    //        int size = width*height;
    //        if (size > maxSize) return 0;
    //        if (CountH(x, y, width, height) < minH) return 0;
    //        return size;
    //    }

    //    public static void Process(string filePath, string forcedFirstSolutionFilePathIfAny)
    //    {
    //        pizza = LoadPizza(filePath);
    //        countHMatrix = pizza.CreateCountMatrix(x=>x);
    //        var rand = new Random(0);
    //        var usedPizza = new ActualMatrix<bool>(pizza.Width, pizza.Height);

    //        List<Slice> slices;
    //        if (!string.IsNullOrEmpty(forcedFirstSolutionFilePathIfAny))
    //            //we load a file with some existng slices
    //            slices = System.IO.File.ReadAllLines(forcedFirstSolutionFilePathIfAny).Select(x => new Slice(x)).ToList();
    //        else //we compute the initial slice using guillotine algo
    //        { 
    //            var s = new Stopwatch(); s.Start();

    //            slices = GuillotineServices.ExtractSlicesToMaximizeScore(pizza.Width, pizza.Height, ScoreFunction).Select(r=> new Slice(r.X, r.Y, r.Width, r.Height)).ToList();

    //            s.Stop(); Console.WriteLine("Guillotine took "+s.Elapsed.TotalSeconds+"s");
    //        }

    //        slices.ForEach(x => x.UpdateUsedPizza(usedPizza));
    //        Console.WriteLine("score#0=" + Score(slices) + " (" + slices.Count + " slices)");
    //        var lastDisplayTime = DateTime.Now;
    //        int previousScore = Score(slices);
    //        const int nbTrials = 1000;
    //        for (int nbEpoch = 1;; ++nbEpoch)
    //        {
    //            MathServices.Shuffle(slices, nbEpoch);
    //            for (int i = 0; i < slices.Count; ++i)
    //                LocalOptimize(usedPizza, slices, i, nbTrials, rand);

    //            var currentScore = Score(slices);
    //            if ((nbEpoch == 1) || ((DateTime.Now - lastDisplayTime).TotalSeconds > 600) || (currentScore>previousScore))
    //            {
    //                lastDisplayTime = DateTime.Now;
    //                //Console.WriteLine("slices count after epoch#" + nbEpoch + " = " + slices.Count);
    //                var msg = "score#" + nbEpoch + "=" + currentScore+" ("+ slices.Count+ " slices)" ;
    //                Console.WriteLine(msg);
    //                System.IO.File.WriteAllText(filePath + "." + currentScore, string.Join(Environment.NewLine, slices.Select(x => x.ToString()).ToArray()));
    //                System.IO.File.AppendAllText(filePath + ".txt", msg + Environment.NewLine);
    //            }
    //            previousScore = currentScore;
    //        }
    //    }


    //    public static int CountH(int x, int y, int width, int height)
    //    {
    //        return AbstractConstMatrix<int>.CountInSubMatrix(countHMatrix, x, y, width, height);
    //    }
    //    private static int Score(IEnumerable<Slice> slices)
    //    {
    //        return slices.Sum(x => x.Count);
    //    }


    //    private static void LocalOptimize(ActualMatrix<bool> usedPizza, List<Slice> slices, int indexToDiscard, int nbTrials, Random rand)
    //    {
    //        var s = slices[indexToDiscard];
    //        int delta = 3*maxSize;
    //        int minX = Math.Max(0, s.X - delta);
    //        int maxX = Math.Min(pizza.Width - 1, s.X + delta);
    //        int minY = Math.Max(0, s.Y - delta);
    //        int maxY = Math.Min(pizza.Height - 1, s.Y + delta);
    //        var neighborIndexes = new List<int>();
    //        for (int i = 0; i < slices.Count; ++i)
    //        {
    //            if (i == indexToDiscard)
    //                continue;
    //            if (slices[i].X >= minX && slices[i].X <= maxX && slices[i].Y >= minY && slices[i].Y <= maxY)
    //                neighborIndexes.Add(i);
    //        }
    //        if (neighborIndexes.Count == 0)
    //            return;
    //        for (int nbPoints = 1 + Math.Min(10, neighborIndexes.Count); nbPoints >= 1; --nbPoints)
    //        {
    //            var toDiscard = new HashSet<int> {indexToDiscard};
    //            for (int i = 1; i < nbPoints; ++i)
    //                toDiscard.Add(neighborIndexes[rand.Next(neighborIndexes.Count)]);
    //            if (LocalOptimize(usedPizza, minX, maxX, minY, maxY, slices, indexToDiscard, toDiscard.ToList(), nbTrials, rand))
    //                return;
    //        }
    //    }

    //    private static bool LocalOptimize(ActualMatrix<bool> usedPizza, int minX, int maxX, int minY, int maxY, List<Slice> slices, int indexLocalOptimized, List<int> indexToDiscard, int nbTrials, Random rand)
    //    {
    //        var oldSlices = new List<Slice>();
    //        indexToDiscard.RemoveAll(x => x >= slices.Count);
    //        if (indexToDiscard.Count == 0)
    //            return false;

    //        foreach (var i in indexToDiscard)
    //        {
    //            oldSlices.Add(slices[i]);
    //            slices[i].RevertUsedPizza(usedPizza);
    //            slices[i] = Slice.EMPTY;
    //        }
    //        //var forbiddenSlice = new HashSet<Slice> (oldSlices);
    //        var forbiddenSlice = new HashSet<Slice> {slices[indexLocalOptimized]};


    //        Debug.Assert(maxX > minX);
    //        Debug.Assert(maxY > minY);
    //        var legalSlices = RandomSlicesFrom(usedPizza, minX, maxX, minY, maxY, nbTrials, rand);
    //        legalSlices.RemoveAll(forbiddenSlice.Contains);
    //        legalSlices.Sort((x, y) => x.CompareTo(y));
    //        var newSlices = new List<Slice>();
    //        DFS(legalSlices, 0, new List<Slice>(), 0, newSlices, 0, 2048);

    //        if (   (Score(newSlices) < Score(oldSlices))
    //            ||((Score(newSlices) == Score(oldSlices))&& (newSlices.Count< oldSlices.Count) )
    //            )
    //        {
    //            oldSlices.ForEach(x => x.UpdateUsedPizza(usedPizza));
    //            for (int i = 0; i < indexToDiscard.Count; ++i)
    //                slices[indexToDiscard[i]] = oldSlices[i];
    //            return false;
    //        }

    //        //new best score
    //        indexToDiscard.Sort();
    //        indexToDiscard.Reverse();

    //        foreach (var i in indexToDiscard)
    //            slices.RemoveAt(i);
    //        slices.AddRange(newSlices);
    //        newSlices.ForEach(x => x.UpdateUsedPizza(usedPizza));
    //        return true;
    //    }

    //    private static void DFS(List<Slice> legalSlices, int currentIndex, List<Slice> path, double pathScore, List<Slice> bestPath, double bestPathScore, int nbRemainingMakeMoves)
    //    {
    //        if (pathScore > bestPathScore)
    //        {
    //            bestPath.Clear();
    //            bestPath.AddRange(path);
    //            bestPathScore = pathScore;
    //        }

    //        if ((currentIndex >= legalSlices.Count) || (nbRemainingMakeMoves <= 0))
    //            return;
    //        var move = legalSlices[currentIndex];
    //        if (move.IntersectAny(path)) //illegal move for current path
    //        {
    //            DFS(legalSlices, currentIndex + 1, path, pathScore, bestPath, bestPathScore, nbRemainingMakeMoves);
    //        }
    //        else
    //        {
    //            DFS(legalSlices, currentIndex + 1, path, pathScore, bestPath, bestPathScore, nbRemainingMakeMoves/2);
    //            path.Add(move);
    //            DFS(legalSlices, currentIndex + 1, path, pathScore + move.Count, bestPath, bestPathScore,
    //                nbRemainingMakeMoves/2);
    //            path.RemoveAt(path.Count - 1);
    //        }
    //    }


    //    private static List<Slice> RandomSlicesFrom(AbstractConstMatrix<bool> usedPizza, int minX, int maxX, int minY, int maxY, int nbTrials, Random rand)
    //    {
    //        Debug.Assert(maxX > minX);
    //        Debug.Assert(maxY > minY);
    //        var results = new HashSet<Slice>();
    //        var usedPoints = new HashSet<Point>();
    //        for (int i = 0; i < nbTrials; ++i)
    //        {
    //            int x = minX + rand.Next(maxX - minX);
    //            int y = minY + rand.Next(maxY - minY);
    //            if (!usedPoints.Add(new Point(x, y)))
    //                continue; //already added
    //            int nextX;
    //            int nextY;
    //            if (NextUnusedPointFrom(usedPizza, x, y, out nextX, out nextY))
    //                results.UnionWith(AllLegalSlicesWith_XY_TopLeft(usedPizza, nextX, nextY));
    //        }
    //        return results.ToList();
    //    }

    //    private static List<Slice> AllLegalSlicesWith_XY_TopLeft(AbstractConstMatrix<bool> usedPizza, int X, int Y)
    //    {
    //        var slices = new List<Slice>();
    //        int maxWidth = Math.Min(pizza.Width - X, maxSize);
    //        int maxHeight = Math.Min(pizza.Height - Y, maxSize);
    //        for (int width = 1; width <= maxWidth; ++width)
    //        {
    //            int X_END = X + width - 1;
    //            if (usedPizza[X_END][Y])
    //            {
    //                break;
    //            }

    //            for (int height = 1; height <= maxHeight; ++height)
    //            {
    //                int count = width*height;
    //                if (count > maxSize)
    //                    break;
    //                int Y_END = Y + height - 1;
    //                if (usedPizza[X_END][Y_END])
    //                {
    //                    maxHeight = Math.Min(height - 1, maxHeight);
    //                    break;
    //                }
    //                if (CountH(X, Y, width, height) >= minH)
    //                    slices.Add(new Slice(X, Y, width, height));
    //            }
    //        }

    //        return slices;
    //    }

    //    private static bool NextUnusedPointFrom(AbstractConstMatrix<bool> usedPizza, int xStart, int yStart, out int nextX, out int nextY)
    //    {
    //        nextX = nextY = 0;
    //        for (int x = xStart; x < usedPizza.Width; ++x)
    //            for (int y = yStart; y < usedPizza.Height; ++y)
    //                if (!usedPizza[x][y])
    //                {
    //                    nextX = x;
    //                    nextY = y;
    //                    return true;
    //                }
    //        return false;
    //    }

    //    /*
    //public bool IsValid(int X, int Y, int Width, int Height)
    //{
    //    if (CountH(X, Y, Width, Height) < minH)
    //        return false;

    //    ;) >= PizzaGoogle.minH);
    //    Debug.Assert(Count <= PizzaGoogle.maxSize);
    //    */

  

    //    private static ActualMatrix<bool> LoadPizza(string filePath)
    //    {
    //        var lines = System.IO.File.ReadAllLines(filePath);
    //        var header = lines[0].Split(',').Select(int.Parse).ToList();

    //        minH = header[2];
    //        maxSize = header[3];

    //        var result = new ActualMatrix<bool>(header[0], header[1]);
    //        for (int rowNum = 0; rowNum < header[1]; ++rowNum)
    //        {
    //            var line = lines[1 + rowNum];
    //            for (int colNum = 0; colNum < line.Length; ++colNum)
    //                result.SetContent(colNum, rowNum, line[colNum] == 'H');
    //        }
    //        return result;
    //    }
    //}
}