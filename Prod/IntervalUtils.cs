using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
        public class IntervalTree
        {
            #region private fields
            private readonly bool _endOfIntervalIsIncludedInInterval;
            //interval containing _center
            private readonly List<Tuple<int, int>> _intervalsSortedByStart;
            //interval containing _center
            private readonly List<Tuple<int, int>> _intervalsSortedByEnd;
            private readonly int _center;
            //intervals that end before _center
            private readonly IntervalTree _left;
            //intervals that start strictly after _center
            private readonly IntervalTree _right;
            #endregion

            private IntervalTree(List<Tuple<int, int>> intervalsSortedByStart, bool endOfIntervalIsIncludedInInterval, int center, IntervalTree left, IntervalTree right)
            {
                _intervalsSortedByStart = intervalsSortedByStart;
                _endOfIntervalIsIncludedInInterval = endOfIntervalIsIncludedInInterval;
                _intervalsSortedByEnd = intervalsSortedByStart.OrderBy(x=>x.Item2).ToList();
                _center = center;
                _left = left;
                _right = right;
            }

            /// <summary>
            /// return the list of intervals containing 'x' in o(log(n) + m) time
            /// (+ o(n log(n) ) preparation time
            /// (where 'm' is the number of interval returned)
            /// </summary>
            /// <param name="x"></param>
            /// <returns>list of intervals containing x</returns>
            public List<Tuple<int, int>> AllIntervalsContaining(int x)
            {
                List<Tuple<int, int>> res;
                if (x < _center || (x == _center && !_endOfIntervalIsIncludedInInterval))
                {
                    res = (_left == null) ? new List<Tuple<int, int>>() : _left.AllIntervalsContaining(x);
                    foreach (var i in _intervalsSortedByStart)
                    {
                        if (i.Item1 > x)
                        {
                            break;
                        }
                        res.Add(i);
                    }
                }
                else
                {
                    res  = (_right == null) ? new List<Tuple<int, int>>() : _right.AllIntervalsContaining(x);
                    for (var index = _intervalsSortedByEnd.Count-1; index >=0; --index)
                    {
                        var i = _intervalsSortedByEnd[index];
                        if ((i.Item2 < x) || (i.Item2 == x && !_endOfIntervalIsIncludedInInterval) )
                        {
                            break;
                        }
                        res.Add(i);
                    }
                }

                return res;
            }

            /// <summary>
            /// return the number of intervals containing 'x' in 
            /// Complexity:         o(log(n))  (+ o(n log(n) ) preparation time)
            ///                     (where 'n' is the total number of interval)
            /// </summary>
            /// <param name="x"></param>
            /// <returns>number of intervals containing x</returns>
            public int CountIntervalsContaining(int x)
            {
                if (x < _center || (x == _center && !_endOfIntervalIsIncludedInInterval))
                {
                    return (_left?.CountIntervalsContaining(x)??0)
                            + MaximumValidIndex(0, _intervalsSortedByStart.Count,idx => (idx == 0) || _intervalsSortedByStart[idx - 1].Item1 <= x);
                }
                else
                {
                    return (_right?.CountIntervalsContaining(x)??0)
                            +MaximumValidIndex(0, _intervalsSortedByEnd.Count, idx => (idx == 0) 
                                || _intervalsSortedByEnd[_intervalsSortedByEnd.Count-idx].Item2 > x
                                || (_endOfIntervalIsIncludedInInterval&& _intervalsSortedByEnd[_intervalsSortedByEnd.Count-idx].Item2 == x) 
                                );
                }
            }

            public static IntervalTree ValueOf(List<Tuple<int, int>> intervals, bool endOfIntervalIsIncludedInInterval, bool intervalAreAlreadySorted = false)
            {
                if (!intervalAreAlreadySorted)
                {
                    intervals = intervals.OrderBy(x => x.Item1).ThenBy(x => x.Item2).ToList();
                    if (!endOfIntervalIsIncludedInInterval)
                    {
                        intervals.RemoveAll(i => i.Item1 == i.Item2);
                    }
                }
                if (intervals.Count == 0)
                {
                    return null;
                }


                var left = new List<Tuple<int, int>>();
                var c = new List<Tuple<int, int>>();
                var right = new List<Tuple<int, int>>();
                var center = intervals[intervals.Count / 2].Item1;

                foreach (var i in intervals)
                {
                    if (i.Item2 < center || (i.Item2 == center && !endOfIntervalIsIncludedInInterval))
                    {
                        left.Add(i);
                    }
                    else if (i.Item1 > center)
                    {
                        right.Add(i);
                    }
                    else
                    {
                        c.Add(i);
                    }
                }

                return new IntervalTree(c, 
                    endOfIntervalIsIncludedInInterval, 
                    center,
                    ValueOf(left, endOfIntervalIsIncludedInInterval, true),
                    ValueOf(right, endOfIntervalIsIncludedInInterval, true));
            }
        }

        /// <summary>
        /// find the element that is present in the most intervals
        /// </summary>
        /// <param name="intervals"></param>
        /// <returns>
        /// Item1 : the element that is present in the most intervals
        /// Item2 : number of times it appears
        /// </returns>
        public static Tuple<double,int> ElementInMostInterval(List<Tuple<double, double>> intervals)
        {
            var data = new List<Tuple<double, bool>>();
            data.AddRange(intervals.Select(x=>Tuple.Create(x.Item1,true)));
            data.AddRange(intervals.Select(x=>Tuple.Create(x.Item2,false)));

            data = data.OrderBy(x => x.Item1).ThenBy(x => x.Item2).ToList();

            int maxCount = 0;
            double valueForMaxCount = 0.0;

            int currentCount = 0;
            foreach (var d in data)
            {
                if (d.Item2) //start of interval
                {
                    ++currentCount;
                    if (currentCount > maxCount)
                    {
                        maxCount = currentCount;
                        valueForMaxCount = d.Item1;
                    }
                }
                else //end of interval
                {
                    --currentCount;
                }
            }
            return Tuple.Create(valueForMaxCount, maxCount);
        }


        /// <summary>
        /// return the minimum point need to have at least one point in each interval
        /// Complexity:         o( n )
        /// </summary>
        /// <param name="intervals"></param>
        /// <param>
        /// true if open interval (where start and end of interval are not included in interval (ex: ]a,b[ )
        /// false for closed or semi open interval : [a,b] or ]a,b] or [a;b[
        /// <name>bothStartAndEndAreExcludedFromInterval</name>
        /// </param>
        /// <returns>
        /// minimum list of point coordinates to have at least one point in each interval
        /// </returns>
        public static List<int> MinimumPointsToCoverAllIntervals(List<Tuple<int, int>> intervals)
        {
            var result  =  new List<int>();
            intervals = intervals.OrderBy(x => x.Item2).ToList();
            foreach (var i in intervals)
            {
                if (result.Count == 0)
                {
                    result.Add(i.Item2);
                }
                else
                {
                    if (result.Last() < i.Item1) //no point in interval
                    {
                        result.Add(i.Item2);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// merge all intervals that intersect and return the new list of all intervals (with merged intervals)
        /// Complexity:         o( n )
        /// </summary>
        /// <param name="intervals"></param>
        /// <param name="bothStartAndEndAreExcludedFromInterval">
        /// true if open interval (where start and end of interval are not included in interval (ex: ]a,b[ )
        /// false for closed or semi open interval : [a,b] or ]a,b] or [a;b[
        /// </param>
        /// <returns>
        /// merged intervals
        /// </returns>
        public static List<Tuple<int, int>> IntervalsUnion(List<Tuple<int, int>> intervals, bool bothStartAndEndAreExcludedFromInterval = false)
        {
            var result  =new List<Tuple<int, int>>();
            var tmp = intervals.Select(x=> Tuple.Create(x.Item1, true)).ToList();
            tmp.AddRange(intervals.Select(x=> Tuple.Create(x.Item2, false)));
            if (bothStartAndEndAreExcludedFromInterval)
            {
                tmp = tmp.OrderBy(x => x.Item1).ThenBy(x => x.Item2).ToList();
            }
            else
            {
                tmp = tmp.OrderBy(x => x.Item1).ThenByDescending(x => x.Item2).ToList();
            }

            int currentStart = 0;
            int nbInCurrentInterval = 0;

            foreach (var e in tmp)
            {
                if (e.Item2) //start of interval
                {
                    if (nbInCurrentInterval == 0)
                    {
                        currentStart = e.Item1;
                    }
                    ++nbInCurrentInterval;
                }
                else //end of interval
                {
                    --nbInCurrentInterval;
                    if (nbInCurrentInterval == 0)
                    {
                        result.Add(Tuple.Create(currentStart, e.Item1));
                    }
                }
            }
            if (nbInCurrentInterval != 0)
            {
                result.Add(Tuple.Create(currentStart, tmp.Last().Item1));
            }
            return result;
        }
    }
}
