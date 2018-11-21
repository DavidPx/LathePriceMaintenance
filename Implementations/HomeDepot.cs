using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace Scraper.Implementations
{
    class HomeDepot : ScraperBase
    {
        public override string FriendlyName => "Home Depot";

        public override void Run()
        {
            var source = new Uri("https://www.homedepot.com/b/Tools-Power-Tools-Woodworking-Tools-Lathes/N-5yc1vZc289");

            var web = new HtmlWeb();
            var document = web.Load(source);

            var containers = document.DocumentNode.SelectNodes("//div[@data-section='gridview']/div[contains(@class,'js-pod')]");

            foreach (var container in containers)
            {
                var rawSku = container.SelectSingleNode(".//div[@class='pod-plp__model']").InnerTextClean();
                var rawPrice = container.SelectSingleNode(".//div[@class='price__numbers']").InnerTextClean();
                var rawManufacturer = container.SelectSingleNode(".//span[@class='pod-plp__brand-name']").InnerTextClean();
                var sourceUri = container.SelectSingleNode(".//a[@data-pod-type='pr']").GetAttributeValue("href", "").MakeFullUri(source);
                
                rawSku = rawSku.Replace("Model#", "").Trim();
                rawPrice = Regex.Replace(rawPrice, @"(\d\d)$", ".$1");
                               
                Add(rawPrice, rawSku, rawManufacturer, sourceUri);
            }

            Save("HomeDepot");
        }
        
    }
}
