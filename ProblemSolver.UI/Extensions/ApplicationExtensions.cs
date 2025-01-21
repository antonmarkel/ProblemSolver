using Microsoft.Extensions.DependencyInjection;
using ProblemSolver.Logic.BotServices.Implementations;
using ProblemSolver.Logic.BotServices.Implementations.CodeExtractors;
using ProblemSolver.Logic.BotServices.Implementations.TaskRequestConverters;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Logic.BotServices.Queues;
using ProblemSolver.Logic.DlServices.Implementations;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.SolverServices.Implementations;
using ProblemSolver.Logic.SolverServices.Interfaces;
using ProblemSolver.Persistence.Repositories.Implementations;
using ProblemSolver.Persistence.Repositories.Interfaces;
using ProblemSolver.UI.Messages;

namespace ProblemSolver.UI.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddBotServices(this IServiceCollection services)
    {
        return services.AddScoped<ICodeExtractor, StandardExtractor>()
            .AddScoped<IMessageConstructor, StandardMessageConstructor>()
            .AddScoped<IAiService, AiService>()
            .AddSingleton<SolutionQueue>();
    }

    public static IServiceCollection AddDlServices(this IServiceCollection services)
    {
        return services.AddScoped<ILoginService, LoginService>()
            .AddScoped<IRegisterService, RegisterService>()
            .AddScoped<ITaskLinkExtractor, TaskLinkExtractor>()
            .AddScoped<ITaskExtractor, TaskExtractor>()
            .AddTransient<ITaskSender, TaskSender>()
            .AddScoped<ICourseSubscriptionService, CourseSubscriptionService>()
            .AddScoped<IDlClientFactory, DlClientFactory>()
            .AddScoped<ISolutionVerdictLogAccessor, SolutionVerdictLogAccessor>();
    }

    public static IServiceCollection AddSolvers(this IServiceCollection services)
    {
        return services.AddSingleton<ISolver, StandardSolver>()
            .AddScoped<ISolverFactory<StandardSolver>, StandardSolverFactory>()
            .AddScoped<ISolverManager, SolverManager>();
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        return services.AddScoped<ISolverRepository, JsonSolverRepository>();
    }

    public static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        return services.AddTransient<MessageHelper>();
    }
}