using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SakuraGenin;
using SakuraJounin.Models;
using StackExchange.Redis;

namespace SakuraJounin.Controllers
{
    [ApiController]
    [Route("/api/sakura")]
    public class SakuraRescueController : ControllerBase
    {
        private readonly ILogger<SakuraRescueController> _logger;
        private readonly IOptions<GameSettings> gameSettings;

        private string SortedSetKey => gameSettings.Value.RedisKey;
        private GameSettings GameSettings => gameSettings.Value;

        public SakuraRescueController(ILogger<SakuraRescueController> logger, IOptions<GameSettings> gameSettings)
        {
            _logger = logger;
            this.gameSettings = gameSettings;
        }

        [HttpGet("userCount")]
        public async Task<IActionResult> GetUserCount([FromQuery]string username, [FromServices]IConnectionMultiplexer redis)
        {
            username = NormalizeName(username);
            var db = redis.GetDatabase();
            var result = await db.SortedSetScoreAsync(SortedSetKey, username);
            return Ok(new ScoreResponse(result ?? 0));
        }

        [HttpPost("rescueSakura")]
        public async Task<IActionResult> SaveSakura([FromQuery] string username, [FromServices] IConnectionMultiplexer redis, [FromServices] Genin.GeninClient geninClient)
        {
            username = NormalizeName(username);

            var jounin = Environment.MachineName;

            var genins = new HashSet<string>();

            //try to get 3 unique genins
            //uniquness provided by the hashset
            for (int i = 0; i < GameSettings.MaxAttempts && genins.Count < GameSettings.RequiredGenins; i++)
            {
                var genin = await geninClient.RequestGenninAsync(new Request
                {
                    //announce yourself to genin service for better logging
                    Name = $"{username} - {jounin}"
                });

                genins.Add(genin.Name);

                //simulate some delay bettween acquiring genins
                await Task.Delay(GameSettings.GeninBackoff);
            }

            var db = redis.GetDatabase();
            if (genins.Count >= GameSettings.RequiredGenins)
            {
                //sakura saved
                var result = await db.SortedSetIncrementAsync(SortedSetKey, username, 1);
                var missionReport = new MissionReport(jounin, genins.ToArray(), true, result);
               
                _logger.LogInformation("Sakura is saved - " + missionReport);
                return Ok(missionReport);
            }
            else
            {
                //sakura not saved :(
                var result = await db.SortedSetScoreAsync(SortedSetKey, username);
                var missionReport = new MissionReport(jounin, genins.ToArray(), false, result ?? 0);

                _logger.LogInformation("Sakura is not saved:( - " + missionReport);
                return Ok(missionReport);
            }
        }

        [HttpGet("highScores")]
        public async Task<IActionResult> GetHighScores([FromServices] IConnectionMultiplexer redis)
        {
            var db = redis.GetDatabase();
            var scores = await db.SortedSetRangeByScoreWithScoresAsync(SortedSetKey, order: Order.Descending, take: 5);
            var mapped = new HighScores(scores.Select(x => new HighScores.HighScore(x.Element.ToString(), x.Score)).ToArray());
            return Ok(mapped);
        }

        private string NormalizeName(string username) => username.Trim().ToLower();
    }
}