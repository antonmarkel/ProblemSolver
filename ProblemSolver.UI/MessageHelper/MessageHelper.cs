using System.Windows;

namespace ProblemSolver.UI.Messages
{
    // Useful when you handling errors and then showing to user what's wrong, I added some
    // simple cases, but if you want, you can modify this class
    public class MessageHelper
    {
        private const string IncorrectAccountDataTitle = "Incorrect Data!";
        private const string IncorrectAccountDataMessage = "Your entered incorrect data, try again!";

        private const string RemoveAccountErrorTitle = "Remove account error!";
        private const string RemoveAccountErrorMessage = "Error occured while trying to remove account!";

        private const string LoginSolverErrorTitle = "Login solver error!";
        private const string LoginSolverErrorMessage = "Error occured while trying to login solver!";

        private const string CourseSubscribeErrorTItle = "Course subscribe error!";
        private const string CourseSubscribeErrorMessage = "Error occured while trying to subscribe to course!";

        private const string EmptyCourseErrorTitle = "Empty course error!";
        private const string EmptyCourseErrorMessage = "Your course Id is empty!";
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

        public void ShowEmptyCourseErrorMessage()
        {
            MessageBox.Show(EmptyCourseErrorMessage, EmptyCourseErrorTitle);
        }
    }
}
