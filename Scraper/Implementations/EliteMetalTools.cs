using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Implementations
{
    class EliteMetalTools : DetailsBase
    {
        protected override Uri StartingUri => new Uri("https://www.elitemetaltools.com/tool-shop/woodworking/lathes?items_per_page=60");

        public override string FriendlyName => "Elite Metal Tools";
        protected override string FileName => nameof(EliteMetalTools);

        protected override string ContainerXPath => "//div[@class = 'col-md-12']//div[@class = 'views-field views-field-nothing']";

        protected override string DetailsLinkXPath => ".//span[@class='category-product-list-title']/a";

        protected override string PriceXPath => "//td[@id='focus_price']";

        protected override string SkuXPath => "//td[@id='focus_sku']";

        protected override string ManufacturerXPath => "//td[contains(@class, 'views-field-field-manufacturer')]";

        public override void Run()
        {
            AddRange();
        }
    }
}
