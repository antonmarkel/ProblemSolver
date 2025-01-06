using System.Collections.Immutable;
using System.Text;
using System.Windows;
using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.SolverServices.Implementations;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.UI
{
    public partial class MainWindow : Window
    {
        private readonly ISolverManager _solverManager;
        private readonly ITaskExtractor _taskExtractor;
        private readonly ILoginService _loginService;
        private readonly ISolverFactory<StandardSolver> _solverFactory;
        private readonly ICourseSubscriptionService _courseSubscriptionService;
        private readonly SolutionQueue _queue;

        public MainWindow(ISolverManager solverManager, ITaskExtractor taskExtractor,
            ISolverFactory<StandardSolver> solverFactory, ICourseSubscriptionService courseSubscriptionService,
            ILoginService loginService, SolutionQueue queue)
        {
            _solverManager = solverManager;
            _taskExtractor = taskExtractor;
            _solverFactory = solverFactory;
            _courseSubscriptionService = courseSubscriptionService;
            _loginService = loginService;
            _queue = queue;

            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Console.WriteLine("Button Clicked! Process will now begin!");
            var solverAccounts = await _solverManager.GetAllSolversAsync();
            int courseId = 1302;

            var solvers = new List<StandardSolver>(solverAccounts.Count);
            foreach (var account in solverAccounts)
            {
                var solver = _solverFactory.CreateSolver(account);
                await _loginService.LoginAsync(account, solver.HttpClient);
                await _courseSubscriptionService.EnsureSubscriptionToCourseAsync(courseId, solver.HttpClient);
                solvers.Add(solver);
            }

            var tasksResult = await _taskExtractor.ExtractTasksAsync(courseId, solvers[0].HttpClient);

            var tasks = tasksResult.AsT0;
            foreach (var solver in solvers)
            {
                var copiedTasks = new List<TaskInfo?>(tasks.Count);
                foreach (var task in tasks) copiedTasks.Add(task);
                Console.WriteLine("Starting solver");
                _ = solver.SolveAsync(copiedTasks.ToImmutableList()!);
            }

            _queue.Start();
        }
    }
}