using ProblemSolver.Logic.BotServices.Interfaces;
using System.Text;

namespace ProblemSolver.Logic.BotServices.Implementations.CodeExtractors
{
    /// <summary>
    ///     Used with current prompts where code is between '''
    /// </summary>
    public class StandardExtractor : ICodeExtractor
    {
        public string ExtractCode(string data)
        {
            string[] codePartSeqs = data.Split("```");

            if (codePartSeqs.Length < 2)
                return "Failed to extract code!";

            string codePart = codePartSeqs[1];
            var codeBuilder = new StringBuilder();
            bool add = false;
            foreach (char symbol in codePart)
            {
                if (add)
                    codeBuilder.Append(symbol);

                if (symbol == '\n')
                    add = true;
            }

            return codeBuilder.ToString();
        }
    }
}
