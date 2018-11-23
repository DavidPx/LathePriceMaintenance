﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Implementations
{
    class BusyBee : ScraperBase
    {
        protected override Uri SourceUri => new Uri("https://www.busybeetools.com/categories/Woodworking/Wood-Lathes/Lathes/");

        protected override string FriendlyName => nameof(BusyBee);

        protected override string ContainerXPath => "//ul[@class='productGrid']/li";

        protected override string PriceXPath => ".//span[contains(@class, 'price--main')]";

        protected override string SkuXPath => ".//div[@data-test-info-type='sku']";

        protected override string SourceUriAnchorXPath => ".//h4/a";
        
        public override void Run()
        {
            AddRangeKnownManufacturer("Craftex");
            Save(FriendlyName);
        }
    }
}