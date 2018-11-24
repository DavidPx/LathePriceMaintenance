using HtmlAgilityPack;
using System;
using System.Net;

namespace Scraper.Bases
{
    /// <summary>
    /// What most implementations will inherit from; defines the common set of xpaths and iterative methods
    /// </summary>
    abstract class XPathBase : ScraperBase
    {
        protected abstract string ContainerXPath { get; }
        protected abstract string PriceXPath { get; }
        protected abstract string SkuXPath { get; }
        protected virtual string ManufacturerXPath { get; }

        protected abstract Uri StartingUri { get; }

        /// <summary>
        ///  Loads the document directly using HtmlAgilityPack
        /// </summary>
        /// <returns></returns>
        protected virtual HtmlDocument LoadDocument()
        {
            var web = new HtmlWeb();

            Console.WriteLine($"Fetching {FriendlyName}...");

            var doc = web.Load(StartingUri);

            if (web.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException($"{FriendlyName} returned {web.StatusCode}, not OK");

            return doc;
        }

        /// <summary>
        /// Find the attributes of the record from the container node and add a price
        /// </summary>
        /// <param name="containerNode"></param>
        /// <param name="manufacturer"></param>
        protected abstract void AddPriceFromContainerNode(HtmlNode containerNode, string manufacturer);


        protected void AddRangeKnownManufacturer(string manufacturer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(manufacturer) && string.IsNullOrWhiteSpace(ManufacturerXPath))
                    throw new InvalidOperationException("Provider either a manufacturer or xpath to get it");

                var document = LoadDocument();

                var containers = document.DocumentNode.SelectNodes(ContainerXPath);

                if (containers == null)
                    throw new InvalidOperationException($"Unable to find containers element on {FriendlyName}");

                Console.WriteLine($"{FriendlyName}, {containers.Count} containers.");

                foreach (var container in containers)
                {
                    AddPriceFromContainerNode(container, manufacturer);
                    Console.Write(".");
                }
                Console.WriteLine("");

                Save(FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception on {FriendlyName}: {e}");
            }
        }

        protected void AddRange()
        {
            if (string.IsNullOrWhiteSpace(ManufacturerXPath))
                throw new ArgumentException("Manufacturer xpath not set", nameof(ManufacturerXPath));
            AddRangeKnownManufacturer(null);
        }
    }
}
