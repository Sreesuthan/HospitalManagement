using Dapper;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MimeKit.Text;
using MimeKit;
using HospitalManagementSystem.Shared;
using MailKit.Net.Smtp;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientHistoryController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PatientHistoryController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("patienthistories")]
        public async Task<ActionResult<List<PatientHistory>>> GetPatientHistories()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<PatientHistory> Patienthistories = await connection.QueryAsync<PatientHistory>("select ph.Id, ph.DocterId, ph.PatientUserName, ph.FirstName, ph.LastName, ph.Gender, ph.Age, ph.PhoneNo, ph.Email, ph.ConsultingTime, ph.Date, ph.VisitedFor, ph.Height, ph.Weight, ph.TemperatureF, ph.BPLevel, ph.SugarBeforeFood, ph.SugarAfterFood, ph.IsMedicineAdded, ph.IsPaymentCompleted, ph.Comments, au.FirstName, au.LastName from PatientHistories as ph inner join ApplicationUsers as au on au.Id = ph.DocterId");
            return Ok(Patienthistories);
        }

        [HttpGet]
        public async Task<ActionResult<List<PatientHistory>>> GetHistories()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<PatientHistory> Patienthistories = await connection.QueryAsync<PatientHistory>("select * from PatientHistories Order by ConsultingTime desc, Date desc");
            return Ok(Patienthistories);
        }

        [HttpGet("prescripedmedicines")]
        public async Task<ActionResult<List<PrescripedMedicine>>> GetAllPrescripedMedicines()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<PrescripedMedicine> prescripedmedicines = await connection.QueryAsync<PrescripedMedicine>("select * from PrescripedMedicines");
            return Ok(prescripedmedicines);
        }

        [HttpGet("patienthistory/{id}")]
        public async Task<ActionResult<PatientHistory>> GetPatientHistory(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var patienthistory = await connection.QueryFirstAsync<PatientHistory>("select au.UserName, au.Specialist, ph.Id, ph.DocterId, ph.PatientUserName, ph.FirstName, ph.LastName, ph.Gender, ph.Age, ph.PhoneNo, ph.Email, ph.ConsultingTime, ph.Date, ph.VisitedFor, ph.Height, ph.Weight, ph.TemperatureF, ph.BPLevel, ph.SugarBeforeFood, ph.SugarAfterFood, ph.Comments, ph.IsPaymentCompleted from PatientHistories as ph inner join ApplicationUsers as au on au.Id = ph.DocterId where ph.Id=@Id",
                new { Id = id });
            return Ok(patienthistory);
        }

        [HttpPost]
        public async Task<ActionResult<List<PatientHistory>>> AddPatientHistory(PatientHistory patientHistory)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into PatientHistories values(@DocterId, @PatientUserName, @FirstName, @LastName, @Gender, @Age, @PhoneNo, @Email, @ConsultingTime, @Date, @VisitedFor, @Height, @Weight, @TemperatureF, @BPLevel, @SugarBeforeFood, @SugarAfterFood, @IsMedicineAdded, @IsPaymentCompleted, @Comments)", patientHistory);
            var patient = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Username=@Username", new { Username = patientHistory.PatientUserName });
            await SendEmail1(patient.Email);
            return Ok();
        }

        [HttpPost("prescripedmedicine")]
        public async Task<ActionResult<List<PrescripedMedicine>>> AddMedicine(PrescripedMedicine prescripedMedicine)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into PrescripedMedicines values(@DocterId, @PatientUserName, @ConsultingTime, @Date, @Name, @price, @MfgDate, @ExpDate, @Prescription, @Quantity, @MedicineId)", prescripedMedicine);
            return Ok(await SelectAllPrescripedMedicines(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<PatientHistory>>> UpdatePatientHistory(PatientHistory patientHistory)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update PatientHistories set IsPaymentCompleted=@IsPaymentCompleted where PatientUserName = @PatientUserName and ConsultingTime = @ConsultingTime and ConsultingTime=@ConsultingTime", patientHistory);

            var patient = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Username=@Username", new { Username = patientHistory.PatientUserName });
            await SendEmail(patient.Email);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<PrescripedMedicine>>> DeletePrescripedMedicine(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from PrescripedMedicines where Id=@Id", new { Id = id });
            return Ok(await SelectAllPrescripedMedicines(connection));
        }

        private static async Task<IEnumerable<PrescripedMedicine>> SelectAllPrescripedMedicines(SqlConnection connection)
        {
            return await connection.QueryAsync<PrescripedMedicine>("select * from PrescripedMedicines");
        }

        private async Task SendEmail(string Email)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var admin = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Role = @Role", new { Role = "Admin"});

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(admin.Email));
            email.To.Add(MailboxAddress.Parse(Email));
            email.Subject = "Payment Status";
            email.Body = new TextPart(TextFormat.Plain) { Text = "Your payment was completed successfully." };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(admin.Email, admin.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        private async Task SendEmail1(string Email)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var admin = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Role = @Role", new { Role = "Admin" });

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(admin.Email));
            email.To.Add(MailboxAddress.Parse(Email));
            email.Subject = "Payment Status";
            email.Body = new TextPart(TextFormat.Plain) { Text = "You have done doctor consultation. You may proceed to pay bill for buy medicines." };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(admin.Email, admin.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
