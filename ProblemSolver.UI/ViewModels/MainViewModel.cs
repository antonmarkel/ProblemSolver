using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.SolverServices.Implementations;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Shared.DL.Models;
using ProblemSolver.Shared.Solvers;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.Shared.Tasks.Enums;
using ProblemSolver.UI.Messages;
using ProblemSolver.UI.Models;
using ProblemSolver.UI.ViewModels;
using ProblemSolver.UI.Views;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

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
    private Timer _updateTimer;

    private List<SolverAccount> _accounts;
    private ObservableCollection<SolutionModel> _solutions;
    public List<StandardSolver> Solvers { get; set; } = [];
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
    private bool _isStartSolvingMethodCompleted = true;

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

    public bool IsStartSolvingMethodCompleted
    {
        get => _isStartSolvingMethodCompleted;
        set
        {
            _isStartSolvingMethodCompleted = value;
            OnPropertyChanged(nameof(IsStartSolvingMethodCompleted));
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
    public ICommand ToggleConsoleCommand { get; }
    public ICommand ShowCompilersCommand { get; }

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
        RefreshAccountsCommand = new RelayCommand(async _ => await RefreshAccounts());
        StartSolvingCommand = new RelayCommand(async _ => await StartSolving(), _ => CanStartSolving());
        ToggleConsoleCommand = new RelayCommand(_ => ToggleConsole());
        ShowCompilersCommand = new RelayCommand(_ => ShowCompilers());

        // Because console toggles automaticly, so we need to turn it off
        HideConsole();

        _updateTimer = new Timer(UpdateSolutions, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
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
                Compiler = viewModel.Option.Compiler,
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

    private bool CanStartSolving()
    {
        if (!IsStartSolvingMethodCompleted)
        {
            return false;
        }
        foreach (var solution in Solutions)
        {
            if (solution.State == SolutionStateEnum.Solving)
            {
                return false;
            }
        }

        return true;
    }

    public void ShowCompilers()
    {
        var compilersWindow = new CompilersInfo()
        {
            DataContext = new CompilersInfoViewModel()
        };
        compilersWindow.Show();
    }

    public async Task StartSolving()
    {
        IsStartSolvingMethodCompleted = false;

        Solutions = new();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (CourseId is null)
        {
            _messageHelper.ShowEmptyCourseErrorMessage();
            IsStartSolvingMethodCompleted = true;
            return;
        }

        Console.WriteLine("STARTING...");
        Solvers = new List<StandardSolver>(Accounts.Count);
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
                    Console.WriteLine("Error while trying to subscribe to course");
                    _messageHelper.ShowCourseSubscribeErrorMessage();
                    continue;
                }
                Solvers.Add(solver);
            }
        }

        var tasksResult = await _taskExtractor.ExtractTasksAsync(CourseId.Value, Solvers[0].HttpClient);
        var tasks = tasksResult.AsT0;

        foreach (var solver in Solvers)
        {
            var copiedTasks = new List<TaskInfo?>(tasks.Count);
            foreach (var task in tasks) { copiedTasks.Add(task); }
            await solver.SolveAsync(copiedTasks.ToImmutableList()!);
            var solution = new SolutionModel()
            {
                AccountName = solver.GetAccountName(),
                State = SolutionStateEnum.Solving,
                Tasks = solver.GetTasks(),
            };

            Solutions.Add(solution);
        }
        _queue.Start();

        IsStartSolvingMethodCompleted = true;
    }

    private async void UpdateSolutions(object state)
    {
        foreach (var solution in Solutions)
        {
            var solver = Solvers.FirstOrDefault(s => s.GetAccountName() == solution.AccountName);
            var tasks = solver.GetTasks();


            Application.Current.Dispatcher.Invoke(() =>
            {
                solution.Tasks = tasks;

                // Проверка состояния решения
                if (solution.Tasks.All(task => task.Value == TaskState.Solved || task.Value == TaskState.NotSolved))
                {
                    solution.State = SolutionStateEnum.Finished;
                }
                OnPropertyChanged(nameof(Solutions));
                OnPropertyChanged(nameof(SelectedSolution));
            });
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    // CONSOLE
    // It's shit code, because idk how to make toggle console smart

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    private static bool IsConsoleVisible = true;

    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    public static void HideConsole()
    {
        var handle = GetConsoleWindow();
        IsConsoleVisible = false;
        ShowWindow(handle, SW_HIDE);
    }

    public static void ShowConsole()
    {
        var handle = GetConsoleWindow();
        IsConsoleVisible = true;
        ShowWindow(handle, SW_SHOW);
    }

    public static void ToggleConsole()
    {
        if (IsConsoleVisible)
        {
            HideConsole();
        }
        else
        {
            ShowConsole();
        }
    }
}
