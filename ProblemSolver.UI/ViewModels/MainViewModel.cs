using Newtonsoft.Json;
using ProblemSolver.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;
using System.Windows;

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

        AddConfigCommand = new RelayCommand(_ => AddConfig());
        EditConfigCommand = new RelayCommand(_ => EditConfig(), _ => SelectedConfig != null);
        RemoveConfigCommand = new RelayCommand(_ => RemoveConfig(), _ => SelectedConfig != null);
        StartConfigCommand = new RelayCommand(parameter => StartConfig(parameter));
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

    private async void StartConfig(object parameter)
    {
        if (parameter is ConfigModel config)
        {

            config.Status = "In Progress";

            //var tasksFromBackend = await _backendService.GetTasksAsync(SelectedConfig.TaskId);

            var tasksFromBackend = new List<TaskModel>
            {
                new TaskModel(){Code="code1", Description="desc1", Name="name1", Status="In progress" },
                new TaskModel(){Code="code2", Description="desc2", Name="name2", Status="In progress" },
                new TaskModel(){Code="code3", Description="desc3", Name="name3", Status="In progress" },
                new TaskModel(){Code="code4", Description="desc4", Name="name4", Status="In progress" },
                new TaskModel(){Code="code5", Description="desc5", Name="name5", Status="In progress" }
            };

            config.Tasks.Clear();

            foreach (var task in tasksFromBackend)
            {
                config.Tasks.Add(task);
            }

            MessageBox.Show("Tasks added!");

            await Task.Delay(5000);
            

            foreach (var task in config.Tasks)
            {
                //var result = await _backendService.ProcessTaskAsync(task, SelectedConfig.Language);
                task.Status = "Completed";
                task.Code = "Just a code";
                await Task.Delay(1000);
            }

            UpdateConfigStatus(config);
        }
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

        OnPropertyChanged(nameof(Configurations));
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
