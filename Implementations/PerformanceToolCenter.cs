using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Scraper.Implementations
{
    class PerformanceToolCenter : ScraperBase
    {
        public override Uri SourceUri => new Uri("https://www.performancetoolcenter.com/lathes/");

        public override string FriendlyName => "Performance Tool Center";

        public override string ContainerXPath => "//div[@id='CategoryContent']//div[@class='ProductDetails']";

        public override string ManufacturerXPath => ".//a";

        public override string PriceXPath => ".//em[contains(@class, 'p-price')]";

        public override string SkuPriceXPath => ".//a";

        public override string SourceUriAnchorXPath => ".//a";

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = node.InnerTextClean();
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
            Save("PerformanceToolCenter");
        }
    }
}
