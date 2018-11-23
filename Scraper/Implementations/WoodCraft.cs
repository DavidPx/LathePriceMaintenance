using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Scraper.Implementations
{
    class WoodCraft : AllInOneBase
    {
        protected override Uri StartingUri => new Uri("https://www.woodcraft.com/categories/lathes");

        public override string FriendlyName => nameof(WoodCraft);
        protected override string FileName => FriendlyName;

        protected override string ContainerXPath => "//div[@class='product-summary__info']";

        protected override string PriceXPath => ".//*[@itemprop='price']";

        protected override string SkuXPath => ".//span[@itemprop='mpn']";

        protected override string SourceUriAnchorXPath => ".//a[@class='product-summary__name-link']";

        protected override string ManufacturerXPath => ".//p[@class='product-summary__name']";

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);

            // normalizing model numbers which only woodcraft has
            raw = raw.Replace("Model", "").Replace("mlarevo", "", StringComparison.OrdinalIgnoreCase).Trim();
            raw = Regex.Replace(raw, @"^1836$", "1836-220");
            return raw.Replace("T40", "T40-Legs");
        }

        protected override string ExtractManufacturer(HtmlNode node)
        {
            var raw = base.ExtractManufacturer(node);
            return Regex.Match(raw, @"\w+").Value;
        }

        protected override Uri ProduceFullSourceUri(string anchorHrefValue)
        {
            // gets rid of the fugly via qs parameter: https://www.woodcraft.com/products/laguna-revo-18-36-lathe-110v-1-5-hp?via=573621bd69702d0676000002%2C573621db69702d0676000d9d%2C5763fea669702d6582000b72
            return new Uri(base.ProduceFullSourceUri(anchorHrefValue).GetLeftPart(UriPartial.Path));
        }

        public override void Run()
        {
            AddRange();
        }
    }
}
