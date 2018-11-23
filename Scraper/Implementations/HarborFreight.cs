using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Scraper.Implementations
{
    class HarborFreight : ScraperBase
    {
        protected override Uri SourceUri => new Uri("https://www.harborfreight.com/catalogsearch/result/index/?dir=asc&order=EAScore%2Cf%2CEAFeatured+Weight%2Cf%2CSale+Rank%2Cf&q=wood+lathe");

        protected override string FriendlyName => "Harbor Freight";

        protected override string ContainerXPath => "//ul[contains(@class,'products-grid')]/li[contains(@class, 'item')]";

        protected override string PriceXPath => ".//div[@class='price-box']/div[@class='sale']";

        protected override string SkuXPath => ".//div[@class='product-ids']";

        protected override string SourceUriAnchorXPath => ".//h2[@class='product-name']/a";

        protected override string ExtractPrice(HtmlNode node)
        {
            var raw = base.ExtractPrice(node);
            return raw.Replace("Only: ", "");
        }

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            return raw.Replace("Item #", "");
        }

        public override void Run()
        {
            AddRangeKnownManufacturer(FriendlyName);
            Save(nameof(HarborFreight));
        }
    }
}
