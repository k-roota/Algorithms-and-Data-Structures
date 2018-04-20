using System;
using System.Collections.Generic;

namespace Lab08
{

    public class Lab08 : MarshalByRefObject
    {

        /// <summary>
        /// funkcja do sprawdzania czy da się ustawić k elementów w odległości co najmniej dist od siebie
        /// </summary>
        /// <param name="a">posortowana tablica elementów</param>
        /// <param name="dist">zadany dystans</param>
        /// <param name="k">liczba elementów do wybrania</param>
        /// <param name="exampleSolution">Wybrane elementy</param>
        /// <returns>true - jeśli zadanie da się zrealizować</returns>
        public bool CanPlaceElementsInDistance(int[] a, int dist, int k, out List<int> exampleSolution)
        {
            exampleSolution = new List<int>() { a[0] };
            int lastObj = a[0];
            k--;
            for(int i=1; i<a.Length; i++)
            {
                if(a[i] - lastObj >= dist)
                {
                    lastObj = a[i];
                    exampleSolution.Add(a[i]);
                    k--;
                    if (k == 0)
                        return true;
                }
            }
            exampleSolution = null;
            return false;
        }

        /// <summary>
        /// Funkcja wybiera k elementów tablicy a, tak aby minimalny dystans pomiędzy dowolnymi dwiema liczbami (spośród k) był maksymalny
        /// </summary>
        /// <param name="a">posortowana tablica elementów</param>
        /// <param name="k">liczba elementów do wybrania</param>
        /// <param name="exampleSolution">Wybrane elementy</param>
        /// <returns>Maksymalny możliwy dystans między wybranymi elementami</returns>
        public int LargestMinDistance(int[] a, int k, out List<int> exampleSolution)
        {
            if (a.Length <= 1 || a.Length > 100000 || k <= 1 || k > a.Length)
                throw new System.ArgumentException();
            if(a[0] < 0 || a[a.Length - 1] > int.MaxValue)
                throw new System.ArgumentException();
            int maxDist = (a[a.Length - 1] - a[0]) / (k - 1) + 1;
            int minDist = a[1] - a[0];
            for(int i=1; i<a.Length - 1; i++)
            {
                int distX = a[i + 1] - a[i];
                if (distX < minDist)
                    minDist = distX;
            }
            int dist = maxDist - 1;
            exampleSolution = null;
            while(true)
            {
                if (CanPlaceElementsInDistance(a, dist, k, out exampleSolution))
                {
                    if (dist == maxDist - 1)
                    {
                        return dist;
                    }
                    else
                    {
                        minDist = dist;
                        dist += (maxDist + 1 - minDist) / 2;
                    }
                }
                else
                {
                    maxDist = dist;
                    dist -= (maxDist + 1 - minDist) / 2;
                }
            }
        }

    }

}
