using System;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Scraper.Bases;

namespace Scraper.Implementations
{
    class Rockler : AllInOneSeleniumBase
    {
        protected override Uri StartingUri => new Uri("https://www.rockler.com/power-tools/lathes");

        public override string FriendlyName => nameof(Rockler);
        protected override string FileName => FriendlyName;

        protected override string ContainerXPath => "//div[@class='product-item-info']";

        protected override string PriceXPath => ".//span[@data-price-type='finalPrice']";

        protected override string SkuXPath => ".//a[@class='product-item-link']";

        protected override string ManufacturerXPath => ".//a[@class='product-item-link']";

        protected override string ExtractManufacturer(HtmlNode node)
        {
            var raw = base.ExtractManufacturer(node);
            return Regex.Match(raw, @"\w+").Value;
        }

        protected override string SourceUriAnchorXPath => ".//a[@class='product-item-link']";
        
        public override void Run()
        {
            AddRange();
        }
    }
}
