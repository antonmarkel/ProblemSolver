using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.Shared.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ConfigModel : INotifyPropertyChanged
{
    private BotEnum _neuralNetworkModel;
    private ProgrammingLanguageEnum _language;
    private int _taskId;
    private ConfigStatusEnum _status = ConfigStatusEnum.Pending;
    private bool _canStart = true;
    private ObservableCollection<TaskInfo> _tasks = new();

    public BotEnum NeuralNetworkModel
    {
        get => _neuralNetworkModel;
        set
        {
            _neuralNetworkModel = value;
            OnPropertyChanged();
        }
    }

    public ProgrammingLanguageEnum Language
    {
        get => _language;
        set
        {
            _language = value;
            OnPropertyChanged();
        }
    }

    public int TaskId
    {
        get => _taskId;
        set
        {
            _taskId = value;
            OnPropertyChanged();
        }
    }

    public ConfigStatusEnum Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public bool CanStart
    {
        get => _canStart;
        set
        {
            _canStart = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<TaskModel> Tasks
    {
        get => _tasks;
        set
        {
            _tasks = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

