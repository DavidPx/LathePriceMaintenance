using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace Scraper.Implementations
{
    class HomeDepot : ScraperBase
    {
        public override string FriendlyName => "Home Depot";

        public override Uri SourceUri => new Uri("https://www.homedepot.com/b/Tools-Power-Tools-Woodworking-Tools-Lathes/N-5yc1vZc289");

        public override string ContainerXPath => "//div[@data-section='gridview']/div[contains(@class,'js-pod')]";

        public override string PriceXPath => ".//div[@class='price__numbers']";

        public override string SkuPriceXPath => ".//div[@class='pod-plp__model']";

        public override string SourceUriAnchorXPath => ".//a[@data-pod-type='pr']";

        public override string ManufacturerXPath => ".//span[@class='pod-plp__brand-name']";

        protected override string ExtractSku(HtmlNode node)
        {
            return node.InnerTextClean().Replace("Model#", "");
        }

        protected override string ExtractPrice(HtmlNode node)
        {
            return Regex.Replace(node.InnerTextClean(), @"(\d\d)$", ".$1");
        }

        protected override Uri ProduceFullSourceUri(string anchorHrefValue)
        {
            return anchorHrefValue.MakeFullUri(SourceUri);
        }

        public override void Run()
        {
            AddRange();
            Save("HomeDepot");
        }
        
    }
}
