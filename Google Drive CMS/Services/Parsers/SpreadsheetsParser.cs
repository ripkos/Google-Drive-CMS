using System.Text.RegularExpressions;
namespace Google_Drive_CMS.Services.Parsers
{
    public partial class SpreadsheetsParser
    {

        private readonly HttpClient client;
        public SpreadsheetsParser(HttpClient httpClient) {
            client = httpClient;
        }
        public static string UrlFromID(string id)
        {
            return $"https://docs.google.com/spreadsheets/u/4/d/{id}/export?format=csv&gid=0";
        }
        public async Task<List<string[]>> GetDataFromSpreadsheetsAsync(string url)
        {
            var list = new List<string[]>();
            var responseBody = await client.GetStringAsync(url);
            var split = responseBody.Split('\n');
            foreach(var line in split)
            {
                var values = Regex().Split(line);
                for(var i=0; i<values.Length; i++)
                {
                    if(values[i] != string.Empty && values[i].StartsWith("\""))
                    {
                        values[i] = values[i][1..^1];
                    }
                }
                list.Add(values);
            }
            return list;
        }

        [GeneratedRegex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")]
        private static partial Regex Regex();
    }
}
