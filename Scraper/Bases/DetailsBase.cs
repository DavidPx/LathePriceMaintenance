using System;
using HtmlAgilityPack;

namespace Scraper.Bases
{
    /// <summary>
    /// Drills into each container link in order to fetch details
    /// </summary>
    abstract class DetailsBase : ContainerXPathBase
    {
        protected abstract string DetailsLinkXPath { get; }

        protected override void AddPriceFromContainerNode(HtmlNode containerNode, string manufacturer)
        {
            var web = new HtmlWeb();
            var detailsUri = new Uri(containerNode.SelectSingleNode(DetailsLinkXPath).GetAttributeValue("href", ""));

            var document = web.Load(detailsUri);

            var docNode = document.DocumentNode;
            AddPriceFromHtmlNode(manufacturer, detailsUri, docNode);
        }
    }
}
