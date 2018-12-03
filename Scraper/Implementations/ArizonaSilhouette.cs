using HtmlAgilityPack;
using Scraper.Bases;
using System;
using System.Text.RegularExpressions;

namespace Scraper.Implementations
{
    class ArizonaSilhouette : AllInOneBase
    {
        public override string FriendlyName => "Arizona Silhouette";

        protected override string SourceUriAnchorXPath => ".//a[1]";

        protected override string ContainerXPath => "//div[@class='product-item']";

        protected override Uri StartingUri => new Uri("http://www.arizonasilhouette.com/category/lathes.html");

        protected override string PriceXPath => ".//div[@class='product-price']/span";

        protected override string SkuXPath => ".//div[@class='product-code']/span";

        protected override string ManufacturerXPath => ".//div[@class='product-name']";

        protected override string ExtractManufacturer(HtmlNode node)
        {
            var raw = base.ExtractManufacturer(node);
            return Regex.Match(raw, @"^\w+").Value;
        }

        protected override string FileName => nameof(ArizonaSilhouette);

        public override void Run()
        {
            AddRange();
        }
    }
}
