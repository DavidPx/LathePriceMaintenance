using System;
using System.Linq;
using HtmlAgilityPack;
using Scraper.Bases;

namespace Scraper.Implementations
{
    class Grizzly : AllInOneBase
    {
        protected override Uri StartingUri => new Uri("http://www.grizzly.com/wood-lathes?pagesize=96");

        public override string FriendlyName => "Grizzly";
        protected override string FileName => FriendlyName;

        protected override string ContainerXPath => "//div[contains(@class, 'searchresult ')]";

        protected override string PriceXPath => ".//div[@class='searchresult-metadata']";

        protected override string SkuXPath => ".//div[@class='searchresult-metadata']";

        protected override string SourceUriAnchorXPath => ".//a[@class='searchresultdesclink']";
        protected override string ManufacturerXPath => ".//a[@class='searchresultdesclink']";

        protected override string ExtractPrice(HtmlNode node)
        {
            return node.GetAttributeValue("data-price", "");
        }

        protected override string ExtractSku(HtmlNode node)
        {
            return node.GetAttributeValue("data-itemnumber", "");
        }

        protected override string ExtractManufacturer(HtmlNode node)
        {
            // Manufacturer is the first word in the product URL
            var href = node.GetAttributeValue("href", "");

            var segments = href.Split('/');
            var biggest = segments.Skip(1).OrderByDescending(s => s.Length).First();

            var bits = biggest.Split('-');

            return 
                string
                    .Join(' ',
                    bits
                    .Take(2)
                    .Where(b =>
                    {
                        int foo;
                        return !int.TryParse(b, out foo);
                    })
                    );
                
        }

        public override void Run()
        {
            AddRange();
        }
    }
}
