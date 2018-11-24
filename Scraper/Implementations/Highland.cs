using HtmlAgilityPack;
using Scraper.Bases;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Scraper.Implementations
{
    class Highland : AllInOneBase
    {
        public override string FriendlyName => "Highland Woodworking";

        protected override string SourceUriAnchorXPath => "./a";

        protected override string ContainerXPath => "//table[@class='product-list']//tr//div[@class='product-list-item']";

        protected override string PriceXPath => ".//div[@class='product-list-price']";

        protected override string SkuXPath => ".//h5/a";

        protected override Uri StartingUri => new Uri("https://www.highlandwoodworking.com/woodturning-lathes.aspx");

        protected override string FileName => nameof(Highland);

        protected override string ManufacturerXPath => ".//h5/a";

        protected override IList<ManufacturerExclusion> Exclusions => new[]
        {
            new ManufacturerExclusion { Manufacturer = "Wheel", SkuExclusionRegexes = new[] { ".+" } },
            new ManufacturerExclusion { Manufacturer = "Rikon", SkuExclusionRegexes = new[] {"Midi", "Mini", "^21$" } },
            new ManufacturerExclusion { Manufacturer = "Oneway", SkuExclusionRegexes = new[] {"^2000$", "^1224-E$", "^17$", "Large", "Outboard", "Toolrest", "Tailstock", "Spindle", "Set", "^12$", "DrillWizard", "Bowl", "Versa-Mount", "Spindle","Power" } }
        };

        protected override string ExtractPrice(HtmlNode node)
        {
            string price;
            var salePrice = node.SelectSingleNode(".//span[@class='product-list-sale-value']");
            if (salePrice != null)
            {
                price = salePrice.InnerTextClean();
            }
            else
            {
                var normal = node.SelectSingleNode(".//span[@class='product-list-cost-value']");
                price = Regex.Match(normal.InnerTextClean(), @"[\d,.]+").Value;
            }
            return price;
        }

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            var sku = Regex.Matches(raw, @"[\w-]+")[1].Value;

            if (sku == "1224" && raw.Contains("Extension", StringComparison.OrdinalIgnoreCase))
            {
                sku += "-E";
            }

            return sku;
        }

        protected override string ExtractManufacturer(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            return Regex.Matches(raw, @"[\w-]+")[0].Value;
        }

        public override void Run()
        {
            AddRange();
        }
    }
}
