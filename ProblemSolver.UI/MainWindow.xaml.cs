using System.Windows;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.Shared.Tasks.Examples;

namespace ProblemSolver.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IBotService _botService;

        public MainWindow(IBotService botService)
        {
            _botService = botService;
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var request = new TaskRequest
            {
                Language = ProgrammingLanguageEnum.Cpp,
                Task = new TaskModel
                {
                    Config = new TaskConfig
                    {
                        InputFile = "br1.in",
                        OutputFile = "br1.out",
                        TimeLimit = 2,
                        MemoryLimit = 64
                    },
                    FormatExamples =
                    [
                        new FormatExample
                        {
                            Input = "Одно целое число X",
                            Output =
                                "Сумма всех делителей числа X"
                        }
                    ],
                    SolutionExamples =
                    [
                        new SolutionExample
                        {
                            InputExample = "12", OutputExample = """28"""
                        }
                    ],
                    TaskText =
                        "Посчитать сумму всех делителей заданного числа X (1 <= I <= 1,000,000). Например, для числа 12 делители - 1, 2, 3, 4, 6, 12. Суммируя их, получаем 28"
                }
            };

            var response = await _botService.ProcessRequestAsync(request);
            InfoTextBlock.Text = $"{response.Code}\n{response.ProgrammingProgrammingLanguage}";
        }
    }
}