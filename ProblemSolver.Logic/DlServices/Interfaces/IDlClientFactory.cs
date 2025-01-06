namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface IDlClientFactory
{
    HttpClient CreateClient();
}