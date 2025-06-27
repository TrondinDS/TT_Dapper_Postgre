using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test1.DB.DbConnectionFactory;

namespace Test1.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TestController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();
            var result = await connection.ExecuteScalarAsync<DateTime>("SELECT NOW()");
            return Ok(result);
        }
    }
}
