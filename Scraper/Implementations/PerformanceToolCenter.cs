using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Scraper.Bases;

namespace Scraper.Implementations
{
    class PerformanceToolCenter : AllInOneBase
    {
        protected override Uri StartingUri => new Uri("https://www.performancetoolcenter.com/lathes/");

        public override string FriendlyName => "Performance Tool Center";
        protected override string FileName => nameof(PerformanceToolCenter);

        protected override string ContainerXPath => "//div[@id='CategoryContent']//div[@class='ProductDetails']";

        protected override string ManufacturerXPath => ".//a";

        protected override string PriceXPath => ".//em[contains(@class, 'p-price')]";

        protected override string SkuXPath => ".//a";

        protected override string SourceUriAnchorXPath => ".//a";

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            raw = Regex.Replace(raw, @"\w+ (MLAREVO \S+|\S+).*", "$1");
            raw = raw.Replace("MLAREVO", "");
            return raw.Trim();
        }

        protected override string ExtractManufacturer(HtmlNode node)
        {
            var raw = base.ExtractManufacturer(node);
            return Regex.Match(raw, @"\w+").Value;
        }

        protected override string ExtractPrice(HtmlNode node)
        {
            var raw = base.ExtractPrice(node);
            return Regex.Match(raw, @"[\$,\.\d]+$").Value;
        }

        public override void Run()
        {
            AddRange();
        }
    }
}
