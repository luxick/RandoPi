using H.Formatters;
using H.Pipes;
using Microsoft.AspNetCore.Mvc;
using RandoPi.Shared;

namespace RandoPi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;

    public DemoController(ILogger<DemoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task Get()
    {
        _logger.LogDebug("Testing Pipe Client");
        await using var client = new PipeClient<Message>(Constants.PipeName);
        client.MessageReceived += (o, args) => _logger.LogDebug("MessageReceived: " + args.Message);
        client.Disconnected += (o, args) => _logger.LogDebug("Disconnected from server");
        client.Connected += (o, args) => _logger.LogDebug("Connected to server");

        await client.ConnectAsync();

        await client.WriteAsync(new Message
        {
            Text = "Hello!",
        });
    }

    [HttpPost(nameof(Echo))]
    public async Task<string> Echo([FromBody] string message)
    {
        await using var client = new PipeClient<Message>(Constants.PipeName, formatter: new SystemTextJsonFormatter());

        var recieved = false;
        var result = new Message();
        
        client.MessageReceived += (o, args) =>
        {
            _logger.LogDebug("MessageReceived: " + args.Message);
            if (args.Message != null)
                result = args.Message;
            recieved = true;
        };
        
        client.Disconnected += (o, args) => _logger.LogDebug("Disconnected from server");
        client.Connected += (o, args) => _logger.LogDebug("Connected to server");

        await client.ConnectAsync();
        await client.WriteAsync(new Message
        {
            MessageType = MessageType.Echo,
            Text = message,
        });
        
        while (!recieved)
            await Task.Delay(50);

        return result.Text;
    }
    
}