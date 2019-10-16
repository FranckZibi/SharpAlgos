using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

// ReSharper disable UnusedMember.Global

namespace SharpAlgos
{

    public class Graph<T>
    {
        #region Private fields
        private readonly IDictionary<T, IDictionary<T, double>> _edgesWithCost = new Dictionary<T, IDictionary<T, double>>();
        private readonly bool _isDirected;
        #endregion
        public Graph(bool isDirected) { this._isDirected = isDirected; }
        public void Add(T from, T to, double cost)
        {
            AddVertex(from);
            AddVertex(to);
            _edgesWithCost[from][to] = cost;
            if (!_isDirected)
            {
                _edgesWithCost[to][@from] = cost;
            }
        }
        public IEnumerable<T> Children(T t)
        {
            IDictionary<T, double> tmp;
            return _edgesWithCost.TryGetValue(t, out tmp) ? tmp.Keys : new List<T>();
        }
        private static List<T> ExtractPath(T start, T end, IDictionary<T, T> prevVertex)
        {
            var path = new List<T> { end };
            for (; ; )
            {
                if (Equals(path[0], start))
                {
                    return path;
                }
                if (path.Count > prevVertex.Count)
                {
                    return null;
                }
                T previousIndex;
                if (!prevVertex.TryGetValue(path[0], out previousIndex))
                {
                    return null;
                }
                path.Insert(0, previousIndex);
            }
        }
        public HashSet<T> Vertices { get { return new HashSet<T>(_edgesWithCost.Keys); } }
        public void AddVertex(T v)
        {
            if (!_edgesWithCost.ContainsKey(v))
            {
                _edgesWithCost[v] = new Dictionary<T, double>();
            }
        }
        public void Remove(T from, T to)
        {
            if (_edgesWithCost.ContainsKey(from))
            {
                _edgesWithCost[@from].Remove(to);
            }
            if (!_isDirected && _edgesWithCost.ContainsKey(to))
            {
                _edgesWithCost[to].Remove(@from);
            }
        }
        public void Remove(T from)
        {
            _edgesWithCost.Remove(from);
            foreach (var edges in _edgesWithCost)
            {
                edges.Value.Remove(@from);
            }
        }
        public int EdgeCount
        {
            get
            {
                var totalEdges = _edgesWithCost.Select(x => x.Value.Count).Sum();
                return _isDirected ? totalEdges : totalEdges / 2;
            }
        }
        public int VerticeCount => _edgesWithCost.Count;
        public List<double> AllEdgeCost()
        {
            var result = new List<double>();
            foreach (var targetWithCost in _edgesWithCost.Values)
            {
                result.AddRange(targetWithCost.Values);
            }

            return result;
        }
        private IDictionary<T, HashSet<T>> ChildrenToParents()
        {
            var result = new Dictionary<T, HashSet<T>>();
            foreach (var start in _edgesWithCost)
                foreach (var end in start.Value)
                {
                    if (!result.ContainsKey(end.Key))
                    {
                        result.Add(end.Key, new HashSet<T>());
                    }
                    result[end.Key].Add(start.Key);
                }
            return result;
        }
        private HashSet<T> Children(IEnumerable<T> parents)
        {
            var result = new HashSet<T>();
            foreach (var parent in parents)
            {
                result.UnionWith(Children(parent));
            }
            return result;
        }
        private static HashSet<T> AllParents(IEnumerable<T> children, IDictionary<T, HashSet<T>> childrenToParent)
        {
            var result = new HashSet<T>();
            HashSet<T> tmp;
            foreach (var c in children)
            {
                if (childrenToParent.TryGetValue(c, out tmp))
                {
                    result.UnionWith(tmp);
                }
            }
            return result;
        }
        public double CostOf(IList<T> validPath)
        {
            double result = 0;
            for (int i = 1; i < validPath.Count; i++)
            {
                result += _edgesWithCost[validPath[i - 1]][validPath[i]];
            }

            int a;
            a = 5;
            return result;
        }


        /*
        Shortest path:
        =============
        if TREE:                                            DFS
        else if Weights is a constant > 0:                  BFS
        else if Weights are all > 0:                        Dijkstra
        else if Weights >0 and <0 & no negative cycles:     Floyd Warshall or Belmman Ford
        else (graph can contains negative cycles):          DFS

        Longest path:
        =============
        if TREE:                                            DFS
        else if Weights is a constant > 0:                  BFS
        else if no positive cycles:                         shortest path with Floyd Warshall or Belmman Ford (first inversing weights)
        else (graph can contains positive cycles):          DFS
        */


        #region DFS (Depth First Search): Find shortest/longest path in o(V) (tree) to o(V!) (dense graph) time.
        //Works for both negative and postive weight
        public List<T> ShortestPath_DFS(T start, T end)
        {
            var bestPath = new List<T>();
            var bestPathScore = double.MaxValue;
            BestPath_DFS_Helper(new List<T> { start }, 0, end, bestPath, ref bestPathScore, true);
            if (bestPath.Count == 0)
            {
                return null;
            }
            return bestPath;
        }
        public List<T> LongestPath_DFS(T start, T end)
        {
            var bestPath = new List<T>();
            var bestPathScore = double.MinValue;
            BestPath_DFS_Helper(new List<T> { start }, 0, end, bestPath, ref bestPathScore, false);
            if (bestPath.Count == 0)
            {
                return null;
            }
            return bestPath;
        }
        private void BestPath_DFS_Helper(IList<T> path, double pathScore, T end, List<T> bestPath, ref double bestPathScore, bool minimizeCost)
        {
            var vertex = path.Last();
            if (Equals(vertex, end))
            {
                if ((minimizeCost && pathScore < bestPathScore)
                    || (!minimizeCost && pathScore > bestPathScore))
                {
                    bestPath.Clear();
                    bestPath.AddRange(path);
                    bestPathScore = pathScore;
                }
                return;
            }
            foreach (var child in Children(vertex))
            {
                if (path.Contains(child))
                {
                    continue; //already used
                }
                path.Add(child);
                BestPath_DFS_Helper(path, pathScore + _edgesWithCost[vertex][child], end, bestPath, ref bestPathScore, minimizeCost);
                path.RemoveAt(path.Count - 1);
            }
        }
        //Find one shortest path from 'start' in o(V) (tree) to o(V!) (dense graph) time
        public List<T> ShortestPath_DFS(T start)
        {
            if (!_edgesWithCost.ContainsKey(start))
            {
                return null;
            }
            var bestPath = new List<T>();
            var bestPathScore = double.MaxValue;
            BestPath_DFS_Helper(new List<T> { start }, 0, bestPath, ref bestPathScore, true);
            return bestPath;
        }
        //Find one longest path from 'start' in o(V) (tree) to o(V!) (dense graph) time
        public List<T> LongestPath_DFS(T start)
        {
            if (!_edgesWithCost.ContainsKey(start))
            {
                return null;
            }
            var bestPath = new List<T>();
            var bestPathScore = double.MinValue;
            BestPath_DFS_Helper(new List<T> { start }, 0, bestPath, ref bestPathScore, false);
            return bestPath;
        }
        private void BestPath_DFS_Helper(List<T> path, double pathScore, List<T> bestPath, ref double bestPathScore, bool minimizeCost)
        {
            var vertex = path.Last();
            foreach (var child in Children(vertex))
            {
                if (path.Contains(child))
                {
                    continue; //already used
                }
                path.Add(child);
                var pathScoreWithChild = pathScore + _edgesWithCost[vertex][child];
                if ((minimizeCost && pathScoreWithChild < bestPathScore)
                    || (!minimizeCost && pathScoreWithChild > bestPathScore))
                {
                    bestPath.Clear();
                    bestPath.AddRange(path);
                    bestPathScore = pathScoreWithChild;
                }
                BestPath_DFS_Helper(path, pathScoreWithChild, bestPath, ref bestPathScore, minimizeCost);
                path.RemoveAt(path.Count - 1);
            }
        }
        //Find longest path from any two vertices in o(V) (tree) to o(V!) (dense graph) time
        //works only for 'connected undidirected graph' or 'strongly connected directed graph'
        public List<T> LongestPathInConnectedGraph_DFS()
        {
            var startVertex = LongestPath_DFS(_edgesWithCost.Keys.First()).Last();
            return LongestPath_DFS(startVertex);
        }
        //Find longest path from any two vertices in o(V^2) (tree) to o(V*V!) (dense graph) time
        //works even for not 'connected undirected graph' or not 'strongly connected directed graph'
        public List<T> LongestPathInNotConnectedGraph_DFS()
        {
            var bestPath = new List<T>();
            foreach (var v in Vertices)
            {
                var tmp = LongestPath_DFS(v);
                if ((tmp != null) && tmp.Count > bestPath.Count)
                {
                    bestPath = tmp;
                }
            }
            return bestPath;
        }
        #endregion

