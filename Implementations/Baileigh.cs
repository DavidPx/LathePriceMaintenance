using System;

namespace Scraper.Implementations
{
    class Baileigh : ScraperBase
    {
        const string manufacturer = "Baileigh";

        public override string FriendlyName => manufacturer;

        public override void Run()
        {
            var source = new Uri("https://www.baileigh.com/woodworking/lathes/lathes");
            
            AddRangeKnownManufacturer(source, "//li[starts-with(@class, 'item')]", ".//span[@class='price']", ".//h2[@class='product-name']", manufacturer, ".//a[@title='View Details']");

            Save(manufacturer);
        }
        
    }
}
