using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.Shared.Solvers;

namespace ProblemSolver.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISolverManager _solverManager;

        public MainWindow(ISolverManager solverManager)
        {
            _solverManager = solverManager;

            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var client = CreateDlClient();
            var accountSettings = new SolverSettings
            {
                AiBot = BotEnum.Meta_Llama_3_1_70B_Instruct,
                Language = ProgrammingLanguageEnum.Cpp,
                Name = "Beta"
            };
            _ = await _solverManager.AddSolverAccountAsync(accountSettings, client);
            accountSettings.Language = ProgrammingLanguageEnum.Python;
            accountSettings.Name = "Beta py";
            _ = await _solverManager.AddSolverAccountAsync(accountSettings, client);
        }

        private HttpClient CreateDlClient()
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://dl.gsu.by/")
            };

            return client;
        }
    }
}