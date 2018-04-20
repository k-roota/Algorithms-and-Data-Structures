using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class CyclesFinder : MarshalByRefObject
    {
        /// <summary>
        /// Sprawdza czy graf jest drzewem
        /// </summary>
        /// <param name="g">Graf</param>
        /// <returns>true jeśli graf jest drzewem</returns>
        public bool IsTree(Graph g)
        {
            if (g.Directed)
                throw new System.ArgumentException();
            int edgesCount = 0;
            Predicate<Edge> onNewEdge = delegate (Edge e)
            {
                if (e.From < e.To)
                    edgesCount++;
                return true;
            };
            g.GeneralSearchAll<EdgesStack>(null, null, onNewEdge, out int cc);
            return (edgesCount == g.VerticesCount - 1 && cc == 1);
        }

        private EdgesStack FindPath(Graph g, int from, int to)
        {
            EdgesStack edgesStack = new EdgesStack();
            Predicate<Edge> onNewEdge = delegate (Edge e)
            {
                if (edgesStack.Empty || e.To != edgesStack.Peek().To)
                {
                    edgesStack.Put(new Edge(e.To, e.From));
                }
                return true;
            };
            Predicate<int> onLeaving = delegate (int n)
            {
                if (!edgesStack.Empty)
                    edgesStack.Get();
                return true;
            };
            Predicate<int> onEntering = delegate (int n)
            {
                return n != to;
            };
            g.GeneralSearchFrom<EdgesStack>(from, onEntering, onLeaving, onNewEdge);

            return edgesStack;
        }

        private bool isSpanningTree(Graph g, Graph t)
        {
            if (!IsTree(t) || t.VerticesCount != g.VerticesCount)
                return false;
            for(int i=0; i<t.VerticesCount; i++)
            {
                foreach(Edge e in t.OutEdges(i))
                {
                    if (e.To > e.From && !t.GetEdgeWeight(e.From, e.To).IsNaN() && g.GetEdgeWeight(e.From, e.To).IsNaN())
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Wyznacza cykle fundamentalne grafu g względem drzewa t.
        /// Każdy cykl fundamentalny zawiera dokadnie jedną krawędź spoza t.
        /// </summary>
        /// <param name="g">Graf</param>
        /// <param name="t">Drzewo rozpinające grafu g</param>
        /// <returns>Tablica cykli fundamentalnych</returns>
        /// <remarks>W przypadku braku cykli zwracać pustą (0-elementową) tablicę, a nie null</remarks>
        public Edge[][] FindFundamentalCycles(Graph g, Graph t)
        {
            if (g.Directed || !isSpanningTree(g, t))
                throw new System.ArgumentException();                
            List<Edge[]> cycleList = new List<Edge[]>();
            EdgesStack edgesStack;
            List<Edge> tmpList = new List<Edge>();

            bool[] vChecked = new bool[g.VerticesCount];
            for(int i=0; i<g.VerticesCount; i++)
            {
                vChecked[i] = true;
                foreach (Edge e in g.OutEdges(i))
                {
                    if(!vChecked[e.To] && t.GetEdgeWeight(e.From, e.To).IsNaN())
                    {
                        edgesStack = FindPath(t, e.From, e.To);
                        edgesStack.Put(e);
                        cycleList.Add(edgesStack.ToArray());
                    }
                }
            }
            return cycleList.ToArray();
        }

        /// <summary>
        /// Dodaje 2 cykle fundamentalne
        /// </summary>
        /// <param name="c1">Pierwszy cykl</param>
        /// <param name="c2">Drugi cykl</param>
        /// <returns>null, jeśli wynikiem nie jest cykl i suma cykli, jeśli wynik jest cyklem</returns>
        public Edge[] AddFundamentalCycles(Edge[] c1, Edge[] c2)
        {
            int maxV = 0;
            foreach (Edge e in c1)
                if (e.To > maxV)
                    maxV = e.To;
            foreach (Edge e in c2)
                if (e.To > maxV)
                    maxV = e.To;
            Graph g = new AdjacencyListsGraph<SimpleAdjacencyList>(false, maxV + 1);
            foreach(Edge e in c1)
            {
                g.AddEdge(e);
            }
            foreach(Edge e in c2)
            {
                if (g.GetEdgeWeight(e.From, e.To).IsNaN())
                    g.AddEdge(e);
                else
                    g.DelEdge(e);
            }
            Edge[] result = null;
            bool moreEdges = false;
            bool[] visited = new bool[g.VerticesCount];
            List<Edge> edgesList = new List<Edge>();
            Predicate<Edge> onNewEdge = delegate (Edge e)
            {
                if (edgesList.Count > 0 && e.To == edgesList[0].From && e.From != edgesList[0].To)
                {
                    edgesList.Add(e);
                    result = edgesList.ToArray();
                }     
                else if (!visited[e.To])
                {
                    if(result == null)
                    {
                        edgesList.Add(e);
                    }
                    else
                    {
                        moreEdges = true;
                        return false;
                    }
                }
                return true;
            };
            Predicate<int> onEntering = delegate (int n)
            {
                visited[n] = true;
                return true;
            };
            g.GeneralSearchAll<EdgesStack>(onEntering, null, onNewEdge, out _);
            if (moreEdges || result == null)
                return null;
            else
                return result;
        }

    }

}
