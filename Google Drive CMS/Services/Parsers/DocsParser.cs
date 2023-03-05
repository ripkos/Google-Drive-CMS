using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Google_Drive_CMS.Services.Parsers
{
    public partial class DocsParser
    {
        private readonly HttpClient _client;
        public DocsParser(HttpClient httpClient)
        {
            _client = httpClient;
        }
        public async Task<DocsDTO> GetDocsAsync(string generalUrl)
        {
            var url = $"https://docs.google.com/feeds/download/documents/export/Export?id={MyRegex().Match(generalUrl).Value[2..^1]}&exportFormat=html";
            var responseBody = await _client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseBody);
            var styleNode = htmlDoc.DocumentNode.SelectSingleNode("//head/style");
            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
            const string xpath = "//*[self::h1 or self::h2 or self::h3 or self::h4]";
            List<(string, string)> headers = new();
            var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
            if (nodes is null)
                return new DocsDTO
                {
                    Style = styleNode?.OuterHtml ?? "",
                    Body = bodyNode?.InnerHtml ?? "",
                    Headers = headers
                };
            headers.AddRange(from node in nodes where !string.IsNullOrEmpty(node?.InnerText ?? "") select new ValueTuple<string, string>(node!.Id, @"" + node.InnerText));
            return new DocsDTO
            {
                Style = styleNode?.OuterHtml ?? "",
                Body = bodyNode?.InnerHtml ?? "",
                Headers = headers
            };
        }

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
