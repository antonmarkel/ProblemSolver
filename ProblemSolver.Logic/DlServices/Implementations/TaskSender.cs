using System.Net.Http.Headers;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Helpers;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Implementations
{
    public class TaskSender : ITaskSender
    {
        /// <summary>
        ///     Saves solutions and sends them
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="client"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task SendToCheckAsync(TaskSolution solution, HttpClient client, string folderName)
        {
            using var formContent = new MultipartFormDataContent
            {
                { new StringContent("/taskqueue.jsp"), "script" },
                { new StringContent($"cid={solution.CourseId}&nid={solution.TaskId}"), "addparam" },
                { new StringContent(""), "ExtChk" },
                { new StringContent(LanguageHelper.LanguageToCompiler(solution.Language)), "language" },
                { new StringContent("50"), "DelTA4 at NIT1 Win10 x64" }
            };

            var stream = await PrepareStreamAsync(solution, folderName);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            formContent.Add(fileContent, "f",
                Path.GetFileName(
                    $"{solution.SolutionName}.{LanguageHelper.LanguageToFileExtension(solution.Language)}"));


            formContent.Add(new StringContent("Отправить"), "bsubmit");

            try
            {
                var response = await client.PostAsync("/upload", formContent);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
                Console.WriteLine("Form submitted successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while submitting the form: " + ex.Message);
            }
        }


        private async Task<Stream> PrepareStreamAsync(TaskSolution solution, string folderName)
        {
            //TODO: configure
            string folderPath = $"solutions/{solution.CourseId}/{folderName}";
            string fileName = $"{solution.SolutionName}.{LanguageHelper.LanguageToFileExtension(solution.Language)}";
            string filePath =
                $"{folderPath}/{fileName}";

            await FileWriter.WriteToFileAsync(folderPath, fileName, solution.Code);

            return File.OpenRead(filePath);
        }
    }
}
    