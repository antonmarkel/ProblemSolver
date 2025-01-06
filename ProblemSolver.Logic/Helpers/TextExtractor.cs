using System.Text;
using HtmlAgilityPack;
using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.Helpers
{
    public static class TextExtractor
    {
        /// <summary>
        ///     Extracts text from web-page excluding styles/scripts/tags.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<TaskInfo> ProcessTaskAsync(TaskLink link, HttpClient client)
        {
            var htmlContentResult = await ExtractHtmlContentAsync(link.Url, client);

            if (htmlContentResult.IsT1)
                return new TaskInfo { CourseId = link.CourseId, TaskId = link.TaskId, Extracted = false };


            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContentResult.AsT0);
            string textContent = ExtractText(htmlDoc.DocumentNode);

            return new TaskInfo
                { CourseId = link.CourseId, TaskId = link.TaskId, Task = textContent };
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

                textBuilder.AppendLine(ExtractText(child));
            }

            return textBuilder.ToString().Trim().Replace("&nbsp;", "");
        }
    }
}
