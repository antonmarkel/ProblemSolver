using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Shared.Bot.Dtos.Responses;

public class CodeResponse
{
    public ProgrammingLanguageEnum ProgrammingProgrammingLanguage { get; }
    public required string Code { get; set; }

    public CodeResponse(ProgrammingLanguageEnum programmingProgrammingLanguage, string code)
    {
        ProgrammingProgrammingLanguage = programmingProgrammingLanguage;
        Code = code;
    }
}