using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using ProblemSolver.Configuration.Solvers;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Implementations;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Helpers;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.DL.Models;
using ProblemSolver.Shared.Solvers;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.Shared.Tasks.Enums;

namespace ProblemSolver.Logic.SolverServices.Implementations;

public class StandardSolver : ISolver
{
    private readonly SolverAccount _account;
    private readonly SolutionQueue _queue;
    private readonly ITaskSender _taskSender;
    private readonly IMessageConstructor _messageConstructor;
    private readonly ISolutionVerdictLogAccessor _verdictAccessor;
    private readonly RetryPolicy _retryPolicy;

    private ImmutableList<TaskInfo> _currentTasks;

    private readonly ConcurrentQueue<TaskSolution>
        _sendingQueue = new(),
        _checkingQueue = new();

    private bool Finished { get; set; }
    private ConcurrentDictionary<long, TaskState> _taskStates;
    private ConcurrentDictionary<long, short> _taskRetries;

    public HttpClient HttpClient { get; }

    public StandardSolver(SolutionQueue queue, HttpClient httpClient,
        SolverAccount account, ITaskSender taskSender, IMessageConstructor messageConstructor,
        ISolutionVerdictLogAccessor verdictAccessor, IOptions<RetryPolicy> retryOptions)
    {
        _queue = queue;
        HttpClient = httpClient;
        _account = account;
        _taskSender = taskSender;
        _messageConstructor = messageConstructor;
        _verdictAccessor = verdictAccessor;
        _retryPolicy = retryOptions.Value;
    }

    public async Task<ConcurrentDictionary<long, TaskState>> SolveAsync(ImmutableList<TaskInfo> tasks)
    {
        _ = ProcessSendingAsync();
        _ = ProcessCheckingAsync();
        _currentTasks = tasks;

        _taskStates = new ConcurrentDictionary<long, TaskState>();
        _taskRetries = new ConcurrentDictionary<long, short>();


        foreach (var task in tasks)
        {
            _taskStates.TryAdd(task.TaskId, TaskState.Awaiting);
            _taskRetries.TryAdd(task.TaskId, 0);
            SendTaskToSolve(task);
        }

        return _taskStates;
    }

    /// <summary>
    ///     Communicating with SolutionQueue to send task to solution
    /// </summary>
    /// <param name="task"></param>
    /// <param name="additionalProps"></param>
    private void SendTaskToSolve(TaskInfo task, string[]? additionalProps = null)
    {
        short attempt = _taskRetries[task.TaskId];
        string message = _messageConstructor.ConvertToMessage(task.Task!, _account.Language, additionalProps);
        _ = FileWriter.WriteToFileAsync($"props/{task.CourseId}/{_account.Name}",
            $"{task.TaskId}_{attempt}.txt", message);

        var solutionRequest = new SolutionRequest
        {
            Language = _account.Language,
            Message = message,
            UseBot = _account.Bot,
            Attempt = attempt,
            BotName = _account.Name,
            TaskId = task.TaskId,
            CourseId = task.CourseId
        };
        _taskStates[task.TaskId] = TaskState.Solving;
        Console.WriteLine($"Sent task to solving! {task.TaskId}");
        _queue.AddTask(solutionRequest, response =>
        {
            _taskStates[task.TaskId] = TaskState.Checking;
            _sendingQueue.Enqueue(new TaskSolution(response, _account.Language, _account.Compiler)
            {
                CourseId = task.CourseId,
                TaskId = task.TaskId,
                SolutionName = $"{task.TaskId}_{attempt}"
            });
            //Create two same objects because of concurrent issues
            _checkingQueue.Enqueue(new TaskSolution(response, _account.Language, _account.Compiler)
            {
                CourseId = task.CourseId,
                TaskId = task.TaskId,
                SolutionName = $"{task.TaskId}_{attempt}"
            });
        });
    }

    /// <summary>
    ///     Handles the situation where task wasn't solved
    /// </summary>
    /// <param name="solution"></param>
    /// <param name="verdict"></param>
    private void ResendTaskToSolve(TaskSolution solution, SolutionVerdict verdict)
    {
        Console.WriteLine(
            $"Trying one more time {solution.TaskId}, attempt: {_taskRetries[solution.TaskId]} verdict: {verdict.Verdict}");
        if (_taskRetries[solution.TaskId] >= _retryPolicy.MaximumNumberOfAttempts)
        {
            _taskStates[solution.TaskId] = TaskState.NotSolved;
            FinishIfReady();

            return;
        }

        Console.WriteLine($"Resending task {solution.TaskId}, {_account.Name}");
        _taskStates[solution.TaskId] = TaskState.Retrying;
        _taskRetries[solution.TaskId] += 1;
        string resolutionProp =
            $"You've already tried to solve this task, but something went wrong, here's the code you've sent\n:{solution.Code}";
        string verdictProp =
            $"The testing system has given the following result.{verdict.Verdict}. It may be useful for you. Again you need to send me only code!";
        //TODO: Add info from test.
        var taskInfo = _currentTasks.FirstOrDefault(t => t.TaskId == solution.TaskId);

        if (taskInfo != null)
            SendTaskToSolve(_currentTasks.First(t => t.TaskId == solution.TaskId),
                [resolutionProp, verdictProp]);
    }

    private async Task ProcessSendingAsync()
    {
        while (!Finished)
        {
            if (!_sendingQueue.IsEmpty)
                if (_sendingQueue.TryDequeue(out var solution))
                {
                    await _taskSender.SendToCheckAsync(solution, HttpClient,
                        $"{_account.Name}");
                    Console.WriteLine($"Solution to {solution.TaskId} was sent!");
                    //Must wait, cause otherwise dl won't allow to send the next task for checking
                    await Task.Delay(10000);
                }

            await Task.Delay(500);
        }

        if (Finished)
            foreach (var pairState in _taskStates)
                Console.WriteLine($"{pairState.Key}:{pairState.Value}");

        Console.WriteLine("Finished sending cycle!");
    }

    /// <summary>
    ///     Checks whether all tasks are solved or tried to be solved the maximum number of times
    /// </summary>
    private void FinishIfReady()
    {
        bool finish = true;
        foreach (var pairState in _taskStates)
            if (!(pairState.Value == TaskState.Solved || pairState.Value == TaskState.NotSolved))
            {
                finish = false;

                break;
            }

        Finished = finish;
    }

    private async Task ProcessCheckingAsync()
    {
        await Task.Delay(1000 * 60);
        Console.WriteLine("Starting checking solutions!");
        while (!Finished)
        {
            if (!_checkingQueue.IsEmpty)
                if (_checkingQueue.TryDequeue(out var solution))
                {
                    var verdict =
                        await _verdictAccessor.GetTaskVerdictAsync(HttpClient, solution.CourseId,
                            solution.TaskId);
                    if (verdict is null)
                    {
                        _checkingQueue.Enqueue(solution);

                        continue;
                    }

                    if (verdict.IsSolved)
                    {
                        _taskStates[verdict.TaskId] = TaskState.Solved;
                        FinishIfReady();
                        Console.WriteLine($"Bot {_account.Name} solved {solution.TaskId}");

                        continue;
                    }

                    ResendTaskToSolve(solution, verdict);
                }

            await Task.Delay(2000);
        }

        Console.WriteLine("Finished checking cycle!");
    }

    public string GetAccountName()
    {
        return _account.Name;
    }
}