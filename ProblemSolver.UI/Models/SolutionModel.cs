using ProblemSolver.Shared.Solvers;
using ProblemSolver.Shared.Tasks.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProblemSolver.UI.Models
{
    // Simple model which provides INotifyPropertyChanged,
    // After you click on "Start" button, all of your accounts are
    // starting to log in, then for every account creating it's own solver, and we
    // are creating for every solver solution model
    public class SolutionModel : INotifyPropertyChanged
    {
        private string _accountName;
        private SolutionStateEnum _state;
        private Dictionary<long, TaskState> _tasks;

        public string AccountName
        {
            get => _accountName;
            set
            {
                _accountName = value;
                OnPropertyChanged();
            }
        }

        public SolutionStateEnum State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<long, TaskState> Tasks
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
}
