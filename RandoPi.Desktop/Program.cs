using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Gtk;
using H.Formatters;
using H.Pipes;
using RandoPi.Shared;
using MessageType = RandoPi.Shared.MessageType;

namespace RandoPi.Desktop;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Task.Run(async () => await StartPipeServer());
        StartApiServer();
        
        Application.Init();
        
        var app = new Application("org.RandoPi.Desktop.RandoPi.Desktop", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);
        
        var win = new MainWindow();
        app.AddWindow(win);
        
        win.Show();
        Application.Run();
    }

    private static void StartApiServer()
    {
        if (Debugger.IsAttached) return;
        
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