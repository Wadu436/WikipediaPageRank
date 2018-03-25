using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikipediaREST;
using Excel = Microsoft.Office.Interop.Excel;

namespace WikipediaPageRank
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter first page (blank for \"PageRank\")");

            string title = Console.ReadLine();

            if(title == "")
            {
                title = "PageRank";
            }

            WikipediaCrawler crawler = new WikipediaCrawler(); //Vindt alle pagina's op Wikipedia met hun links
            crawler.Crawl(title, 0);

            crawler.SanitizeDictionary(); //Kuis alle links op die naar een pagina verwijzen die we niet gevonden hebben.

            Dictionary<string, List<string>> linkDictionary = crawler.pageDict; //Haal de lijst met links uit de crawler
            Dictionary<string, decimal> pagerankDictionary = new Dictionary<string, decimal>(); //Maak nieuwe lijst om de PageRank waarden bij te houden
            foreach(var t in linkDictionary)
            {
                pagerankDictionary.Add(t.Key, 1);
            }

            pagerankDictionary = WikipediaPR.CalculateAndUpdatePageRank(linkDictionary, pagerankDictionary); //Bereken de PageRank waarden

            PRSort prSort = new PRSort(pagerankDictionary); 
            List<string> sortedTitles = prSort.QuickSort(); //Sorteer de PageRank waarden

            //Geef de PageRank weer in een Excel bestand
            Excel.Application excelPagerank = new Excel.Application() { Visible = false };  
            Excel.Workbook ePRWorkbook = excelPagerank.Workbooks.Add();
            Excel.Worksheet ePRWorksheet = ePRWorkbook.ActiveSheet;

            ePRWorksheet.Cells[1, 1] = "Page Name";
            ePRWorksheet.Cells[1, 2] = "PageRank";

            int i = 2;
            foreach (string p in sortedTitles)
            {
                ePRWorksheet.Cells[i, 1] = p;
                ePRWorksheet.Cells[i, 2] = pagerankDictionary[p];
                i++;
            }

            ePRWorksheet.Range["A1", "B1"].EntireColumn.AutoFit();

            Console.WriteLine("Opening Excel...");
            excelPagerank.Visible = true;

            Console.ReadKey();
        }
    }
}
