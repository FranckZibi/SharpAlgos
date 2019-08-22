using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public class PriorityQueue<T>
    {
        private readonly bool isMinPriorityQueue;
        private int count;
        private readonly SortedDictionary<double, HashSet<T>> q = new SortedDictionary<double, HashSet<T>>();

        public PriorityQueue(bool isMinPriorityQueue)
        {
            this.isMinPriorityQueue = isMinPriorityQueue;
        }
        public int Count {get {return count;}}
        public void Enqueue(T t, double priority)
        {
            if (!isMinPriorityQueue)
            {
                priority = InvertPriority(priority);
            }
            if (!q.ContainsKey(priority))
            {
                q.Add(priority, new HashSet<T>());
            }
            q[priority].Add(t);
            ++count;
        }
        public void UpdatePriority(T t, double oldPriority, double newPriority)
        {
            if (!isMinPriorityQueue)
            {
                oldPriority = InvertPriority (oldPriority);
            }
            q[oldPriority].Remove(t);
            --count;
            Enqueue(t, newPriority);
        }
        private static double InvertPriority(double priority)
        {
            if (priority == double.MaxValue)
            {
                return double.MinValue;
            }
            if (priority == double.MinValue)
            {
                return double.MaxValue;
            }
            return -priority;
        }
        public T Dequeue()
        {
            foreach (var e in q.Values)
            {
                if (e.Count != 0)
                {
                    var result = e.First();
                    e.Remove(result);
                    --count;
                    return result;
                }
            }
            return default(T);
        }
    }
}