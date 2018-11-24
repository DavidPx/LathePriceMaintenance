using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Scraper.Implementations
{
    /// <summary>
    /// I'm using Packard to get OneWay's pricing.
    /// </summary>
    class Packard : ScraperBase
    {
        private const string Manufacturer = "Oneway";

        public override string FriendlyName => nameof(Packard);

        protected override string FileName => FriendlyName;
        
        public override void Run()
        {
            var web = new HtmlWeb();

            // Packard code, SKU
            var hpUpgrades = new[] { "2HP", "3HP" }; // Packard's codes for the HP upgrades on the page

            Extract(
                web, 
                new Dictionary<string, string> { { "1224B", "1224" } }, 
                hpUpgrades, 
                new Uri("http://www.packardwoodworks.com/Merchant2/merchant.mvc?Screen=CTGY&Store_Code=packard&Category_Code=lathes-oneway-1224"));

            Extract(
                web,
                new Dictionary<string, string> { { "1640", "1640" } },
                hpUpgrades,
                new Uri("http://www.packardwoodworks.com/Merchant2/merchant.mvc?Screen=CTGY&Store_Code=packard&Category_Code=lathes-oneway-1640"));

            Extract(
                web,
                new Dictionary<string, string> { { "2016", "2016" }, { "2036", "2036" }, { "2416", "2416" }, { "2436", "2436" } },
                hpUpgrades,
                new Uri("http://www.packardwoodworks.com/Merchant2/merchant.mvc?Screen=CTGY&Store_Code=packard&Category_Code=lathes-oneway-20-24-ct"));

            Save(FileName);
        }

        private void Extract(HtmlWeb web, IDictionary<string, string> skus, IList<string> hpUpgrades, Uri uri)
        {
            var prices = GetAllPricesFromTable(web, uri);

            var hpUpgradesOnPage = prices.Where(p => hpUpgrades.Any(x => x == p.Key));

            foreach (var codeAndSku in skus)
            {
                if (prices.ContainsKey(codeAndSku.Key))
                {
                    var matchingRow = prices[codeAndSku.Key];

                    // base model
                    Add(matchingRow, codeAndSku.Value, Manufacturer, uri);

                    // upgrades!
                    if (hpUpgradesOnPage.Any())
                    {
                        foreach (var hpUpgrade in hpUpgradesOnPage)
                        {
                            Add(hpUpgrade.Value + matchingRow, $"{codeAndSku.Value}-{hpUpgrade.Key}", Manufacturer, uri);
                        }
                    }
                    
                }
            }
        }

        private IDictionary<string, decimal> GetAllPricesFromTable(HtmlWeb web, Uri uri)
        {
            var doc = web.Load(uri);

            var products = doc.DocumentNode.SelectNodes("//table[@class='prod_list']/tr[.//td]"); // only select TRs with TDs

            var results = new Dictionary<string, decimal>();
            foreach (var productRow in products)
            {
                var code = productRow.SelectSingleNode("./td[1]").InnerTextClean();
                var price = ExtractPrice(productRow.SelectSingleNode("./td[3]"));

                if (decimal.TryParse(price, NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out decimal parsedPrice))
                {
                    results.Add(code, parsedPrice);
                }
            }
            return results;
        }
    }
}
