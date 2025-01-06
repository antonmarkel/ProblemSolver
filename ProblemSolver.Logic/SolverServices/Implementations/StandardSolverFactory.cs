using Microsoft.Extensions.Options;
using ProblemSolver.Configuration.Solvers;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Implementations;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Shared.DL.Models;

namespace ProblemSolver.Logic.SolverServices.Implementations
{
    public class StandardSolverFactory : ISolverFactory<StandardSolver>
    {
        private readonly SolutionQueue _queue;
        private readonly IDlClientFactory _clientFactory;
        private readonly ITaskSender _taskSender;
        private readonly IMessageConstructor _messageConstructor;
        private readonly ISolutionVerdictLogAccessor _verdictLogAccessor;
        private readonly IOptions<RetryPolicy> _retryOptions;

        public StandardSolverFactory(SolutionQueue queue, IDlClientFactory clientFactory,
            ITaskSender taskSender, IMessageConstructor messageConstructor,
            ISolutionVerdictLogAccessor verdictLogAccessor, IOptions<RetryPolicy> retryOptions)
        {
            _queue = queue;
            _clientFactory = clientFactory;
            _taskSender = taskSender;
            _messageConstructor = messageConstructor;
            _verdictLogAccessor = verdictLogAccessor;
            _retryOptions = retryOptions;
        }

        public StandardSolver CreateSolver(SolverAccount account)
        {
            var httpClient = _clientFactory.CreateClient();

            return new StandardSolver(_queue, httpClient,
                account, _taskSender, _messageConstructor, _verdictLogAccessor, _retryOptions);
        }
    }
}
