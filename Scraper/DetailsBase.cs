using System;
using HtmlAgilityPack;

namespace Scraper
{
    abstract class DetailsBase : ScraperBase
    {
        protected abstract string DetailsLinkXPath { get; }

        protected override void AddPriceFromContainerNode(HtmlNode containerNode, string manufacturer)
        {
            var web = new HtmlWeb();
            web.PreRequest += HandlePreRequest;
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
