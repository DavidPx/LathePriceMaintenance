using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;

namespace Scraper.Bases
{
    /// <summary>
    /// A base that works off a list of page links, scraping from a similar structure for each
    /// </summary>
    internal abstract class ListOfUrisBase : PriceDetailsXPathBase
    {
        private readonly PageLoadMode loadMode;

        protected ListOfUrisBase(PageLoadMode loadMode)
        {
            this.loadMode = loadMode;
        }

        protected void AddFromUris(Uri[] uris, string manufacturer)
        {
            Console.WriteLine($"Fetching {FriendlyName}, iterating through {uris.Length} pages...");

            ChromeDriver driver = null;

            try
            {
                if (loadMode == PageLoadMode.Selenium)
                {
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("headless");

                    driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);
                }

                foreach (var pageUri in uris)
                {
                    HtmlDocument document;

                    if (loadMode == PageLoadMode.Selenium)
                    {
                        var nav = driver.Navigate();
                        
                        nav.GoToUrl(pageUri);
                        var source = driver.PageSource;

                        document = new HtmlDocument();
                        document.LoadHtml(source);
                    }
                    else
                    {
                        var web = new HtmlWeb();
                        document = web.Load(pageUri);
                    }

                    AddPriceFromHtmlNode(manufacturer, pageUri, document.DocumentNode);
                }
            }
            finally
            {
                driver.Dispose();
            }
        }

        protected void AddFromUris(Uri[] uris)
        {
            AddFromUris(uris, null);
        }
        
    }
}
