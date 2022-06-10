using System.Diagnostics;
using RandoPi.Shared;
using Timer = System.Timers.Timer;

namespace RandoPi.Api;

public static class Program
{
    public static void Main(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapControllers();
        
        StartMainProcessWatchdog(app);

        app.Run();
    }

    /// <summary>
    /// Starts a watchdog. Will shut down the API server if the main proccess has exited
    /// </summary>
    /// <param name="app">Webapplication instance</param>
    private static void StartMainProcessWatchdog(IHost app) =>
        Task.Run(() =>
        {
            // Give the timer a grace period for the main process to start up
            Task.Delay(5000);

            var timer = new Timer(1000);
            timer.Elapsed += (_, _) =>
            {
                // Identify the main process by its name
                var processes = Process.GetProcessesByName(Constants.MainProcessName);
                if (processes.Length != 0) return;
        
                Console.WriteLine("Main proccess not running. Terminating...");
                app.StopAsync();
            };
            timer.AutoReset = true;
            timer.Enabled = true;
        });
}