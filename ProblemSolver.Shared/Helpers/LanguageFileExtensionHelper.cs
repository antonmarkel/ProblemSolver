using ProblemSolver.Shared.Bot.Enums;

public class LanguageFileExtensionHelper
{
    private static readonly Dictionary<ProgrammingLanguageEnum, string> _fileExtensions = new()
    {
        {ProgrammingLanguageEnum.Python, "py" },
        {ProgrammingLanguageEnum.Cpp,  "cpp"}
    };

    public string GetFileExtension(ProgrammingLanguageEnum programmingLanguage)
    {
        if (_fileExtensions.TryGetValue(programmingLanguage, out var extension))
        {
            return extension;
        }

        throw new Exception();
    }
}