        #region BFS (Breadth First Search): Find one shortest path between 'start' and 'end' in o(V) time
        //Works only for constant and positive weight for all edges
        public List<T> ShortestPath_BFS(T start, T end)
        {
            var toProcess = new Queue<T>();
            var prevVertex = new Dictionary<T, T>();
            toProcess.Enqueue(start);
            prevVertex[start] = start;
            while (toProcess.Count != 0)
            {
                var current = toProcess.Dequeue();
                if (Equals(current, end))
                {
                    return ExtractPath(start, end, prevVertex);
                }
                var children = Children(current);
                //children = children.OrderBy(x => x);
                foreach (var c in children)
                {
                    if (!prevVertex.ContainsKey(c))
                    {
                        prevVertex[c] = current;
                        toProcess.Enqueue(c);
                    }
                }
            }
            return null; //no path from 'start' to 'end'
        }
        //Find longest path from any two vertices in o(V) time
        //Works only for constant and positive weight for all edges, and for connected graph
        public List<T> LongestPathInConnectedGraph_BFS()
        {
            var v = _edgesWithCost.Keys.First();
            var longest_from_v = LongestPath_BFS(v);
            return LongestPath_BFS(longest_from_v.Last());
        }
        //Find longest path from vertice 'start' in o(V) time
        //Works only for constant and positive weight for all edges
        public List<T> LongestPath_BFS(T start)
        {
            var end = AllReachable_ByDepth(start).Last().First();
            return ShortestPath_BFS(start, end);
        }
        public List<IList<T>> All_ShortestPath_BFS(T start, T end)
        {
            var allPaths = new List<IList<T>>();
            var allReachableForEachDepth = new List<HashSet<T>> { new HashSet<T> { start } };
            var allUsedSoFar = new HashSet<T> { start };
            for (; ; )
            {
                var lastReachable = allReachableForEachDepth[allReachableForEachDepth.Count - 1];
                var newReachable = Children(lastReachable);
                newReachable.ExceptWith(allUsedSoFar);
                if (newReachable.Count == 0)
                {
                    return allPaths;
                }
                if (newReachable.Contains(end))
                {
                    allReachableForEachDepth.Add(new HashSet<T> { end });
                    break;
                }
                allUsedSoFar.UnionWith(newReachable);
                allReachableForEachDepth.Add(newReachable);
            }
            var childrenToParents = ChildrenToParents();
            for (int i = allReachableForEachDepth.Count - 2; i >= 1; --i)
            {
                allReachableForEachDepth[i].IntersectWith(AllParents(allReachableForEachDepth[i + 1], childrenToParents));
            }
            All_ShortestPath_BFS_Helper(allReachableForEachDepth, new List<T> { start }, allPaths);
            return allPaths;
        }
        private void All_ShortestPath_BFS_Helper(IList<HashSet<T>> allReachableForEachDepth, IList<T> path, IList<IList<T>> allPaths)
        {
            if (path.Count >= allReachableForEachDepth.Count)
            {
                allPaths.Add(new List<T>(path));
                return;
            }
            foreach (var c in Children(path.Last()).Intersect(allReachableForEachDepth[path.Count]))
            {
                path.Add(c);
                All_ShortestPath_BFS_Helper(allReachableForEachDepth, path, allPaths);
                path.RemoveAt(path.Count - 1);
            }
        }
        //All vertices reachable from 'start' in o(V) time (using a BFS search)
        // index '0' of the list : the 'start' vertice itself
        // index '1' of the list : new vertices reachable in 1 move (minimum)
        // index '2' of the list : new vertices reachable in 2 move (minimum)
        // etc...
        // look only for vertices reachable in less the 'maxDepth' moves
        public List<HashSet<T>> AllReachable_ByDepth(T start, int maxDepth = int.MaxValue)
        {
            var visited = new Dictionary<T, int>();
            visited[start] = 0;
            var toProcess = new Queue<T>();
            toProcess.Enqueue(start);
            int observedMaxDepth = 0;
            while (toProcess.Count != 0)
            {
                var current = toProcess.Dequeue();
                foreach (var child in Children(current))
                {
                    if (visited.ContainsKey(child))
                    {
                        continue;
                    }
                    int newDepth = visited[current] + 1;
                    if (newDepth > maxDepth)
                    {
                        continue;
                    }
                    visited[child] = newDepth;
                    observedMaxDepth = Math.Max(observedMaxDepth, newDepth);
                    toProcess.Enqueue(child);
                }
            }
            var result = new List<HashSet<T>>();
            while (result.Count <= observedMaxDepth)
            {
                result.Add(new HashSet<T>());
            }
            foreach (var e in visited)
            {
                result[e.Value].Add(e.Key);
            }
            return result;
        }
        //All vertices reachable from 'start' in o(V) time (using a BFS search)
        //returns only vertices that are reachable in less then 'maxDepth' moves
        //always includes the starting vertice 'start'
        public List<T> AllReachable(T start, int maxDepth = int.MaxValue) {return new List<T>(AllReachable_ByDepth(start, maxDepth).SelectMany(x => x));}
        #endregion

        #region Dijkstra: Find one shortest path in o (E  +  V Log(V) ) time
        //Works only for weights >= 0
        public List<T> ShortestPath_Dijkstra(T start, T end)
        {
            var unvisitedVertices = new HashSet<T>();
            var minDistance = new PriorityQueue<T>(true);
            var infiniteCost = double.MaxValue;
            var minCostToReachVertex = new Dictionary<T, double>();
            var prevVertex = new Dictionary<T, T>();
            foreach (var v in _edgesWithCost.Keys)
            {
                minCostToReachVertex[v] = infiniteCost;
                minDistance.Enqueue(v, Equals(v, start) ? 0 : infiniteCost);
                unvisitedVertices.Add(v);
            }
            minCostToReachVertex[start] = 0;
            while (minDistance.Count != 0)
            {
                var nearestNeighbour = minDistance.Dequeue();
                unvisitedVertices.Remove(nearestNeighbour);
                var bestCostToNearestNeighbour = minCostToReachVertex[nearestNeighbour];
                if (bestCostToNearestNeighbour == infiniteCost)
                {
                    return null;
                }
                if (Equals(nearestNeighbour, end))
                {
                    break;
                }
                foreach (var childOfNearestNeighbour in unvisitedVertices.Intersect(Children(nearestNeighbour)))
                {
                    var newCostIfUsingNearestNeighbour = bestCostToNearestNeighbour + _edgesWithCost[nearestNeighbour][childOfNearestNeighbour];
                    var previousCostWithoutNearestNeighbour = minCostToReachVertex[childOfNearestNeighbour];
                    if ((previousCostWithoutNearestNeighbour == infiniteCost) //there was no known path from 'start' to 'end' before using 'nearestNeighbour'
                         || (newCostIfUsingNearestNeighbour < previousCostWithoutNearestNeighbour) //less expensive path
                        )
                    {
                        minDistance.UpdatePriority(childOfNearestNeighbour, previousCostWithoutNearestNeighbour, newCostIfUsingNearestNeighbour);
                        minCostToReachVertex[childOfNearestNeighbour] = newCostIfUsingNearestNeighbour;
                        prevVertex[childOfNearestNeighbour] = nearestNeighbour;
                    }
                }
            }
            return ExtractPath(start, end, prevVertex);
        }
        #endregion

