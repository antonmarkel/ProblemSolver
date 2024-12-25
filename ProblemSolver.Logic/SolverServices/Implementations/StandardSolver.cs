using System.Collections.Concurrent;
using System.Collections.Immutable;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.DL.Models;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.Shared.Tasks.Enums;

namespace ProblemSolver.Logic.SolverServices.Implementations;

public class StandardSolver : ISolver
{
    private readonly ImmutableList<TaskInfo> _tasks;
    private readonly SolverAccount _account;
    private readonly SolutionQueue _queue;
    private readonly HttpClient _httpClient;
    private readonly ITaskSender _taskSender;
    private readonly IMessageConstructor _messageConstructor;
    private readonly ConcurrentQueue<TaskSolution> _checkingQueue = new();
    public ConcurrentDictionary<long, TaskState> TaskStates { get; }

    public StandardSolver(ImmutableList<TaskInfo> tasks, SolutionQueue queue, HttpClient httpClient,
        SolverAccount account, ITaskSender taskSender, IMessageConstructor messageConstructor)
    {
        _tasks = tasks;
        _queue = queue;
        _httpClient = httpClient;
        _account = account;
        _taskSender = taskSender;
        _messageConstructor = messageConstructor;

        TaskStates = new ConcurrentDictionary<long, TaskState>();
        foreach (var task in tasks) TaskStates.TryAdd(task.TaskId, TaskState.Awaiting);
    }

    public async Task SolveAsync()
    {
        _ = Task.Run(ProcessCheckingAsync);

        foreach (var task in _tasks)
        {
            string message = _messageConstructor.ConvertToMessage(task.Task!, _account.Language);
            var solutionRequest = new SolutionRequest
            {
                Language = _account.Language,
                Message = message,
                UseBot = _account.Bot
            };

            Console.WriteLine($"Sent task to solving! {task.TaskId}");
            _queue.AddTask(solutionRequest, response =>
            {
                Console.WriteLine($"We've got code for {task.TaskId}");
                _checkingQueue.Enqueue(new TaskSolution(response, _account.Language)
                {
                    CourseId = task.CourseId,
                    TaskId = task.TaskId
                });
            });
        }
    }


    private async Task ProcessCheckingAsync()
    {
        while (true)
        {
            if (!_checkingQueue.IsEmpty)
                if (_checkingQueue.TryDequeue(out var solution))
                {
                    await _taskSender.SendToCheckAsync(solution, _httpClient);
                    Console.WriteLine($"Solution to {solution.TaskId} was sent!");
                    await Task.Delay(10000);
                }

            await Task.Delay(500);
        }
    }
}