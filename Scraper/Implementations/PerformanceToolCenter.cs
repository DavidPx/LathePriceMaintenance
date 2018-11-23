using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Scraper.Implementations
{
    class PerformanceToolCenter : ScraperBase
    {
        protected override Uri SourceUri => new Uri("https://www.performancetoolcenter.com/lathes/");

        protected override string FriendlyName => "Performance Tool Center";

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
            Save("PerformanceToolCenter");
        }
    }
}
