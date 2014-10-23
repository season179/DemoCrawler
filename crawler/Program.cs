using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            ICrawler crawler = (ICrawler)Activator.CreateInstance(Type.GetType("crawler.Trulia"));
            crawler.StartTimer("Trulia");
            crawler.Crawl();
            crawler.StopTimer("Trulia");

            Console.ReadLine();
        }
    }
}