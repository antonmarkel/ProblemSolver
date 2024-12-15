using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Shared.Bot.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.IO;
using System.Windows.Input;
using ProblemSolver.UI.DL.Auth;
using System.Net.Http;
using System.Collections.Concurrent;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.UI.Configuration;

public class ApplicationViewModel : INotifyPropertyChanged
{

    private readonly IBotService _botService;
    private readonly IAuthService _authService;
    private readonly ITaskExtractor _taskExtractor;
    private readonly ITaskTextProcessor _taskTextProcessor;
    private readonly ITaskSender _taskSender;

    private readonly HttpClient _loginClient;
    private readonly LoginRequest _loginRequest;

    private readonly LanguageFileExtensionHelper _languageFileExtensionHelper;

    public ICommand SolveCommand { get; }

    public List<ProgrammingLanguageEnum>? Languages { get; }
    private ProgrammingLanguageEnum _selectedLanguage;
    public ProgrammingLanguageEnum SelectedLanguage
    {
        get
        {
            return _selectedLanguage;
        }

        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged();
            }
        }
    }

    public List<BotEnum> BotsModels { get; }
    private BotEnum _selectedBotModel;
    public BotEnum SelectedBotModel
    {
        get
        {
            return _selectedBotModel;
        }

        set
        {
            if (_selectedBotModel != value)
            {
                _selectedBotModel = value;
                OnPropertyChanged();
            }
        }
    }

    private CourseModel _courseModel;

    public int CourseId
    {
        get
        {
            return _courseModel.Id;
        }

        set
        {
            if (_courseModel.Id != value)
            {
                _courseModel.Id = value;
                OnPropertyChanged();
            }
        }
    }

    private ProblemModel _selectedProblem;
    public ObservableCollection<ProblemModel> Problems { get; set; }
    public ProblemModel SelectedProblem
    {
        get
        {
            return _selectedProblem;
        }

        set
        {
            if (_selectedProblem != value)
            {
                _selectedProblem = value;
                OnPropertyChanged();
            }
        }
    }


    public ApplicationViewModel(IBotService botService, IAuthService authService, ITaskExtractor taskExtractor,
            ITaskTextProcessor taskTextProcessor, ITaskSender taskSender)
    {
        _botService = botService;
        _authService = authService;
        _taskExtractor = taskExtractor;
        _taskTextProcessor = taskTextProcessor;
        _taskSender = taskSender;

        _loginClient = new BotConfig().GetHttpClient();
        _loginRequest = new BotConfig().GetLoginRequest();

        _languageFileExtensionHelper = new LanguageFileExtensionHelper();

        Languages = new List<ProgrammingLanguageEnum>(Enum.GetValues(typeof(ProgrammingLanguageEnum)) as ProgrammingLanguageEnum[]);
        SelectedLanguage = Languages[0];

        BotsModels = new List<BotEnum>(Enum.GetValues(typeof(BotEnum)) as BotEnum[]);
        SelectedBotModel = BotsModels[0];

        SolveCommand = new RelayCommand(Solve);

        _courseModel = new CourseModel();

        Problems = new ObservableCollection<ProblemModel>()
        {
            new ProblemModel {Name="1",Description="<html><body><h1>HTML Content for Problem 1</h1></body></html>",Solution="public class Program {\n public static void Main() {\n Console.WriteLine(\"Solution 1\");\n }\n}"},
            new ProblemModel {Name="2",Description="<html><body><h1>HTML Content for Problem 2</h1></body></html>",Solution="2"},
            new ProblemModel {Name="3",Description="<html><body><h1>HTML Content for Problem 3</h1></body></html>",Solution="3"}
        };

        

        Console.WriteLine("Ctor worked");
    }


    private async void Solve()
    {

        MessageBox.Show("Solving....");
        Console.WriteLine("MESSAGE BOX PASSED");

        //int courseId = 1374;

        //using var client = new HttpClient
        //{
        //    BaseAddress = new Uri("https://dl.gsu.by/")
        //};

        //var loginRequest = new LoginRequest
        //{
        //    Id = 193391,
        //    Password = "sweety_bot"
        //};

        var loginResult = await _authService.LoginAsync(_loginRequest, _loginClient);
        if (loginResult.IsT1)
        {
            Console.WriteLine("Failed to log in!");
            MessageBox.Show("Failed to log in!");
            return;
        }

        Console.WriteLine("Logged in successfully");
        MessageBox.Show("Logged in successfully");

        var extractResult = await _taskExtractor.ExtractAsync(CourseId, _loginClient);

        if (extractResult.IsT1)
        {
            MessageBox.Show("Failed to get tasks");
            return;
        }

        Console.WriteLine("Got tasks successfully");

        MessageBox.Show("Got tasks successfully");

        var taskInfos = await _taskTextProcessor.ProcessTasksAsync(extractResult.AsT0, _loginClient);
        Console.WriteLine("Extracted tasks successfully");

        // Language, BotModel, Language Extension, BotService, CourseId

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
                        UseBot = SelectedBotModel
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
            await _taskSender.SendToCheckAsync(sendData.Item1, _loginClient, sendData.Item2, sendData.Item3);
            await Task.Delay(10000);
        }

        Console.WriteLine("Finished!");
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}
