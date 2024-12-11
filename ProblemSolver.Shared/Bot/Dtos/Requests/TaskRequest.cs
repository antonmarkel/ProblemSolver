using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Shared.Bot.Dtos.Requests;

public class TaskRequest
{
    public required TaskModel Task { get; set; }
    public required ProgrammingLanguageEnum Language { get; set; }
    public BotEnum UseBot { get; set; } = BotEnum.Meta_Llama_3_1_70B_Instruct;
}