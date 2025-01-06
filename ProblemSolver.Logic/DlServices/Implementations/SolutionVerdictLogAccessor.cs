using System.Text;
using System.Text.RegularExpressions;
using ProblemSolver.Shared.Solvers;

namespace ProblemSolver.Logic.DlServices.Implementations
{
    public class SolutionVerdictLogAccessor : ISolutionVerdictLogAccessor
    {
        public async Task<SolutionVerdict?> GetTaskVerdictAsync(HttpClient client, long courseId, long taskId)
        {
            await client.GetAsync($"cdesk.asp?id={courseId}&rules=ok");
            var logData = new Dictionary<string, string>
                {
                    //TODO:MAKE dynamic - extract from: DateTime.Now - (period)
                    { "fday", "26" },
                    { "fmonth", "10" },
                    { "fyear", "2024" },
                    { "tday", "26" },
                    { "tmonth", "12" },
                    { "tyear", "2024" }
                };
                var logContent = new FormUrlEncodedContent(logData);

                var logsResp = await client.PostAsync("log.asp", logContent);
                byte[] responseBody = await logsResp.Content.ReadAsByteArrayAsync();
                var windows1251 = Encoding.GetEncoding("windows-1251");
                string decodedText = windows1251.GetString(responseBody);

                var regex = new Regex(
                    @"<td[^>]*><font[^>]*>&nbsp;&nbsp;<a href=""task\.jsp\?nid=(?<taskID>\d+)&cid=(?<courseID>\d+)"">(?<taskName>.*?)</a>&nbsp;&nbsp;</font></td>\s*
<td[^>]*><font[^>]*><a href=""log-dbt\.asp\?id=(?<logId>\d+)"">(?<result>\d+) / (?<maximum>\d+)</a>&nbsp;</font></td>\s*
<td[^>]*><font[^>]*>(?<verdict>.*?)&nbsp;</font></td>");

                var matches = regex.Matches(decodedText);

                foreach (Match match in matches)
                {
                    string id = match.Groups["taskID"].Value;

                    if (id == taskId.ToString())
                        return new SolutionVerdict
                        {
                            TaskId = taskId,
                            CourseId = courseId,
                            IsSolved = match.Groups["result"].Value == match.Groups["maximum"].Value,
                            Verdict = match.Groups["verdict"].Value.Replace("&nbsp;", "")
                        };
                }

                return null;
        }
    }
}
