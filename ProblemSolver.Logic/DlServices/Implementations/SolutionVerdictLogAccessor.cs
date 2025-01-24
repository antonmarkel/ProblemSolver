using ProblemSolver.Shared.Solvers;
using System.Text;
using System.Text.RegularExpressions;

namespace ProblemSolver.Logic.DlServices.Implementations
{
    public class SolutionVerdictLogAccessor : ISolutionVerdictLogAccessor
    {
        public async Task<SolutionVerdict?> GetTaskVerdictAsync(HttpClient client, long courseId, long taskId)
        {
            //You can see below we have request for log.asp, there's not any specifying which one course's logs we want to check.
            //So first we must visit the page of the course we want to check our task in.(I guess necessary cookies are being set when we send this request)
            await client.GetAsync($"cdesk.asp?id={courseId}&rules=ok");
            var logData = new Dictionary<string, string>
            {
                //TODO:Maybe make it according to the current date. Like extract a day from current date
                { "fday", $"{DateTime.Now.Day}" },
                { "fmonth", $"{DateTime.Now.Month}" },
                { "fyear", $"{DateTime.Now.Month}" },
                { "tday", $"{DateTime.Now.Day}" },
                { "tmonth",  $"{DateTime.Now.Month}" },
                { "tyear",  $"{DateTime.Now.Year}" }
            };
            var logContent = new FormUrlEncodedContent(logData);

            var logsResp = await client.PostAsync("log.asp", logContent);
            byte[] responseBody = await logsResp.Content.ReadAsByteArrayAsync();
            var windows1251 = Encoding.GetEncoding("windows-1251");
            string decodedText = windows1251.GetString(responseBody);

            //the response contains a table with solution verdict, we extract all of them.
            //TODO: it's not necessary to extract all the logs of course, only the one task we're intrested in.(I could do it myself, but I'm a f*cking lazy ass)
            //TODO: or on the contrary make one request to check the solution for several tasks.
            //TODO: it's the thing that does not work on every course for now.
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
                        IsSolved = match.Groups["result"].Value ==
                                   match.Groups["maximum"]
                                       .Value, //usually if task is solver we have score like (40/40).(result/maximum)
                        Verdict = match.Groups["verdict"].Value
                            .Replace("&nbsp;",
                                "") //I have no idea what is &nbsp, I just remove it for a clearer output.
                    };
            }

            return null;
        }
    }
}
