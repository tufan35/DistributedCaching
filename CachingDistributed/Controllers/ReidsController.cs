using CachingDistributed.Services;
using Microsoft.AspNetCore.Mvc;

namespace CachingDistributed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReidsController : ControllerBase
    {
        [HttpGet("[action]/{key}/{value}")]
        public async Task<IActionResult> SetValue(string key, string value)
        {
            var redis = await RedisService.RedisMasterDb();
            await redis.StringSetAsync(key, value);
            return Ok();
        }
        [HttpGet("[action]/{key}")]
        public async Task<IActionResult> GetValue(string key)
        {
            var redis = await RedisService.RedisMasterDb();
            var data = await redis.StringGetAsync(key);
            return Ok(data.ToString());
        }
    }
}
