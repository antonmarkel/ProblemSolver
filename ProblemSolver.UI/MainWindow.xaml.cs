using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.DlServices.Interfaces;
namespace ProblemSolver.UI
{
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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is ProblemModel selectedProblem)
            {
                var container = listView.ItemContainerGenerator.ContainerFromItem(selectedProblem) as ListViewItem;
                if (container != null)
                {
                    var webBrowser = FindVisualChild<WebBrowser>(container);
                    if (webBrowser != null)
                    {
                        webBrowser.NavigateToString(selectedProblem.Description);
                    }
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}   