        #region Bellman-Ford Algorhitm: find all shortest path starting from 'start' in o (V E) time
        //Works for negative weight
        //Doesn't work if negative cycles (returns null if graph contains negative cycles)
        public List<T> BestPathBellmanFord(T start, T end, bool minimizeCost)
        {
            var infiniteCost = minimizeCost ? double.MaxValue : double.MinValue;
            var prevVertex = new Dictionary<T, T>();
            var bestDistanceFromStart = new Dictionary<T, double>();
            var allVertices = Vertices;
            foreach (var v in allVertices)
            {
                bestDistanceFromStart[v] = infiniteCost;
            }
            bestDistanceFromStart[start] = 0;

            //a shortest path (without cycle) between any vertices 'u' & 'v' takes at most 'V' nodes and 'V-1' edges
            //we'll make 'V-1' steps and, at each step:
            //  for each existing edge (u,v)
            //      we'll use this edge (u,v) to improve the best path between 'start' & 'v'
            for (int i = 0; i < allVertices.Count; ++i) // 'V-1' steps
            {
                foreach (var e in _edgesWithCost)
                {
                    var u = e.Key;
                    foreach (var targetWithCost in e.Value)
                    {
                        var v = targetWithCost.Key;
                        if (bestDistanceFromStart[u] == infiniteCost)
                        {
                            continue;
                        }
                        var newCost = bestDistanceFromStart[u] + targetWithCost.Value;
                        if ((bestDistanceFromStart[v] == infiniteCost)
                            || (minimizeCost && (newCost < bestDistanceFromStart[v]))
                            || (!minimizeCost && (newCost > bestDistanceFromStart[v]))
                            )
                        {
                            bestDistanceFromStart[v] = newCost;
                            prevVertex[v] = u;
                        }
                    }
                }
            }

            //Negative-weight cycle detection:
            //With the 'V-1' steps above:
            //  we have computed (in 'bestDistanceFromStart') all best path from 'start' using at most 'V-1' edges
            //We'll make one more step: 
            //  if this step can improve any best path, it means that the new best path is using a negative cycle
            foreach (var e in _edgesWithCost)
            {
                var u = e.Key;
                foreach (var targetWithCost in e.Value)
                {
                    var v = targetWithCost.Key;
                    if ((bestDistanceFromStart[u] == infiniteCost) || (bestDistanceFromStart[v] == infiniteCost))
                    {
                        continue;
                    }
                    var newCost = bestDistanceFromStart[u] + targetWithCost.Value;
                    //if distance between 'u' & 'v' can be improved 
                    if ((minimizeCost && (newCost < bestDistanceFromStart[v]))
                    //||(!minimizeCost && (newCost > vertexToCost[v]))
                    )
                    {
                        return null; //Graph contains a negative-weight cycle
                    }
                }
            }
            return ExtractPath(start, end, prevVertex);
        }
        //find the sortest path between 'start' and 'end' using at most 'maxDepth' edges in o(maxDepth * E) time 
        public double BestPathBellmanFordWithMaxDepth(T start, T end, int maxDepth)
        {
            //bestDistanceFromStart[v][d] : shortest path from 'start' to 'v' using at most 'd' edges  
            var bestDistanceFromStart = new Dictionary<T, double[]>();
            foreach (var v in Vertices)
            {
                bestDistanceFromStart[v] = Enumerable.Repeat(Equals(v, start) ? 0 : double.MaxValue, 1 + maxDepth).ToArray();
            }
            for (int depth = 1; depth <= maxDepth; ++depth)
            {
                foreach (var e in _edgesWithCost)
                {
                    var edgeStart = e.Key;
                    var shortestPathToEdgeStartAtPreviousDepth = bestDistanceFromStart[edgeStart][depth - 1];
                    if (shortestPathToEdgeStartAtPreviousDepth == double.MaxValue)
                    {
                        continue;
                    }
                    foreach (var edgeEndWithCost in e.Value)
                    {
                        var edgeEnd = edgeEndWithCost.Key;
                        var edgeCost = edgeEndWithCost.Value;
                        //we check if we can improve the path 'start' => 'edgeEnd' using the current edge 'e'
                        bestDistanceFromStart[edgeEnd][depth] = Math.Min(bestDistanceFromStart[edgeEnd][depth], shortestPathToEdgeStartAtPreviousDepth + edgeCost);
                    }
                }
            }
            return (bestDistanceFromStart[end][maxDepth] == double.MaxValue) ? -1 : bestDistanceFromStart[end][maxDepth];
        }
        #endregion

        #region Floyd Warshall Algorithm: find all shortest path between any vertex in o (V^3) time
        //Works for negative weight
        //Doesn't work if negative cycles (returns null if graph contains negative cycles)
        public Tuple<double, List<T>> BestPathFloydWarshall(T start, T end, bool minimizeCost)
        {
            IDictionary<T, IDictionary<T, T>> pathToNext;
            IDictionary<T, IDictionary<T, double>> costResult = FloydWarshallAlgorithm(minimizeCost, out pathToNext);

            if (costResult == null || (!pathToNext.ContainsKey(start)) || (!pathToNext[start].ContainsKey(end)))
            {
                return null;
            }
            var bestPath = ExtractBestPathFloydWarshallAlgorithm(start, end, pathToNext);
            if (bestPath == null)
            {
                return null;
            }
            var bestCost = CostOf(bestPath);
            return Tuple.Create(bestCost, bestPath);
        }
        // minimizeCost:
        //      true if we are looking for the less expensive path from any of vertices
        //      false if we are looking for the most expensive path from any pair of vertices
        // return value:
        //      a result matrix where costResult[start][end] is the optimal cost from 'start' to 'end', or infiniteCost if there are no such paths
        private IDictionary<T, IDictionary<T, double>> FloydWarshallAlgorithm(bool minimizeCost, out IDictionary<T, IDictionary<T, T>> pathToNext)
        {
            var infiniteCost = minimizeCost ? double.MaxValue : double.MinValue;
            var costResult = new Dictionary<T, IDictionary<T, double>>();
            pathToNext = new Dictionary<T, IDictionary<T, T>>();
            var allVertices = Vertices.ToList();
            foreach (var u in allVertices)
            {
                costResult[u] = new Dictionary<T, double>();
                pathToNext[u] = new Dictionary<T, T>();
                foreach (var v in allVertices)
                {
                    costResult[u][v] = infiniteCost;
                }
                costResult[u][u] = 0;
            }
            foreach (var e in _edgesWithCost) //We add all existing edge
            {
                var u = e.Key;
                foreach (var toWithCost in e.Value)
                {
                    var v = toWithCost.Key;
                    costResult[u][v] = toWithCost.Value;
                    pathToNext[u][v] = v;
                }
            }

            foreach (var i in allVertices) // intermediate vertex
            {
                foreach (var u in allVertices)
                {
                    foreach (var v in allVertices)
                    {
                        //costResult[u][v] is the minimal cost from 'u' to 'v' using only vertices between 0 and 'i-1';
                        //we check if using vertex 'i' can improve the result
                        double uiCost = costResult[u][i];
                        if (uiCost == infiniteCost) // no path from 'u' to 'i' at this point
                        {
                            continue;
                        }
                        double ivCost = costResult[i][v];
                        if (ivCost == infiniteCost) // no path from 'i' to 'v' at this point
                        {
                            continue;
                        }
                        var uivCost = uiCost + ivCost;
                        var uvCost = costResult[u][v];
                        if ((uvCost == infiniteCost) //there was no known path from 'u' to 'v' before using vertex 'i'
                             || (minimizeCost && (uivCost < uvCost)) //less expensive path
                             || ((!minimizeCost) && (uivCost > uvCost)) //most expensive path
                            )
                        {
                            costResult[u][v] = uivCost;
                            pathToNext[u][v] = pathToNext[u][i];
                        }
                    }
                    if (costResult[u][u] < 0) //negative cycle detected
                    {
                        pathToNext = null;
                        return null;
                    }
                }
            }
            return costResult;
        }
        private List<T> ExtractBestPathFloydWarshallAlgorithm(T start, T end, IDictionary<T, IDictionary<T, T>> pathToNext)
        {
            var path = new List<T> { start };
            if (Equals(start, end))
            {
                return path;
            }
            if (pathToNext == null || !pathToNext.ContainsKey(start) || !pathToNext[start].ContainsKey(end))
            {
                return null;
            }
            while (!Equals(start, end))
            {
                start = pathToNext[start][end];
                path.Add(start);
            }
            return path;
        }
        #endregion


        #region 2-SAT

