using System;
using System.Collections.Generic;

namespace Scraper.Implementations
{
    class FactoryAuthorizedOutlet : AllInOneBase
    {
        public override string FriendlyName => "Factory Authorized Outlet";

        protected override string SourceUriAnchorXPath => ".//a";

        protected override string ContainerXPath => "//div[contains(@class, 'products-grid')]/ol/li";

        protected override string PriceXPath => ".//span[@class='price']";

        protected override string SkuXPath => ".//span[@class='model-number']";

        protected override Uri StartingUri => new Uri("https://www.factoryauthorizedoutlet.com/tools-equipment/power-tools/lathes?item_type=14497&product_list_limit=160");

        protected override string FileName => nameof(FactoryAuthorizedOutlet);

        protected override string ManufacturerXPath => ".//span[@class='brand-name']";

        protected override IList<ManufacturerExclusion> Exclusions => new[] 
        {
            new ManufacturerExclusion
            {
                Manufacturer = "Jet",
                SkuExclusionRegexes = new []
                {
                    @"^3[23]\d+"
                }
            }
        };

        public override void Run()
        {
            AddRange();
        }
    }
}
