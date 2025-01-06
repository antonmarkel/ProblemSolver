using System.Text;
using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.DL.Models;

namespace ProblemSolver.Logic.DlServices.Implementations;

public class LoginService : ILoginService
{
    public async Task<OneOf<Success, WrongCredentials>> LoginAsync(SolverAccount account, HttpClient client)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Dictionary<string, string> formData = new()
        {
            { "id", account.Id },
            { "password", account.Password },
            { "lng", "ru" },
            { "logon", "submit" }
        };

        var content = new FormUrlEncodedContent(formData);
        var loginResponse = await client.PostAsync("logon.asp", content);

        string response = await loginResponse.Content.ReadAsStringAsync();

        if (response.Contains("Distance Learning Belarus | Login Failed"))
            return new WrongCredentials();

        return new Success();
    }
}