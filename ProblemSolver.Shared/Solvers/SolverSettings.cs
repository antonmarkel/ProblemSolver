using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Shared.Solvers
{
    public class SolverSettings
    {
        public required string Name { get; set; }
        public required ProgrammingLanguageEnum Language { get; set; }
        public required CompilerEnum Compiler { get; set; }
        public required BotEnum AiBot { get; set; }
    }
}
