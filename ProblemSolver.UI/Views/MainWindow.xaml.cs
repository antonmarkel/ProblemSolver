using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.SolverServices.Implementations;
using ProblemSolver.Logic.SolverServices.Interfaces;
using System.Windows;


namespace ProblemSolver.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(ISolverManager solverManager, ITaskExtractor taskExtractor, ILoginService loginService,
            ISolverFactory<StandardSolver> solverFactory, IDlClientFactory clientFactory, ICourseSubscriptionService courseSubscriptionService, SolutionQueue queue)
        {
            InitializeComponent();
            DataContext = new MainViewModel(solverManager, taskExtractor, loginService, solverFactory, clientFactory, courseSubscriptionService, queue);
        }
    }
}
