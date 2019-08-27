// ReSharper disable UnusedMember.Global
using System;
using System.Collections.Generic;

namespace CSharpContestProject
{
    public static class Utils
    {
        public static void LocalPrint(string content)
        {
            Console.WriteLine(content);
        }
        public static void LocalPrintArray(IEnumerable<string> content)
        {
            Console.WriteLine(string.Join(Environment.NewLine, new List<string>(content).ToArray()));
        }


        

/*

        int[][] maze = Utils.CreateMatrix(str[0].Length, str.Count, 0);
            for (int y = 0; y<str.Count; ++y)
            {
                //str[y] = str[y].Replace(" ", "");
                int x = 0;
                foreach (var c in str[y])
                    maze[x++][y] = c;
            }
    */

/*
public List<T> AllReachableFrom(T start)
{
    //var visited = new Dictionary<T,int>();visited[start] = 0;
    var visited = new HashSet<T> {start};
    var toProcess = new Queue<T>();
    toProcess.Enqueue(start);
    while (toProcess.Count != 0)
    {
        var current = toProcess.Dequeue();
        foreach (var child in Children(current))
        {
            //if (visited.ContainsKey(child))
            if (visited.Contains(child)) 
                continue;
            //visited[child] = visited[current] + 1;
            visited.Add(child);
            toProcess.Enqueue(child);
        }
    }
    return visited.ToList();
}*/       

}
}
// ReSharper restore UnusedMember.Global


