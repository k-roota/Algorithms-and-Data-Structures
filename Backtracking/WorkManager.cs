using System;
using System.Linq;

namespace ASD
{
    public class WorkManager : MarshalByRefObject
    {
        /// <summary>
        /// Implementacja wersji 1
        /// W tablicy blocks zapisane s� wagi wszystkich blok�w do przypisania robotnikom.
        /// Ka�dy z nich powinien mie� przypisane bloki sumie wag r�wnej expectedBlockSum.
        /// Metoda zwraca tablic� przypisuj�c� ka�demu z blok�w jedn� z warto�ci:
        /// 1 - je�li blok zosta� przydzielony 1. robotnikowi
        /// 2 - je�li blok zosta� przydzielony 2. robotnikowi
        /// 0 - je�li blok nie zosta� przydzielony do �adnego robotnika
        /// Je�li wymaganego podzia�u nie da si� zrealizowa� metoda zwraca null.
        /// </summary>
        public int[] DivideWorkersWork(int[] blocks, int expectedBlockSum)
        {
            if (PreCheck(blocks, expectedBlockSum, out int[] owners))
            {
                return owners;
            }
            if (DivideWorkersWorkPartly(blocks, expectedBlockSum, owners, 0, false, -1))
                return owners;
            else
                return null;
        }

        /// <summary>
        /// Implementacja wersji 2
        /// Parametry i wynik s� analogiczne do wersji 1.
        /// </summary>
        public int[] DivideWorkWithClosestBlocksCount(int[] blocks, int expectedBlockSum)
        {
            if(PreCheck(blocks, expectedBlockSum, out int[] owners))
            {
                return owners;
            }
            minDifference = int.MaxValue;
            if (DivideWorkersWorkPartlyBest(blocks, expectedBlockSum, owners, 0, false, -1, 0, 0))
                return bestOwners;
            else
                return null;
        }

        int minDifference;
        int[] bestOwners;

        private bool PreCheck(int[] blocks, int expectedBlockSum, out int[] owners)
        {
            if (expectedBlockSum == 0)
            {
                owners = new int[blocks.Length];
                return true;
            }
            bool possible1 = false;
            bool possible2 = false;
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] <= expectedBlockSum)
                {
                    if (!possible1)
                    {
                        possible1 = true;
                    }
                    else if (!possible2)
                    {
                        possible2 = true;
                    }
                    else
                        break;
                }
            }
            if (!possible2)
            {
                owners = null;
                return true;
            }
            if (blocks.Sum() < 2 * expectedBlockSum)
            {
                owners = null;
                return true;
            }
            owners = new int[blocks.Length];
            return false;
        }

        private bool DivideWorkersWorkPartly(int[] blocks, int expectedBlockSum, int[] owners, int sum, bool firstWorkerFound, int lastId)
        {
            bool solved = false;
            for (int i = lastId + 1; i < blocks.Length; i++)
            {
                if (owners[i] == 0)
                {
                    int newSum = sum + blocks[i];
                    if (!firstWorkerFound)
                    {
                        owners[i] = 1;
                        if (newSum < expectedBlockSum)
                        {
                            solved = DivideWorkersWorkPartly(blocks, expectedBlockSum, owners, newSum, false, i);
                        }
                        else if (newSum == expectedBlockSum)
                        {
                            solved = DivideWorkersWorkPartly(blocks, expectedBlockSum, owners, 0, true, -1);
                        }
                    }       
                    else
                    {
                        owners[i] = 2;
                        if (newSum < expectedBlockSum)
                        {
                            solved = DivideWorkersWorkPartly(blocks, expectedBlockSum, owners, newSum, true, i);
                        }
                        else if (newSum == expectedBlockSum)
                        {
                            solved = true;
                        }
                    }
                    if(solved)
                    {
                        return true;  
                    }
                    else
                    {
                        owners[i] = 0;
                    }
                }
            }
            return solved;     
        }

        private bool DivideWorkersWorkPartlyBest(int[] blocks, int expectedBlockSum, int[] owners, int sum, bool firstWorkerFound, int lastId, int firstCount, int secondCount)
        {
            bool solved = false;
            bool solvedOnce = false;
            for (int i = lastId + 1; i < blocks.Length; i++)
            {
                if (owners[i] == 0)
                {
                    int newSum = sum + blocks[i];
                    if (!firstWorkerFound)
                    {
                        owners[i] = 1;
                        if (newSum < expectedBlockSum)
                        {
                            solved = DivideWorkersWorkPartlyBest(blocks, expectedBlockSum, owners, newSum, false, i, firstCount + 1, secondCount);
                        }
                        else if (newSum == expectedBlockSum)
                        {
                            solved = DivideWorkersWorkPartlyBest(blocks, expectedBlockSum, owners, 0, true, -1, firstCount + 1, secondCount);
                        }
                    }       
                    else
                    {
                        owners[i] = 2;
                        if (newSum < expectedBlockSum)
                        {
                            solved = DivideWorkersWorkPartlyBest(blocks, expectedBlockSum, owners, newSum, true, i, firstCount, secondCount + 1);
                        }
                        else if (newSum == expectedBlockSum)
                        {
                            solved = true;
                            int difference = Math.Abs(firstCount - secondCount - 1);
                            if (difference < minDifference)
                            {
                                minDifference = difference;
                                bestOwners = (int[])owners.Clone();
                            }
                        }
                    }
                    if (minDifference == 0)
                        return true;
                    if(solved)
                    {
                        solvedOnce = true;  
                    }
                    owners[i] = 0;
                }
            }
            return solvedOnce;
        }

// Mo�na dopisywa� pola i metody pomocnicze

    }
}

