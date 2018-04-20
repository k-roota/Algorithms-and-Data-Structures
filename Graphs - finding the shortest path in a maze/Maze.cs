using ASD.Graphs;
using System;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Maze : MarshalByRefObject
    {
        private string pathToString(Edge[] path, int dimH, int dimW)
        {
            StringBuilder stringBuilder = new StringBuilder(path.Length);
            int layerSize = dimH * dimW;
            foreach(Edge e in path)
            {
                int change = e.To % layerSize - e.From % layerSize;
                if(change == dimW)
                {
                    stringBuilder.Append('S');
                }
                else if(change == -dimW)
                {
                    stringBuilder.Append('N');
                }
                else if(change == 1)
                {
                    stringBuilder.Append('E');
                }
                else
                {
                    stringBuilder.Append('W');
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            int dimH = maze.GetLength(0);
            int dimW = maze.GetLength(1);
            Graph g = new AdjacencyListsGraph<AVLAdjacencyList>(true, dimH * dimW);
            int s = 0;
            int e = 0;
            for(int i=0; i<dimH; i++)
            {
                for(int j=0; j<dimW; j++)
                {
                    if(maze[i,j] != 'X')
                    {
                        if(maze[i,j] == 'S')
                        {
                            s = i * dimW + j;
                        }
                        else if(maze[i,j] == 'E')
                        {
                            e = i * dimW + j;
                        }

                        if (j + 1 < dimW)
                        {
                            g.AddEdge(i * dimW + (j + 1), i * dimW + j, 1);
                        }
                        if (i + 1 < dimH)
                        {
                            g.AddEdge((i + 1) * dimW + j, i * dimW + j, 1);
                        }
                        if (j - 1 >= 0)
                        {
                            g.AddEdge(i * dimW + (j - 1), i * dimW + j, 1);
                        }
                        if (i - 1 >= 0)
                        {
                            g.AddEdge((i - 1) * dimW + j, i * dimW + j, 1);
                        }
                    }
                    else if(withDynamite)
                    {
                        if (j + 1 < dimW)
                        {
                            g.AddEdge(i * dimW + (j + 1), i * dimW + j, t);
                        }
                        if (i + 1 < dimH)
                        {
                            g.AddEdge((i + 1) * dimW + j, i * dimW + j, t);
                        }
                        if (j - 1 >= 0)
                        {
                            g.AddEdge(i * dimW + (j - 1), i * dimW + j, t);
                        }
                        if (i - 1 >= 0)
                        {
                            g.AddEdge((i - 1) * dimW + j, i * dimW + j, t);
                        }
                    }
                }
            }
            g.DijkstraShortestPaths(s, out PathsInfo[] info);
            double pathLength = info[e].Dist;      
            if (!pathLength.IsNaN())
            {
                path = pathToString(PathsInfo.ConstructPath(s, e, info), dimH, dimW);
                return (int)pathLength;
            }
            path = string.Empty;
            return -1;
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            int dimH = maze.GetLength(0);
            int dimW = maze.GetLength(1);
            int layersCount = k + 1;
            int layerSize = dimW * dimH;
            Graph g = new AdjacencyListsGraph<AVLAdjacencyList>(true, layersCount * layerSize);
            int s = 0;
            int e = 0;
            for (int i = 0; i < dimH; i++)
            {
                for (int j = 0; j < dimW; j++)
                {
                    if (maze[i, j] != 'X')
                    {
                        if (maze[i, j] == 'S')
                        {
                            s = i * dimW + j;
                        }
                        else if (maze[i, j] == 'E')
                        {
                            e = i * dimW + j;
                        }

                        if (j + 1 < dimW)
                        {
                            for(int z = 0; z<layersCount; z++)
                            {
                                g.AddEdge(z * layerSize + i * dimW + (j + 1), z * layerSize +  i * dimW + j, 1);
                            }
                            
                        }
                        if (i + 1 < dimH)
                        {
                            for (int z = 0; z < layersCount; z++)
                            {
                                g.AddEdge(z * layerSize + (i + 1) * dimW + j, z * layerSize + i * dimW + j, 1);
                            }
                        }
                        if (j - 1 >= 0)
                        {
                            for (int z = 0; z < layersCount; z++)
                            {
                                g.AddEdge(z * layerSize + i * dimW + (j - 1), z * layerSize + i * dimW + j, 1);
                            }
                        }
                        if (i - 1 >= 0)
                        {
                            for (int z = 0; z < layersCount; z++)
                            {
                                g.AddEdge(z * layerSize + (i - 1) * dimW + j, z * layerSize + i * dimW + j, 1);
                            }
                        }
                    }
                    else
                    {
                        if (j + 1 < dimW)
                        {
                            for (int z = 1; z < layersCount; z++)
                            {
                                g.AddEdge((z-1) * layerSize + i * dimW + (j + 1), z * layerSize + i * dimW + j, t);
                            }

                        }
                        if (i + 1 < dimH)
                        {
                            for (int z = 1; z < layersCount; z++)
                            {
                                g.AddEdge((z-1) * layerSize + (i + 1) * dimW + j, z * layerSize + i * dimW + j, t);
                            }
                        }
                        if (j - 1 >= 0)
                        {
                            for (int z = 1; z < layersCount; z++)
                            {
                                g.AddEdge((z-1) * layerSize + i * dimW + (j - 1), z * layerSize + i * dimW + j, t);
                            }
                        }
                        if (i - 1 >= 0)
                        {
                            for (int z = 1; z < layersCount; z++)
                            {
                                g.AddEdge((z-1) * layerSize + (i - 1) * dimW + j, z * layerSize + i * dimW + j, t);
                            }
                        }
                    }
                }
            }
            g.DijkstraShortestPaths(s, out PathsInfo[] info);
            double minLength = double.MaxValue;
            int minId = -1;
            bool found = false;
            for (int z=0; z<layersCount; z++)
            {
                int id = z * layerSize + e;
                double length = info[id].Dist;
                if (!length.IsNaN() && length < minLength)
                {
                    minLength = length;
                    minId = id;
                    found = true;
                }
            }
            if (found)
            {
                path = pathToString(PathsInfo.ConstructPath(s, minId, info), dimH, dimW);
                return (int)minLength;
            }
            path = string.Empty;
            return -1;
        }
        
    }
}