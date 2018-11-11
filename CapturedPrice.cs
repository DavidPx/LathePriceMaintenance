using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper
{
    public class CapturedPrice
    {
        // Price	ManufacturerSku	Manufacturer	Source	Extraction_Time
        public decimal Price { get; set; }
        public string ManufacturerSku { get; set; }
        public string Manufacturer { get; set; }
        public Uri Source { get; set; }
        public DateTimeOffset Extraction_Time { get; set; }
    }
}
