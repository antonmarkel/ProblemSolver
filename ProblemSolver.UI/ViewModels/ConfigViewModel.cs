using ProblemSolver.Shared.Bot.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class ConfigViewModel : INotifyPropertyChanged
{
    private ConfigModel _config;

    public ConfigModel Config
    {
        get => _config;
        set
        {
            _config = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<TaskModel> Tasks { get; }
    public ObservableCollection<BotEnum> NeuralNetworkModels { get; set; }
    public ObservableCollection<ProgrammingLanguageEnum> Languages { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public ConfigViewModel(ConfigModel config)
    {
        Config = config;
        Tasks = config.Tasks;

        NeuralNetworkModels = new ObservableCollection<BotEnum>(Enum.GetValues(typeof(BotEnum)).Cast<BotEnum>());
        Languages = new ObservableCollection<ProgrammingLanguageEnum>(Enum.GetValues(typeof(ProgrammingLanguageEnum)).Cast<ProgrammingLanguageEnum>());

        SaveCommand = new RelayCommand(_ => Save());
        CancelCommand = new RelayCommand(_ => Cancel());
    }

    private void Save()
    {
        var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
        if (window != null)
        {
            window.DialogResult = true;
            window.Close();
        }
    }

    private void Cancel()
    {
        var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);
        if (window != null)
        {
            window.DialogResult = false;
            window.Close();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
