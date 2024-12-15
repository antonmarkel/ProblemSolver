using Newtonsoft.Json;
using System.IO;
using ProblemSolver.Configuration.Bot;
using System.Net.Http;
using ProblemSolver.UI.DL.Auth;

namespace ProblemSolver.UI.Configuration;
public class BotConfig
{
    public BotConnectionConfig? BotConnectionConfig { get; set; }
    public BotLoginConfig? BotLoginConfig { get; set; }

    private BotConfig Get()
    {
        string jsonPath = "..\\..\\..\\Configuration\\appsettings.json";
        string jsonData = File.ReadAllText(jsonPath);
        BotConfig? config = JsonConvert.DeserializeObject<BotConfig>(jsonData);

        return config;
    }

    public HttpClient GetHttpClient()
    {
        var client = new HttpClient()
        {
            BaseAddress = new Uri(Get().BotLoginConfig.BaseAddress)
        };

        return client;
    }

    public LoginRequest GetLoginRequest()
    {
        var loginRequest = new LoginRequest
        {
            Id = Get().BotLoginConfig.LoginId,
            Password = Get().BotLoginConfig.Password
        };

        return loginRequest;
    }
}