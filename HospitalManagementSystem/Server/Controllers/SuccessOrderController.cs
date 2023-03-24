using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuccessOrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SuccessOrderController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("successorders")]
        public async Task<ActionResult<List<SuccessOrders>>> GetAllSuccessOrders()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuccessOrders> successOrders = await connection.QueryAsync<SuccessOrders>("Select au1.Username, ap.ConsultingTime, ap.Date, so.OrderDate, so.TotalPrice,so.PrescribedMedicinesId, so.SuggestedTestsId from SuccessOrders as so inner join ApplicationUsers as au1 on so.PatientId = au1.Id inner join Appointments as ap on so.AppointmentId = ap.Id Order by so.OrderDate");
            return Ok(successOrders);                                                                                                                                 
        }

        [HttpGet("invoice")]
        public async Task<ActionResult<List<SuccessOrders>>> GetAllOrders()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuccessOrders> successOrders = await connection.QueryAsync<SuccessOrders>("Select * from SuccessOrders order by InvoiceId desc");
            return Ok(successOrders);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuccessOrders>>> AddSuccessOrder(List<SuccessOrders> orders)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            foreach (var order in orders)
            {
                await connection.ExecuteAsync("Insert into SuccessOrders values(@DocterId, @PatientId, @AppointmentId, @PrescribedMedicinesId, @SuggestedTestsId, @ConsultationFee, @GstId, @TotalPrice, @OrderDate, @InvoiceId, @Gst)", order);
            }
            return Ok(await SelectAllSuccessOrders(connection));
        }

        private static async Task<IEnumerable<SuccessOrders>> SelectAllSuccessOrders(SqlConnection connection)
        {
            return await connection.QueryAsync<SuccessOrders>("select * from SuccessOrders order by InvoiceId desc");
        }
    }
}
