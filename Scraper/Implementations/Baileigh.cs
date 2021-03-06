﻿using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Scraper.Bases;

namespace Scraper.Implementations
{
    class Baileigh : AllInOneBase
    {
        const string manufacturer = "Baileigh";

        protected override Uri StartingUri => new Uri("https://www.baileigh.com/woodworking/lathes/lathes");

        public override string FriendlyName => manufacturer;
        protected override string FileName => FriendlyName;

        protected override string ContainerXPath => "//li[starts-with(@class, 'item')]";

        protected override string PriceXPath => ".//span[@class='price']";

        protected override string SkuXPath => ".//h2[@class='product-name']";

        protected override string SourceUriAnchorXPath => ".//a[@title='View Details']";

        protected override string ExtractSku(HtmlNode node)
        {
            // Bench Top Wood Lathe WL-1218VS
            var raw = base.ExtractSku(node);
            return Regex.Match(raw, @"\S+$").Value;
        }

        public override void Run()
        {
            AddRangeKnownManufacturer(manufacturer);
        }
        
    }
}
