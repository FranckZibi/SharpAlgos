using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace SharpAlgos
{
    public interface IAction
    {
        void Apply(int[,] result);
        bool IsIncludedIn(IAction mainAction);

    }

    public class Paint : IAction
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Size { get; set; }

        public int X_Last => Row + Size - 1;
        public int Y_Last => Col + Size - 1;

        public void Apply(int[,] target)
        {
            for (int rowNum = Row; rowNum < Row + Size; ++rowNum)
                for (int colNum = Col; colNum < Col + Size; ++colNum)
                {
                    Debug.Assert(Utils.IsValidCoordinate(target,rowNum, colNum));
                    ++target[rowNum,colNum];
                }
        }


        public bool IsIncludedIn(IAction mainAction)
        {
            var o = mainAction as Paint;
            if (o == null)
            {
                return false;
            }
            return Row >= o.Row && Col >= o.Col && X_Last <= o.X_Last && Y_Last <= o.Y_Last;
        }


        public double ComputeScoreIfApply(bool[,] target, int[,] current)
        {
            var N = 0.0;
            var B = 0.0;
            for (int rowNum = Row; rowNum < Row + Size; ++rowNum)
                for (int colNum = Col; colNum < Col + Size; ++colNum)
                {
                    Debug.Assert(Utils.IsValidCoordinate(target,rowNum, colNum));
                    int currentValue = current[rowNum,colNum];
                    Debug.Assert(currentValue >= 0);

                    /*if (currentValue != 0) continue; //gris
                    if (target[rowNum,colNum]) //target = paint
                        ++N;
                    else
                        ++B;
                    continue;*/




                    if (target[rowNum,colNum]) //target = paint
                    {
                        if (currentValue == 0)
                        {
                            ++N; //we rightly paint a square to paint
                        }
                    }
                    else // target = empty
                    {
                        if (currentValue == 0)
                        {
                            ++B; //we wrongly paint an empty square
                        }
                    }
                }
            return N / (1 + B);
        }
        public void Unapply(int[,] current)
        {
            for (int rowNum = Row; rowNum < Row + Size; ++rowNum)
                for (int colNum = Col; colNum < Col + Size; ++colNum)
                {
                    Debug.Assert(Utils.IsValidCoordinate(current,rowNum, colNum));
                    Debug.Assert(current[rowNum,colNum] >= 1);
                    --current[rowNum,colNum];
                }
        }

        public Paint(int row, int col, int size)
        {
            Row = row;
            Col = col;
            Size = size;
        }
        public override string ToString()
        {
            return "FILL," + Row + "," + Col + "," + Size;
        }

    }

    public class Erase : IAction
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Erase(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public override string ToString()
        {
            return "ERASE," + Row + "," + Col;
        }

        public void Apply(int[,] result)
        {
            if (Utils.IsValidCoordinate(result,Row, Col))
            {
                result[Row,Col] = 0;
            }
        }

        public bool IsIncludedIn(IAction mainAction)
        {
            return false;
        }

    }




    public static class ArtOptimal
    {
        public static KeyValuePair<double, Paint> ComputeBestPossibleScore(int row, int y, bool[,] target, int[,] current)
        {
            Paint paint = null;
            int maxSize = Math.Min(target.GetLength(0) - row, target.GetLength(1) - y);

            //var scores = new List<double>();
            double currentMaxScore = 0;
            for (int currentSize = 1; currentSize <= maxSize; ++currentSize)
            {
                var tmpPaint = new Paint(row, y, currentSize);
                var currentScore = tmpPaint.ComputeScoreIfApply(target, current);
                //scores.Add(currentScore);

                if (currentScore > currentMaxScore)
                {
                    currentMaxScore = currentScore;
                    paint = tmpPaint;
                    continue;
                }

                if (currentScore <= currentMaxScore)
                {
                    break;
                }
            }
            return new KeyValuePair<double, Paint>(currentMaxScore, paint);
        }

        public static KeyValuePair<double, Paint> ChooseNextBestPaint(bool[,] target, int[,] current, int nbAllowedTries, Random rand, HashSet<Point> alreadyProcessedPoints)
        {
            var bestPointScore = new KeyValuePair<double, Paint>(0.0, null);



            var points = new List<Point>();
            for (int trial = 0; trial < nbAllowedTries; ++trial)
            {
                var point = new Point(rand.Next(target.GetLength(0)), rand.Next(target.GetLength(1)));
                if (alreadyProcessedPoints.Add(point))
                {
                    points.Add(point);
                }
            }
            /*
            var pointScores = ThreadManager<KeyValuePair<double, Paint>>.ComputeForAllDatas(points, p => ComputeBestPossibleScore(p.X, p.Y, target, current));
            var bestPointScore = pointScores[0];
            foreach (var pointScore in pointScores)
                if (pointScore.Key > bestPointScore.Key)
                    bestPointScore = pointScore;
            return bestPointScore;
            */

            //Parallel.For

            foreach (var p in points)
            {
                var pointScore = ComputeBestPossibleScore(p.X, p.Y, target, current);
                if (pointScore.Key > bestPointScore.Key)
                {
                    //bestPointScore = pointScore;
                    bestPointScore = ChooseNextBestPaint(target, current, nbAllowedTries, p.X, p.Y, pointScore, rand, alreadyProcessedPoints);
                }
            }
            return bestPointScore;
        }
        public static KeyValuePair<double, Paint> ChooseNextBestPaint(bool[,] target, int[,] current, int nbAllowedTries, int row, int col, KeyValuePair<double, Paint> xyScore, Random rand, HashSet<Point> alreadyProcessedPoints)
        {
            var result = xyScore;
            for (int numTrial = 0; numTrial < nbAllowedTries; ++numTrial)
            {
                const int delta = 20;
                int row0 = row + rand.Next(-delta, delta);
                int col0 = col + rand.Next(-delta, delta);
                if (!Utils.IsValidCoordinate(current,row0, col0))
                {
                    continue;
                }
                if (!alreadyProcessedPoints.Add(new Point(row0, col0)))
                {
                    continue;
                }
                var tmpScore = ComputeBestPossibleScore(row0, col0, target, current);
                if (tmpScore.Key > result.Key)
                {
                    result = ChooseNextBestPaint(target, current, row0, col0, nbAllowedTries, tmpScore, rand, alreadyProcessedPoints);
                }
            }
            return result;
        }

        public static void Process(string filePath)
        {
            var target = LoadDrawing(filePath);
            var actions = new List<IAction>();
            var current = ComputeDrawing(target, actions);

            var rand = new Random(19);

            int nbConsecutiveWrongs = 0;

            for (int i = 0; i < target.Length; ++i)
            {
                var alreadyProcessedPoints = new HashSet<Point>();
                var nextBestPaint = ChooseNextBestPaint(target, current, 20000, rand, alreadyProcessedPoints);
                if (nextBestPaint.Value == null)
                {
                    ++nbConsecutiveWrongs;
                    if (nbConsecutiveWrongs > 2000)
                    {
                        break;
                    }
                    continue;
                }
                nbConsecutiveWrongs = 0;
                actions.Add(nextBestPaint.Value);
                nextBestPaint.Value.Apply(current);
            }

            int nbCorrected = 0;
            for (int i = 0; i < actions.Count; ++i)
                for (int j = 0; j < actions.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    if (actions[j].IsIncludedIn(actions[i]))
                    {
                        ++nbCorrected;
                        actions.RemoveAt(j);
                        if (i >= j)
                        {
                            --i;
                        }
                        j--;
                    }
                }


            Console.WriteLine("nbCorrected = " + nbCorrected);
            var missing = AddNeededActions(target, current);
            Console.WriteLine("missing = " + missing.Count);
            actions.AddRange(missing);
            Console.WriteLine("score = " + actions.Count);
            System.IO.File.WriteAllText(filePath + "." + DateTime.Now.Ticks, string.Join(Environment.NewLine, actions.Select(x => x.ToString()).ToArray()));
        }



        public static bool[,] LoadDrawing(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            var xy = lines[0].Split(',').Select(int.Parse).ToList();

            var result = new bool[xy[0], xy[1]];
            for (int colNum = 0; colNum < xy[1]; ++colNum)
            {
                var line = lines[1 + colNum];
                for (int rowNum = 0; rowNum < line.Length; ++rowNum)
                {
                    result[rowNum, colNum] = line[rowNum] == '#';
                }
            }
            return result;
        }

        public static int[,] ComputeDrawing(bool[,] drawing, List<IAction> actions)
        {
            var result = new int[drawing.GetLength(0), drawing.GetLength(1)];
            foreach (var a in actions)
            {
                a.Apply(result);
            }
            return result;
        }

        public static List<IAction> AddNeededActions(bool[,] target, int[,] current)
        {
            var missingActions = new List<IAction>();
            for (int colNum = 0; colNum < target.GetLength(1); ++colNum)
                for (int rowNum = 0; rowNum < target.GetLength(0); ++rowNum)
                {
                    if (target[rowNum,colNum])
                    {
                        if (current[rowNum,colNum] == 0)
                        {
                            missingActions.Add(new Paint(rowNum, colNum, 1));
                        }
                    }
                    else
                    {
                        if (current[rowNum,colNum] >= 1)
                        {
                            missingActions.Add(new Erase(rowNum, colNum));
                        }
                    }
                }
            return missingActions;
        }




    }
}
