using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;

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

        protected void Add(CapturedPrice price)
        {
            prices.Add(price);
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
                Source = source,
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
