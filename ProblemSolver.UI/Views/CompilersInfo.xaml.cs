using ProblemSolver.UI.ViewModels;
using System.Windows;

namespace ProblemSolver.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для CompilersInfo.xaml
    /// </summary>
    public partial class CompilersInfo : Window
    {
        public CompilersInfo()
        {
            InitializeComponent();
            DataContext = new CompilersInfoViewModel();
        }
    }
}
