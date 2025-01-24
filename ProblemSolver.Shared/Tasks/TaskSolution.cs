using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Shared.Tasks
{
    public class TaskSolution
    {
        public TaskSolution(string code, ProgrammingLanguageEnum language, string compiler)
        {
            Code = code;
            Language = language;
            Compiler = compiler;
        }

        public required string SolutionName { get; set; }
        public long CourseId { get; set; }
        public long TaskId { get; set; }
        public string Code { get; }
        public ProgrammingLanguageEnum Language { get; }
        public string Compiler { get; set; }
    }
}