        /// <summary>
        /// Solve 2SAT Problem in o(E+V) Time with clauses as input
        /// </summary>
        /// <param name="clauses">
        ///     List of clauses: all of them must be satisfied at the same time
        ///     Each element of the list (a Tuple) if a disjunction:
        ///         at least one of the 2 constraints (Tuple.Item1 or Tuple.Item2) must be satisfied 
        /// </param>
        /// <param name="Complement">compute the complement of a constraint a => ~a , ~a => a</param>
        /// <param name="AreValidAtSameTime">true if the 2 constraints can be valid at same time
        /// Those constraints will always come from the original clauses list
        /// Only needed if 'distinctConstraintsAreAlwaysValidAtSameTime' is false
        /// </param>
        /// <param name="distinctConstraintsAreAlwaysValidAtSameTime">
        ///  true if 2 distinct constraints are always valid at same time
        ///  so only 'a' and '~a' are not valid at same time, all other combinations of constraints are valid
        /// </param>
        /// <returns>
        /// For each clause, the constraint (Tuple.Item1 or Tuple.Item2) to satisfy the 2SAT problem
        /// null if it is not possible to satisfy all clauses at the same time
        /// </returns>
        public static List<T> Solve2SAT(List<Tuple<T, T>> clauses, Func<T, T> Complement, Func<T, T, bool> AreValidAtSameTime, bool distinctConstraintsAreAlwaysValidAtSameTime)
        {
            //We construct the implication graph
            var implicationGraph = new Graph<T>(true);
            foreach (var disjunction in clauses)
            {
                //a disjunction (a or b) is equivalent to ( (~a => b) and (~b => a) )
                implicationGraph.Add(Complement(disjunction.Item1), disjunction.Item2, 1);
                implicationGraph.Add(Complement(disjunction.Item2), disjunction.Item1, 1);
            }
            if (!distinctConstraintsAreAlwaysValidAtSameTime)
            { 
                for (int first = 0; first < clauses.Count; ++first)
                {
                    var constraint1 = clauses[first].Item1;
                    var constraint2 = clauses[first].Item2;
                    for (int other = first + 1; other < clauses.Count; ++other)
                    {
                        var otherConstraint1 = clauses[other].Item1;
                        var otherConstraint2 = clauses[other].Item2;
                        if (!AreValidAtSameTime(clauses[first].Item1, (clauses[other].Item1)))
                        {
                            implicationGraph.Add(constraint1, Complement(otherConstraint1), 1);
                            implicationGraph.Add(otherConstraint1, Complement(constraint1), 1);
                        }
                        if (!AreValidAtSameTime(clauses[first].Item1, (clauses[other].Item2)))
                        {
                            implicationGraph.Add(constraint1, Complement(otherConstraint2), 1);
                            implicationGraph.Add(otherConstraint2, Complement(constraint1), 1);
                        }
                        if (!AreValidAtSameTime(clauses[first].Item2, (clauses[other].Item1)))
                        {
                            implicationGraph.Add(constraint2, Complement(otherConstraint1), 1);
                            implicationGraph.Add(otherConstraint1, Complement(constraint2), 1);
                        }
                        if (!AreValidAtSameTime(clauses[first].Item2, (clauses[other].Item2)))
                        {
                            implicationGraph.Add(constraint2, Complement(otherConstraint2), 1);
                            implicationGraph.Add(otherConstraint2, Complement(constraint2), 1);
                        }
                    }
                }
            }

            var sccInTopologicalOrder = implicationGraph.ExtractStronglyConnectedComponents();
            var constraintsThatMustBeSatisfied = new HashSet<T>();
            var constraintToSccId = new Dictionary<T, int>(); //'constraint' to associated 'strongly connected component id'
            for (var sccId = 0; sccId < sccInTopologicalOrder.Count; sccId++)
            {
                var component = sccInTopologicalOrder[sccId];
                foreach (var constraintId in component)
                {
                    var constraintIdComplement = Complement(constraintId);
                    if (constraintToSccId.TryGetValue(constraintIdComplement, out var sccIdOfConstraintComplement))
                    {
                        if (sccIdOfConstraintComplement == sccId)
                        {
                            return null; //both the constraint and its complement are in the same SCC : no solution
                        }
                        constraintsThatMustBeSatisfied.Add(sccIdOfConstraintComplement <= sccId ? constraintId : constraintIdComplement);
                    }
                    constraintToSccId[constraintId] = sccId;
                }
            }
            var solutions = new List<T>();
            foreach (var clause in clauses)
            {
                if (constraintsThatMustBeSatisfied.Contains(clause.Item1) || constraintsThatMustBeSatisfied.Contains(Complement(clause.Item2)))
                {
                    solutions.Add(clause.Item1);
                }
                else
                {
                    solutions.Add(clause.Item2);
                }
            }
            return solutions;
        }

        #endregion

        #region Strongly connected components
        /// <summary>
        /// Strongly connected component Detection in o(|V| + |E|)   ( => o(n) for tree ,  o(n^2) for fully connected graph)
        /// Strongly connected component: Largest sub graph where for any 2 vertices 'u' & 'v' of this sub graph, there is a path from 'u' to 'v' and a path from 'v' to 'u'
        /// </summary>
        /// <returns>
        /// List of Strongly connected component in Topological order
        /// </returns>
        public List<List<T>> ExtractStronglyConnectedComponents()
        {
            var stronglyConnectedComponents = new List<List<T>>();
            var dfsIndex = new Dictionary<T, int>();
            var lowLinkDfsIndex = new Dictionary<T, int>();
            var allVertices = Vertices;
            foreach (var v in allVertices)
            {
                dfsIndex[v] = lowLinkDfsIndex[v] = -1;
            }
            var nextDfsIndex = 0;
            var verticesInStack = new Stack<T>();
            foreach (var v in allVertices)
            {
                if (dfsIndex[v] == -1)
                {
                    StronglyConnectedComponent(v, stronglyConnectedComponents, dfsIndex, lowLinkDfsIndex, verticesInStack, ref nextDfsIndex);
                }
            }
            //the SCC are currently in inverse Topological order: we need to invert the order to make them in topological order
            stronglyConnectedComponents.Reverse();
            return stronglyConnectedComponents;
        }
        private void StronglyConnectedComponent(T v, List<List<T>> stronglyConnectedComponents, IDictionary<T, int> dfsIndex, IDictionary<T, int> sccId, Stack<T> stack, ref int nextDfsIndex)
        {
            Debug.Assert(dfsIndex[v] == -1);
            dfsIndex[v] = nextDfsIndex;
            //the sccId associated with 'v' will remain at 'nextDfsIndex' if 'v' is the root of a scc (or if it is a single point scc)
            //it will change to a lower sccId if 'v' belongs to a scc associated with a vertex already meet previously during the dfs
            sccId[v] = nextDfsIndex;
            ++nextDfsIndex;
            stack.Push(v);

            foreach (var w in Children(v))
            {
                if (dfsIndex[w] == -1)
                {
                    //Child 'w' has not yet been visited; recurse on it
                    StronglyConnectedComponent(w, stronglyConnectedComponents, dfsIndex, sccId, stack, ref nextDfsIndex);
                    sccId[v] = Math.Min(sccId[v], sccId[w]);
                    continue;
                }
                if (stack.Contains(w)) //Child 'w' is in stack and hence in the current SCC
                {
                    sccId[v] = Math.Min(sccId[v], dfsIndex[w]);
                }
            }
            // If 'v' is a root vertex, pop the stack and generate a new SCC
            if (dfsIndex[v] == sccId[v])
            {
                var newStronglyConnectedComponent = new List<T>();
                for (; ; )
                {
                    var index = stack.Pop();
                    newStronglyConnectedComponent.Add(index);
                    if (Equals(index, v))
                    {
                        break;
                    }
                }
                stronglyConnectedComponents.Add(newStronglyConnectedComponent);
            }
        }
        #endregion

        #region Cycles detection: Find all cycles in  'o(|V| + [E|) x #Cycles' using Johnson algo
        public List<List<T>> AllCycles()
        {
            var allCycles = new List<List<T>>();
            foreach (var scc in ExtractStronglyConnectedComponents())
            {
                var areAllowed = new HashSet<T>(scc);
                var allCyclesFromStronglyConnectedComponent = new List<List<T>>();
                foreach (var vertex in scc)
                {
                    AllCyclesFromStronglyConnectedComponent(vertex, new List<T>(), new HashSet<T>(), new Dictionary<T, HashSet<T>>(), allCyclesFromStronglyConnectedComponent, areAllowed);
                    areAllowed.Remove(vertex);
                }
                allCycles.AddRange(allCyclesFromStronglyConnectedComponent);
            }
            return allCycles;
        }
        private bool AllCyclesFromStronglyConnectedComponent(T currentVertex, IList<T> path, HashSet<T> blocked, Dictionary<T, HashSet<T>> blockedMap, List<List<T>> allCycles, HashSet<T> areAllowed)
        {
            path.Add(currentVertex);
            var foundCycles = false;
            foreach (var child in Children(currentVertex))
            {
                if (!areAllowed.Contains(child))
                {
                    continue;
                }
                if (Equals(child, path[0]))
                {
                    allCycles.Add(new List<T>(path));
                    foundCycles = true;
                    continue;
                }
                if (!blocked.Contains(child))
                {
                    blocked.Add(child);
                    foundCycles = foundCycles || AllCyclesFromStronglyConnectedComponent(child, path, blocked, blockedMap, allCycles, areAllowed);
                }
            }

            if (foundCycles)
            {
                Unblock(currentVertex, blocked, blockedMap);
            }
            else
            {
                //if no cycle is found with current vertex then don't unblock it. But find all its neighbors and add this
                //vertex to their blockedMap. If any of those neighbors ever get unblocked then unblock current vertex as well.
                foreach (var w in Children(currentVertex))
                {
                    if (!areAllowed.Contains(w))
                    {
                        continue;
                    }
                    if (!blockedMap.ContainsKey(w))
                    {
                        blockedMap.Add(w, new HashSet<T>());
                    }
                    blockedMap[w].Add(currentVertex);
                }
            }
            path.RemoveAt(path.Count - 1);
            return foundCycles;
        }
        private static void Unblock(T u, HashSet<T> blocked, Dictionary<T, HashSet<T>> blockedMap)
        {
            blocked.Remove(u);
            if (!blockedMap.ContainsKey(u))
            {
                return;
            }
            foreach (var v in blockedMap[u].Where(blocked.Contains))
            {
                Unblock(v, blocked, blockedMap);
            }
            blockedMap.Remove(u);
        }
        #endregion


