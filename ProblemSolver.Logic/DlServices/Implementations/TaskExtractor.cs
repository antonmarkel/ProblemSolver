using System.Text.RegularExpressions;
using OneOf;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Results;

namespace ProblemSolver.Logic.DlServices.Implementations
{
    public class TaskExtractor : ITaskExtractor
    {
        public async Task<OneOf<List<string>, Failed>> ExtractAsync(long courseId, HttpClient client)
        {
            var treeTaskResponse = await client.GetAsync($"tasktree.jsp?cid={courseId}");
            string content = await treeTaskResponse.Content.ReadAsStringAsync();

            var regex = new Regex(@"task\.jsp\?nid=(\d+)&cid=(\d+)");
            var matches = regex.Matches(content);

            List<string> urls = new(matches.Count);

            foreach (Match match in matches)
                urls.Add(match.Value);

            if (urls.Count == 0)
                return new Failed();

            return urls;
        }
    }
}
