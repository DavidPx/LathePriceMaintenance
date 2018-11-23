using System.Collections.Generic;

namespace Scraper
{
    class ManufacturerExclusion
    {
        public string Manufacturer { get; set; }
        public IList<string> SkuExclusionRegexes { get; set; }
    }
}
