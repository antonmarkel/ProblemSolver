using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Shared.Bot.Dtos.Requests
{
    public class SolutionRequest
    {
        public string Message { get; set; }
        public BotEnum UseBot { get; set; } = BotEnum.Meta_Llama_3_1_70B_Instruct;
        public ProgrammingLanguageEnum Language { get; set; }
    }
}
