using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Gtk;
using H.Formatters;
using H.Pipes;
using RandoPi.Shared;
using MessageType = RandoPi.Shared.MessageType;

namespace RandoPi;

public static class Program
{
    public static PipeServer<Message> PipeServer;
    
    [STAThread]
    public static void Main(string[] args)
    {
        Task.Run(async () => await StartPipeServer());
        
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = "RandoPi.Api.exe"
        };
        
#if !DEBUG
        StaticData.ApiServerWorker = Process.Start(startInfo);
#endif

        Application.Init();
        
        var app = new Application("org.RandoPi.RandoPi", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);
        
        var win = new MainWindow();
        app.AddWindow(win);
        
        win.Show();
        Application.Run();
    }

    private static async Task StartPipeServer()
    {
        PipeServer = new PipeServer<Message>(Constants.PipeName, formatter: new SystemTextJsonFormatter());
        PipeServer.ClientConnected += (o, args) =>
        {
            Console.WriteLine($"Client {args.Connection.PipeName} is now connected!");
        };
        PipeServer.ClientDisconnected += (o, args) =>
        {
            Console.WriteLine($"Client {args.Connection.PipeName} disconnected");
        };
        PipeServer.MessageReceived += async (sender, args) =>
        {
            Console.WriteLine($"Client {args.Connection.PipeName} says: {args.Message}");

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

        await PipeServer.StartAsync();
    }
}