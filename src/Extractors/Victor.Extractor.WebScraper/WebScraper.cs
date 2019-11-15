using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using CsQuery;

namespace Victor
{
    public class WebScraper : Api
    {
        static WebScraper() {}


        //public static Regex urlRegex = new Regex(@"(http|https):\/\/[\w\-_]+(\.[\w\-_] +)+[\w\-\.,@?^=%&amp;:\/~\+#]*[\w\-\@?^=%&amp;\/~\+#]", RegexOptions.Compiled);
        public static Regex urlRegex = new Regex(@"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=% &amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Compiled);
        
        public static Link[] ExtractLinksFromHtmlFrag(string html)
        {
            CQ dom = html;
            var links = dom["a"];
            if (links != null && links.Count() > 0)
            {
                return links.Select(l => new Link()
                {
                    Uri = l.HasAttribute("href") && Uri.TryCreate(l.GetAttribute("href"), UriKind.RelativeOrAbsolute, out Uri u) ? u : null,
                    HtmlAttributes = l.Attributes.ToDictionary(a => a.Key, a => a.Value),
                    InnerHtml = l.InnerHTML,
                    InnerText = l.InnerText
                }).ToArray();

            }
            else
            {
                return Array.Empty<Link>();
            }
        }

        public static string ExtractTextFromHtmlFrag(string html)
        {
            CQ dom = html;
            return dom.Text();
        }

        public static string RemoveUrlsFromText(string text) => urlRegex.Replace(text, "");

        public static async Task<byte[]> GetImageFromUrlAsync(string url)
        {
            try
            {
                return await HttpClient.GetByteArrayAsync(url);
            }
            catch(Exception e)
            {
                Error(e, "Error occurred reading URL {0}.", url);
                return null;
            }
        }  
    }
}
