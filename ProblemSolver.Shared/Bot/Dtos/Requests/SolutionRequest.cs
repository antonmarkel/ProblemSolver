using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Shared.Bot.Dtos.Requests
{
    public class SolutionRequest
    {
        public required string Message { get; set; }
        public BotEnum UseBot { get; set; } = BotEnum.Meta_Llama_3_1_70B_Instruct;
        public ProgrammingLanguageEnum Language { get; set; }
        public required string BotName { get; set; }
        public required short Attempt { get; set; }
        public required long TaskId { get; set; }
        public required long CourseId { get; set; }
    }
}
