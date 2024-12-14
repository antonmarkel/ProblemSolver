namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface ITaskSender
{
    Task SendToCheckAsync(string filePath, HttpClient client, long courseId, long taskId);
}