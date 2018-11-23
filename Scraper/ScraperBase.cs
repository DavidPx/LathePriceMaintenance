using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml.XPath;

namespace Scraper
{
    public abstract class ScraperBase
    {
        DirectoryInfo outputLocation;
        List<CapturedPrice> prices;

        public ScraperBase()
            : this(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "output")))
        {
        }

        public ScraperBase(DirectoryInfo outputLocation)
        {
            this.outputLocation = outputLocation;
            prices = new List<CapturedPrice>();
        }

        public abstract void Run();

        protected abstract Uri SourceUri { get; }
        protected abstract string FriendlyName { get; }

        protected abstract string ContainerXPath { get; }
        protected abstract string PriceXPath { get; }
        protected abstract string SkuXPath { get; }
        protected abstract string SourceUriAnchorXPath { get; }
        protected virtual string ManufacturerXPath { get; }

        protected void Add(CapturedPrice price)
        {
            prices.Add(price);
        }

        protected virtual string ExtractPrice(HtmlNode node)
        {
            return node.InnerTextClean();
        }

        protected virtual string ExtractSku(HtmlNode node)
        {
            return node.InnerTextClean();
        }

        protected virtual string ExtractManufacturer(HtmlNode node)
        {
            return node.InnerTextClean();
        }

        protected virtual Uri ProduceFullSourceUri(string anchorHrefValue)
        {
            if (string.IsNullOrWhiteSpace(anchorHrefValue))
                throw new ArgumentException("Anchor href value is bad", nameof(anchorHrefValue));

            if (anchorHrefValue.StartsWith("/"))
                return new Uri(SourceUri.AbsoluteUri + anchorHrefValue);

            return new Uri(anchorHrefValue);
        }

        protected virtual bool HandlePreRequest(HttpWebRequest request)
        {
            return true;
        }

        protected void AddRangeKnownManufacturer(string manufacturer)
        {
            if (string.IsNullOrWhiteSpace(manufacturer) && string.IsNullOrWhiteSpace(ManufacturerXPath))
                throw new InvalidOperationException("Provider either a manufacturer or xpath to get it");

            var web = new HtmlWeb();
            web.PreRequest += HandlePreRequest;
            
            var document = web.Load(SourceUri);

            if (web.StatusCode != HttpStatusCode.OK)
                throw new InvalidOperationException($"{FriendlyName} returned {web.StatusCode}, not OK");
            
            var containers = document.DocumentNode.SelectNodes(ContainerXPath);

            if (containers == null)
                throw new InvalidOperationException($"Unable to find containers element on {FriendlyName}");

            foreach (var container in containers)
            {
                var priceNode = container.SelectSingleNode(PriceXPath);
                if (priceNode == null)
                {
                    Console.WriteLine($"Unable to find price node! Skipping.  inner text of container: {container.InnerTextClean()}");
                    continue;
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

        protected void AddRange()
        {
            if (string.IsNullOrWhiteSpace(ManufacturerXPath))
                throw new ArgumentException("Manufacturer xpath not set", nameof(ManufacturerXPath));
            AddRangeKnownManufacturer(null);
        }

        protected void Add(string price, string manufacturerSku, string manufacturer, string source)
        {
            Add(price, manufacturerSku, manufacturer, new Uri(source));
        }

        protected void Add(string price, string manufacturerSku, string manufacturer, Uri source)
        {
            if (decimal.TryParse(price, NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out decimal parsedPrice))
            {
                var data = new CapturedPrice
                {
                    Price = parsedPrice,
                    ManufacturerSku = manufacturerSku.Trim(),
                    Manufacturer = manufacturer.Trim(),
                    Source = source.ToString(),
                    Extraction_Time = DateTimeOffset.Now
                };
                prices.Add(data);
            }
            else
            {
                Console.WriteLine($"Unable to parse price '{price}' for {manufacturerSku}, {source}");
            }
        }

        protected void Save(string fileNameNoExtension)
        {
            if (!outputLocation.Exists)
                outputLocation.Create();

            var file = new FileInfo(Path.Combine(outputLocation.FullName, fileNameNoExtension + ".csv"));

            using (var writer = new StreamWriter(file.FullName))
            {
                var csv = new CsvWriter(writer, false);

                csv.WriteRecords(prices);
            }
        }
    }
}
