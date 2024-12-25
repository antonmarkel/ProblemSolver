using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using ProblemSolver.Configuration.Bot;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;

namespace ProblemSolver.Logic.BotServices.Queues
{
    public class SolutionQueue
    {
        private readonly SolutionQueueConfig _queueConfig;
        private readonly IBotService _botService;
        private readonly ICodeExtractor _codeExtractor;
        private SemaphoreSlim _queueSemaphore;
        private ConcurrentQueue<(Action<string>, SolutionRequest)> _queue;

        private bool _stopped = true;

        public SolutionQueue(IOptions<SolutionQueueConfig> queueConfig, IBotService botService,
            ICodeExtractor codeExtractor)
        {
            _queueConfig = queueConfig.Value;
            _botService = botService;
            _codeExtractor = codeExtractor;
            _queueSemaphore = new SemaphoreSlim(_queueConfig.DegreeOfParallelism);
            _queue = new ConcurrentQueue<(Action<string>, SolutionRequest)>();
        }

        public void AddTask(SolutionRequest request, Action<string> callback)
        {
            _queue.Enqueue(new(callback, request));
        }

        public void Start()
        {
            _stopped = false;
            _ = ProcessQueueAsync();

        }

        public void Finish()
        {
            _stopped = true;
        }

        public async Task ProcessQueueAsync()
        {
            while (!_stopped)
            {
                await _queueSemaphore.WaitAsync();

                if (_queue.TryDequeue(out var queueItem))
                {
                    try
                    {
                        var callback = queueItem.Item1;
                        var request = queueItem.Item2;

                        var result = await _botService.ProcessRequestAsync(request);

                        if (result.IsT0)
                        {
                            string code = _codeExtractor.ExtractCode(result.AsT0);
                            callback.Invoke(code);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing queue item: {ex.Message}");
                    }
                    finally
                    {
                        _queueSemaphore.Release();
                    }
                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }
    }
}
