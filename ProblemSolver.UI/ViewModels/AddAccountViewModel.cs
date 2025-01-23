using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class AddAccountViewModel : INotifyPropertyChanged
{
    public OptionModel Option { get; set; }

    public ObservableCollection<BotEnum> AiBots { get; set; }
    public ObservableCollection<ProgrammingLanguageEnum> Languages { get; set; }

    private ObservableCollection<CompilerEnum> _compilers;

    public ObservableCollection<CompilerEnum> Compilers
    {
        get => _compilers;
        set
        {
            _compilers = value;
            OnPropertyChanged(nameof(Compilers));
        }
    }

    public ICommand SaveCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    public AddAccountViewModel()
    {
        AiBots = new ObservableCollection<BotEnum>(Enum.GetValues(typeof(BotEnum)).Cast<BotEnum>());
        Languages = new ObservableCollection<ProgrammingLanguageEnum>(Enum.GetValues(typeof(ProgrammingLanguageEnum)).Cast<ProgrammingLanguageEnum>());
        Compilers = new ObservableCollection<CompilerEnum>(Enum.GetValues(typeof(CompilerEnum)).Cast<CompilerEnum>());

        Option = new OptionModel();

        SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
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

    private bool CanSave()
    {
        if (Option.AccountName == null) { return false; }
        if (Option.Compiler == null) { return false; }
        return true;
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
