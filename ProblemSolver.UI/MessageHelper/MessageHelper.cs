using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProblemSolver.UI.Messages
{
    public class MessageHelper
    {
        private const string IncorrectAccountDataTitle = "Incorrect Data!";
        private const string IncorrectAccountDataMessage = "Your entered incorrect data, try again!";

        private const string RemoveAccountErrorTitle = "Remove account error!";
        private const string RemoveAccountErrorMessage = "Error occured while trying to remove account!";

        public void ShowIncorrectAccountDataMessage()
        {
            MessageBox.Show(IncorrectAccountDataMessage, IncorrectAccountDataTitle);
        }

        public void ShowRemoveAccountErrorMessage()
        {
            MessageBox.Show(RemoveAccountErrorMessage, RemoveAccountErrorTitle);
        }
    }
}
