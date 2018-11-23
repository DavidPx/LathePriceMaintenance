using System;
using HtmlAgilityPack;

namespace Scraper.Implementations
{
    class Grizzly : ScraperBase
    {
        protected override Uri SourceUri => new Uri("http://www.grizzly.com/wood-lathes?pagesize=96");

        protected override string FriendlyName => "Grizzly";

        protected override string ContainerXPath => "//div[contains(@class, 'searchresult ')]";

        protected override string PriceXPath => ".//div[@class='searchresult-metadata']";

        protected override string SkuPriceXPath => ".//div[@class='searchresult-metadata']";

        protected override string SourceUriAnchorXPath => ".//a[@class='btn btn-cta']";

        protected override string ExtractPrice(HtmlNode node)
        {
            return node.GetAttributeValue("data-price", "");
        }

        protected override string ExtractSku(HtmlNode node)
        {
            return node.GetAttributeValue("data-itemnumber", "");
        }

        public override void Run()
        {
            AddRangeKnownManufacturer(FriendlyName);
            Save(FriendlyName);
        }
    }
}
