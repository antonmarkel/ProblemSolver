using Microsoft.Extensions.DependencyInjection;
using ProblemSolver.Logic.BotServices.Implementations;
using ProblemSolver.Logic.BotServices.Implementations.CodeExtractors;
using ProblemSolver.Logic.BotServices.Implementations.TaskRequestConverters;
using ProblemSolver.Logic.BotServices.Interfaces;

namespace ProblemSolver.UI.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddBotServices(this IServiceCollection services)
        {
            return services.AddScoped<ICodeExtractor, StandardExtractor>()
                .AddScoped<ITaskRequestConverter, StandardConverter>()
                .AddScoped<IBotService, BotService>();
        }
    }
}
