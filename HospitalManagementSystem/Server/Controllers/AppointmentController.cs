using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AppointmentController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("appointments")]
        public async Task<ActionResult<List<Appointment>>> GetAllAppointments()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Appointment> appointments = await connection.QueryAsync<Appointment>("select * from Appointments");
            return Ok(appointments);
        }

        [HttpGet("patientsdetails")]
        public async Task<ActionResult<List<Appointment>>> GetAllAppointmentConfirmedPatientDetails()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Appointment> appointments = await connection.QueryAsync<Appointment>("select ap.Id, ap.DocterId, ap.PatientUserName, ap.ConsultingTime, ap.Date, ap.IsConfirmed, ap.VisitingFor, ap.IsNew, au.FirstName, au.Gender, au.Age, au.PhoneNo from Appointments as ap inner join ApplicationUsers as au on au.UserName = ap.PatientUserName Order by ap.ConsultingTime, ap.Date");
            return Ok(appointments);
        }

        [HttpGet("doctordetails")]
        public async Task<ActionResult<List<Appointment>>> GetAllAppointmentConfirmedDoctorDetails()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Appointment> appointments = await connection.QueryAsync<Appointment>("select ap.Id, ap.DocterId, ap.PatientUserName, ap.ConsultingTime, ap.Date, ap.IsNew, au.Specialist from Appointments as ap inner join ApplicationUsers as au on au.Id = ap.DocterId");
            return Ok(appointments);
        }

        [HttpGet("patient/{id}")]
        public async Task<ActionResult<Appointment>> GetPatient(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var patient = await connection.QueryFirstAsync<Appointment>("select ap.Id, ap.DocterId, ap.PatientUserName, ap.ConsultingTime, ap.Date, ap.VisitingFor, ap.IsNew, au.FirstName, au.LastName, au.Gender, au.Age, au.PhoneNo, au.Email from Appointments as ap inner join ApplicationUsers as au on au.UserName = ap.PatientUserName where ap.Id=@Id",
                new { Id = id });
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<List<Appointment>>> AddAppointment(Appointment appointment)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into Appointments values(@DocterId, @RejectReason, @ConsultingTime, @Date, @IsConfirmed, @VisitingFor, @PatientUserName, @IsNew)", appointment);
            return Ok(await SelectAllAppointments(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Appointment>>> UpdateAppointment(Appointment appointment)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Appointments set RejectReason=@RejectReason, IsConfirmed=@IsConfirmed, @IsNew=IsNew where Id = @Id", appointment);
            return Ok(await SelectAllAppointments(connection));
        }

        private static async Task<IEnumerable<Appointment>> SelectAllAppointments(SqlConnection connection)
        {
            return await connection.QueryAsync<Appointment>("select * from Appointments");
        }
    }
}
