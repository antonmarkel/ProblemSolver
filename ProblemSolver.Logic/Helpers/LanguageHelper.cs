using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Logic.Helpers
{
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

        public static string LanguageToCompiler(ProgrammingLanguageEnum language)
        {
            switch (language)
            {
                case ProgrammingLanguageEnum.Cpp:
                    return "g131x64";
                case ProgrammingLanguageEnum.Python:
                    return "py311";
            }

            return ".unknown";
        }
    }
}
