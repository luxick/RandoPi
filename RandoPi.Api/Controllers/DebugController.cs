using luxick.Result;
using Microsoft.AspNetCore.Mvc;
using RandoPi.Shared;

namespace RandoPi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : BaseController
{
    private readonly ILogger<DebugController> _logger;

    public DebugController(ILogger<DebugController> logger) : base(logger)
    {
        _logger = logger;
    }

    [HttpPost(nameof(Echo))]
    public async Task<Result<string>> Echo([FromBody] string message)
    {
        try
        {
            var msg = new Message
            {
                MessageType = MessageType.Echo,
                Text = message
            };

            var result = await PassMessage(msg);
            return new Ok<string>(result.Text);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured");
            return new Error<string>(e);
        }
    }
    
}