using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        public static double Distance(Tuple<int, int, int> a, Tuple<int, int, int> b) { return Math.Sqrt(Math.Pow(a.Item1 - b.Item1, 2) + Math.Pow(a.Item2 - b.Item2, 2) + Math.Pow(a.Item3 - b.Item3, 2)); }
        public static double Distance(Tuple<int, int> a, Tuple<int, int> b) { return Math.Sqrt(Math.Pow(a.Item1 - b.Item1, 2) + Math.Pow(a.Item2 - b.Item2, 2)); }
        public static double Distance(Tuple<double, double, double> a, Tuple<double, double, double> b) { return Math.Sqrt(Math.Pow(a.Item1 - b.Item1, 2) + Math.Pow(a.Item2 - b.Item2, 2) + Math.Pow(a.Item3 - b.Item3, 2)); }
        public static double Distance(Tuple<double, double> a, Tuple<double, double> b) { return Math.Sqrt(Math.Pow(a.Item1 - b.Item1, 2) + Math.Pow(a.Item2 - b.Item2, 2)); }
        public static double Distance(Point a, Point b) { return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)); }

        // -1 if 'a=>b=>c' is clockwise (right turn)
        // +1 if 'a=>b=>c' counter clockwise (left turn)
        //  0 if 'a=>b=>c' colinear (aligned) 
        public static int Direction(Point a, Point b, Point c)
        {
            var direction = ((long) (a.X - c.X)) * (b.Y - c.Y) - ((long) (b.X - c.X)) * (a.Y - c.Y);
            return direction != 0 ? Math.Sign(direction) : 0;
        }
        public static int Direction(Tuple<double,double> a, Tuple<double, double> b, Tuple<double, double> c)
        {
            var direction = (a.Item1 - c.Item1) * (b.Item2 - c.Item2) - (b.Item1 - c.Item1) * (a.Item2 - c.Item2);
            return Math.Abs(direction)>1e-8 ? Math.Sign(direction) : 0;
        }

        //returns the lower and upper part of the convex hull in o(n log(n)) time using Andrew's monotone chain convex hull algorithm
        //  the lower and upper part will always share the same first and last point
        public static void ConvexLowerAndUpperHull(IEnumerable<Point> allPoints, out List<Point> lower, out List<Point> upper)
        {
            var sortedPoints = new List<Point>(allPoints);
            sortedPoints.Sort((a, b) => a.X.Equals(b.X) ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));
            upper = new List<Point>();
            foreach (var t in sortedPoints)
            {
                while ((upper.Count >= 2) && Direction(upper[upper.Count - 2], upper[upper.Count - 1], t) >= 0)
                {
                    upper.RemoveAt(upper.Count -1); // 'upper[lower.Count-2]=>upper[lower.Count-1]=>t is counter clockwise or aligned
                }
                upper.Add(t);
            }
            lower = new List<Point>();
            foreach (var t in sortedPoints)
            {
                while ((lower.Count >= 2) && Direction(lower[lower.Count - 2], lower[lower.Count - 1], t) <= 0)
                {
                    lower.RemoveAt(lower.Count -1); // 'lower[lower.Count-2]=>lower[lower.Count-1]=>t is clockwise or aligned
                }
                lower.Add(t);
            }
        }

        //return the area of a polygon in o(n) time
        public static double GetArea(IList<Point> points)
        {
            double signedArea = 0;
            for (int i = 0; i < points.Count; i++)
            {
                int prevI = (i == 0) ? points.Count - 1 : i - 1;
                signedArea += ((double) points[prevI].X) * points[i].Y - ((double) points[i].X) * points[prevI].Y;
            }
            return Math.Abs(signedArea / 2); // 'signedArea' is negative if the polygon is oriented clockwise. 
        }

        //return the smallest circle including all points 'p' in o(n) time
        // return.Item1: the radius of the circle / return.Item2: center of the circle
        public static Tuple<double, Tuple<double, double>> SmallestCircleIncludingAllPoints(IEnumerable<Tuple<double, double>> p)
        {
            return SmallestCircleIncludingAllPoints_Helper(p.ToList(), new List<Tuple<double, double>>(), new Random(0));
        }
        private static Tuple<double, Tuple<double, double>> SmallestCircleIncludingAllPoints_Helper(List<Tuple<double, double>> P, List<Tuple<double, double>> R, Random r)
        {
            if (P.Count == 0 || R.Count >= 3)
            {
                if (R.Count == 1)
                {
                    return Tuple.Create(0.0, R[0]);
                }
                if (R.Count == 2)
                {
                    return Tuple.Create(Distance(R[0], R[1])/2.0, Tuple.Create((R[0].Item1+ R[1].Item1) /2.0, (R[0].Item2 + R[1].Item2) / 2.0));
                }
                return CirclePassingByAllPointsIfAny(R); //if the points of R are cocircular: return the circle they determine
            }

            var randomIndexInP = r.Next(P.Count);
            var p = P[randomIndexInP];
            P[randomIndexInP] = P.Last();
            P.RemoveAt(P.Count-1);
            var D = SmallestCircleIncludingAllPoints_Helper(P, R, r);
            if (D != null && Distance(p, D.Item2) <= D.Item1)
            {
                P.Add(p);
                return D; //p was already included in smallest circle
            }
            R.Add(p);
            var result = SmallestCircleIncludingAllPoints_Helper(P, R, r);
            R.RemoveAt(R.Count-1);
            P.Add(p);
            return result;

        }
        //returns the coordinate of the circle passing by all points 'P' in o(N) time
        //returns null if there are no such circle or if there are an infinite number of such circles
        private static Tuple<double, Tuple<double, double>> CirclePassingByAllPointsIfAny(List<Tuple<double, double>> P)
        {
            if (P.Count < 3)
            {
                return null; //infinite number of circles
            }
            var x1 = P[0].Item1; var y1 = P[0].Item2;
            var x2 = P[1].Item1; var y2 = P[1].Item2;
            var x3 = P[2].Item1; var y3 = P[2].Item2;
            if (x1==x2||x2==x3||x1==x3||y1==y2||y2==y3||y1==y3|| Direction(P[0], P[1], P[2]) == 0)
            {
                return null;
            }
            var mr = (y2 - y1) / (x2 - x1);
            var mt = (y3 - y2) / (x3 - x2);
            var x = (mr * mt * (y3 - y1) + mr * (x2 + x3) - mt * (x1 + x2)) / (2 * (mr - mt));
            var y = (-1.0 / mr) * (x - (x1 + x2) / 2.0) + (y1 + y2) / 2;
            var center = Tuple.Create(x, y);
            var radius = Distance(center, P[0]);
            foreach(var p in P.Skip(3)) //we ensure that all remaining points are also passing in the circle
            {
                if (Math.Abs(Distance(center, p) - radius) > 1e-8)
                {
                    return null;
                }
            }
            return Tuple.Create(radius, center);
        }

        //returns the convex hull in o(n log(n)) time using Andrew's monotone chain convex hull algorithm
        public static List<Point> ConvexHull(IEnumerable<Point> allPoints)
        {
            List<Point> lower, upper;
            ConvexLowerAndUpperHull(allPoints, out lower, out upper);
            if (upper.Count <= 1)
            {
                return upper;
            }
            upper.RemoveAt(upper.Count - 1);
            upper.Reverse();
            upper.RemoveAt(upper.Count - 1);
            lower.AddRange(upper);
            return lower;
        }

        #region nearest points in o( n log(n)^2 ) and farthest points in o( n log(n) )
        //find the minimum distance among 'points' (between the 2 nearest points) in o( n log(n)^2 ) time
        //  step1 : sort all points according X coordinate
        //  step2: search for min distance among first half of all points (recursively)
        //  step3: search for min distance among second half of all points (recursively)
        //  step4: check if a point in first half of points is very near a point in second half of points
        public static double MinDistance(List<Point> points)
        {
            points.Sort((a, b) => a.X.Equals(b.X) ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));
            var minDistance = double.MaxValue;
            var nearest1 = points.First();
            var nearest2 = nearest1;
            MinDistanceHelper(points, 0, points.Count - 1, ref minDistance, ref nearest1, ref nearest2);
            return minDistance; //return new KeyValuePair<Point, Point>(nearest1, nearest2);
        }
        //sortedPoints : all points sorted by X coordinate
        private static void MinDistanceHelper(IList<Point> sortedPoints, int i, int j, ref double minDistance, ref Point nearest1, ref Point nearest2)
        {
            int nbElements = j - i + 1;
            if (nbElements <= 2) //no need to make subcall
            {
                if (nbElements == 2 && Distance(sortedPoints[i], sortedPoints[j]) < minDistance)
                {
                    minDistance = Distance(sortedPoints[i], sortedPoints[j]);
                    nearest1 = sortedPoints[i];
                    nearest2 = sortedPoints[j];
                }
                return;
            }
            var midIndex = (i + j) / 2;
            //we look for nearest points in range [0,midIndex]
            MinDistanceHelper(sortedPoints, i, midIndex, ref minDistance, ref nearest1, ref nearest2);
            //we look for nearest points in range [midIndex+1,end]
            MinDistanceHelper(sortedPoints, midIndex + 1, j, ref minDistance, ref nearest1, ref nearest2);

            //we check if a point in range [0,midIndex] is very near (<minDistance) a point in range [midIndex+1,end]
            var midX = sortedPoints[midIndex].X;
            var
                strip = new List<Point>(); //all points with a X coordinate very near (<minDistance) sortedPoints[midIndex].X 
            for (int k = midIndex + 1; (k <= j) && ((sortedPoints[k].X - midX) < minDistance); ++k)
            {
                strip.Add(sortedPoints[k]);
            }
            if (strip.Count == 0)
            {
                return;
            }
            for (int k = midIndex; (k >= i) && (midX - sortedPoints[k].X) < minDistance; --k)
            {
                strip.Add(sortedPoints[k]);
            }
            strip.Sort((a, b) => a.Y.CompareTo(b.Y));
            for (int i0 = 0; i0 < strip.Count; ++i0)
            {
                for (int j0 = i0 + 1; j0 < strip.Count && (strip[j0].Y - strip[i0].Y) < minDistance; ++j0)
                {
                    var curDistance = Distance(strip[i0], strip[j0]);
                    if (curDistance < minDistance)
                    {
                        minDistance = curDistance;
                        nearest1 = strip[i0];
                        nearest2 = strip[j0];
                    }
                }
            }
        }

        //find the 2 maximum distance among 'points' in o( n log(n) ) time
        //  step 1: compute the convex hull in o(n log(n) time
        //  step 2: find the farthest points in this convex hull using Rotating Calipers in o(n) time
        public static double MaxDistance(IEnumerable<Point> points)
        {
            List<Point> lower, upper;
            ConvexLowerAndUpperHull(points, out lower, out upper);
            //var farthest1 = lower[0];var farthest2 = lower[0]; //uncomment to retrieve the points at max distance
            double maxDistance = 0;
            int i = 0;
            int j = lower.Count - 1;
            while (i < upper.Count - 1 || j > 0) //we use Rotating Calipers
            {
                var curDistance = Distance(upper[i], lower[j]);
                if (curDistance > maxDistance)
                {
                    maxDistance = curDistance;
                    //farthest1 = upper[i];farthest2 = lower[j]; //uncomment to retrieve the points at max distance
                }
                if (i == upper.Count - 1)
                {
                    --j;
                }
                else if (j == 0)
                {
                    ++i;
                }
                else if (((long) (upper[i + 1].Y - upper[i].Y)) * (lower[j].X - lower[j - 1].X) >
                         ((long) (lower[j].Y - lower[j - 1].Y)) * (upper[i + 1].X - upper[i].X))
                {
                    ++i;
                }
                else
                {
                    --j;
                }
            }
            return maxDistance; //return new KeyValuePair<Point, Point>(farthest1, farthest2);
        }
        #endregion

        //area of the overlapping rectangles in o(n^3) time
        public static long OverlappingSurfaceOfAllRectangles(List<Tuple<int, int, int, int>> rect)
        {
            //each rectangle is a Tuple<left,top,right,bottom> (right>left && top>bottom)
            var all_Y = new HashSet<int>(rect.Select(r => r.Item2).Union(rect.Select(r => r.Item4))).OrderBy(t => t).ToList();
            var all_X = new HashSet<int>(rect.Select(r => r.Item1).Union(rect.Select(r => r.Item3))).OrderBy(t => t).ToList();
            long overlappingArea = 0;
            for (int row = 0; row < all_Y.Count - 1; ++row)
            {
                int bottom = all_Y[row];
                int top = all_Y[row + 1];
                for (int col = 0; col < all_X.Count - 1; ++col)
                {
                    var left = all_X[col];
                    var right = all_X[col + 1];
                    int nbOverlappingRectangles = 0;
                    foreach (var r in rect)
                    {
                        if (left < r.Item1 || right > r.Item3 || bottom < r.Item4 || top > r.Item2)
                        {
                            continue;
                        }
                        ++nbOverlappingRectangles;
                        if (nbOverlappingRectangles >= 2)
                        {
                            overlappingArea += (right - left) * (top - bottom);
                            break;
                        }
                    }
                }
            }
            return overlappingArea;
        }
    }
}