        #region Find cycle with minimum average weigth in o (V E) time using Karp alog
        public Tuple<List<T>, double> CycleWithMinimumAverageWeight()
        {
            Tuple<List<T>, double> bestSoFar = null;
            foreach (var scc in ExtractStronglyConnectedComponents())
            {
                var res = CycleWithMinimumAverageWeight(scc[0], scc);
                if (res != null && (bestSoFar==null || res.Item2<bestSoFar.Item2))
                {
                    bestSoFar = res;
                }
            }
            return bestSoFar;
        }
        /// <summary>
        /// Finding the cycle with minimum average weight, given a root node 'start'
        /// </summary>
        /// <param name="start"></param>
        /// <param name="allowedVertices">
        /// null if we can use all vertices of the graph
        /// </param>
        /// <returns>
        /// null if no cycle exist in the graph
        /// Tuple.Item1 = cycle with minimum average weight
        /// Tuple.Item2 = minimum average weight for a cycle in the graph
        /// </returns>
        public Tuple<List<T>,double> CycleWithMinimumAverageWeight(T start, List<T> allowedVertices = null)
        {
            allowedVertices = allowedVertices ?? Vertices.ToList();
            if (allowedVertices.Count <= 1)
            {
                return null;
            }
            int n = VerticeCount;
            var shortestPath_from_start_to_v_using_X_Edges = new Dictionary<T, List<double>>();
            var prec = new Dictionary<int, IDictionary<T, T>>();
            //we compute the minimum distance for #edges = 0
            foreach (var v in allowedVertices)
            {
                shortestPath_from_start_to_v_using_X_Edges[v] =  new List<double> { Equals(v, start) ? 0 : double.MaxValue };
            }
            for(int edgeCount = 1;edgeCount<=n;++edgeCount)
            {
                prec[edgeCount] = new Dictionary<T, T>();
                foreach (var v in allowedVertices)
                {
                    shortestPath_from_start_to_v_using_X_Edges[v].Add(double.MaxValue);
                }
                foreach (var node in allowedVertices)
                {
                    var curForU = shortestPath_from_start_to_v_using_X_Edges[node];
                    var cost_start_to_u_PrevEdge = curForU[edgeCount - 1];
                    foreach (var neighbor in Children(node))
                    {
                        var shortedPath_neighbor = shortestPath_from_start_to_v_using_X_Edges[neighbor];
                        var newCost = cost_start_to_u_PrevEdge + _edgesWithCost[node][neighbor];
                        if (newCost < shortedPath_neighbor[edgeCount])
                        {
                            prec[edgeCount][neighbor] = node;
                            shortedPath_neighbor[edgeCount] = newCost;
                        }
                    }
                }

            }

            double minimumCycleMean = double.MaxValue;
            var argmin_node = default(T);
            int argmin_k = -1;
            foreach (var node in allowedVertices)
            {
                double valMax_for_node = Double.MinValue;
                int argmax = -1;
                List<double> shortestPathByDepth = shortestPath_from_start_to_v_using_X_Edges[node];
                for (int k = 0; k < n; ++k)
                {
                    if (shortestPathByDepth[n] == double.MaxValue || shortestPathByDepth[k] == double.MaxValue)
                    {
                        continue;
                    }
                    double alt = (shortestPathByDepth[n] - shortestPathByDepth[k]) / (n - k);
                    if (alt >= valMax_for_node)
                    {
                        valMax_for_node = alt;
                        argmax = k;
                    }
                }

                if (argmax != -1 && valMax_for_node!=double.MinValue && valMax_for_node < minimumCycleMean)
                {
                    //var cycleStart = node;
                    //for (int l = n; l > argmax; --l) {cycleStart = prec[l][cycleStart];}
                    //if (!Equals(node,cycleStart)) continue;
                    minimumCycleMean = valMax_for_node;
                    argmin_node = node;
                    argmin_k = argmax;
                }
            }


            //uncomment if we just want to compute the mean of the cycle
            //return minimumCycleMean;

            if (minimumCycleMean == double.MaxValue)
            {
                return null;
            }
            var path = new List<T>();
            for (int l = n; l > argmin_k; --l)
            {
                path.Add(argmin_node);
                argmin_node = prec[l][argmin_node];
            }

            path.Reverse();
            return Tuple.Create(path, minimumCycleMean);
        }
        #endregion



        #region Forrest detection
        public IList<HashSet<T>> AllForrests()
        {
            var result = new List<HashSet<T>>();
            var childrenToParent = ChildrenToParents();
            foreach (var v in Vertices)
            {
                if (IndexOf(result, v) == -1)
                {
                    result.Add(ForrestContaining(v, childrenToParent));
                }
            }
            return result;
        }
        private HashSet<T> ForrestContaining(T i, IDictionary<T, HashSet<T>> childrenToParent)
        {
            var result = new HashSet<T> { i };
            for (; ; )
            {
                int countBeforeAdding = result.Count;
                result.UnionWith(Children(result));
                result.UnionWith(AllParents(result, childrenToParent));
                if (countBeforeAdding == result.Count)
                {
                    return result;
                }
            }
        }
        private static int IndexOf(IList<HashSet<T>> d, T i)
        {
            for (int index = 0; index < d.Count; ++index)
            {
                if (d[index].Contains(i))
                {
                    return index;
                }
            }
            return -1;

        }
        #endregion

        #region Minimum Spanning Tree in o(E log(E)) time (= E log(V) time because E <= V^2)
        public Graph<T> MinimumSpanningTree()
        {
            var sortedEdges = new List<Tuple<T, T, double>>();
            foreach (var e in _edgesWithCost)
            {
                foreach (var cost in e.Value)
                {
                    sortedEdges.Add(Tuple.Create(e.Key, cost.Key, cost.Value));
                }
            }

            sortedEdges.Sort((x, y) => x.Item3.CompareTo(y.Item3));
            var g = new Graph<T>(false);
            var uf = new UnionFind<T>();
            foreach (var s in sortedEdges)
            {
                if (uf.Union(s.Item1, s.Item2))
                {
                    g.Add(s.Item1, s.Item2, s.Item3);
                }
            }
            return g;
        }
        #endregion


        #region Topological Sort
        /// <summary>
        /// return a topological sort of the graph in o(V) time
        /// The graph must be a DAG (directed , no cycles)
        /// </summary>
        /// <returns>list of vertices, in topological order</returns>
        public List<T> TopolologicalSort()
        {
            var parentCount = Vertices.ToDictionary(x => x, x => 0);
            foreach (var v in Vertices)
            {
                foreach (var c in Children(v))
                {
                    ++parentCount[c];
                }
            }

            var result = new List<T>();
            var Q = parentCount.Where(t => t.Value == 0).Select(t => t.Key).ToList();
            while (Q.Count != 0)
            {
                var v = Q[Q.Count - 1];
                Q.RemoveAt(Q.Count-1);
                result.Add(v);
                foreach(var c in Children(v))
                { 
                    parentCount[c] -= 1;
                    if (parentCount[c] == 0)
                    {
                        Q.Add(c);
                    }
                }
            }
            return result;
        }

        #endregion

