using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly IConfiguration _config;

        public LeaveRequestController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("leaveRequests")]
        public async Task<ActionResult<List<LeaveRequest>>> GetAllRequests()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<LeaveRequest> leaveRequests = await connection.QueryAsync<LeaveRequest>("select * from LeaveRequests");
            return Ok(leaveRequests);
        }

        [HttpPost]
        public async Task<ActionResult<List<LeaveRequest>>> AddAppointment(LeaveRequest request)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into LeaveRequests values(@DocterUserName, @Date, @IsConfirmed, @RejectReason, @LeaveReason)", request);
            return Ok(await SelectAllRequests(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<LeaveRequest>>> UpdateAppointment(LeaveRequest request)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update LeaveRequests set DocterUserName=@DocterUserName, RejectReason=@RejectReason, IsConfirmed=@IsConfirmed where DocterUserName = @DocterUserName", request);
            return Ok(await SelectAllRequests(connection));
        }


        private static async Task<IEnumerable<LeaveRequest>> SelectAllRequests(SqlConnection connection)
        {
            return await connection.QueryAsync<LeaveRequest>("select * from LeaveRequests");
        }
    }
}
