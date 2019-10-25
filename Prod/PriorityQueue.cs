using System.Collections.Generic;
using System.Linq;

namespace SharpAlgos
{
    public class PriorityQueue<T>
    {
        private readonly bool _isMinPriorityQueue;
        private int _count;
        private readonly SortedDictionary<double, HashSet<T>> _q = new SortedDictionary<double, HashSet<T>>();

        public PriorityQueue(bool isMinPriorityQueue)
        {
            _isMinPriorityQueue = isMinPriorityQueue;
        }
        public int Count => _count;

        /// <summary>
        /// Add new element (with priority = 'priority') in the queue
        /// Complexity:         o( log(n) )
        /// </summary>
        /// <param name="t">the element to add</param>
        /// <param name="priority">the associate priority</param>
        public void Enqueue(T t, double priority)
        {
            if (!_isMinPriorityQueue)
            {
                priority = InvertPriority(priority);
            }
            if (!_q.ContainsKey(priority))
            {
                _q.Add(priority, new HashSet<T>());
            }
            _q[priority].Add(t);
            ++_count;
        }

        /// <summary>
        /// Update the priority of an existing element 't' from 'oldPriority' to 'newPriority'
        /// Complexity:         o( log(n) )
        /// </summary>
        /// <param name="t">an existing element in the queue</param>
        /// <param name="oldPriority">the old element priority</param>
        /// <param name="newPriority">the new element priority</param>
        public void UpdatePriority(T t, double oldPriority, double newPriority)
        {
            if (!_isMinPriorityQueue)
            {
                oldPriority = InvertPriority (oldPriority);
            }
            _q[oldPriority].Remove(t);
            --_count;
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
            foreach (var e in _q.Values)
            {
                if (e.Count != 0)
                {
                    var result = e.First();
                    e.Remove(result);
                    --_count;
                    return result;
                }
            }
            return default(T);
        }
    }
}