using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Shared.DL.Models;

public class SolverAccount
{
    public required string Name { get; set; }
    public required string Nick { get; set; }
    public required string Id { get; set; }
    public required string Password { get; set; }
    public required ProgrammingLanguageEnum Language { get; set; }
    public required string Compiler { get; set; }
    public required BotEnum Bot { get; set; }
}