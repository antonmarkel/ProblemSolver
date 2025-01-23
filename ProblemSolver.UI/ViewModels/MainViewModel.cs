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
using ProblemSolver.UI.Messages;
using System.Text;
using ProblemSolver.Shared.Tasks;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using ProblemSolver.UI.Models;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ISolverManager _solverManager;
    private readonly ITaskExtractor _taskExtractor;
    private readonly ILoginService _loginService;
    private readonly ISolverFactory<StandardSolver> _solverFactory;
    private readonly IDlClientFactory _clientFactory;
    private readonly ICourseSubscriptionService _courseSubscriptionService;
    private readonly SolutionQueue _queue;
    private readonly MessageHelper _messageHelper;

    private List<SolverAccount> _accounts;
    private ObservableCollection<SolutionModel> _solutions;
    public List<SolverAccount> Accounts
    {
        get => _accounts;
        set
        {
            _accounts = value;
            OnPropertyChanged(nameof(Accounts));
        }
    }

    public ObservableCollection<SolutionModel> Solutions
    {
        get => _solutions;
        set
        {
            _solutions = value;
            OnPropertyChanged(nameof(Solutions));
        }
    }

    private SolverAccount _selectedSolver;
    private SolutionModel _selectedSolution;

    public SolverAccount SelectedAccount
    {
        get => _selectedSolver;
        set
        {
            _selectedSolver = value;
            OnPropertyChanged();
        }
    }

    public SolutionModel SelectedSolution
    {
        get => _selectedSolution;
        set
        {
            _selectedSolution = value;
            OnPropertyChanged();
        }
    }

    private bool _canAddAccount = true;
    private bool _canRemoveAccount = true;
    private bool _canRefreshAccounts = true;
    private bool _canStartSolving = true;

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

    public bool CanRefreshAccounts
    {
        get => _canRefreshAccounts;
        set
        {
            _canRefreshAccounts = value;
            OnPropertyChanged(nameof(CanRefreshAccounts));
            ((RelayCommand)RefreshAccountsCommand).RaiseCanExecuteChanged();
        }
    }

    public bool CanStartSolving
    {
        get => _canStartSolving;
        set
        {
            _canStartSolving = value;
            OnPropertyChanged(nameof(CanStartSolving));
            ((RelayCommand)StartSolvingCommand).RaiseCanExecuteChanged();  
        }
    }

    private long? _courseId;
    public long? CourseId
    {
        get => _courseId;
        set
        {
            _courseId = value;
            OnPropertyChanged(nameof(CourseId));
        }
    }

    public ICommand AddAccountCommand { get; }
    public ICommand RemoveAccountCommand { get; }
    public ICommand RefreshAccountsCommand { get; }
    public ICommand StartSolvingCommand { get; }

    public ICommand ConCommand { get; }

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
        Solutions = new();

        AddAccountCommand = new RelayCommand(async _ => await AddAccount(), _ => CanAddAccount);
        RemoveAccountCommand = new RelayCommand(async _ => await RemoveAccount(), _ => SelectedAccount != null && CanRemoveAccount);
        RefreshAccountsCommand = new RelayCommand(async _ => await  RefreshAccounts());
        StartSolvingCommand = new RelayCommand(async _ => await StartSolving(), _ => CanStartSolving);
        ConCommand = new RelayCommand(_ => Con());
    }

    public async Task RefreshAccounts()
    {
        CanRefreshAccounts = false;

        Accounts = await _solverManager.GetAllSolversAsync();
        OnPropertyChanged(nameof(Accounts));

        CanRefreshAccounts = true;
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

    public async Task StartSolving()
    {

        CanStartSolving = false;

        Solutions = new();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (CourseId is null)
        {
            _messageHelper.ShowEmptyCourseErrorMessage();
            CanStartSolving = true;
            return;
        }

        Console.WriteLine("Button CLICKED");
        var solvers = new List<StandardSolver>(Accounts.Count);
        foreach (var account in Accounts)
        {
            var solver = _solverFactory.CreateSolver(account);
            var loginResult = await _loginService.LoginAsync(account, solver.HttpClient);
            if (loginResult.IsT1)
            {
                Console.WriteLine("Error while trying to login");
                _messageHelper.ShowLoginSolverErrorMessage();
                continue;
            }
            else
            {
                var courseResult = await _courseSubscriptionService.
                    EnsureSubscriptionToCourseAsync(CourseId.Value, solver.HttpClient);
                if (courseResult.IsT1)
                {
                    _messageHelper.ShowCourseSubscribeErrorMessage();
                    continue;
                }
                solvers.Add(solver);
            }
        }

        var tasksResult = await _taskExtractor.ExtractTasksAsync(CourseId.Value, solvers[0].HttpClient);
        var tasks = tasksResult.AsT0;

        foreach(var solver in solvers)
        {
            var copiedTasks = new List<TaskInfo?>(tasks.Count);
            foreach(var task in tasks) { copiedTasks.Add(task); }

            var solution = new SolutionModel()
            {
                AccountName = solver.GetAccountName(),
                State = SolutionStateEnum.Solving,
                Tasks = await solver.SolveAsync(copiedTasks.ToImmutableList()!),
            };
            
            Solutions.Add(solution);
        }

        _queue.Start();

        CanStartSolving = true;
    }

    public void Con()
    {
        foreach (var task in SelectedSolution.Tasks)
        {
            Console.WriteLine($"{task.Key} --- {task.Value}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
