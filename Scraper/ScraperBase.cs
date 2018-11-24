using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

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
            Exclusions = new List<ManufacturerExclusion>();
            this.outputLocation = outputLocation;
            prices = new List<CapturedPrice>();
        }

        /// <summary>
        /// SKUs matching one of these won't be added
        /// </summary>
        protected virtual IList<ManufacturerExclusion> Exclusions { get; }

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
                
        public abstract string FriendlyName { get; }
        protected abstract string FileName { get; }
        
        protected void Add(string price, string manufacturerSku, string manufacturer, string source)
        {
            Add(price, manufacturerSku, manufacturer, new Uri(source));
        }

        protected void Add(string price, string manufacturerSku, string manufacturer, Uri source)
        {
            if (decimal.TryParse(price, NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out decimal parsedPrice))
            {
                Add(parsedPrice, manufacturerSku, manufacturer, source);
            }
            else
            {
                Console.WriteLine($"Unable to parse price '{price}' for {manufacturerSku}, {source}");
            }
        }

        protected void Add(decimal price, string manufacturerSku, string manufacturer, Uri source)
        {
            if (Exclusions.Any(
                r => manufacturer.Equals(r.Manufacturer, StringComparison.OrdinalIgnoreCase)
                && r.SkuExclusionRegexes.Any(x => Regex.IsMatch(manufacturerSku, x))))
                return;

            var data = new CapturedPrice
            {
                Price = price,
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
