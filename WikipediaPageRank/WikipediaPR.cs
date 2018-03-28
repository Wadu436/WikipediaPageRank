using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikipediaREST;

namespace WikipediaPageRank
{
    class WikipediaPR
    {
        const decimal dampingFactor = 0.85m;

        static public Dictionary<string, decimal> CalculateAndUpdatePageRank(Dictionary<string, List<string>> linkDictionary, Dictionary<string, decimal> pagerankDictionary)
        {
            DateTime start = DateTime.Now;

            decimal DeltaPR;
            decimal threshold = 0.000000000000000000001m * linkDictionary.Count;
            Console.WriteLine("Threshold: {0}", threshold);
            Dictionary<string, decimal> updatedPageRankDictionary;

            do
            {
                DeltaPR = 0;
                updatedPageRankDictionary = PageRankRecursionStep(linkDictionary, pagerankDictionary); //Één iteratie

                foreach(var p in updatedPageRankDictionary) //Bereken hoeveel verschil er is tussen deze iteratie en de vorige
                {
                    DeltaPR += Math.Abs(p.Value - pagerankDictionary[p.Key]);
                }
                Console.WriteLine("Delta PR = {0}", DeltaPR);
                pagerankDictionary = updatedPageRankDictionary;
            } while (DeltaPR > threshold); //Zolang het verschil te groot is, blijf verder itereren

            Console.WriteLine("Calculated PageRank for {0} pages in {1} seconds.", pagerankDictionary.Count, (DateTime.Now - start).TotalSeconds.ToString().Split(',')[0] + "," + (DateTime.Now - start).TotalSeconds.ToString().Split(',')[1].Remove(2));
            return pagerankDictionary;
        }

        static private Dictionary<string, decimal> PageRankRecursionStep(Dictionary<string, List<string>> linkDictionary, Dictionary<string, decimal> pagerankDictionary)
        {
            decimal percentageRandomSurfer = 1 / (decimal)linkDictionary.Count;

            Dictionary<string, decimal> updatedPageRankDictionary = new Dictionary<string, decimal>();
            foreach (var pr in pagerankDictionary)
            {
                updatedPageRankDictionary.Add(pr.Key, 0);
            }

            decimal deadEndPR = 0;

            foreach (var f in linkDictionary) //Voor elke pagina 
            {
                if (f.Value.Count > 0) //Als de pagina naar een andere verwijst
                {
                    decimal percentagePerLink = 1 / (decimal)f.Value.Count;
                    foreach (var t in f.Value)
                    {
                        updatedPageRankDictionary[t] += dampingFactor * pagerankDictionary[f.Key] * percentagePerLink; //Verdeel de PageRank over alle naar verwezen pagina's
                    }
                }
                else //Als de pagina niet naar een andere verwijst
                {
                    deadEndPR += pagerankDictionary[f.Key]; //Voeg de PR toe aan PR van alle doodlopende pagina's
                }
                updatedPageRankDictionary[f.Key] += 1 - dampingFactor; //Voeg waarde van de random surfer toe
            }

            foreach (var f in linkDictionary) //Verdeel de PR van alle doodlopende pagina's over de andere pagina's
            {
                updatedPageRankDictionary[f.Key] += dampingFactor * deadEndPR * percentageRandomSurfer; 
            }
            return updatedPageRankDictionary;
        }
    }
}
