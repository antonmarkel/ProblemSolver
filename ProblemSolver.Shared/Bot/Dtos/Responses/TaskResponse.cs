using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Shared.Bot.Dtos.Responses;

public class TaskResponse
{
    public ProgrammingLanguageEnum ProgrammingProgrammingLanguage { get; }
    public string Code { get; set; }

    public TaskResponse(ProgrammingLanguageEnum programmingProgrammingLanguage, string code)
    {
        ProgrammingProgrammingLanguage = programmingProgrammingLanguage;
        Code = code;
    }
}