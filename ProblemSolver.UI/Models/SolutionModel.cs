using ProblemSolver.Shared.Solvers;
using ProblemSolver.Shared.Tasks.Enums;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProblemSolver.UI.Models
{
    public class SolutionModel : INotifyPropertyChanged
    {
        public string AccountName { get; set; }
        private SolutionStateEnum _state;
        private ConcurrentDictionary<long, TaskState> _tasks;

        public SolutionStateEnum State 
        { 
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public ConcurrentDictionary<long, TaskState> Tasks
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
