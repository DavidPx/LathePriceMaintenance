using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Scraper.Implementations
{
    class WoodworkersEmporium : ScraperBase
    {
        protected override Uri SourceUri => new Uri("https://www.woodworkersemporium.com/shop-by-category/machinery/lathes/?sort=alphaasc&limit=100&mode=4");

        protected override string FriendlyName => "Woodworker's Emmporium";

        protected override string ContainerXPath => "//li[@class='product']";

        protected override string PriceXPath => ".//span[contains(@class,'price--main')]";

        protected override string SkuPriceXPath => ".//*[@class='card-title']";

        protected override string SourceUriAnchorXPath => ".//*[@class='card-title']/a";

        protected override string ManufacturerXPath => ".//p[@data-test-info-type='brandName']";

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            return Regex.Replace(raw, @"(Harvey Turbo|\w+) (\S+) .*", "$2");
        }

        public override void Run()
        {
            AddRange();
            Save("WoodworkersEmporium");
        }
    }
}
