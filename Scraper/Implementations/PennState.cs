using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Scraper.Bases;

namespace Scraper.Implementations
{
    class PennState : AllInOneSeleniumBase
    {
        public override string FriendlyName => "Penn State Industries";

        protected override string SourceUriAnchorXPath => ".//a[@class='product-name']";

        protected override string ContainerXPath => "//div[@id='searchspring-search_results']//div[@class='item product']";

        protected override string PriceXPath => ".//div[contains(@class, 'CategoryProdPrice')]";

        protected override string SkuXPath => ".//div[contains(@class, 'summarycode')]";

        protected override Uri StartingUri => new Uri("https://www.pennstateind.com/store/mini-lathes.html");

        protected override string FileName => nameof(PennState);

        protected override string ExtractPrice(HtmlNode node)
        {
            var raw = base.ExtractPrice(node);
            return Regex.Match(raw, @"[\d,\.]+").Value;
        }

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            return Regex.Match(raw, @"\S+$").Value;
        }

        public override void Run()
        {
            AddRangeKnownManufacturer(FriendlyName);
        }
    }
}
