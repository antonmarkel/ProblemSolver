using ProblemSolver.Shared.DL.Models;

namespace ProblemSolver.Persistence.Repositories.Interfaces;

public interface ISolverRepository
{
    Task<List<SolverAccount>> GetAllAccountsAsync();
    Task AddAccountAsync(SolverAccount account);
    Task UpdateAccountAsync(SolverAccount account);
    Task RemoveAccountAsync(string id);
}