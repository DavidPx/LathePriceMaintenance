using HtmlAgilityPack;
using System;

namespace Scraper
{
    public static class Extensions
    {
        public static string InnerTextClean(this HtmlNode node)
        {
            return System.Net.WebUtility.HtmlDecode(node.InnerText)
                .Replace('\n', ' ')
                .Trim();
        }

        public static Uri MakeFullUri(this string relativePath, Uri source)
        {
            return new Uri(new Uri(source.AbsoluteUri), relativePath);
        }
    }
}
