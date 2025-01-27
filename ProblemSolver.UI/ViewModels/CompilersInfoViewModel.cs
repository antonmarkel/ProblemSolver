namespace ProblemSolver.UI.ViewModels
{
    // Now it's hardcode, but you can change it, because for every course
    // there are different compilers, so it could doesn't work
    // This viewmodel provides information for user about all compilers
    // dl.gsu.by has
    public class CompilersInfoViewModel
    {
        public Dictionary<string, string> Compilers { get; set; }

        public CompilersInfoViewModel()
        {
            Compilers = new Dictionary<string, string>
            {
                { "g53d", "C++14 GCC 5.3.0 Safe Mode" },
                { "g73", "C++17 GCC 7.3.0" },
                { "g131x64", "C++23 GCC 13.1.0 x64" },
                { "py311", "Python 3.11.3" },
                { "py37", "Python 3.7.4" },
                { "pypy", "Python PyPy 2.7.10" },
            };
        }
    }
}
