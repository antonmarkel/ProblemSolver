﻿using System.Text.RegularExpressions;
using OneOf;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Implementations
{
    public class TaskExtractor : ITaskExtractor
    {
        public async Task<OneOf<List<TaskLink>, Failed>> ExtractAsync(long courseId, HttpClient client)
        {
            var treeTaskResponse = await client.GetAsync($"tasktree.jsp?cid={courseId}");
            string content = await treeTaskResponse.Content.ReadAsStringAsync();

            var regex = new Regex(@"task\.jsp\?nid=(\d+)&cid=(\d+)");
            var matches = regex.Matches(content);

            List<TaskLink> urls = new(matches.Count);

            foreach (Match match in matches)
            {
                long nid = long.Parse(match.Groups[1].Value);
                long cid = long.Parse(match.Groups[2].Value);
                urls.Add(new TaskLink { Id = nid, Url = $"taskview.jsp?nid={nid}&cid={cid}&showcfg=1" });
            }

            if (urls.Count == 0)
                return new Failed();

            return urls;
        }
    }
}
