using ProblemSolver.Shared.Tasks.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class TaskModel : INotifyPropertyChanged
{
    public long Id { get; private set; }
    private TaskState _status;

    public TaskState Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

