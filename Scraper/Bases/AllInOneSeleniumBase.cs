using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace Scraper.Bases
{
    /// <summary>
    /// Loads the document using Selenium Chromedriver
    /// </summary>
    abstract class AllInOneSeleniumBase : AllInOneBase
    {
        protected override HtmlDocument LoadDocumentWithSelenium()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("headless");

            // The Chromedriver package places the driver EXE in the output directory
            using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions))
            {
                var nav = driver.Navigate();
                nav.GoToUrl(StartingUri);
                var source = driver.PageSource;

                var doc = new HtmlDocument();
                doc.LoadHtml(source);
                return doc;
            }
        }
    }
}
