using System.Collections.Generic;

public class Graph
{
    public List<List<int>> g = new();
    public Graph(List<List<int>> g)
    {
        this.g = Utility.DeepClone(g);
    }
}