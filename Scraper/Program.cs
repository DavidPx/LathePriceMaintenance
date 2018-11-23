using Scraper.Implementations;
using System;
using System.Collections.Generic;

namespace Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var scrapers = new List<ScraperBase>
            {
                new Baileigh(),
                new HomeDepot(),
                new PerformanceToolCenter(),
                new WoodworkersEmporium(),
                new Grizzly(),
                new AcmeTools(),
                new BusyBee(),
                new WoodCraft(),
                new Rockler(),
                new HarborFreight(),
                new EliteMetalTools()
            };

            foreach (var scraper in scrapers)
            {
                scraper.Run();
            }

            //new EliteMetalTools().Run();
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
