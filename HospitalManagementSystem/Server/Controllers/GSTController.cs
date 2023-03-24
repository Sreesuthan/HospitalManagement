using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using HospitalManagementSystem.Shared;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GSTController : ControllerBase
    {
        private readonly IConfiguration _config;

        public GSTController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<GST>>> GetAllGSTValues()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<GST> gst = await connection.QueryAsync<GST>("select * from Gst order by Id desc");
            return Ok(gst);
        }

        [HttpPost]
        public async Task<ActionResult<List<GST>>> AddMedicine(GST gst)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into Gst values(@Value, @ChangedFrom)", gst);
            return Ok();
        }
    }
}
