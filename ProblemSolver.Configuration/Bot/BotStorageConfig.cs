namespace ProblemSolver.Configuration.Bot;

/// <summary>
///     Config for file-storage repository.
/// </summary>
public class BotStorageConfig
{
    /// <summary>
    ///     Path to the file where bots will be stored
    /// </summary>
    public string FilePath { get; set; } = null!;
}