using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemSolver.Shared.Bot.Enums
{
    public enum CompilerEnum
    {
        [Description("Python 3.3.2")]
        PY,
        [Description("Python 3.7.4")]
        py37,
        [Description("C++14 GCC 5.3.0 Safe Mode")]
        g53d,
        [Description("C++17 7.3.0")]
        g73,
        [Description("MS Visual C++ 9.0")]
        MVC9
    }
}
