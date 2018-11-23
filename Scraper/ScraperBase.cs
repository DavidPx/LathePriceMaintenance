using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
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
        protected abstract string SkuPriceXPath { get; }
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

        protected void AddRangeKnownManufacturer(string manufacturer)
        {
            if (string.IsNullOrWhiteSpace(manufacturer) && string.IsNullOrWhiteSpace(ManufacturerXPath))
                throw new InvalidOperationException("Provider either a manufacturer or xpath to get it");

            var web = new HtmlWeb();
            var document = web.Load(SourceUri);

            var containers = document.DocumentNode.SelectNodes(ContainerXPath);

            foreach (var container in containers)
            {
                var rawPrice = ExtractPrice(container.SelectSingleNode(PriceXPath));
                var rawSku = ExtractSku(container.SelectSingleNode(SkuPriceXPath));
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
            var data = new CapturedPrice
            {
                Price = decimal.Parse(price, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowCurrencySymbol),
                ManufacturerSku = manufacturerSku.Trim(),
                Manufacturer = manufacturer.Trim(),
                Source = source.ToString(),
                Extraction_Time = DateTimeOffset.Now
            };
            prices.Add(data);
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
