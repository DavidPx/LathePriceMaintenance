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

        public abstract string FriendlyName { get; }

        protected void Add(CapturedPrice price)
        {
            prices.Add(price);
        }

        protected void AddRangeKnownManufacturer(Uri page, string containerPath, string pricePath, string manufacturerSkuPath, string manufacturer, string sourcePath)
        {
            var web = new HtmlWeb();
            var document = web.Load(page);

            var containers = document.DocumentNode.SelectNodes(containerPath);

            foreach (var container in containers)
            {
                var rawPrice = container.SelectSingleNode(pricePath).InnerTextClean();
                var rawSku = container.SelectSingleNode(manufacturerSkuPath).InnerTextClean();
                var sourceUri = container.SelectSingleNode(sourcePath).GetAttributeValue("href", "");

                if (string.IsNullOrWhiteSpace(sourceUri))
                    throw new InvalidOperationException("Source URI not found");

                Add(rawPrice, rawSku, manufacturer, sourceUri);
            }
        }

        protected void AddRange(Uri page, string containerPath, string pricePath, string manufacturerSkuPath, string manufacturerPath, string sourcePath)
        {
            var web = new HtmlWeb();
            var document = web.Load(page);

            var containers = document.DocumentNode.SelectNodes(containerPath);

            foreach (var container in containers)
            {
                var rawPrice = container.SelectSingleNode(pricePath).InnerTextClean();
                var rawSku = container.SelectSingleNode(manufacturerSkuPath).InnerTextClean();
                var sourceUri = container.SelectSingleNode(sourcePath).GetAttributeValue("href", "");
                var rawManufacturer = container.SelectSingleNode(manufacturerPath).InnerTextClean();

                if (string.IsNullOrWhiteSpace(sourceUri))
                    throw new InvalidOperationException("Source URI not found");

                Add(rawPrice, rawSku, rawManufacturer, sourceUri);
            }
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
