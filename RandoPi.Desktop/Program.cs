using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Gtk;
using H.Formatters;
using H.Pipes;
using Microsoft.Extensions.Configuration;
using RandoPi.Shared;
using MessageType = RandoPi.Shared.MessageType;

namespace RandoPi.Desktop;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        LoadAppSettings();

        if (StaticData.AppSettings.StartApiServer)
            StartApiServer();
        
        Application.Init();
        
        var app = new Application("org.RandoPi.Desktop", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);
        
        var win = new MainWindow();
        app.AddWindow(win);
        
        win.Show();
        Application.Run();
    }

    private static void LoadAppSettings()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.local.json", true)
            .Build();
        StaticData.AppSettings = config.Get<AppSettings>();
    }

    private static void StartApiServer()
    {
        // Pipe server for communication from API server
        Task.Run(async () => await StartPipeServer());
        
        // Execute the server as a separate process
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "RandoPi.Api.exe"
        };
        StaticData.ApiServerWorker = Process.Start(startInfo);
    }

    private static async Task StartPipeServer()
    {
        var server = new PipeServer<Message>(Constants.PipeName, formatter: new SystemTextJsonFormatter());
        server.MessageReceived += async (sender, args) =>
        {
            Console.WriteLine($"Message from API: {args.Message?.MessageType}");

            var msg = args.Message;
            if (msg == null) return;

            switch (msg.MessageType)
            {
                case MessageType.None:
                    msg.Text = "invaid message";
                    await args.Connection.WriteAsync(msg);
                    break;
                case MessageType.Echo:
                    msg.Text += " recieved " + DateTime.Now;
                    await args.Connection.WriteAsync(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        };
        Console.WriteLine("Main process pipe server running");

        await server.StartAsync();
    }
}