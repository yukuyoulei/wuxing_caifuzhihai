using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WuXingGameBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnityWebSocketAdapter : ControllerBase
    {
        private readonly ILogger<UnityWebSocketAdapter> _logger;

        public UnityWebSocketAdapter(ILogger<UnityWebSocketAdapter> logger)
        {
            _logger = logger;
        }

        [HttpGet("ws")]
        public async Task<IActionResult> Connect()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
                return BadRequest();

            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var buffer = new byte[1024 * 4];
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var msg = JsonSerializer.Deserialize<dynamic>(json);
                    _logger.LogInformation($"Unity WS: {msg}");
                    // TODO: 转发到 GameHub 或 MongoDB
                }
            }
            return new EmptyResult();
        }

        [HttpPost("element/select")]
        public IActionResult SelectElement([FromBody] SelectElementRequest req)
        {
            // TODO: 调用 PlayerController 更新元素
            return Ok(new { status = "ok", element = req.Element });
        }

        public record SelectElementRequest(string PlayerId, string Element);
    }
}