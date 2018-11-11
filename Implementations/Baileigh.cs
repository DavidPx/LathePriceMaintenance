using HtmlAgilityPack;
using System;

namespace Scraper.Implementations
{
    class Baileigh : ScraperBase
    {
        const string manufacturer = "Baileigh";

        public override void Run()
        {
            var source = new Uri("https://www.baileigh.com/woodworking/lathes/lathes");

            var web = new HtmlWeb();
            var document = web.Load(source);
            
            var containers = document.DocumentNode.SelectNodes("//li[starts-with(@class, 'item')]");
            
            foreach (var container in containers)
            {
                var rawPrice = container.SelectSingleNode(".//span[@class='price']").InnerText;
                var rawSku = container.SelectSingleNode(".//h2[@class='product-name']").InnerText;
                
                Add(rawPrice, rawSku, manufacturer, source.ToString());
            }

            Save(manufacturer);
        }
    }
}
