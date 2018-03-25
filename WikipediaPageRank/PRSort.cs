using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikipediaPageRank
{
    class PRSort
    {
        private Dictionary<string, decimal> pagerankDictionary;
        private List<string> sortedKeys;
        public PRSort(Dictionary<string, decimal> dictionary)
        {
            pagerankDictionary = dictionary;
            sortedKeys = new List<string>(dictionary.Keys);
        }

        public List<string> QuickSort() //Implementatie van het quicksort algoritme, meer details beschikbaar op wikipedia voor geinteresseerden
        {
            DateTime start = DateTime.Now;
            QuickSortRecursion(0, sortedKeys.Count - 1);

            string time = (DateTime.Now - start).TotalSeconds.ToString();

            if (time.Contains(','))
                time = time.Split(',')[0] + "," + (DateTime.Now - start).TotalSeconds.ToString().Split(',')[1].Remove(2);

            Console.WriteLine("Sorted {0} elements in {1} seconds.", sortedKeys.Count, time);

            return sortedKeys;
        }

        private void QuickSortRecursion(int low, int high)
        {
            if(low < high)
            {
                int partitionIndex = _partition(low, high);

                QuickSortRecursion(low, partitionIndex - 1); //array before pi
                QuickSortRecursion(partitionIndex + 1, high); //array after pi
            }
        }

        private int _partition(int low, int high)
        {
            string pivot = sortedKeys[high];
            decimal pivotPR = pagerankDictionary[pivot];

            int i = low - 1;

            for(int j = low; j < high; j++)
            {
                if (pivotPR < pagerankDictionary[sortedKeys[j]])
                {
                    i++;
                    string iKey = sortedKeys[i];
                    string jKey = sortedKeys[j];
                    sortedKeys[i] = jKey;
                    sortedKeys[j] = iKey;
                }
            }

            string iPlusOneKey = sortedKeys[i + 1];
            string highKey = sortedKeys[high];
            sortedKeys[high] = iPlusOneKey;
            sortedKeys[i+1] = highKey;

            return (i + 1);
        }
    }
}
