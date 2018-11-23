using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Scraper.Implementations
{
    class AcmeTools : AllInOneBase
    {
        protected override Uri StartingUri => new Uri("http://www.acmetools.com/shop/tools/woodworking-lathes");  // new Uri("http://www.acmetools.com/shop/tools/woodworking-lathes?fetchFacets=true#facet:&facetLimit:&productBeginIndex:0&orderBy:5&pageView:grid&minPrice:&maxPrice:&pageSize:96");

        public override string FriendlyName => "Acme Tools";
        protected override string FileName => nameof(AcmeTools);

        protected override string ContainerXPath => "//div[@class='product_listing_container']//li";

        protected override string PriceXPath => ".//tr[contains(@class, 'now-price')]/td[@class='amount']";

        protected override string SkuXPath => ".//div[@class='product_name']";

        protected override string SourceUriAnchorXPath => ".//div[@class='product_name']//a";

        protected override string ManufacturerXPath => ".//div[@class='product_name']";

        protected override string ExtractPrice(HtmlNode node)
        {
            if (node == null) return ""; // Sometimes Acme doesn't show the price...  
            var raw = base.ExtractPrice(node);
            return Regex.Replace(raw, @"[\s\$,]", "");
        }

        protected override string ExtractManufacturer(HtmlNode node)
        {
            var raw = base.ExtractManufacturer(node);
            return Regex.Match(raw, @"[^-]+").Value;
        }

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            return Regex.Replace(raw, @"^[^-]+-([\w-]+) .+$", "$1");
        }

        public override void Run()
        {
            AddRange();
        }
    }
}
