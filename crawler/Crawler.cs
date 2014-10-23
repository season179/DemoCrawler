using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ExportToExcel;
using HtmlAgilityPack;

namespace crawler
{
    public interface ICrawler
    {
        void StartTimer(string Name);
        void Crawl();
        void StopTimer(string Name);
    }

    public abstract class Crawler : ICrawler
    {
        public static List<Agent> Agents = new List<Agent>();
        Stopwatch stopWatch = new Stopwatch();
        public void StartTimer(string Name)
        {
            stopWatch.Start();
            Console.WriteLine("Start crawling " + Name);
        }

        public void StopTimer(string Name)
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Finished crawling " + Name + ". RunTime " + elapsedTime);
        }

        public abstract void Crawl();

        protected virtual string HttpRequest(string URL, string Method = "GET")
        {
            string result = string.Empty;

            WebRequest request = WebRequest.Create(URL);
            request.Method = Method;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            try
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                throw ex;
            }

            return result;
        }

        protected virtual string GetLastPageNo(string URL)
        {
            string RawData = HttpRequest(URL);
            if (!string.IsNullOrEmpty(RawData))
            {
                return ParseLastPageNo(RawData);
            }

            return string.Empty;
        }

        protected virtual void GetAgentDetails(string RawData)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(RawData);
            
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='mediaBody agent_entry']"))
            {
                Agent Agent = new Agent();
                Agent.Name = ParseName(node);
                Agent.PhoneNo = ParsePhoneNo(node);
                Agents.Add(Agent);
            }
        }

        private string ParseName(HtmlNode node)
        {
            return node.SelectSingleNode("descendant::h5[@class='typeEmphasize ellipsis_overflow mvn agent_name_link pan man']").InnerText;
        }

        private string ParsePhoneNo(HtmlNode node)
        {
            HtmlNode result = node.SelectSingleNode("descendant::p[@class='mvn h7 pre_badges']");
            return result != null ? result.InnerText : string.Empty;
        }

        private string ParseLastPageNo(string RawData)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(RawData);
            var node = doc.DocumentNode.SelectSingleNode("//a[@class='mhs pagination_page']");

            return node != null ? node.InnerText : string.Empty; ;
        }

        protected string CreateCSV<T>(List<T> data)
        {
            var properties = typeof(T).GetProperties();
            var result = new StringBuilder();

            foreach (var row in data)
            {
                var values = properties.Select(p => p.GetValue(row, null));
                var line = string.Join(",", values);
                result.AppendLine(line);
            }

            return result.ToString();
        }

        protected void WriteOutput(string name, string processedData)
        {
            WriteCSV(name, processedData);
        }

        protected void WriteCSV(string name, string processedData)
        {
            System.IO.File.WriteAllText(@"C:\" + name + ".csv", processedData);
        }

        protected void WriteOutput<T>(string name, List<T> obj)
        {
            CreateExcelFile.CreateExcelDocument(obj, name);
        }
    }

    public class Agent
    {
        public string Name { get; set; }
        public string PhoneNo { get; set; }
    }

}
