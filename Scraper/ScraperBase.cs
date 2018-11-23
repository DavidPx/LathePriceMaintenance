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
    abstract class ScraperBase
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

        protected abstract string ContainerXPath { get; }
        protected abstract string PriceXPath { get; }
        protected abstract string SkuXPath { get; }
        protected virtual string ManufacturerXPath { get; }

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

        public abstract void Run();

        protected void Add(CapturedPrice price)
        {
            prices.Add(price);
        }

        protected virtual bool HandlePreRequest(HttpWebRequest request)
        {
            return true;
        }

        protected abstract Uri StartingUri { get; }
        public abstract string FriendlyName { get; }
        protected abstract string FileName { get; }

        protected abstract void AddPriceFromContainerNode(HtmlNode containerNode, string manufacturer);

        protected void AddRangeKnownManufacturer(string manufacturer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(manufacturer) && string.IsNullOrWhiteSpace(ManufacturerXPath))
                    throw new InvalidOperationException("Provider either a manufacturer or xpath to get it");

                var web = new HtmlWeb();
                web.PreRequest += HandlePreRequest;

                Console.WriteLine($"Fetching {FriendlyName}...");

                var document = web.Load(StartingUri);

                if (web.StatusCode != HttpStatusCode.OK)
                    throw new InvalidOperationException($"{FriendlyName} returned {web.StatusCode}, not OK");

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
