using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using ProblemSolver.Configuration.Bot;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.Helpers;
using ProblemSolver.Shared.Bot.Dtos.Requests;

namespace ProblemSolver.Logic.BotServices.Queues
{
    public class SolutionQueue
    {
        private readonly SolutionQueueConfig _queueConfig;
        private readonly IBotService _botService;
        private readonly ICodeExtractor _codeExtractor;
        private List<Task> Tasks { get; }
        private ConcurrentQueue<(Action<string>, SolutionRequest)> _queue;

        private bool _stopped = true;

        public SolutionQueue(IOptions<SolutionQueueConfig> queueConfig, IBotService botService,
            ICodeExtractor codeExtractor)
        {
            _queueConfig = queueConfig.Value;
            _botService = botService;
            _codeExtractor = codeExtractor;

            Tasks = new List<Task>(_queueConfig.DegreeOfParallelism);
            for (int i = 0; i < Tasks.Capacity; i++)
                Tasks.Add(Task.CompletedTask);

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
                for (int i = 0; i < Tasks.Count; i++)
                    if (Tasks[i].IsCompleted)
                        if (_queue.TryDequeue(out var queueItem))
                        {
                            Tasks[i] = ProcessQueueItemAsync(queueItem);
                            Console.WriteLine("Got a free slot to process! Sent a new task");
                        }

                await Task.Delay(100);
            }
        }

        private async Task ProcessQueueItemAsync((Action<string> callback, SolutionRequest request) queueItem)
        {
            try
            {
                var callback = queueItem.callback;
                var request = queueItem.request;

                Console.WriteLine("Adding task to solutions");
                var result = await _botService.ProcessRequestAsync(request);

                if (result.IsT0)
                {
                    await FileWriter.WriteToFileAsync($"ai-answers/{request.CourseId}/{request.BotName}",
                        $"{request.TaskId}.txt", result.AsT0);

                    string code = _codeExtractor.ExtractCode(result.AsT0);
                    callback.Invoke(code);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing queue item: {ex.Message}");
            }
        }
    }
}
