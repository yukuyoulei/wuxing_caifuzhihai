using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WuXingGameBackend.Models;

namespace WuXingGameBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IMongoCollection<Player> _players;

        public PlayerController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("WuXingGame");
            _players = database.GetCollection<Player>("Players");
        }

        [HttpGet("{id}")]
        public async Task<Player?> Get(string id)
        {
            return await _players.Find(p => p.PlayerId == id).FirstOrDefaultAsync();
        }

        [HttpPost("init")]
        public async Task<IActionResult> InitPlayer([FromBody] string playerId)
        {
            var player = new Player
            {
                PlayerId = playerId,
                Yin = 10,
                Yang = 10,
                Elements = new Dictionary<string, int>
                {
                    {"金",10},{"木",10},{"水",10},{"火",10},{"土",10}
                },
                PosX = 0,
                PosY = 0
            };
            await _players.InsertOneAsync(player);
            return Ok(player);
        }

        [HttpPut("element")]
        public async Task<IActionResult> UpdateElement([FromBody] UpdateElementRequest req)
        {
            var update = Builders<Player>.Update.Inc(p => p.Elements[req.Element], -1);
            await _players.UpdateOneAsync(p => p.PlayerId == req.PlayerId, update);
            return Ok();
        }

        public class UpdateElementRequest
        {
            public string PlayerId { get; set; }
            public string Element { get; set; }
        }
    }
}