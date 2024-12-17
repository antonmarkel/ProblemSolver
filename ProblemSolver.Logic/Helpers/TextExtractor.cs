using HtmlAgilityPack;
using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Tasks;
using System.Text.RegularExpressions;
using System.Text;

namespace ProblemSolver.Logic.Helpers
{
    public static class TextExtractor
    {
        public static async Task<List<TaskInfo>> ProcessTasksAsync(List<TaskLink> taskLinks, HttpClient client)
        {
            var infoList = new List<TaskInfo>(taskLinks.Count);
            foreach (var link in taskLinks)
            {
                var htmlContentResult = await ExtractHtmlContentAsync(link.Url, client);
                var info = htmlContentResult.Match<TaskInfo>(
                    info => new TaskInfo { Info = info, IsExtracted = true, TaskId = link.Id },
                    _ => new TaskInfo { IsExtracted = false, TaskId = link.Id });

                if (info.IsExtracted)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(info.Info);
                    string textContent = ExtractText(htmlDoc.DocumentNode);
                    if (textContent.Length < 10) info.IsExtracted = false;
                    info.Info = Regex.Unescape(textContent);
                }

                infoList.Add(info);
            }

            return infoList;
        }

        private static async Task<OneOf<string, Failed>> ExtractHtmlContentAsync(string url, HttpClient client)
        {
            try
            {
                var apiResponse = await client.GetAsync(url);

                if (apiResponse.IsSuccessStatusCode)
                {
                    byte[] byteContent = await apiResponse.Content.ReadAsByteArrayAsync();
                    string response = Encoding.GetEncoding("Windows-1251").GetString(byteContent);

                    return response;
                }

                return new Failed();
            }
            catch
            {
                return new Failed();
            }
        }

        private static string ExtractText(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
                return node.InnerText.Trim();

            if (node.Name == "script" || node.Name == "style")
                return string.Empty;

            var textBuilder = new StringBuilder();
            foreach (var child in node.ChildNodes)
            {
                if (child.Name == "script" || child.Name == "style")
                    continue;

                textBuilder.Append(ExtractText(child));
            }

            return textBuilder.ToString().Trim().Replace("&nbsp;", "");
        }
    }
}
