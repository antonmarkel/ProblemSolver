using Newtonsoft.Json;
using ProblemSolver.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;

public class MainViewModel : INotifyPropertyChanged
{
    private ConfigModel _selectedConfig;

    public ObservableCollection<ConfigModel> Configurations { get; set; }
    public ConfigModel SelectedConfig
    {
        get => _selectedConfig;
        set
        {
            _selectedConfig = value;
            OnPropertyChanged();
            CheckAllTasksStatus();
        }
    }

    public ICommand AddConfigCommand { get; }
    public ICommand EditConfigCommand { get; }
    public ICommand RemoveConfigCommand { get; }

    public ICommand StartConfigCommand { get; }

    public MainViewModel()
    {
        Configurations = new ObservableCollection<ConfigModel>();

        AddConfigCommand = new RelayCommand(AddConfig);
        EditConfigCommand = new RelayCommand(EditConfig, () => SelectedConfig != null);
        RemoveConfigCommand = new RelayCommand(RemoveConfig, () => SelectedConfig != null);
        StartConfigCommand = new RelayCommand(StartConfig, () => SelectedConfig != null);
    }

    private void AddConfig()
    {
        var newConfig = new ConfigModel();
        OpenConfigWindow(newConfig);
        SaveConfigurations();
    }

    private void EditConfig()
    {
        OpenConfigWindow(SelectedConfig);
        SaveConfigurations();
    }

    private void RemoveConfig()
    {
        Configurations.Remove(SelectedConfig);
        SaveConfigurations();
    }

    private async void StartConfig()
    {
        SelectedConfig.Status = "In Progress";

        //var tasksFromBackend = await _backendService.GetTasksAsync(SelectedConfig.TaskId);

        //SelectedConfig.Tasks.Clear();

        //foreach (var task in tasksFromBackend)
        //{
        //    SelectedConfig.Tasks.Add(task);
        //}

        await Task.Delay(3000);

        //foreach(var task in SelectedConfig.Tasks)
        //{
        //    var result = await _backendService.ProcessTaskAsync(task, SelectedConfig.Language);
        //    task.Status = result.Status;
        //    task.Code = result.Code;
        //} 

        SelectedConfig.Status = SelectedConfig.Tasks.All(t => t.Status == "Completed") ? "Completed" : "In Progress";
    }

    private void OpenConfigWindow(ConfigModel config)
    {
        var configViewModel = new ConfigViewModel(config);

        var configWindow = new ConfigWindow { DataContext = configViewModel };

        configWindow.ShowDialog();

        if (configWindow.DialogResult == true)
        {
            if (!Configurations.Contains(config))
            {
                Configurations.Add(config);
            }
        }
        UpdateConfigStatus(config);
    }

    private ObservableCollection<ConfigModel> LoadConfigurations()
    {
        if (File.Exists("configs.json"))
        {
            var json = File.ReadAllText("configs.json");
            return JsonConvert.DeserializeObject<ObservableCollection<ConfigModel>>(json) ?? new ObservableCollection<ConfigModel>();
        }

        return new ObservableCollection<ConfigModel>();
    }

    private void SaveConfigurations()
    {
        var json = JsonConvert.SerializeObject(Configurations, Formatting.Indented);
        File.WriteAllText("configs.json", json);
    }

    private void UpdateConfigStatus(ConfigModel config)
    {
        if (!config.Tasks.Any())
        {
            config.Status = "Pending";
        }

        else
        {
            config.Status = config.Tasks.All(task => task.Status == "Completed") ? "Completed" : "In Progress";
        }
    }
    private void CheckAllTasksStatus()
    {
        foreach (var config in Configurations)
        {
            UpdateConfigStatus(config);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
