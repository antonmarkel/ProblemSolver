using System.Net.Http;
using System.Text.Json;
using System.Windows;
using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Results;
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

        public MainWindow(IBotService botService, IAuthService authService, ITaskExtractor taskExtractor)
        {
            _botService = botService;
            _authService = authService;
            _taskExtractor = taskExtractor;
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://dl.gsu.by/")
            };
            var loginRequest = new LoginRequest
            {
                Id = 178538,
                Password = "Markelov2005*15"
            };

            OneOf<Success, WrongCredentials> loginResult = await _authService.LoginAsync(loginRequest, client);
            if (loginResult.IsT1)
            {
                InfoTextBlock.Text = "failed to log in";

                return;
            }

            OneOf<List<string>, Failed> extractResult = await _taskExtractor.ExtractAsync(720, client);
            if (extractResult.IsT1)
            {
                InfoTextBlock.Text = "failed to get tasks";

                return;
            }

            InfoTextBlock.Text = JsonSerializer.Serialize(extractResult.AsT0);
        }
    }
}