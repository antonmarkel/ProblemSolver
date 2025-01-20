using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class AddAccountViewModel : INotifyPropertyChanged
{
    private OptionModel _option;
    public OptionModel Option
    {
        get => _option;
        set
        {
            _option = value;
            OnPropertyChanged(nameof(Option));
        }
    }

    public ObservableCollection<BotEnum> AiBots { get; set; }
    public ObservableCollection<ProgrammingLanguageEnum> Languages { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public AddAccountViewModel()
    {
        Option = new OptionModel();

        AiBots = new ObservableCollection<BotEnum>(Enum.GetValues(typeof(BotEnum)).Cast<BotEnum>());
        Languages = new ObservableCollection<ProgrammingLanguageEnum>(Enum.GetValues(typeof(ProgrammingLanguageEnum)).Cast<ProgrammingLanguageEnum>());

        SaveCommand = new RelayCommand(_ => Save());
        CancelCommand = new RelayCommand(_ => Cancel());
    }

    private async void Save()
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
