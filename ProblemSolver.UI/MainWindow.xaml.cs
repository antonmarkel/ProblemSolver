using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Windows;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.UI.DL.Auth;

namespace ProblemSolver.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IBotService _botService;
        private readonly IAuthService _authService;
        private readonly ITaskExtractor _taskExtractor;
        private readonly ITaskTextProcessor _taskTextProcessor;
        private readonly ITaskSender _taskSender;

        public MainWindow(IBotService botService, IAuthService authService, ITaskExtractor taskExtractor,
            ITaskTextProcessor taskTextProcessor, ITaskSender taskSender)
        {
            _botService = botService;
            _authService = authService;
            _taskExtractor = taskExtractor;
            _taskTextProcessor = taskTextProcessor;
            _taskSender = taskSender;


            InitializeComponent();

            DataContext = new ApplicationViewModel(botService, authService, taskExtractor, taskTextProcessor, taskSender);
        }
    }
}