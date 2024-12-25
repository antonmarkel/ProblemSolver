﻿using System.Text.Json;
using Microsoft.Extensions.Options;
using ProblemSolver.Configuration.Bot;
using ProblemSolver.Persistence.Repositories.Interfaces;
using ProblemSolver.Shared.DL.Models;

namespace ProblemSolver.Persistence.Repositories.Implementations
{
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
                File.Create(_config.FilePath);

            await File.WriteAllTextAsync(Path.Combine(_config.FilePath), "[]");
        }

        public async Task<List<SolverAccount>> GetAllAccountsAsync()
        {
            await EnsureFileExistsAsync();

            string json = await File.ReadAllTextAsync(_config.FilePath);

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
