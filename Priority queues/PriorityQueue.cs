
using System;
using System.Collections.Generic;

namespace ASD
{

public interface IPriorityQueue
    {
    void Put(int p);     // wstawia element do kolejki
    int GetMax();        // pobiera maksymalny element z kolejki (element jest usuwany z kolejki)
    int ShowMax();       // pokazuje maksymalny element kolejki (element pozostaje w kolejce)
    int Count { get; }   // liczba elementów kolejki
    }


public class LazyPriorityQueue : MarshalByRefObject, IPriorityQueue
    {

        List<int> list;

        public LazyPriorityQueue()
        {
            list = new List<int>();
        }

        public void Put(int p)
        {
            list.Add(p);
        }

        int ShowMaxID()
        {
            int maxID = 0;
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i] > list[maxID])
                    maxID = i;
            }
            return maxID;
        }

        public int GetMax()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Access to empty queue");
            else
            {
                int maxID = ShowMaxID();
                int max = list[maxID];
                list.RemoveAt(maxID);
                return max;
            }
        }

        public int ShowMax()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Access to empty queue");
            else
            {
                return list[ShowMaxID()];
            }
        }

        public int Count
        {
            get {
                return list.Count;
            }
        }

    } // LazyPriorityQueue


public class EagerPriorityQueue : MarshalByRefObject, IPriorityQueue
    {

        List<int> list;

        public EagerPriorityQueue()
        {
            list = new List<int>();
        }

        public void Put(int p)
        {
            list.Add(p);
            if(list.Count >= 2)
            {
                int i = list.Count - 1;
                while(i>0 && list[i-1] > p)
                {
                    list[i] = list[i - 1];
                    i -= 1;
                }
                list[i] = p;
            }
        }

        public int GetMax()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Access to empty queue");
            else
            {
                int max = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return max;
            }
        }

        public int ShowMax()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Access to empty queue");
            else
                return list[list.Count - 1];
        }

        public int Count
        {
            get {
                return list.Count;
            }
        }

    } // EagerPriorityQueue


public class HeapPriorityQueue : MarshalByRefObject, IPriorityQueue
    {

        List<int> list;

        public HeapPriorityQueue()
        {
            list = new List<int>();
            list.Add(int.MaxValue);
        }

        public void Put(int p)
        {
            list.Add(p);
            int i = list.Count - 1;
            while(list[i/2] < p)
            {
                list[i] = list[i / 2];
                i /= 2;
            }
            list[i] = p;
        }

        public int GetMax()
        {
            if (list.Count == 1)
                throw new InvalidOperationException("Access to empty queue");
            else
            {
                int max = list[1];
                int p =  list[1] = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                if(list.Count > 2)
                {
                    int i = 1;
                    while (2 * i < list.Count)
                    {
                        if (2 * i + 1 < list.Count && list[2 * i + 1] > list[2 * i])
                        {
                            if (list[2 * i + 1] > p)
                            {
                                list[i] = list[2 * i + 1];
                                i = 2 * i + 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (list[2 * i] > p)
                            {
                                list[i] = list[2 * i];
                                i *= 2;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    list[i] = p;
                }
                return max;
            }
        }

        public int ShowMax()
        {
            if (list.Count == 1)
                throw new InvalidOperationException("Access to empty queue");
            else
            {
                return list[1];
            }
        }

        public int Count
        {
            get {
                return list.Count - 1;
            }
        }

    } // HeapPriorityQueue

}
