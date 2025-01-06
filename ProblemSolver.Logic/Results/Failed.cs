namespace ProblemSolver.Logic.Results;

//An empty struct for returning type that indicates that method finished unsuccessfully

public struct Failed
{
    //TODO: I haven't used this one, but it would be better to have more info why sth failed.
    public string ErrorMessage { get; set; }
}