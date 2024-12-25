using System.Net.Http.Headers;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Helpers;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Implementations
{
    public class TaskSender : ITaskSender
    {
        public async Task SendToCheckAsync(TaskSolution solution, HttpClient client)
        {
            using var formContent = new MultipartFormDataContent
            {
                { new StringContent("/taskqueue.jsp"), "script" },
                { new StringContent($"cid={solution.CourseId}&nid={solution.TaskId}"), "addparam" },
                { new StringContent(""), "ExtChk" },
                { new StringContent(LanguageHelper.LanguageToCompiler(solution.Language)), "language" },
                { new StringContent("50"), "DelTA4 at NIT1 Win10 x64" }
            };

            string filePath =
                $"solutions/{solution.CourseId}/{solution.TaskId}.{LanguageHelper.LanguageToFileExtension(solution.Language)}";

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.Create(filePath);
            await File.WriteAllTextAsync(filePath, solution.Code);

            var fileStream = File.OpenRead(filePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            formContent.Add(fileContent, "f", Path.GetFileName(filePath));


            formContent.Add(new StringContent("Отправить"), "bsubmit");

            try
            {
                var response = await client.PostAsync("/upload", formContent);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Form submitted successfully!");
                Console.WriteLine("Server Response:");
                Console.WriteLine(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while submitting the form: " + ex.Message);
            }
        }
    }
}
