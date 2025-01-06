using Newtonsoft.Json;
using ProblemSolver.UI.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;
using ProblemSolver.Shared.Tasks.Enums;
using ProblemSolver.Logic.SolverServices.Interfaces;

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
    public ICommand StartAllConfigCommand { get; }

    public MainViewModel()
    {
        // TODO: Replace List of congigurations with repository
        Configurations = new ObservableCollection<ConfigModel>();

        AddConfigCommand = new RelayCommand(_ => AddConfig());
        EditConfigCommand = new RelayCommand(_ => CheckConfig(), _ => SelectedConfig != null);
        RemoveConfigCommand = new RelayCommand(_ => RemoveConfig(), _ => SelectedConfig != null);
        StartConfigCommand = new RelayCommand(parameter => StartConfig(parameter), parameter => CanExecuteStartConfig(parameter));
        StartAllConfigCommand = new RelayCommand(_ => StartAllConfigs(), _ => CanExecuteStartAllConfig());
    }

    private void AddConfig()
    {
        var newConfig = new ConfigModel();
        AddConfigWindow(newConfig);
        SaveConfigurations();
    }

    private void CheckConfig()
    {
        CheckConfigWindow(SelectedConfig);
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
            if (config.Status != ConfigStatusEnum.InProgress)
            {

                config.Status = ConfigStatusEnum.InProgress;
                config.CanStart = false;

                ((RelayCommand)StartConfigCommand).RaiseCanExecuteChanged();

                //var tasksFromBackend = await _backendService.GetTasksAsync(SelectedConfig.TaskId);

                var tasksFromBackend = new List<TaskModel>
                {
                new TaskModel(){Description="desc1", Name="name1", Status=TaskState.Awaiting },
                new TaskModel(){Description="desc2", Name="name2", Status=TaskState.Awaiting },
                new TaskModel(){Description="desc3", Name="name3", Status=TaskState.Awaiting },
                new TaskModel(){Description="desc4", Name="name4", Status=TaskState.Awaiting },
                new TaskModel(){Description="desc5", Name="name5", Status=TaskState.Awaiting }
                };

                config.Tasks.Clear();

                foreach (var task in tasksFromBackend)
                {
                    config.Tasks.Add(task);
                }

                Console.WriteLine($"Tasks added (taskID = {config.TaskId})");

                await Task.Delay(5000);


                foreach (var task in config.Tasks)
                {
                    //var result = await _backendService.ProcessTaskAsync(task, SelectedConfig.Language);
                    task.Status = TaskState.Solved;
                    await Task.Delay(1000);
                }

                UpdateConfigStatus(config);
            }
        }
    }

    private bool CanExecuteStartConfig(object parameter)
    { 
        if(parameter is ConfigModel config)
        {
            return config.CanStart;
        }
        return false;
    }

    private void StartAllConfigs()
    {
        foreach(var config in Configurations)
        {
            StartConfig(config);        
        }
    }

    private bool CanExecuteStartAllConfig()
    {
        if (Configurations == null || Configurations.Count == 0)
        {
            return false;
        }

        foreach(var config in Configurations)
        {
            if(!config.CanStart)
            {
                return false;
            }
        }

        return true;
    }


    private void AddConfigWindow(ConfigModel config)
    {
        var configViewModel = new ConfigViewModel(config);

        var configWindow = new AddConfigWindow { DataContext = configViewModel };

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

    private void CheckConfigWindow(ConfigModel config)
    {
        var configViewModel = new ConfigViewModel(config);

        var configWindow = new CheckConfigWindow { DataContext = configViewModel };

        configWindow.ShowDialog();

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
            config.Status = ConfigStatusEnum.Pending;
        }

        else
        {
            config.Status = config.Tasks.All(task => task.Status == TaskState.Solved) ? ConfigStatusEnum.Completed : ConfigStatusEnum.InProgress;
            config.CanStart = config.Tasks.All(task => task.Status == TaskState.Solved) ? true : false;
        }

        ((RelayCommand)StartConfigCommand).RaiseCanExecuteChanged();
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
