using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TestsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("tests")]
        public async Task<ActionResult<List<Tests>>> GetAllTests()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Tests> tests = await connection.QueryAsync<Tests>("select * from Tests");
            return Ok(tests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Tests>>> GetMedicine(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var test = await connection.QueryFirstAsync<Tests>("select * from Tests Where Id=@Id", new { Id = id });
            return Ok(test);
        }

        [HttpPost("tests")]
        public async Task<ActionResult<List<Tests>>> AddTest(Tests tests)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into Tests values(@Name, @Price)", tests);
            return Ok();
        }

        [HttpPut("tests")]
        public async Task<ActionResult<List<Tests>>> UpdateTest(Tests tests)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Tests set Name=@Name, Price=@Price where Id = @Id", tests);
            return Ok();
        }

        [HttpGet("suggestedtests")]
        public async Task<ActionResult<List<SuggestedTests>>> GetAllSuggestedTests()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuggestedTests> prescripedmedicines = await connection.QueryAsync<SuggestedTests>("select * from SuggestedTests");
            return Ok(prescripedmedicines);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuggestedTests>>> AddTest(SuggestedTests suggestedTests)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into SuggestedTests values(@DocterId, @PatientUserName, @ConsultingTime, @Date, @Name, @price, @Quantity)", suggestedTests);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<SuggestedTests>>> DeleteSuggestedTest(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from SuggestedTests where Id=@Id", new { Id = id });
            return Ok(await SelectAllSuggestedTests(connection));
        }

        private static async Task<IEnumerable<SuggestedTests>> SelectAllSuggestedTests(SqlConnection connection)
        {
            return await connection.QueryAsync<SuggestedTests>("select * from SuggestedTests");
        }
    }
}