        #region Hamiltonian Path (path using all vertices exactly once) & Hamiltonian Circuit (must return to start vertex) in o(V^2 2^V) time
        //https://en.wikipedia.org/wiki/Held%E2%80%93Karp_algorithm
        //Solve the Travelling salesman problem using Held–Karp algorithm in o(V^2 2^V) time (and o(V 2^V) memory)
        //  The goal is to find an Hamiltonian Circuit: 
        //  a path starting and ending at the same vertex and using each vertex exactly once
        public IList<T> TravellingSalesmanProblem(T start)
        {
            if (!Vertices.Contains(start) || Vertices.Count > 30)
            {
                return null;
            }
            var verticesWithoutStart = Vertices.ToList();
            verticesWithoutStart.Remove(start);
            int maxMask = (1 << verticesWithoutStart.Count) - 1;

            //minCost[end][S] = minimum cost to go from 'start' to 'endIndex' visiting exactly once each vertex in 'S'
            //minCost[end][end] = minimum cost to go from 'start' to 'end' visiting only 'end' = distance from 'start' to 'end'
            var minCost = new double[verticesWithoutStart.Count, 1 + maxMask];
            minCost.SetAll(double.MaxValue);

            //we compute 'minCost'
            for (int sizeSet = 1; sizeSet <= verticesWithoutStart.Count; ++sizeSet)
                for (int endIndex = 0; endIndex < verticesWithoutStart.Count; ++endIndex)
                {
                    TravellingSalesmanProblem_Helper(start, endIndex, minCost, verticesWithoutStart, 0, sizeSet, 0, new List<int>());
                }

            //we build the hamiltonian circuit
            var indexResult = new List<int>();
            int allowedVertices = maxMask;
            while (allowedVertices != 0)
            {
                var previous = indexResult.Count == 0 ? start : verticesWithoutStart[indexResult[0]];
                int currentMinIndex = -1;
                double currentMin = double.MaxValue;
                for (int i = 0; i < verticesWithoutStart.Count; ++i)
                {
                    double lastEdgeCost;
                    if ((minCost[i,allowedVertices] != double.MaxValue) && TryEdgeCost(verticesWithoutStart[i], previous, out lastEdgeCost) && ((minCost[i,allowedVertices] + lastEdgeCost) < currentMin))
                    {
                        currentMinIndex = i;
                        currentMin = minCost[i,allowedVertices] + lastEdgeCost;
                    }
                }
                if (currentMinIndex == -1)
                {
                    return null;
                }
                indexResult.Insert(0, currentMinIndex);
                allowedVertices -= 1 << currentMinIndex;
            }
            var result = indexResult.Select(i => verticesWithoutStart[i]).ToList();
            result.Insert(0, start);
            result.Add(start);
            return result;
        }
        private bool TryEdgeCost(T start, T end, out double edgeCost)
        {
            edgeCost = 0;
            IDictionary<T, double> tmp;
            return _edgesWithCost.TryGetValue(start, out tmp) && tmp.TryGetValue(end, out edgeCost);
        }
        private void TravellingSalesmanProblem_Helper(T start, int endIndex, double[,] minCost, List<T> verticesWithoutStart, int nextPossibleIndexInVerticesWithoutStart, int sizeSet, int currentMask, List<int> path)
        {
            if (path.Count >= sizeSet)
            {
                if (0 == (currentMask & (1 << endIndex)))
                {
                    return;
                }
                Debug.Assert(path.Contains(endIndex));
                double newEdgeCost;
                if (sizeSet == 1)
                {
                    Debug.Assert(path[0] == endIndex);
                    if (TryEdgeCost(start, verticesWithoutStart[endIndex], out newEdgeCost))
                    {
                        minCost[endIndex,currentMask] = newEdgeCost;
                    }
                    return;
                }
                var minimalCost = double.MaxValue;
                var currentMaskWithoutEndIndex = currentMask - (1 << endIndex);
                foreach (var x in path)
                {
                    if (x == endIndex)
                    {
                        continue;
                    }
                    var newCost = minCost[x,currentMaskWithoutEndIndex];
                    if ((newCost == double.MaxValue) || !TryEdgeCost(verticesWithoutStart[x], verticesWithoutStart[endIndex], out newEdgeCost))
                    {
                        continue;
                    }
                    var newMinCost = newCost + newEdgeCost;
                    if (newMinCost < minimalCost)
                    {
                        minimalCost = newMinCost;
                        minCost[endIndex,currentMask] = newMinCost;
                    }
                }
                return;
            }

            int maxRemainingVertices = verticesWithoutStart.Count - nextPossibleIndexInVerticesWithoutStart;
            if (path.Count + maxRemainingVertices < sizeSet)
            {
                return;
            }
            TravellingSalesmanProblem_Helper(start, endIndex, minCost, verticesWithoutStart, nextPossibleIndexInVerticesWithoutStart + 1, sizeSet, currentMask, path);
            path.Add(nextPossibleIndexInVerticesWithoutStart);
            TravellingSalesmanProblem_Helper(start, endIndex, minCost, verticesWithoutStart, nextPossibleIndexInVerticesWithoutStart + 1, sizeSet, currentMask + (1 << nextPossibleIndexInVerticesWithoutStart), path);
            path.RemoveAt(path.Count - 1);
        }

        //Solve the bitonic tour in o(V^2) CPU (and o(V) memory)
        //The bitonic tour is a special kind of 'Travelling salesman problem' starting and ending at index#0:
        // it must:
        //      start at first vertex (index#0)
        //      visit any vertices from the first to the last index in increasing order
        //      reach the last vertex (last index)
        //      then come back to the first vertex visiting all unvisited vertices (those not visited in the left to right trip)
        public static List<int> BitonicTour(double[][] edgeCosts)
        {
            int verticesCount = edgeCosts.Length;

            //cumDistance[i] : distance for going from every vertex between '0' and 'i'
            var cumDistance = new double[verticesCount];
            for (int i = 1; i < verticesCount; ++i)
            {
                cumDistance[i] = cumDistance[i - 1] + edgeCosts[i - 1][i];
            }

            //shortestBitonicTour[n] : length of shortest bitonic tour: n ~~> 0 ~~> n-1
            var shortestBitonicTour = new double[verticesCount];
            shortestBitonicTour[1] = edgeCosts[1][0];
            // bestK[n] : the first vertex encountered in the shortest bitonic tour: n ~~> 0 ~~> n-1
            var bestK = new int[verticesCount];

            for (int n = 2; n < shortestBitonicTour.Length; ++n)
            {
                //we compute the length of the shortest bitonic tour: n ~~~> 0 ~~~> n-1
                shortestBitonicTour[n] = double.MaxValue;
                for (int k = 0; k <= n - 2; ++k)
                {
                    //we compute the length of the shortest bitonic tour if we go from vertex 'n' directly to vertex 'k' : 
                    //      n>k ~~~> 0 ~~~> k+1>k+2>...>n-1
                    //    = n>k + shortestBitonicTour[k+1] +  k+1>k+2>...>n-1
                    var shortestBitonicTourIfGoingDirectlyFrom_n_to_k = edgeCosts[n][k] + shortestBitonicTour[k + 1] + cumDistance[n - 1] - cumDistance[k + 1];
                    if (shortestBitonicTourIfGoingDirectlyFrom_n_to_k < shortestBitonicTour[n])
                    {
                        shortestBitonicTour[n] = shortestBitonicTourIfGoingDirectlyFrom_n_to_k;
                        bestK[n] = k;
                    }
                }
            }

            //uncomment the following line if we just want to compute the cost of the bitonic tour
            //return shortestBitonicTour.Last() + edgeCosts[verticesCount - 2][verticesCount - 1];

            //we build the path related to the shortest bitonic tour
            var path = new List<int> { verticesCount - 1, bestK.Last() };
            while (path.Last() != 0)
            {
                int N = path.Last() + 1;
                int K = bestK[N];
                //in the bitonic tour N ~~~> 0 ~~~> N-1 , the first vertex we reach (the one just after 'N') is vertex 'K'
                //We are looking for the last vertices in this bitonic tour (those just before 'N-1' in the path 0 ~~~> N-1) to add them in the path
                for (int vertexIndex = N - 2; vertexIndex > K; --vertexIndex)
                {
                    path.Add(vertexIndex);
                }
                path.Add(bestK[K + 1]);
            }
            path.Reverse();
            var usedVertices = new HashSet<int>(path);
            for (int vertexIndex = verticesCount - 2; vertexIndex >= 0; --vertexIndex)
            {
                if (!usedVertices.Contains(vertexIndex))
                {
                    path.Add(vertexIndex);
                }
            }
            path.Add(0);
            return path;
        }
        #endregion


