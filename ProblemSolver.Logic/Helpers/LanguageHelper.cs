using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Logic.Helpers
{
    //TODO: Improve this one, some courses have a different set up for compilers.
    /// <summary>
    ///     Responsible for getting constant settings for programming language
    /// </summary>
    public static class LanguageHelper
    {
        public static string LanguageToFileExtension(ProgrammingLanguageEnum language)
        {
            switch (language)
            {
                case ProgrammingLanguageEnum.Cpp:
                    return "cpp";
                case ProgrammingLanguageEnum.Python:
                    return "py";
            }

            return ".unknown";
        }

        public static string GetComplierName(CompilerEnum compiler)
        {
            switch (compiler)
            {
                case CompilerEnum.PY:
                    return "PY";
                case CompilerEnum.py37:
                    return "py37";
                case CompilerEnum.MVC9:
                    return "MVC9";
                case CompilerEnum.g53d:
                    return "g53d";
                case CompilerEnum.g73:
                    return "g73";
                default:
                    return ".unknown";
            }  
        }
    }
}
