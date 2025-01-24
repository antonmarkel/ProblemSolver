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
    }
}
