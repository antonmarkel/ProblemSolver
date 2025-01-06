namespace ProblemSolver.Shared.Tasks.Enums
{
    public enum TaskState : byte
    {
        None = 0,
        Awaiting = 1,
        Solving = 2,
        Checking = 3,
        Retrying = 4,
        Solved = 5,
        NotSolved = 6
    }
}
