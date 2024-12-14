using Newtonsoft.Json;
using System.IO;
using ProblemSolver.Configuration.Bot;
using System.Diagnostics.CodeAnalysis;

public class Config
{
    public BotConnectionConfig? BotConnectionConfig { get; set; }
    public BotLoginConfig? BotLoginConfig { get; set; }

    public Config Get()
    {
        string jsonPath = "..\\..\\..\\Configuration\\appsettings.json";
        string jsonData = File.ReadAllText(jsonPath);
        Config? config = JsonConvert.DeserializeObject<Config>(jsonData);

        return config;
    }
}