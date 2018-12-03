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
                new EliteMetalTools(),
                new FactoryAuthorizedOutlet(),
                new CraftSupplies(),
                new PennState(),
                new Packard(),
                new Highland(),
                new Robust(),
                new Amazon(),
                new ArizonaSilhouette()
            };

            Console.WriteLine("Press Enter to run all, or key in the number of the scraper to run:");
            int i = 0;

            foreach (var scraper in scrapers)
            {
                Console.WriteLine($"[{i++}]: {scraper.FriendlyName}");
            }

            var entry = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(entry))
            {
                foreach (var scraper in scrapers)
                {
                    scraper.Run();
                }
            }
            else
            {
                if (int.TryParse(entry, out var selection))
                {
                    if (selection < 0 || selection >= scrapers.Count)
                    {
                        Console.WriteLine("input out of range");
                    }
                    scrapers[selection].Run();
                }
                else
                {
                    Console.WriteLine("invalid selection");
                }
            }
            
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
