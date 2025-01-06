namespace ProblemSolver.Configuration.Bot;

public class BotConnectionConfig
{
    /// <summary>
    ///     The size of buffer that will be received from web-socket(DLAPI communication)
    /// </summary>
    public int BufferSize { get; set; }

    /// <summary>
    ///     The duration of keeping connection with server.
    ///     (It would be a better practice to wait while server finishes it's sending. But it's the way for now)
    /// </summary>
    public int TimeOutInSeconds { get; set; }

    /// <summary>
    ///     Connection string to web socket host. E.g. wss://dl.gsu.by/ai/chat/ws
    /// </summary>
    public required string ConnectionString { get; set; }
}