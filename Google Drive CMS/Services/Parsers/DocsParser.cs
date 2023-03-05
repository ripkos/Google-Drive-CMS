using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Google_Drive_CMS.Services.Parsers
{
    public partial class DocsParser
    {
        private readonly HttpClient client;
        public DocsParser()
        {
        }
        public DocsParser(HttpClient httpClinet)
        {
            client = httpClinet;
        }

        public async Task<DocsDTO> GetDocsAsync(string generalUrl)
        {
            var url = $"https://docs.google.com/feeds/download/documents/export/Export?id={MyRegex().Match(generalUrl).Value[2..^1]}&exportFormat=html";
            string responseBody = await client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseBody);
            var styleNode = htmlDoc.DocumentNode.SelectSingleNode("//head/style");
            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
            var xpath = "//*[self::h1 or self::h2 or self::h3 or self::h4]";
            List<(string, string)> headers = new();
            var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
            if(nodes is not null)
            {
                foreach (var node in nodes)
                {
                    if (!string.IsNullOrEmpty(node?.InnerText ?? ""))
                        headers.Add(new(node!.Id, @"" + node.InnerText));
                }
            }
            return new DocsDTO
            {
                Style = styleNode?.OuterHtml ?? "",
                Body = bodyNode?.InnerHtml ?? "",
                Headers = headers
            };
        }//

        [GeneratedRegex("d/.+/")]
        private static partial Regex MyRegex();
    }

    public class DocsDTO
    {
        public string Style { get; set; }
        public string Body { get; set; }
        public List<(string,string)> Headers { get; set; }
    }
    
}
