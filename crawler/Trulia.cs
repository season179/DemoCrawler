using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler
{
    public class Trulia : Crawler
    {
        public override void Crawl()
        {
            string BaseUrl = ConfigurationManager.AppSettings["BASE_URL"].ToString();
            string directory = ConfigurationManager.AppSettings["DIRECTORY"].ToString();
            short StartPageNo = Convert.ToInt16(ConfigurationManager.AppSettings["START_PAGE"]);

            // Get last page no.
            string LastPage = ConfigurationManager.AppSettings["LAST_PAGE"];
            short LastPageNo = string.IsNullOrEmpty(LastPage) ? Convert.ToInt16(GetLastPageNo(BaseUrl + directory)) : Convert.ToInt16(LastPage);

            Parallel.For(StartPageNo,
                        LastPageNo,
                //new ParallelOptions { MaxDegreeOfParallelism = 3 },
                        y =>
                        {
                            string UrlToCrawl = BaseUrl + directory + (y > StartPageNo ? y.ToString() + "_p" : string.Empty);
                            Console.WriteLine("Crawling trulia page no.: " + y.ToString());
                            string Response = HttpRequest(UrlToCrawl);
                            GetAgentDetails(Response);
                        });

            //for (short page = StartPageNo; page <= LastPageNo; page++)
            //{
            //    string UrlToCrawl = BaseUrl + directory + (page > StartPageNo ? page.ToString() + "_p" : string.Empty);
            //    Console.WriteLine("Crawling trulia page no.: " + page.ToString());
            //    string Response = HttpRequest(UrlToCrawl);
            //    GetAgentDetails(Response);
            //}

            WriteOutput("C:\\NewYorkAgents.xlsx", Agents);
        }
    }
}
