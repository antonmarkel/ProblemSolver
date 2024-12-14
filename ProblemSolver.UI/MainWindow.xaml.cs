using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Windows;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.UI.DL.Auth;

namespace ProblemSolver.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IBotService _botService;
        private readonly IAuthService _authService;
        private readonly ITaskExtractor _taskExtractor;
        private readonly ITaskTextProcessor _taskTextProcessor;
        private readonly ITaskSender _taskSender;

        public MainWindow(IBotService botService, IAuthService authService, ITaskExtractor taskExtractor,
            ITaskTextProcessor taskTextProcessor, ITaskSender taskSender)
        {
            _botService = botService;
            _authService = authService;
            _taskExtractor = taskExtractor;
            _taskTextProcessor = taskTextProcessor;
            _taskSender = taskSender;
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            int courseId = 1374;
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://dl.gsu.by/")
            };
            var loginRequest = new LoginRequest
            {
                Id = 193391,
                Password = "sweety_bot"
            };

            var loginResult = await _authService.LoginAsync(loginRequest, client);
            if (loginResult.IsT1)
            {
                InfoTextBlock.Text = "failed to log in";

                return;
            }

            Console.WriteLine("Logged in successfully");
            var extractResult = await _taskExtractor.ExtractAsync(courseId, client);
            if (extractResult.IsT1)
            {
                InfoTextBlock.Text = "failed to get tasks";

                return;
            }

            Console.WriteLine("Got tasks successfully");
            var taskInfos = await _taskTextProcessor.ProcessTasksAsync(extractResult.AsT0, client);
            Console.WriteLine("Extracted tasks successfully");
            int count = 0;
            var tasks = new List<Task>();
            var codeToSave = new ConcurrentStack<(string, string)>();
            var infoToSave = new ConcurrentStack<(string, string)>();
            var toSend = new List<(string, long, long)>();


            string solutionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sol");
            Directory.CreateDirectory(solutionsPath);
            string coursePath = Path.Combine(solutionsPath, $"c{courseId}");
            Directory.CreateDirectory(coursePath);
            foreach (var info in taskInfos)
                if (info.IsExtracted)
                {
                    count++;
                    var task = Task.Run(async () =>
                    {
                        var request = new TaskRequest
                        {
                            Language = ProgrammingLanguageEnum.Cpp,
                            Task = new TaskInfo { Info = info.Info, IsExtracted = true, TaskId = info.TaskId },
                            UseBot = BotEnum.Meta_Llama_3_1_70B_Instruct
                        };

                        var codeResponse = await _botService.ProcessRequestAsync(request);


                        if (codeResponse.Code == "Failed")
                        {
                            Console.WriteLine($"Cannot extract code! {count}");

                            return;
                        }

                        string taskPath = Path.Combine(coursePath, $"info{request.Task.TaskId}.txt");
                        infoToSave.Push(new ValueTuple<string, string>(codeResponse.Code, taskPath));

                        string filePath = Path.Combine(coursePath, $"t{request.Task.TaskId}.cpp");
                        codeToSave.Push(new ValueTuple<string, string>(request.Task.Info!, filePath));

                        toSend.Add(new ValueTuple<string, long, long>(filePath, courseId, request.Task.TaskId));
                    });
                    await Task.Delay(500);
                    tasks.Add(task);
                }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Tasks were solved!");

            while (!codeToSave.IsEmpty)
            {
                (string, string) data;
                if (codeToSave.TryPop(out data)) await File.WriteAllTextAsync(data.Item2, data.Item1);
            }

            Console.WriteLine("Code was saved!");
            while (!infoToSave.IsEmpty)
            {
                (string, string) data;
                if (infoToSave.TryPop(out data)) await File.WriteAllTextAsync(data.Item2, data.Item1);
            }

            Console.WriteLine("Info was saved");
            foreach (var sendData in toSend)
            {
                await _taskSender.SendToCheckAsync(sendData.Item1, client, sendData.Item2, sendData.Item3);
                await Task.Delay(10000);
            }

            Console.WriteLine("Finished!");
        }
    }
}