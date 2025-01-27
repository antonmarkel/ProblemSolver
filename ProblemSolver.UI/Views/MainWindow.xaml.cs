using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.SolverServices.Implementations;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.UI.Messages;
using System.Windows;


namespace ProblemSolver.UI
{
    public partial class MainWindow : Window
    {
        // DI for MainViewModel
        public MainWindow(ISolverManager solverManager, ITaskExtractor taskExtractor, ILoginService loginService,
            ISolverFactory<StandardSolver> solverFactory, IDlClientFactory clientFactory,
            ICourseSubscriptionService courseSubscriptionService, SolutionQueue queue, MessageHelper messageHelper)
        {
            InitializeComponent();
            DataContext = new MainViewModel(solverManager, taskExtractor, loginService,
                solverFactory, clientFactory, courseSubscriptionService, queue, messageHelper);
        }
    }
}
