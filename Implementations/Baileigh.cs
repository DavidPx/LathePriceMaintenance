using System;

namespace Scraper.Implementations
{
    class Baileigh : ScraperBase
    {
        const string manufacturer = "Baileigh";

        public override Uri SourceUri => new Uri("https://www.baileigh.com/woodworking/lathes/lathes");

        public override string FriendlyName => manufacturer;

        public override string ContainerXPath => "//li[starts-with(@class, 'item')]";

        public override string PriceXPath => ".//span[@class='price']";

        public override string SkuPriceXPath => ".//h2[@class='product-name']";

        public override string SourceUriAnchorXPath => ".//a[@title='View Details']";

        public override string ManufacturerXPath => manufacturer;

        public override void Run()
        {
            AddRangeKnownManufacturer(manufacturer);
            Save(manufacturer);
        }
        
    }
}
