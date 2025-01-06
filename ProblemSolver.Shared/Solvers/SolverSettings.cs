using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Shared.Solvers
{
    public class SolverSettings
    {
        public required string Name { get; set; }
        public ProgrammingLanguageEnum Language { get; set; }
        public BotEnum AiBot { get; set; }
    }
}
