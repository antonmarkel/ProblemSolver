using ProblemSolver.UI.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.SolverServices.Implementations;
using ProblemSolver.Shared.DL.Models;
using ProblemSolver.Shared.Solvers;
using ProblemSolver.UI.ViewModels;
using System.Windows;
using ProblemSolver.UI.Messages;

public class MainViewModel : INotifyPropertyChanged
{
    private SolverAccount _selectedSolver;

    private readonly ISolverManager _solverManager;
    private readonly ITaskExtractor _taskExtractor;
    private readonly ILoginService _loginService;
    private readonly ISolverFactory<StandardSolver> _solverFactory;
    private readonly IDlClientFactory _clientFactory;
    private readonly ICourseSubscriptionService _courseSubscriptionService;
    private readonly SolutionQueue _queue;
    private readonly MessageHelper _messageHelper;

    private bool _canAddAccount = true;
    private bool _canEditAccount = true;
    private bool _canRemoveAccount = true;

    public bool CanAddAccount
    { 
        get => _canAddAccount;
        set
        {
            _canAddAccount = value;
            OnPropertyChanged(nameof(CanAddAccount));
            ((RelayCommand)AddAccountCommand).RaiseCanExecuteChanged();
        }
    }

    public bool CanEditAccount
    {
        get => _canEditAccount;
        set
        {
            _canEditAccount = value;
            OnPropertyChanged(nameof(CanAddAccount));
            ((RelayCommand)EditAccountCommand).RaiseCanExecuteChanged();
        }
    }

    public bool CanRemoveAccount
    {
        get => _canRemoveAccount;
        set
        {
            _canRemoveAccount = value;
            OnPropertyChanged(nameof(CanRemoveAccount));
            ((RelayCommand)RemoveAccountCommand).RaiseCanExecuteChanged();
        }
    }


    private List<SolverAccount> _accounts;
    public List<SolverAccount> Accounts
    { 
        get => _accounts;
        set
        {
            _accounts = value;
            OnPropertyChanged(nameof(Accounts));
        }
    }
    public SolverAccount SelectedAccount
    {
        get => _selectedSolver;
        set
        {
            _selectedSolver = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddAccountCommand { get; }
    public ICommand EditAccountCommand { get; }
    public ICommand RemoveAccountCommand { get; }
    public ICommand RefreshAccountsCommand { get;}

    public MainViewModel(ISolverManager solverManager, ITaskExtractor taskExtractor, ILoginService loginService,
            ISolverFactory<StandardSolver> solverFactory, IDlClientFactory clientFactory,
            ICourseSubscriptionService courseSubscriptionService, SolutionQueue queue, MessageHelper messageHelper)
    {
        _solverManager = solverManager;
        _taskExtractor = taskExtractor;
        _loginService = loginService;
        _solverFactory = solverFactory;
        _clientFactory = clientFactory;
        _courseSubscriptionService = courseSubscriptionService;
        _queue = queue;
        _messageHelper = messageHelper;

        Accounts = _solverManager.GetAllSolversSync();

        AddAccountCommand = new RelayCommand(async _ => await AddAccount(), _ => CanAddAccount);
        EditAccountCommand = new RelayCommand(async _ => await EditAccount(), _ => SelectedAccount != null && CanEditAccount);
        RemoveAccountCommand = new RelayCommand(async _ => await RemoveAccount(), _ => SelectedAccount != null && CanRemoveAccount);
        RefreshAccountsCommand = new RelayCommand(async _ => await  RefreshAccounts());
    }

    public async Task RefreshAccounts()
    {
        Accounts = await _solverManager.GetAllSolversAsync();
        OnPropertyChanged(nameof(Accounts));
    }

    public async Task AddAccount()
    {
        CanAddAccount = false;

        var addAccountWindow = new AddAccountWindow()
        {
            DataContext = new AddAccountViewModel()
        };

        if (addAccountWindow.ShowDialog() == true)
        {
            var viewModel = (AddAccountViewModel)addAccountWindow.DataContext;

            var settings = new SolverSettings()
            {
                AiBot = viewModel.Option.AiBot,
                Language = viewModel.Option.Language,
                Name = viewModel.Option.AccountName,
            };
            var client = _clientFactory.CreateClient();

            var result = await _solverManager.AddSolverAccountAsync(settings, client);

            if (result.IsT1)
            {
                _messageHelper.ShowIncorrectAccountDataMessage();
            }

            Accounts = await _solverManager.GetAllSolversAsync();
            OnPropertyChanged(nameof(Accounts));
        }

        CanAddAccount = true;
    }

    public async Task EditAccount()
    {
        CanEditAccount = false;

        var editAccountWindow = new EditAccountWindow()
        {
            DataContext = new EditAccountViewModel(SelectedAccount)
        };

        if (editAccountWindow.ShowDialog() == true)
        {
            var viewModel = (EditAccountViewModel)editAccountWindow.DataContext;

            SelectedAccount.Bot = viewModel.Option.AiBot;
            SelectedAccount.Language = viewModel.Option.Language;
            SelectedAccount.Name = viewModel.Option.AccountName;

            var result = await _solverManager.UpdateSolverAccountAsync(SelectedAccount);

            if (result.IsT1)
            {
                _messageHelper.ShowIncorrectAccountDataMessage();
            }

            Accounts = await _solverManager.GetAllSolversAsync();
            OnPropertyChanged(nameof(Accounts));
        }

        CanEditAccount = true;
    }

    public async Task RemoveAccount()
    {
        CanRemoveAccount = false;

        var result = await _solverManager.RemoveSolverAccountAsync(SelectedAccount);
        if (result.IsT1)
        {
            _messageHelper.ShowRemoveAccountErrorMessage();
        }
        Accounts = await _solverManager.GetAllSolversAsync();
        OnPropertyChanged(nameof(Accounts));

        CanRemoveAccount = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
