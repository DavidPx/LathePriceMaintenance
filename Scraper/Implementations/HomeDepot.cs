﻿using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using Scraper.Bases;

namespace Scraper.Implementations
{
    class HomeDepot : AllInOneBase
    {
        public override string FriendlyName => "Home Depot";
        protected override string FileName => nameof(HomeDepot);

        protected override Uri StartingUri => new Uri("https://www.homedepot.com/b/Tools-Power-Tools-Woodworking-Tools-Lathes/N-5yc1vZc289");

        protected override string ContainerXPath => "//div[@data-section='gridview']/div[contains(@class,'js-pod')]";

        protected override string PriceXPath => ".//div[@class='price__numbers']";

        protected override string SkuXPath => ".//div[@class='pod-plp__model']";

        protected override string SourceUriAnchorXPath => ".//a[@data-pod-type='pr']";

        protected override string ManufacturerXPath => ".//span[@class='pod-plp__brand-name']";

        protected override string ExtractSku(HtmlNode node)
        {
            var raw = base.ExtractSku(node);
            return raw.Replace("Model#", "");
        }

        protected override string ExtractPrice(HtmlNode node)
        {
            var raw = base.ExtractPrice(node);
            return Regex.Replace(raw, @"(\d\d)$", ".$1");
        }

        protected override Uri ProduceFullSourceUri(string anchorHrefValue)
        {
            return anchorHrefValue.MakeFullUri(StartingUri);
        }

        public override void Run()
        {
            AddRange();
        }
        
    }
}
