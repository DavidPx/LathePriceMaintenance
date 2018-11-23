using Scraper.Implementations;
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
                new WoodCraft()
            };

            //foreach (var scraper in scrapers)
            //{
            //    scraper.Run();
            //}

            new WoodCraft().Run();
            
        }
    }
}
