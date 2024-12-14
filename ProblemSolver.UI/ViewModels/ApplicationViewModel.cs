using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Shared.Bot.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class ApplicationViewModel : INotifyPropertyChanged
{

    private readonly IBotService _botService;
    private readonly IAuthService _authService;
    private readonly ITaskExtractor _taskExtractor;
    private readonly ITaskTextProcessor _taskTextProcessor;
    private readonly ITaskSender _taskSender;

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

    public List<BotEnum> Bots { get; }
    private BotEnum _selectedBot;
    public BotEnum SelectedBot
    {
        get
        {
            return _selectedBot;
        }

        set
        {
            if (_selectedBot != value)
            {
                _selectedBot = value;
                OnPropertyChanged();
            }
        }
    }

    private CourseModel _courseModel;

    public int Id
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


    public ApplicationViewModel(IBotService botService, IAuthService authService, ITaskExtractor taskExtractor,
            ITaskTextProcessor taskTextProcessor, ITaskSender taskSender)
    {
        _botService = botService;
        _authService = authService;
        _taskExtractor = taskExtractor;
        _taskTextProcessor = taskTextProcessor;
        _taskSender = taskSender;

        Languages = new List<ProgrammingLanguageEnum>(Enum.GetValues(typeof(ProgrammingLanguageEnum)) as ProgrammingLanguageEnum[]);
        SelectedLanguage = Languages[0];

        Bots = new List<BotEnum>(Enum.GetValues(typeof(BotEnum)) as BotEnum[]);
        SelectedBot = Bots[0];

        SolveCommand = new RelayCommand(Solve);

        _courseModel = new CourseModel();

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

        //var loginResult = await _authService.LoginAsync(loginRequest, client);
        //if (loginResult.IsT1)
        //{
        //    InfoTextBlock.Text = "failed to log in";

        //    return;
        //}

        //Console.WriteLine("Logged in successfully");
        //var extractResult = await _taskExtractor.ExtractAsync(courseId, client);
        //if (extractResult.IsT1)
        //{
        //    InfoTextBlock.Text = "failed to get tasks";

        //    return;
        //}

        //Console.WriteLine("Got tasks successfully");
        //var taskInfos = await _taskTextProcessor.ProcessTasksAsync(extractResult.AsT0, client);
        //Console.WriteLine("Extracted tasks successfully");
        //int count = 0;
        //var tasks = new List<Task>();
        //var codeToSave = new ConcurrentStack<(string, string)>();
        //var infoToSave = new ConcurrentStack<(string, string)>();
        //var toSend = new List<(string, long, long)>();


        //string solutionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sol");
        //Directory.CreateDirectory(solutionsPath);
        //string coursePath = Path.Combine(solutionsPath, $"c{courseId}");
        //Directory.CreateDirectory(coursePath);
        //foreach (var info in taskInfos)
        //    if (info.IsExtracted)
        //    {
        //        count++;
        //        var task = Task.Run(async () =>
        //        {
        //            var request = new TaskRequest
        //            {
        //                Language = ProgrammingLanguageEnum.Cpp,
        //                Task = new TaskInfo { Info = info.Info, IsExtracted = true, TaskId = info.TaskId },
        //                UseBot = BotEnum.Meta_Llama_3_1_70B_Instruct
        //            };

        //            var codeResponse = await _botService.ProcessRequestAsync(request);


        //            if (codeResponse.Code == "Failed")
        //            {
        //                Console.WriteLine($"Cannot extract code! {count}");

        //                return;
        //            }

        //            string taskPath = Path.Combine(coursePath, $"info{request.Task.TaskId}.txt");
        //            infoToSave.Push(new ValueTuple<string, string>(request.Task.Info!, taskPath));

        //            string filePath = Path.Combine(coursePath, $"t{request.Task.TaskId}.cpp");
        //            codeToSave.Push(new ValueTuple<string, string>(codeResponse.Code, filePath));

        //            toSend.Add(new ValueTuple<string, long, long>(filePath, courseId, request.Task.TaskId));
        //        });
        //        await Task.Delay(500);
        //        tasks.Add(task);
        //    }

        //Task.WaitAll(tasks.ToArray());
        //Console.WriteLine("Tasks were solved!");

        //while (!codeToSave.IsEmpty)
        //{
        //    (string, string) data;
        //    if (codeToSave.TryPop(out data)) await File.WriteAllTextAsync(data.Item2, data.Item1);
        //}

        //Console.WriteLine("Code was saved!");
        //while (!infoToSave.IsEmpty)
        //{
        //    (string, string) data;
        //    if (infoToSave.TryPop(out data)) await File.WriteAllTextAsync(data.Item2, data.Item1);
        //}

        //Console.WriteLine("Info was saved");
        //foreach (var sendData in toSend)
        //{
        //    await _taskSender.SendToCheckAsync(sendData.Item1, client, sendData.Item2, sendData.Item3);
        //    await Task.Delay(10000);
        //}

        //Console.WriteLine("Finished!");
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}
