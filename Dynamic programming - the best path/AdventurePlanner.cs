using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
    /// <summary>
    /// struktura przechowująca punkt
    /// </summary>
    [Serializable]
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point (int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    public class AdventurePlanner: MarshalByRefObject
    {
        /// <summary>
        /// największy rozmiar tablicy, którą wyświetlamy
        /// ustaw na 0, żeby nic nie wyświetlać
        /// </summary>
        public int MaxToShow = 0;

      
        /// <summary>
        /// Znajduje optymalną pod względem liczby znalezionych skarbów ścieżkę,
        /// zaczynającą się w lewym górnym rogu mapy (0,0), a kończącą się w prawym
        /// dolnym rogu (X-Y-1).
        /// Za każdym razem możemy wykonać albo krok w prawo albo krok w dół.
        /// Pierwszym polem ścieżki powinno być (0,0), a ostatnim polem (X-1,Y-1).        
        /// </summary>
        /// <param name="treasure">liczba znalezionych skarbów</param>
        /// <param name="path">znaleziona ścieżka</param>
        /// <remarks>
        /// Złożoność rozwiązania to O(X * Y).
        /// </remarks>
        /// <returns></returns>
        public int FindPathThere(int[,] treasure, out List<Point> path)
        {
            int x = treasure.GetLength(0);
            int y = treasure.GetLength(1);
            int[,] treasureCount = new int[x, y];
            Point[,] prevPoint = new Point[x, y];
            treasureCount[0, 0] = treasure[0, 0];
            for(int i=1; i<x; i++)
            {
                treasureCount[i, 0] = treasureCount[i - 1, 0] + treasure[i, 0];
                prevPoint[i, 0] = new Point(i-1, 0);
            }
            for(int i=1; i<y; i++)
            {
                treasureCount[0, i] = treasureCount[0, i - 1] + treasure[0, i];
                prevPoint[0, i] = new Point(0, i-1);
            }
            for(int i=1; i<x; i++)
            {
                for(int j=1; j<y; j++)
                {
                    if(treasureCount[i-1,j] > treasureCount[i, j-1])
                    {
                        treasureCount[i, j] = treasure[i, j] + treasureCount[i - 1, j];
                        prevPoint[i, j] = new Point(i-1, j);
                    }
                    else
                    {
                        treasureCount[i, j] = treasure[i, j] + treasureCount[i, j - 1];
                        prevPoint[i, j] = new Point(i, j-1);
                    }
                }
            }
            path = new List<Point>();
            path.Add(new Point(x-1, y-1));
            int ix = x-1;
            int iy = y-1;
            while(ix > 0 || iy > 0)
            {
                path.Add(prevPoint[ix, iy]);
                int ixTmp = prevPoint[ix, iy].X;
                int iyTmp = prevPoint[ix, iy].Y;
                ix = ixTmp;
                iy = iyTmp;
            }
            path.Reverse();
            return treasureCount[x - 1, y - 1];
        }

      
        /// <summary>
        /// Znajduje optymalną pod względem liczby znalezionych skarbów ścieżkę,
        /// zaczynającą się w lewym górnym rogu mapy (0,0), dochodzącą do prawego dolnego rogu (X-1,Y-1), a 
        /// następnie wracającą do lewego górnego rogu (0,0).
        /// W pierwszy etapie możemy wykonać albo krok w prawo albo krok w dół. Po osiągnięciu pola (x-1,Y-1)
        /// zacynamy wracać - teraz możemy wykonywać algo krok w prawo albo krok w górę.
        /// Pierwszym i ostatnim polem ścieżki powinno być (0,0).
        /// Możemy założyć, że X,Y >= 2.
        /// </summary>
        /// <param name="treasure">liczba znalezionych skarbów</param>
        /// <param name="path">znaleziona ścieżka</param>
        /// <remarks>
        /// Złożoność rozwiązania to O(X^2 * Y) lub O(X * Y^2).
        /// </remarks>
        /// <returns></returns>
        public int FindPathThereAndBack(int[,] treasure, out List<Point> path)
        {
            path = new List<Point>();
            int dimH = treasure.GetLength(0);
            int dimW = treasure.GetLength(1);
            int diagCount = dimH + dimW - 1;
            int[,] treasureCount = new int[dimH, dimH];
            int[,] newTreasureCount;
            bool[,,,] isPrevAbove = new bool[dimH, dimW, dimH, 2];

            if (dimH == 1)
            {
                int result = 0;
                for (int i=0; i<dimW; i++)
                {
                    result += treasure[0, i];
                    path.Add(new Point(0, i));
                }
                for(int i=dimW - 2; i >=0; i--)
                {
                    path.Add(new Point(0, i));
                }
                return result;
            }
            else if(dimW == 1)
            {
                int result = 0;
                for(int i=0; i<dimH; i++)
                {
                    result += treasure[i, 0];
                    path.Add(new Point(i, 0));
                }
                for (int i = dimH - 2; i >= 0; i--)
                {
                    path.Add(new Point(i, 0));
                }
                return result;
            }
            else
            {
                treasureCount[0, 0] = treasure[0, 0];
                treasureCount[0, 1] = treasure[1, 0] + treasure[0, 1] + treasureCount[0, 0];

                isPrevAbove[0, 1, 1, 0] = false;
                isPrevAbove[0, 1, 1, 1] = true;
            }
            Point p2 = new Point();
            for(int diag = 2; diag<diagCount - 1; diag++)
            {
                newTreasureCount = new int[dimH, dimH];
                for (Point p = StartPoint(diag, dimW); p.X < dimH - 1 && p.Y > 0; p.X++, p.Y--)
                {
                    for (p2.X = p.X + 1, p2.Y = p.Y - 1; p2.X < dimH && p2.Y >= 0; p2.X++, p2.Y--)
                    {
                        List<Point> prev1 = new List<Point>(), prev2 = new List<Point>();
                        prev1.Add(new Point(p.X, p.Y - 1));
                        if (p.X - 1 >= 0)
                            prev1.Add(new Point(p.X - 1, p.Y));
                        prev2.Add(new Point(p2.X - 1, p2.Y));
                        if (p2.Y - 1 >= 0)
                            prev2.Add(new Point(p2.X, p2.Y - 1));
                        int prevPosMax1 = -1, prevPosMax2 = -1;
                        newTreasureCount[p.X, p2.X] = -1;
                        foreach (var prevPoint1 in prev1)
                        {
                            foreach(var prevPoint2 in prev2)
                            {
                                if(prevPoint1.X != prevPoint2.X || prevPoint1.Y != prevPoint2.Y)
                                {
                                    if (treasureCount[prevPoint1.X, prevPoint2.X] > newTreasureCount[p.X, p2.X])
                                    {
                                        prevPosMax1 = prevPoint1.X;
                                        prevPosMax2 = prevPoint2.X;
                                        newTreasureCount[p.X, p2.X] = treasureCount[prevPoint1.X, prevPoint2.X];
                                    }     
                                }
                            }
                        }
                        newTreasureCount[p.X, p2.X] += treasure[p.X, p.Y] + treasure[p2.X, p2.Y];
                        isPrevAbove[p.X, p.Y, p2.X, 0] = prevPosMax1 != p.X;
                        isPrevAbove[p.X, p.Y, p2.X, 1] = prevPosMax2 != p2.X;
                    }
                }
                treasureCount = newTreasureCount;
            }
            List<Point>[] halfPath = TableToPath(isPrevAbove, new int[] { dimH - 2, dimW - 1, dimH - 1 });
            halfPath[0].Reverse();
            path.AddRange(halfPath[0]);
            path.Add(new Point(dimH - 1, dimW - 1));
            path.AddRange(halfPath[1]);
            return treasureCount[dimH - 2, dimH - 1] + treasure[dimH - 1, dimW - 1];
        }

        List<Point>[] TableToPath(bool[,,,] isPrevAbove, int[] pos)
        {
            List<Point>[] halfPath = new List<Point>[2];
            for (int i = 0; i < 2; i++)
                halfPath[i] = new List<Point>();
            Point[] p = new Point[2];
            p[0] = new Point(pos[0], pos[1]);
            p[1] = new Point(pos[2], pos[0] + pos[1] - pos[2]);
            while (p[0].X >= 0 && p[0].Y >= 0)
            {
                for(int i=0; i<2; i++)
                {
                    halfPath[i].Add(p[i]);
                    if (isPrevAbove[pos[0], pos[1], pos[2], i])
                    {
                        p[i] = new Point(p[i].X - 1, p[i].Y);
                    }
                    else
                    {
                        p[i] = new Point(p[i].X, p[i].Y - 1);
                    }
                }               
                pos[0] = p[0].X;
                pos[1] = p[0].Y;
                pos[2] = p[1].X;
            }
            return halfPath;
        }

        Point StartPoint(int diag, int dimW)
        {
            Point point = new Point();
            if (diag >= dimW)
            {
                point.Y = dimW - 1;
                point.X = diag - point.Y;
            }
            else
            {
                point.Y = diag;
                point.X = 0;
            }
            return point;
        }
    }
}
