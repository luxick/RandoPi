using H.Formatters;
using H.Pipes;
using Microsoft.AspNetCore.Mvc;
using RandoPi.Shared;

namespace RandoPi.Api.Controllers;

/// <summary>
/// Base class for shared functionality of all controllers
/// </summary>
public class BaseController : ControllerBase
{
    private readonly ILogger<BaseController> _logger;

    public BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Pass a message to the main process and return the answer
    /// </summary>
    /// <param name="message">Message from the client</param>
    /// <returns>Message answer from the main process</returns>
    protected async Task<Message> PassMessage(Message message)
    {
        await using var client = new PipeClient<Message>(Constants.PipeName, formatter: new SystemTextJsonFormatter());

        var recieved = false;
        var result = new Message();
        
        client.Disconnected += (o, args) => _logger.LogInformation("Disconnected from main process");
        client.Connected += (o, args) => _logger.LogInformation("Connected to main process");
        client.MessageReceived += (o, args) =>
        {
            _logger.LogInformation("MessageReceived: {}", args.Message?.MessageType);
            if (args.Message != null)
                result = args.Message;
            recieved = true;
        };
        
        await client.ConnectAsync();
        await client.WriteAsync(message);
        
        while (!recieved)
            await Task.Delay(50);

        return result;
    }
}