        #region Euler Path (path using all edges exactly once) & Euler Cycle (must also return to start vertex) in O(|E|+|V|) time 
        //if mustBeACycle = true
        //  returns a Euler Cycle starting&ending at 'start' in O(|E|+|V|) time (or null if it doesn't exists)
        //  for digraph:
        //      Works only if it is strongly connected and if for each vertex: Count(parent vertices) = Count(children vertices)
        //  for undirected graph:
        //      Works only if it is connected and if for each vertex:  Count(children vertices) is even
        //if mustBeACycle = false
        //  returns a Euler Path starting at 'start' in O(|E|+|V|) time (or null if it doesn't exists)
        //  for digraph:
        //      Works only if it is strongly connected and if for each vertex (except 0 or 2 vertices) :  Count(parent vertices) = Count(children vertices)
        //      For the 2 vertices where Count(parent vertices) <> Count(children vertices) we must have:
        //          one vertex with Count(parent vertices) = Count(children vertices)-1 => it is the mandatory starting vertex
        //          one vertex with Count(parent vertices)-1 = Count(children vertices) => it is the mandatory ending vertex
        //  for undirected graph:
        //      Works only if it is strongly connected and if for each vertex (except 0 or 2 vertices) :  Count(children vertices) is even
        public IList<T> EulerPath(T start, bool mustBeACycle)
        {
            if (!ValidDepartureForEulerPath(mustBeACycle).Contains(start))
            {
                return null;
            }
            var availableEdges = new Dictionary<T, List<T>>();
            foreach (var edge in _edgesWithCost)
            {
                var children = edge.Value.Keys.ToList(); //children of vertex 'edge.Key'
                children.Sort();
                children.Reverse();
                availableEdges[edge.Key] = children;
            }
            var eulerPath = new List<T>();
            var stack = new List<T> { start };
            while (stack.Count != 0)
            {
                List<T> targets;
                while (availableEdges.TryGetValue(stack.Last(), out targets) && targets.Count != 0)
                {
                    var endEdge = targets.Last();
                    targets.RemoveAt(targets.Count - 1);
                    if (!_isDirected && availableEdges.ContainsKey(endEdge))
                    {
                        availableEdges[endEdge].Remove(stack.Last());
                    }
                    stack.Add(endEdge);
                }
                eulerPath.Insert(0, stack.Last());
                stack.RemoveAt(stack.Count - 1);
            }
            return eulerPath;
        }
        public IList<T> EulerPath(bool mustBeACycle)
        {
            var validDeparture = ValidDepartureForEulerPath(mustBeACycle);
            return validDeparture.Count == 0 ? null : EulerPath(validDeparture.First(), mustBeACycle);
        }
        private List<T> ValidDepartureForEulerPath(bool mustBeACycle)
        {
            var allVertices = Vertices.ToList();
            if (_isDirected)
            {
                //directed graph
                //if for all vertices Count(parent vertices) = Count(children vertices)
                //      all vertices are valid departure squares, both for a euler cycle and a euler path (all euler path are euler cycles)
                //else if there are exactly 2 vertices where Count(parent vertices) != Count(children vertices) and with a difference of +1 and -1
                //      there are no Euler cycle , but there is 1! Euler path 
                //else
                //      there is no Euler path, no Euler Cycles
                var mandatoryStart = new List<T>();
                var mandatoryEnd = new List<T>();
                var childToParents = ChildrenToParents();
                foreach (var v in allVertices)
                {
                    int deltaOutEdgeMinusInEdge = Children(v).Count() - (childToParents.ContainsKey(v) ? childToParents[v].Count : 0);
                    if (Math.Abs(deltaOutEdgeMinusInEdge) >= 2)
                    {
                        return new List<T>(); //no Euler path or Euler Cycle can exists
                    }
                    if (deltaOutEdgeMinusInEdge == 1)
                    {
                        mandatoryStart.Add(v);
                    }
                    if (deltaOutEdgeMinusInEdge == -1)
                    {
                        mandatoryEnd.Add(v);
                    }
                }
                if ((mandatoryStart.Count == 0) && (mandatoryEnd.Count == 0))
                {
                    return new List<T>(allVertices); //all vertices are valid departure squares, both for a euler cycle and a euler path (all euler path are euler cycles)
                }
                if ((mandatoryStart.Count == 1) && (mandatoryEnd.Count == 1) && (!mustBeACycle))
                {
                    return mandatoryStart; //there is no Euler Cycle, but there is an Euler path starting at 'mandatoryStart[0]' and ending at 'mandatoryEnd[0]'
                }
                return new List<T>(); //no valid departure vertex
            }
            else
            {
                //undirected graph
                //if for all vertices Count(children vertices) is even
                //      all vertices are valid departure squares, both for a euler cycle and a euler path (all euler path are euler cycles)
                //if there are exactly 2 vertices where Count(children vertices) is odd
                //      there are no Euler cycle , but there are 2 Euler path 
                //else
                //      there is no Euler path&no Euler Cycles
                var mandatoryStart = new List<T>();
                foreach (var v in allVertices)
                {
                    if (Children(v).Count() % 2 != 0)
                    {
                        mandatoryStart.Add(v);
                    }
                }
                if (mandatoryStart.Count == 0)
                {
                    return new List<T>(allVertices); //Euler Cycle detected
                }
                mandatoryStart.Sort();
                if ((mandatoryStart.Count == 2) && (!mustBeACycle))
                {
                    return mandatoryStart; //there is no Euler Cycle, but there is 2 Euler paths starting at 'mandatoryStart[0]' and 'mandatoryStart[1]'
                }
                return new List<T>();
            }
        }
        #endregion

        //Solve Max Flow problem in a graph in o (V |E|^2) time using Edmond Karp Algorithm
        //Solve MinCut problem in a graph in o (V |E|^2) time using Edmond Karp Algorithm
        #region Max Flow & Min Cut Problem
        public double MaximumFlowUsingEdmondsKarp(T source, T destination, out Graph<T> maxFlowGraph)
        {
            maxFlowGraph = new Graph<T>(true); //initially no flow in the graph
            double maxFlowArrivingAtDestination = 0;
            for (; ; )
            {
                //we look for an augmenting path in the residual graph using BFS
                var residualGraph = GetResidualGraph(maxFlowGraph);
                var augmentingPath = residualGraph.ShortestPath_BFS(source, destination);
                if (augmentingPath == null)
                {
                    break;
                }
                var maxAddedFlowInShortedPath = double.MaxValue;
                for (int i = 1; i < augmentingPath.Count; ++i)
                {
                    maxAddedFlowInShortedPath = Math.Min(maxAddedFlowInShortedPath, residualGraph._edgesWithCost[augmentingPath[i - 1]][augmentingPath[i]]);
                }
                maxFlowArrivingAtDestination += maxAddedFlowInShortedPath;
                for (int i = 1; i < augmentingPath.Count; ++i)
                {
                    var start = augmentingPath[i - 1];
                    var end = augmentingPath[i];
                    if (!maxFlowGraph.HasEdge(start, end))
                    {
                        maxFlowGraph.Add(start, end, 0);
                    }
                    maxFlowGraph._edgesWithCost[start][end] += maxAddedFlowInShortedPath;
                    if (!maxFlowGraph.HasEdge(end, start))
                    {
                        maxFlowGraph.Add(end, start, 0);
                    }
                    maxFlowGraph._edgesWithCost[end][start] -= maxAddedFlowInShortedPath;
                }
            }

            return maxFlowArrivingAtDestination;
        }
        public bool HasEdge(T from, T to) { return _edgesWithCost.ContainsKey(from) && _edgesWithCost[from].ContainsKey(to); }
        private Graph<T> GetResidualGraph(Graph<T> flowGraph)
        {
            var residualGraph = new Graph<T>(true);
            foreach (var allCapacities in _edgesWithCost)
            {
                var start = allCapacities.Key;
                foreach (var endWithCapacity in allCapacities.Value)
                {
                    var end = endWithCapacity.Key;
                    var flow = flowGraph.HasEdge(start, end) ? flowGraph._edgesWithCost[start][end] : 0;
                    var capacity = endWithCapacity.Value;
                    if (flow < capacity)
                    {
                        residualGraph.Add(start, end, capacity - flow);
                    }
                    if (flow > 0)
                    {
                        residualGraph.Add(end, start, flow);
                    }
                }
            }
            return residualGraph;
        }


        //we want to disconnect path from 'source' to 'destination' (no problem to keep path from 'destination' to 'source')
        //by deleting some edges so that the total cost of deleted edges is minimal
        //if there is no path from source to destination : no edges need to be deleted
        //return the minimal cost of deleted edges, and put in 'deletedEdges' the list of edges to delete
        public double MinCut(T source, T destination, out List<KeyValuePair<T, T>> deletedEdges)
        {
            Graph<T> maxFlowGraph;
            double minimumCostOfEdgesToDeleteToRemovePathFromSourceToDestination = MaximumFlowUsingEdmondsKarp(source, destination, out maxFlowGraph);
            var verticesFromSourceCut = new HashSet<T>(GetResidualGraph(maxFlowGraph).AllReachable(source));
            deletedEdges = new List<KeyValuePair<T, T>>();
            foreach (var edge in _edgesWithCost)
                foreach (var dest in edge.Value)
                {
                    if (verticesFromSourceCut.Contains(edge.Key) && !verticesFromSourceCut.Contains(dest.Key))
                    {
                        deletedEdges.Add(new KeyValuePair<T, T>(edge.Key, dest.Key));
                    }
                }
            return minimumCostOfEdgesToDeleteToRemovePathFromSourceToDestination;
        }
        #endregion

        #region Maze
        public static Graph<Point> LoadMaze(char[,] maze)
        {
            var g = new Graph<Point>(false);
            foreach (var from in Utils.AllPoints(maze))
            {
                g._edgesWithCost.Add(from, new Dictionary<Point, double>());
                foreach (var to in Utils.AllPointsHorizontalVertical(maze, from))
                {
                    var cost = EdgeValue(maze[from.X,from.Y], maze[to.X,to.Y]);
                    if (!double.IsNaN(cost))
                    {
                        g._edgesWithCost[from][to] = cost;
                    }
                }
            }
            return g;
        }
        private static double EdgeValue(char start, char end)
        {
            if ((start == '#') || (end == '#'))
            {
                return double.NaN;
            }
            return 1.0;
        }
        #endregion

    }


