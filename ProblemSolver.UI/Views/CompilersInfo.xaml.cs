using ProblemSolver.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
