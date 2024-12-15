using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.Tasks;
using System.Collections.Concurrent;
using System.IO;

public class CustomFileWriter
{
    public async void StartWritingFiles(List<TaskInfo> taskInfos)
    {
        int count = 0;
        var tasks = new List<Task>();
        var codeToSave = new ConcurrentStack<(string, string)>();
        var infoToSave = new ConcurrentStack<(string, string)>();
        var toSend = new List<(string, long, long)>();


        string solutionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sol");
        Directory.CreateDirectory(solutionsPath);
        string coursePath = Path.Combine(solutionsPath, $"c{CourseId}");
        Directory.CreateDirectory(coursePath);
        foreach (var info in taskInfos)
            if (info.IsExtracted)
            {
                count++;
                var task = Task.Run(async () =>
                {
                    var request = new TaskRequest
                    {
                        Language = SelectedLanguage,
                        Task = new TaskInfo { Info = info.Info, IsExtracted = true, TaskId = info.TaskId },
                        UseBot = SelectedBot
                    };

                    var codeResponse = await _botService.ProcessRequestAsync(request);


                    if (codeResponse.Code == "Failed")
                    {
                        Console.WriteLine($"Cannot extract code! {count}");

                        return;
                    }

                    string taskPath = Path.Combine(coursePath, $"info{request.Task.TaskId}.txt");
                    infoToSave.Push(new ValueTuple<string, string>(request.Task.Info!, taskPath));

                    string filePath = Path.Combine(coursePath, $"t{request.Task.TaskId}.cpp");
                    codeToSave.Push(new ValueTuple<string, string>(codeResponse.Code, filePath));

                    toSend.Add(new ValueTuple<string, long, long>(filePath, CourseId, request.Task.TaskId));
                });
                await Task.Delay(500);
                tasks.Add(task);
            }

        Task.WaitAll(tasks.ToArray());
    }
}