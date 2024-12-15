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

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public ConfigViewModel(ConfigModel config)
    {
        Tasks = config.Tasks;

        Config = config;
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
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
