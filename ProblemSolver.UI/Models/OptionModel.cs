using ProblemSolver.Shared.Bot.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProblemSolver.UI.Models
{
    public class OptionModel : INotifyPropertyChanged
    {
        private string _accountName = string.Empty;
        private BotEnum _aiBot;
        private ProgrammingLanguageEnum _language;
        private CompilerEnum _compiler;

        public string AccountName
        {
            get => _accountName;
            set
            {
                _accountName = value;
                OnPropertyChanged();
            }
        }

        public BotEnum AiBot
        {
            get => _aiBot;
            set
            {
                _aiBot = value;
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

        public CompilerEnum Compiler 
        {
            get => _compiler;
            set
            {
                _compiler = value;
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
