﻿using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProblemSolver.Configuration.Bot;
using ProblemSolver.UI.Extensions;

namespace ProblemSolver.UI;

public partial class App : Application
    {
        public IHost AppHost { get; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("Configuration/appsettings.json", false, true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<BotConnectionConfig>(
                        context.Configuration.GetSection(nameof(BotConnectionConfig)));

                    services
                        .AddLogging()
                        .AddBotServices()
                        .AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            AppHost.Dispose();
            base.OnExit(e);
        }
    }


