using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Scraper.Implementations
{
    class Rockler : AllInOneBase
    {
        protected override Uri StartingUri => new Uri("https://www.rockler.com/power-tools/lathes");

        protected override string FriendlyName => nameof(Rockler);
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

        protected override bool HandlePreRequest(HttpWebRequest request)
        {
            request.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            return true;
        }

        public override void Run()
        {
            AddRange();
        }
    }
}
