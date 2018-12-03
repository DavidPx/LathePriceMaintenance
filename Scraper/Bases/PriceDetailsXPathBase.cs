using HtmlAgilityPack;
using System;
using System.Net;

namespace Scraper.Bases
{
    /// <summary>
    /// A deep implementation exposing xpaths to the basic attributes about a product but not how to access the product.
    /// </summary>
    abstract class PriceDetailsXPathBase : ScraperBase
    {
        protected abstract string PriceXPath { get; }
        protected abstract string SkuXPath { get; }
        protected virtual string ManufacturerXPath { get; }


        protected void AddPriceFromHtmlNode(string manufacturer, Uri uri, HtmlNode node)
        {
            var rawPrice = base.ExtractPrice(node.SelectSingleNode(PriceXPath));
            var rawSku = base.ExtractSku(node.SelectSingleNode(SkuXPath));
            string rawManufacturer;

            if (string.IsNullOrEmpty(manufacturer))
            {
                rawManufacturer = base.ExtractManufacturer(node.SelectSingleNode(ManufacturerXPath));
            }
            else
            {
                rawManufacturer = manufacturer;
            }

            Add(rawPrice, rawSku, rawManufacturer, uri);
        }

    }
}
