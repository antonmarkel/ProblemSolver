namespace ProblemSolver.Configuration.Bot
{
    public class SolutionQueueConfig
    {
        /// <summary>
        ///     The number that restricts the concurrent number of task run at the same time.(It doesn't mean that we have
        ///     concurrent threads.)
        ///     Tested with number up to 30.
        ///     Simply speaking, if you have this value equals to 10,
        ///     that means that 10 connections with ai will be kept at the same time -> 10 tasks solving at the same time.
        /// </summary>
        public ushort DegreeOfParallelism { get; set; }
    }
}
