using System;
using HtmlAgilityPack;

namespace Scraper.Bases
{
    /// <summary>
    /// Drills into each container link in order to fetch details
    /// </summary>
    abstract class DetailsBase : XPathBase
    {
        protected abstract string DetailsLinkXPath { get; }

        protected override void AddPriceFromContainerNode(HtmlNode containerNode, string manufacturer)
        {
            var web = new HtmlWeb();
            var detailsUri = new Uri(containerNode.SelectSingleNode(DetailsLinkXPath).GetAttributeValue("href", ""));

            var document = web.Load(detailsUri);

            var docNode = document.DocumentNode;

            var rawPrice = base.ExtractPrice(docNode.SelectSingleNode(PriceXPath));
            var rawSku = base.ExtractSku(docNode.SelectSingleNode(SkuXPath));
            string rawManufacturer;

            if (string.IsNullOrEmpty(manufacturer))
            {
                rawManufacturer = base.ExtractManufacturer(docNode.SelectSingleNode(ManufacturerXPath));
            }
            else
            {
                rawManufacturer = manufacturer;
            }

            Add(rawPrice, rawSku, rawManufacturer, detailsUri);
        }
    }
}
