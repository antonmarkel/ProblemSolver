using System.Text.Json;
using Microsoft.Extensions.Options;
using ProblemSolver.Configuration.Bot;
using ProblemSolver.Persistence.Repositories.Interfaces;
using ProblemSolver.Shared.DL.Models;

namespace ProblemSolver.Persistence.Repositories.Implementations
{
    /// <summary>
    ///     Straightforward repository for json file
    /// </summary>
    //TODO: Use a database for storing accounts.
    public class JsonSolverRepository : ISolverRepository
    {
        private readonly BotStorageConfig _config;

        public JsonSolverRepository(IOptions<BotStorageConfig> config)
        {
            _config = config.Value;
        }

        private async Task EnsureFileExistsAsync()
        {
            if (!File.Exists(_config.FilePath))
            {
                File.Create(_config.FilePath);
                await File.WriteAllTextAsync(Path.Combine(_config.FilePath), "[]");
            }
        }

        private void EnsureFileExistsSync()
        {
            if (!File.Exists(_config.FilePath) || string.IsNullOrEmpty(File.ReadAllText(_config.FilePath)))
            {
                using (FileStream fs = File.Create(_config.FilePath))
                {
                   
                }

                File.WriteAllText(Path.Combine(_config.FilePath), "[]");
            }
        }

        public async Task<List<SolverAccount>> GetAllAccountsAsync()
        {
            await EnsureFileExistsAsync();

            string json = await File.ReadAllTextAsync(_config.FilePath);

            return JsonSerializer.Deserialize<List<SolverAccount>>(json) ?? new List<SolverAccount>();
        }

        public List<SolverAccount> GetAllAccountsSync()
        {
            EnsureFileExistsSync();

            string json = File.ReadAllText(_config.FilePath);

            return JsonSerializer.Deserialize<List<SolverAccount>>(json) ?? new List<SolverAccount>();
        }

        public async Task UpdateAccountAsync(SolverAccount account)
        {
            await RemoveAccountAsync(account.Id);
            await AddAccountAsync(account);
        }


        public async Task AddAccountAsync(SolverAccount account)
        {
            var accounts = await GetAllAccountsAsync();
            accounts.Add(account);
            await SaveAccountsAsync(accounts);
        }

        public async Task RemoveAccountAsync(string id)
        {
            var accounts = await GetAllAccountsAsync();
            var accountToRemove = accounts.Find(a => a.Id == id);

            if (accountToRemove != null)
            {
                accounts.Remove(accountToRemove);
                await SaveAccountsAsync(accounts);
            }
        }

        private async Task SaveAccountsAsync(List<SolverAccount> accounts)
        {
            string json =
                JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_config.FilePath, json);
        }
    }
}
