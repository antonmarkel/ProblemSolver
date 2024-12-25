using System.Text;
using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Results;
using ProblemSolver.UI.DL.Auth;

namespace ProblemSolver.Logic.DlServices.Implementations;

public class LoginService : ILoginService
{
    public async Task<OneOf<Success, WrongCredentials>> LoginAsync(LoginInfo request, HttpClient client)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Dictionary<string, string> formData = new()
        {
            { "id", request.Id },
            { "password", request.Password },
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