using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Scraper.Implementations
{
    class Robust : ScraperBase
    {
        public override string FriendlyName => nameof(Robust);

        protected override string FileName => FriendlyName;

        public override void Run()
        {
            Console.WriteLine($"Fetching {FriendlyName}...");

            var web = new HtmlWeb();

            Extract(
                web,
                new Uri("http://www.turnrobust.com/product/american-beauty-2/"), 
                new Dictionary<string, string> { { "product-4077", "AB-Std" }, { "product-4078", "AB-Lng" } },
                new Dictionary<string, string> { { "product-4079", "3HP" } }
                );

            Extract(
                web,
                new Uri("http://www.turnrobust.com/product/sweet-16-2/"),
                new Dictionary<string, string> { { "product-4098", "S16-Shrt" }, { "product-4099", "S16-Std" }, { "product-4100", "S16-Lng" } },
                new Dictionary<string, string> { { "product-4101", "2HP" } }
                );

            Extract(
                web,
                new Uri("http://www.turnrobust.com/product/scout/"),
                new Dictionary<string, string> { { "product-4107", "Scout" } },
                new Dictionary<string, string> { { "product-3448", "1.5HP" }, { "product-3458", "Legs" } }
                );

            Save(FileName);
        }

        private void Extract(HtmlWeb web, Uri uri, Dictionary<string, string> skus, Dictionary<string, string> upgrades)
        {
            var rows = GetAllPricesFromTable(web, uri);

            foreach (var sku in skus)
            {
                if (rows.ContainsKey(sku.Key))
                {
                    var basePrice = rows[sku.Key];

                    // Base model
                    string modelNumber = sku.Value;
                    Add(basePrice, modelNumber, FriendlyName, uri);

                    // upgrades
                    // this produces a 2-D list of upgrade combinations.  {a,b} -> {a}, {b}, {a,b}.  https://stackoverflow.com/a/30082360/89176
                    var allCombos =
                        Enumerable.Range(1, (1 << upgrades.Count) - 1)
                        .Select(i => upgrades.Where((upgrade, n) => ((1 << n) & i) != 0));

                    foreach (var combination in allCombos)
                    {
                        var totalPrice = basePrice; // base price
                        var combinedSku = modelNumber; // base model number

                        // combination is a list of possible upgrades
                        foreach (var upgrade in combination)
                        {
                            if (rows.ContainsKey(upgrade.Key))
                            {
                                totalPrice += rows[upgrade.Key];
                                combinedSku += $"-{upgrade.Value}";
                            }
                            else
                            {
                                Console.WriteLine($"Upgrade {upgrade.Key} not found on {uri}!");
                            }
                        }
                        Add(totalPrice, combinedSku, FriendlyName, uri);
                    }
                }
                else
                {
                    Console.WriteLine($"Sku {sku.Key} not found on {uri}!");
                }
            }
        }

        

        // product-<id>, price
        private IDictionary<string, decimal> GetAllPricesFromTable(HtmlWeb web, Uri uri)
        {
            var doc = web.Load(uri);

            var products = doc.DocumentNode.SelectNodes("//table[contains(@class, 'woocommerce-grouped-product-list group_table')]//tr");

            var results = new Dictionary<string, decimal>();
            foreach (var productRow in products)
            {
                var code = productRow.GetAttributeValue("id", ""); // product-123

                if (string.IsNullOrWhiteSpace(code))
                {
                    Console.WriteLine($"table row without product ID: {productRow.InnerText}");
                    continue;
                }

                var price = ExtractPrice(productRow.SelectSingleNode(".//span[contains(@class, 'amount')]")).Replace("$", "").Trim();

                if (decimal.TryParse(price, NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out decimal parsedPrice))
                {
                    results.Add(code, parsedPrice);
                }
                else
                {
                    Console.WriteLine($"unable to parse price for {code} on {uri}");
                }

            }
            return results;
        }
    }
}
