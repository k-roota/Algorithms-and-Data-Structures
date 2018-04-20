using ASD.Graphs;
using System;

namespace ASD
{

    // Klasy Lab03Helper NIE WOLNO ZMIENIAĆ !!!
    public class Lab03Helper : System.MarshalByRefObject
    {
        public Graph SquareOfGraph(Graph graph) => graph.SquareOfGraph();
        public Graph LineGraph(Graph graph, out (int x, int y)[] names) => graph.LineGraph(out names);
        public int VertexColoring(Graph graph, out int[] colors) => graph.VertexColoring(out colors);
        public int StrongEdgeColoring(Graph graph, out Graph coloredGraph) => graph.StrongEdgeColoring(out coloredGraph);
    }

    // Uwagi do wszystkich metod
    // 1) Grafy wynikowe powinny być reprezentowane w taki sam sposób jak grafy będące parametrami
    // 2) Grafów będących parametrami nie wolno zmieniać
    static class Lab03
    {

        // 0.5 pkt
        // Funkcja zwracajaca kwadrat grafu graph.
        // Kwadratem grafu nazywamy graf o takim samym zbiorze wierzcholkow jak graf bazowy,
        // 2 wierzcholki polaczone sa krawedzia jesli w grafie bazowym byly polaczone krawedzia badz sciezka zlozona z 2 krawedzi
        public static Graph SquareOfGraph(this Graph graph)
        {
            Graph resultG = graph.Clone();
            for(int i=0; i<graph.VerticesCount; i++)
            {
                foreach(Edge e in graph.OutEdges(i))
                {
                    foreach(Edge e2 in graph.OutEdges(e.To))
                    {
                        if(e2.To != i)
                            resultG.AddEdge(i, e2.To);
                    }
                }
            }
            return resultG;
        }

        // 2 pkt
        // Funkcja zwracająca Graf krawedziowy grafu graph
        // Wierzcholki grafu krawedziwego odpowiadaja krawedziom grafu bazowego,
        // 2 wierzcholki grafu krawedziwego polaczone sa krawedzia
        // jesli w grafie bazowym z krawędzi odpowiadającej pierwszemu z nic można przejść 
        // na krawędź odpowiadającą drugiemu z nich przez wspólny wierzchołek.
        //
        // (w grafie skierowanym: 2 wierzcholki grafu krawedziwego polaczone sa krawedzia
        // jesli wierzcholek koncowy krawedzi odpowiadajacej pierwszemu z nich
        // jest wierzcholkiem poczatkowym krawedzi odpowiadajacej drugiemu z nich)
        //
        // do tablicy names nalezy wpisac numery wierzcholkow grafu krawedziowego,
        // np. dla wierzcholka powstalego z krawedzi <0,1> do tabeli zapisujemy krotke (0, 1) - przyda się w dalszych etapach
        //
        // UWAGA: Graf bazowy może być skierowany lub nieskierowany, graf krawędziowy zawsze jest nieskierowany.
        public static Graph LineGraph(this Graph graph, out (int x, int y)[] names)
        {
            // Moze warto stworzyc...
            // graf pomocniczy o takiej samej strukturze krawedzi co pierwotny, 
            // waga krawedzi jest numer krawedzi w grafie (taka sztuczka - to beda numery wierzcholkow w grafie krawedzowym)
            Graph helpG = graph.IsolatedVerticesGraph();
            EdgesQueue edgesQueue = new EdgesQueue();
            int lastNr = 0;
            bool[] wasChecked = new bool[graph.VerticesCount];
            for(int i=0; i<graph.VerticesCount; i++)
            {
                wasChecked[i] = true;
                foreach (Edge e in graph.OutEdges(i))
                {
                    if(graph.Directed == true || wasChecked[e.To] == false)
                    {
                        helpG.AddEdge(i, e.To, lastNr);
                        edgesQueue.Put(e);
                        lastNr++;
                    }
                }
            }
            names = new(int x, int y)[edgesQueue.Count];
            int count = 0;
            while (!edgesQueue.Empty)
            {
                Edge e = edgesQueue.Get();
                names[count] = (e.From, e.To);
                count++;
            }
            Graph resultG = graph.IsolatedVerticesGraph(false, lastNr);
            for(int i=0; i<graph.VerticesCount; i++)
            {
                foreach(Edge e in helpG.OutEdges(i))
                {
                    foreach(Edge e2 in helpG.OutEdges(e.To))
                    {
                        if(graph.Directed || e2.To != i)
                        {
                            resultG.AddEdge((int)e.Weight, (int)e2.Weight);
                        }
                    }
                }
            }
            return resultG;
        }

        // 1 pkt
        // Funkcja znajdujaca poprawne kolorowanie wierzcholkow grafu graph
        // Kolorowanie wierzcholkow jest poprawne, gdy kazde dwa sasiadujace wierzcholki maja rozne kolory
        // Funkcja ma szukać kolorowania wedlug nastepujacego algorytmu zachlannego:
        //
        // Dla wszystkich wierzcholkow (od 0 do n-1) 
        //      pokoloruj wierzcholek v na najmniejszy mozliwy kolor (czyli taki, na ktory nie sa pomalowani jego sasiedzi)
        //
        // Nalezy zwrocic liczbe kolorow, a w tablicy colors zapamietac kolory dla poszczegolnych wierzcholkow
        //
        // UWAGA: Dla grafów skierowanych metoda powinna zgłaszać wyjątek ArgumentException
        public static int VertexColoring(this Graph graph, out int[] colors)
        {
            if (graph.Directed)
                throw new ArgumentException();
            colors = new int[graph.VerticesCount];
            for(int i=0; i<graph.VerticesCount; i++)
            {
                colors[i] = graph.VerticesCount;
            }
            int colorCount = 0;
            for(int i=0; i<graph.VerticesCount; i++)
            {
                bool[] usedColors = new bool[graph.VerticesCount+1];
                foreach(Edge e in graph.OutEdges(i))
                {
                    usedColors[colors[e.To]] = true;
                }
                int color = 0;
                while(usedColors[color])
                {
                    color++;
                }
                colors[i] = color;
                if (color+1 > colorCount)
                    colorCount = color+1;
            }
            return colorCount;
        }

        // 0.5 pkt
        // Funkcja znajdujaca silne kolorowanie krawedzi grafu graph
        // Silne kolorowanie krawedzi grafu jest poprawne gdy kazde dwie krawedzie, ktore sa ze soba sasiednie
        // albo sa polaczone inna krawedzia, maja rozne kolory.
        //
        // Nalezy zwrocic nowy graf, ktory bedzie kopia zadanego grafu, ale w wagach krawedzi zostana zapisane znalezione kolory
        // 
        // Wskazowka - to bardzo proste. Nalezy tu wykorzystac wszystkie poprzednie funkcje. 
        // Zastanowic sie co mozemy powiedziec o kolorowaniu wierzcholkow kwadratu grafu krawedziowego - jak sie ma do silnego kolorowania krawedzi grafu bazowego
        public static int StrongEdgeColoring(this Graph graph, out Graph coloredGraph)
        {
            int colorCount = VertexColoring(SquareOfGraph(LineGraph(graph, out (int x, int y)[]names)), out int[] colors);
            coloredGraph = graph.Clone();
            for(int i=0; i<names.Length; i++)
            {
                coloredGraph.ModifyEdgeWeight(names[i].x, names[i].y, colors[i]);
            }
            return colorCount;
        }
    }
}
