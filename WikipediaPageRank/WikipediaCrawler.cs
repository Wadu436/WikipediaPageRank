using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WikipediaREST;
using System.Threading;

namespace WikipediaPageRank
{
    class WikipediaCrawler
    {
        private WikipediaClient _client;
        public Dictionary<string, List<string>> pageDict;

        private const int maxDepth = 2;

        public WikipediaCrawler()
        {
            pageDict = new Dictionary<string, List<string>>();
            _client = new WikipediaClient();
        }

        public void Crawl(string start, int depth)
        {
            DateTime startTime = DateTime.Now;

            List<string> _nextBatch = new List<string>() { start };
            Dictionary<string, List<string>> _tempDict = new Dictionary<string, List<string>>();
            int _depth = 0;

            //while() //Depth-limited crawling
            while(_depth < depth || (_nextBatch.Count > 0 && depth == 0)) //Crawl until you're dead/found all pages
            //while (_nextBatch.Count > 0 || pageDict.Count < 1000000)
            {
                _tempDict = _client.GetLinks(_nextBatch);

                _nextBatch.Clear();

                foreach(var page in _tempDict)
                {
                    foreach(string t in page.Value)
                    {
                        if (!(pageDict.ContainsKey(t) || _tempDict.ContainsKey(t)))
                        {
                            _nextBatch.Add(t);
                        }
                    }
                    pageDict.Add(page.Key, page.Value);
                }

                //Console.WriteLine("Cleaning up _nextBatch of duplicates");
                _nextBatch = _nextBatch.Distinct().ToList();

                _tempDict.Clear();
                _depth++;

                Console.WriteLine("{2}:{3}:{4} - Current Depth: {0}; Amount of articles crawled: {1}", _depth, pageDict.Count, DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0')); //hh:mm:ss
            }

            Console.WriteLine("Crawled {0} pages in {1} seconds", pageDict.Count, (DateTime.Now - startTime).TotalSeconds.ToString().Split(',')[0] + "," + (DateTime.Now - startTime).TotalSeconds.ToString().Split(',')[1].Remove(2));
        }

        public void SanitizeDictionary()
        {
            DateTime start = DateTime.Now;

            Dictionary<string, List<string>> newPageDict = new Dictionary<string, List<string>>();

            int number = 0;
            foreach(var entry in pageDict)
            {
                List<string> sanitizedLinksTo = entry.Value.ToList();
                foreach (string s in entry.Value)
                {
                    if (!pageDict.ContainsKey(s))
                    {
                        sanitizedLinksTo.Remove(s);
                        number++;
                    }
                }
                newPageDict.Add(entry.Key, sanitizedLinksTo);
            }

            pageDict = newPageDict;

            string time = (DateTime.Now - start).TotalSeconds.ToString();

            if(time.Contains(','))
                time = time.Split(',')[0] + "," + (DateTime.Now - start).TotalSeconds.ToString().Split(',')[1].Remove(2);

            Console.WriteLine("Sanitized {0} links in {1} seconds", number, time);
        }
    }
}
