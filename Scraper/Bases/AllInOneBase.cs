using HtmlAgilityPack;
using System;

namespace Scraper.Bases
{
    /// <summary>
    /// Gets all details from the information in each container
    /// </summary>
    abstract class AllInOneBase : ScraperBase
    {
        protected abstract string SourceUriAnchorXPath { get; }
        
        protected virtual Uri ProduceFullSourceUri(string anchorHrefValue)
        {
            if (string.IsNullOrWhiteSpace(anchorHrefValue))
                throw new ArgumentException("Anchor href value is bad", nameof(anchorHrefValue));

            if (anchorHrefValue.StartsWith("/"))
                return new Uri(StartingUri.GetLeftPart(UriPartial.Authority) + anchorHrefValue);

            return new Uri(anchorHrefValue);
        }
        
        protected override void AddPriceFromContainerNode(HtmlNode container, string manufacturer)
        {
            var priceNode = container.SelectSingleNode(PriceXPath);
            if (priceNode == null)
            {
                Console.WriteLine($"Unable to find price node! Skipping.  inner text of container: {container.InnerTextClean()}");
                return;
            }

            var rawPrice = ExtractPrice(priceNode);
            var rawSku = ExtractSku(container.SelectSingleNode(SkuXPath));
            var sourceUri = ProduceFullSourceUri(container.SelectSingleNode(SourceUriAnchorXPath).GetAttributeValue("href", ""));

            string rawManufacturer;
            if (string.IsNullOrWhiteSpace(manufacturer))
                rawManufacturer = ExtractManufacturer(container.SelectSingleNode(ManufacturerXPath));
            else
                rawManufacturer = manufacturer;

            Add(rawPrice, rawSku, rawManufacturer, sourceUri);
        }
    }
}
