using Newtonsoft.Json;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Shared.Bot.Enums;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

public class AppViewModel : INotifyPropertyChanged
{

    private readonly IBotService _botService;
    private readonly IAuthService _authService;
    private readonly ITaskExtractor _taskExtractor;
    private readonly ITaskTextProcessor _taskTextProcessor;
    private readonly ITaskSender _taskSender;

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


    public AppViewModel(IBotService botService, IAuthService authService, ITaskExtractor taskExtractor,
            ITaskTextProcessor taskTextProcessor, ITaskSender taskSender)
    {
        _botService = botService;
        _authService = authService;
        _taskExtractor = taskExtractor;
        _taskTextProcessor = taskTextProcessor;
        _taskSender = taskSender;

        Languages = new List<ProgrammingLanguageEnum>(Enum.GetValues(typeof(ProgrammingLanguageEnum)) as ProgrammingLanguageEnum[]);
        SelectedLanguage = Languages[0];

        _courseModel = new CourseModel();

        string jsonPath = "..\\..\\..\\Config\\Config.json";

        Console.WriteLine(jsonPath);

        string jsonData = File.ReadAllText(jsonPath);

        Config? config = JsonConvert.DeserializeObject<Config>(jsonData);

        Console.WriteLine(config?.BaseAddress);
        Console.WriteLine(config?.LoginId);
        Console.WriteLine(config?.Password);
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}
