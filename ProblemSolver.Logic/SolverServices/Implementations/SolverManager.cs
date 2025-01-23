using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Results;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Persistence.Repositories.Interfaces;
using ProblemSolver.Shared.DL.Models;
using ProblemSolver.Shared.Solvers;

namespace ProblemSolver.Logic.SolverServices.Implementations;

public class SolverManager : ISolverManager
{
    private readonly IRegisterService _registerService;
    private readonly ISolverRepository _solverRepository;

    public SolverManager(IRegisterService registerService, ISolverRepository solverRepository)
    {
        _registerService = registerService;
        _solverRepository = solverRepository;
    }

    public async Task<OneOf<Success, Failed>> AddSolverAccountAsync(SolverSettings settings, HttpClient client)
    {
        var registerResult = await _registerService.RegisterAsync(settings, client);

        if (registerResult.IsT1)
            return registerResult.AsT1;

        var registerInfo = registerResult.AsT0;
        var solverAccount = new SolverAccount
        {
            Id = registerInfo.Id,
            Name = registerInfo.Name,
            Nick = registerInfo.Nick,
            Password = registerInfo.Password, //6bcd6e6b-bd00-4d,23c40d53-e50e-4
            Bot = settings.AiBot,
            Language = settings.Language,
            Compiler = settings.Compiler
        };

        await _solverRepository.AddAccountAsync(solverAccount);

        return new Success();
    }

    public async Task<OneOf<Success,Failed>> RemoveSolverAccountAsync(SolverAccount account)
    {
        await _solverRepository.RemoveAccountAsync(account.Id);
        return new Success();
    }

    public async Task<OneOf<Success,Failed>> UpdateSolverAccountAsync(SolverAccount account)
    {
        await _solverRepository.UpdateAccountAsync(account);
        return new Success();
    }

    public async Task<List<SolverAccount>> GetAllSolversAsync()
    {
        return await _solverRepository.GetAllAccountsAsync();
    }

    public List<SolverAccount> GetAllSolversSync()
    {
        return _solverRepository.GetAllAccountsSync();
    }
}