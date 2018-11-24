using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Scraper.Bases;

namespace Scraper.Implementations
{
    class CraftSupplies : AllInOneBase
    {
        public override string FriendlyName => "Craft Supplies";

        protected override string SourceUriAnchorXPath => "./a";

        protected override string ContainerXPath => "//div[@id='Families']/div[contains(@class, 'familyCpct')]";

        protected override string PriceXPath => ".//span[@class='price']";

        protected override string SkuXPath => "./a/span";

        protected override string ManufacturerXPath => "./a/span";

        protected override Uri StartingUri => new Uri("https://www.woodturnerscatalog.com/t/216/Lathes?page=All");

        protected override string FileName => nameof(CraftSupplies);

        protected override string ExtractManufacturer(HtmlNode node)
        {
            var raw = base.ExtractManufacturer(node);
            return Regex.Match(raw, @"^\w+").Value;
        }

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            var words = Regex.Matches(raw, @"[\w-]+");
            var sku = words[1].Value;

            if (words[0].Value.Equals("Jet", StringComparison.OrdinalIgnoreCase))
            {
                // JET 1840 DVR Lathe vs. 1840 EVS.  Jet has conflicting sized-based models
                sku += words[2].Value;
            }

            return sku;
        }

        public override void Run()
        {
            AddRange();
        }
    }
}
