using System.Net.Http.Headers;
using ProblemSolver.Logic.DlServices.Interfaces;

namespace ProblemSolver.Logic.DlServices.Implementations
{
    public class TaskSender : ITaskSender
    {
        public async Task SendToCheckAsync(string filePath, HttpClient client, long courseId, long taskId)
        {
            using var formContent = new MultipartFormDataContent
            {
                { new StringContent("/taskqueue.jsp"), "script" },
                { new StringContent($"cid={courseId}&nid={taskId}"), "addparam" },
                { new StringContent(""), "ExtChk" },
                { new StringContent("g131x64"), "language" },
                { new StringContent("50"), "DelTA4 at NIT1 Win10 x64" }
            };

            if (File.Exists(filePath))
            {
                var fileStream = File.OpenRead(filePath);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                formContent.Add(fileContent, "f", Path.GetFileName(filePath));
            }
            else
            {
                Console.WriteLine("File not found. Please provide a correct file path.");

                return;
            }

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