    public static partial class Utils
    {
        #region Couplage et flots
        //we have a bipartite graph with 'U' sources, 'V' destinations and 'E' edges
        //we would like to maximize the number of edges connecting (distinct) sources to (distinct) destination
        //(each edge can NOT share a same source or a same destination)
        //It is not needed to have all sources and target connected
        //Time complexity is o( |V| |E| ) 
        public static IDictionary<U, V> MaxMatchingInBipartiteGraph<U, V>(IDictionary<U, IEnumerable<V>> bipartiteGraph)
        {
            var matchDestinationToSource = new Dictionary<V, U>();
            foreach (var source in new List<U>(bipartiteGraph.Keys)) //We try to add a match between 'source' and a destination vertex
            {
                ImproveMatchingInBipartiteGraph(source, bipartiteGraph, new HashSet<V>(), matchDestinationToSource);
            }
            return matchDestinationToSource.ToDictionary(x => x.Value, x => x.Key);
        }
        private static bool ImproveMatchingInBipartiteGraph<U, V>(U source, IDictionary<U, IEnumerable<V>> bipartiteGraph, HashSet<V> forbiddenDestination, IDictionary<V, U> matchDestinationToSource)
        {
            foreach (var destination in new List<V>(bipartiteGraph[source]))
            {
                if (  //destination is not forbidden
                    forbiddenDestination.Add(destination)
                    //'destination' has not been used yet or 'destination' was already used by a source vertex but we can re assign the source vertex to another better matching
                    && (!matchDestinationToSource.ContainsKey(destination) || ImproveMatchingInBipartiteGraph(matchDestinationToSource[destination], bipartiteGraph, forbiddenDestination, matchDestinationToSource)))
                {
                    matchDestinationToSource[destination] = source;
                    return true;
                }
            }
            return false;
        }

        //We have a bipartite graph 'V sources' and 'U destinations' (so with V*U edges)
        //each edge has an associated cost
        //we would like to find the perfect matching (connecting each source to a single distinct destination, so using 'V' edges)
        //maximizing the total value of the 'V' selected edges
        // time complexity is in o(V^3) 
        public static int Maximize_SumCost_ForPerfectMatching_InBipartiteGraph(int[,] bipartiteGraph, out int[] bestMatching)
        {
            var completeBipartiteGraph = bipartiteGraph;
            if (bipartiteGraph.GetLength(0) < bipartiteGraph.GetLength(1))
            {
                completeBipartiteGraph = bipartiteGraph.Resize(bipartiteGraph.GetLength(1), bipartiteGraph.GetLength(1), 0);
            }
            var result = Maximize_SumCost_ForPerfectMatchingsIn_CompleteBipartiteGraph(completeBipartiteGraph, out bestMatching);
            if (bipartiteGraph.GetLength(0) < completeBipartiteGraph.GetLength(0))
            {
                bestMatching = bestMatching.Take(bipartiteGraph.GetLength(0)).ToArray();
            }
            return result;
        }
        //Same as above but trying to minimize the total value of the 'V' selected edges
        public static int Minimize_SumCost_ForPerfectMatching_InBipartiteGraph(int[,] bipartiteGraph, out int[] bestMatching)
        {
            return -Maximize_SumCost_ForPerfectMatching_InBipartiteGraph(bipartiteGraph.Select(x => -x), out bestMatching);
        }

        //We have a bipartite graph 'V sources' x 'U destinations' (so with V*U edges)
        //we would like to find the perfect matching (connecting each source to a single distinct destination, so using 'V' edges)
        //maximizing the minimum value of the 'V' selected edges
        //returns this minimum value of the 'V' selected edges
        // time complexity is in o(V^3) 
        public static int Maximize_MinimumCost_ForPerfectMatching_InBipartiteGraph(int[,] bipartiteGraph, out int[] bestMatching)
        {
            var orderedDistinctCost = new HashSet<int>(bipartiteGraph.ToArray()).ToList();
            orderedDistinctCost.Sort();
            var min = orderedDistinctCost.First();
            var max = orderedDistinctCost.Last();
            const int invalidValue = -1000000;
            while (min < max)
            {
                var middle = (min + max + 1) / 2;
                var updateBipartiteGraph = bipartiteGraph.Select(x => x < middle ? invalidValue : x);
                Maximize_SumCost_ForPerfectMatching_InBipartiteGraph(updateBipartiteGraph, out bestMatching);
                bool isValid = true;
                for (int v = 0; v < bestMatching.Length; ++v)
                {
                    if (updateBipartiteGraph[v, bestMatching[v]] == invalidValue)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (isValid)
                {
                    min = middle;
                }
                else
                {
                    max = middle - 1;
                }
            }
            Maximize_SumCost_ForPerfectMatching_InBipartiteGraph(bipartiteGraph.Select(x => x < min ? invalidValue : x), out bestMatching);
            return min;
        }
        //same as above but trying to minimize the maximum value of the 'V' selected edges
        //returns this maximum value of the 'V' selected edges
        public static int Minimize_MaximumCost_ForPerfectMatching_InBipartiteGraph(int[,] bipartiteGraph,out int[] bestMatching)
        {
            return -Maximize_MinimumCost_ForPerfectMatching_InBipartiteGraph(bipartiteGraph.Select(x=>-x), out bestMatching);
        }

        //we have a complete bipartite graph 'V sources' x 'V destination' (so with V^2 edges)
        //each edge has an associated cost
        //we would like to find the perfect matching (connecting each source to a single distinct destination, so using 'V' edges)
        //maximizing the total value of the 'V' selected edges
        // time complexity is in o(V^3) 
        private static int Maximize_SumCost_ForPerfectMatchingsIn_CompleteBipartiteGraph(int[,] completeBipartiteGraph, out int[] bestMatching)
        {
            int n = completeBipartiteGraph.GetLength(0);
            var U = Enumerable.Range(0, n).ToList();
            var V = Enumerable.Range(0, n).ToList();
            bestMatching = Enumerable.Repeat(-1, n).ToArray();
            var mv = Enumerable.Repeat(-1, n).ToList();
            var lu = new List<int>();
            for(int row =0;row<completeBipartiteGraph.GetLength(0);++row)
            {
                lu.Add(completeBipartiteGraph.MaxInRow(row));
            }
            var lv = Enumerable.Repeat(0, n).ToList();
            foreach (var root in U)
            {
                var au = new bool[n];
                au[root] = true;
                var Av = Enumerable.Repeat(-1, n).ToList();
                var marge = new List<KeyValuePair<int, int>>();
                V.ForEach(vTmp => marge.Add(new KeyValuePair<int, int>(lu[root] + lv[vTmp] - completeBipartiteGraph[root,vTmp], root)));
                int v;
                for (; ; )
                {
                    v = -1;
                    foreach (var vTmp in V.Where(x => Av[x] == -1))
                    {
                        if ((v == -1) || (marge[vTmp].Key < marge[v].Key) || ((marge[vTmp].Key == marge[v].Key) && (marge[vTmp].Value < marge[v].Value)))
                        {
                            v = vTmp;
                        }
                    }
                    int delta = marge[v].Key;
                    int u = marge[v].Value;
                    if (delta > 0)
                    {
                        foreach (var uTmp in U.Where(uTmp => au[uTmp]))
                        {
                            lu[uTmp] -= delta;
                        }
                        foreach (var vTmp in V)
                        {
                            if (Av[vTmp] != -1)
                            {
                                lv[vTmp] += delta;
                            }
                            else
                            {
                                marge[vTmp] = new KeyValuePair<int, int>(marge[vTmp].Key - delta, marge[vTmp].Value);
                            }
                        }
                    }
                    Av[v] = u;
                    if (mv[v] == -1)
                    {
                        break;
                    }
                    var u1 = mv[v];
                    au[u1] = true;
                    foreach (var v1 in V)
                    {
                        if (Av[v1] == -1)
                        {
                            var alt = lu[u1] + lv[v1] - completeBipartiteGraph[u1,v1];
                            if (marge[v1].Key > alt)
                            {
                                marge[v1] = new KeyValuePair<int, int>(alt, u1);
                            }
                        }
                    }
                }
                while (v != -1)
                {
                    var u = Av[v];
                    var prev = bestMatching[u];
                    mv[v] = u;
                    bestMatching[u] = v;
                    v = prev;
                }
            }
            return lu.Sum() + lv.Sum();
        }
        #endregion


    }
}