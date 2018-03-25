using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace WikipediaREST
{
    public class WikipediaClient
    {
        RestClient client;

        private Dictionary<string, List<string>> linkDict;

        public WikipediaClient()
        {
            client = new RestClient()
            {
                BaseUrl = new Uri("https://en.wikipedia.org/w/api.php")
            };
        }

        public Dictionary<string, List<string>> GetLinks(List<string> titles)
        {
            linkDict = new Dictionary<string, List<string>>();
            
            StringBuilder titleParameterBuilder = new StringBuilder();

            WikipediaPropLinks responseData = new WikipediaPropLinks();

            var request = new RestRequest(Method.POST);
            request.AddParameter("action", "query");
            request.AddParameter("prop", "links");
            request.AddParameter("pllimit", "max");
            request.AddParameter("format", "json");

            string[] titleParameters = new string[(int)Math.Ceiling((double)titles.Count / 50)];

            for(int i = 0; 50*i < titles.Count; i++)
            {
                titleParameterBuilder.Clear();
                for (int j = 0; j < 50 && 50*i + j < titles.Count; j++)
                {
                    titleParameterBuilder.AppendFormat("|{0}", titles[50*i+j]);
                    linkDict.Add(titles[50*i+j], new List<string>(new List<string>()));
                }
                titleParameterBuilder.Remove(0, 1);
                titleParameters[i] = titleParameterBuilder.ToString();
            }

            foreach(string titleParameter in titleParameters)
            { 
                //Build title from list of pages to get the outgoing links from
                request.AddOrUpdateParameter("titles", titleParameter);

                do
                {
                    //Check if the last request gave a 'plcontinue' value, if so, set that parameter
                    if (responseData.Continue != null)
                    {
                        request.AddOrUpdateParameter("plcontinue", responseData.Continue.Plcontinue);
                    }

                    DateTime previousTime = DateTime.Now;
                    //Execute the request
                    var response = client.Execute(request);
                    previousTime = DateTime.Now;

                    //Throw an error if there's an error
                    if (response.ErrorException != null)
                    {
                        const string message = "Error retrieving response.  Check inner details for more info.";
                        var exception = new ApplicationException(message, response.ErrorException);
                        throw exception;
                    }

                    //Deserialize the JSON response from Wikipedia using Newtonsoft's JSON.Net and Quicktype generated class 'WikipediaPropLinks'
                    try
                    {
                        responseData = WikipediaPropLinks.FromJson(response.Content);
                    }
                    catch
                    {
                        continue;
                    }


                    //Process the deserialized JSON
                    foreach (Page p in responseData.Query.Pages.Values)
                    {
                        //Make sure the Page object has links before continuing
                        if (p.Links != null)
                        {
                            foreach (Link l in p.Links)
                            {
                                //Filter some links out
                                if (!(l.Title.Contains("Template") || l.Title.Contains("File:") || l.Title.Contains("Wikipedia:") || l.Title.Contains("Help:") || l.Title.Contains("File:") || l.Title.Contains("Module:") || l.Title.Contains("Category:")))
                                {
                                    linkDict[p.Title].Add(l.Title);
                                }
                            }
                        }
                    }

                } while (responseData.Batchcomplete != ""); //continue until Wikipedia has confirmed all links have been sent back
            }

            return linkDict;
        }

        public Dictionary<string, List<string>> GetLinks(string title)
        {
            return GetLinks(new List<string>() { title });
        }
    }
}
