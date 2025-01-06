namespace ProblemSolver.Logic.BotServices.Interfaces;

/// <summary>
///     Interface that responsible for extracting code from ai response
/// </summary>
public interface ICodeExtractor
{
    public string ExtractCode(string data);
}