using System.Windows;

namespace ProblemSolver.UI.Messages
{
    public class MessageHelper
    {
        private const string IncorrectAccountDataTitle = "Incorrect Data!";
        private const string IncorrectAccountDataMessage = "Your entered incorrect data, try again!";

        private const string RemoveAccountErrorTitle = "Remove account error!";
        private const string RemoveAccountErrorMessage = "Error occured while trying to remove account!";

        private const string LoginSolverErrorTitle = "Login solver error!";
        private const string LoginSolverErrorMessage = "Error occured while trying to login solver";

        private const string CourseSubscribeErrorTItle = "Course subscribe error!";
        private const string CourseSubscribeErrorMessage = "Error occured while trying to subscribe to course";

        public void ShowIncorrectAccountDataMessage()
        {
            MessageBox.Show(IncorrectAccountDataMessage, IncorrectAccountDataTitle);
        }

        public void ShowRemoveAccountErrorMessage()
        {
            MessageBox.Show(RemoveAccountErrorMessage, RemoveAccountErrorTitle);
        }

        public void ShowLoginSolverErrorMessage()
        {
            MessageBox.Show(LoginSolverErrorMessage, LoginSolverErrorTitle);
        }

        public void ShowCourseSubscribeErrorMessage()
        {
            MessageBox.Show(CourseSubscribeErrorMessage, CourseSubscribeErrorTItle);
        }
    }
}
