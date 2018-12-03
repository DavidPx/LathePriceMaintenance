using HtmlAgilityPack;
using Scraper.Bases;
using System;
using System.Text.RegularExpressions;

namespace Scraper.Implementations
{
    class Amazon : ListOfUrisBase
    {
        internal Amazon() : base(PageLoadMode.HtmlAgilityPack)
        {
        }

        public override string FriendlyName => nameof(Amazon);

        protected override string PriceXPath => "(//span[contains(@class, 'olpOfferPrice')])[1]";

        protected override string SkuXPath => "//h1/div[contains(@class, 'olpSubHeadingSection')]";

        protected override string FileName => nameof(Amazon);

        protected override string ManufacturerXPath => SkuXPath;

        protected override string ExtractManufacturer(HtmlNode node)
        {
            // WEN 3420 8" by 12" Variable Speed Benchtop Wood Lathe
            var raw = base.ExtractManufacturer(node);
            return Regex.Matches(raw, @"\w+")[0].Value;
        }

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            return Regex.Matches(raw, @"\w+")[1].Value;
        }

        public override void Run()
        {
            var uris = new Uri[]
            {
                new Uri("https://www.amazon.com/gp/offer-listing/B072JBP61N")
            };
            AddFromUris(uris);
        }
    }
}
