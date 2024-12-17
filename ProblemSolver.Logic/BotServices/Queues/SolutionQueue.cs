using Microsoft.Extensions.Options;
using ProblemSolver.Configuration.Bot;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using System.Collections.Concurrent;

namespace ProblemSolver.Logic.BotServices.Queues
{
    public class SolutionQueue
    {
        private readonly SolutionQueueConfig _queueConfig;
        private readonly IBotService _botService;
        private SemaphoreSlim _queueSemaphore;
        private ConcurrentQueue<(Action<string>, SolutionRequest)> _queue;

        private bool _stopped = true;

        public SolutionQueue(IOptions<SolutionQueueConfig> queueConfig, IBotService botService)
        {
            _queueConfig = queueConfig.Value;
            _botService = botService;
            _queueSemaphore = new SemaphoreSlim(_queueConfig.DegreeOfParallelism);
            _queue = new ConcurrentQueue<(Action<string>, SolutionRequest)>();
        }

        public void AddTask(SolutionRequest request, Action<string> callback)
        {
            _queue.Enqueue(new(callback, request));
        }

        public void Start()
        {
            _stopped = true;
        }

        public void Finish()
        {
            _stopped = true;
        }

        public async Task ProcessQueueAsync()
        {
            while (!_stopped)
            {
                if(!_queue.TryDequeue(out _))
                {
                    await Task.Delay(100);
                    continue;
                }

                _ = Task.Run(async () =>
                {
                    await _queueSemaphore.WaitAsync();

                    if (_queue.TryDequeue(out var queueItem))
                    {
                        var callback = queueItem.Item1;
                        var request = queueItem.Item2;
                        var result = await _botService.ProcessRequestAsync(request);
                        if (result.IsT0)
                        {
                            callback.Invoke(result.AsT0);
                        }
                    }

                    _queueSemaphore.Release();
                });
        
            }
        }
    }
}
