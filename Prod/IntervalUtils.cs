using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public static partial class Utils
    {
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
    }
}