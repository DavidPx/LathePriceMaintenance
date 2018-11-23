using System;

namespace Scraper.Implementations
{
    class Baileigh : ScraperBase
    {
        const string manufacturer = "Baileigh";

        protected override Uri SourceUri => new Uri("https://www.baileigh.com/woodworking/lathes/lathes");

        protected override string FriendlyName => manufacturer;

        protected override string ContainerXPath => "//li[starts-with(@class, 'item')]";

        protected override string PriceXPath => ".//span[@class='price']";

        protected override string SkuXPath => ".//h2[@class='product-name']";

        protected override string SourceUriAnchorXPath => ".//a[@title='View Details']";
        
        public override void Run()
        {
            AddRangeKnownManufacturer(manufacturer);
            Save(manufacturer);
        }
        
    }
}
