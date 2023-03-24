using Azure.Core;
using Dapper;
using HospitalManagementSystem.Client.Pages;
using HospitalManagementSystem.Server.Data;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Text;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MailKit.Net.Smtp;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public UserController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register request)
        {
            if (_context.ApplicationUsers.Any(u => u.Username == request.Username))
            {
                return BadRequest("patient already exists.");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var token = CreateRandomToken();
            var member = new ApplicationUser
            {
                Username = request.Username,
                Email = request.Email,
                Role = "Patient",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                Age = request.Age,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Zip = request.Zip,
                PhoneNo = request.PhoneNo,
                ConfirmationToken = token
            };


            _context.ApplicationUsers.Add(member);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("docregister")]
        public async Task<IActionResult> DocRegister(DocRegister request)
        {
            if (_context.ApplicationUsers.Any(u => u.Username == request.Username))
            {
                return BadRequest("Doctor already exists.");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var token = CreateRandomToken();
            var member = new ApplicationUser
            {
                Username = request.Username,
                Email = request.Email,
                Role = "Doctor",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                FirstName = request.FirstName,
                ConsultingTime = request.ConsultingTime,
                LastName = request.LastName,
                Qualification = request.Qualification,
                Experience = request.Experience,
                Specialist = request.Specialist,
                Address = request.Address,
                PhoneNo = request.PhoneNo,
                ConfirmationToken = token
            };


            _context.ApplicationUsers.Add(member);
            await _context.SaveChangesAsync();
            await SendEmail(member.Email);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login request)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return BadRequest("Username is Incorrect!");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect!");
            }

            if (user.IsBlocked)
            {
                return BadRequest("You are blocked or inactive. You can't login! Please contact Admin!!");
            }

            //if (user.IsConfirmed == false)
            //{
            //    return BadRequest("Email not confirmed!");
            //}
            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private static string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        [HttpGet("states")]
        public async Task<ActionResult<List<States>>> GetAllStates()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<States> states = await connection.QueryAsync<States>("select * from States order by State");
            return Ok(states);
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<List<ApplicationUser>>> GetAllDoctors()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<ApplicationUser> doctors = await connection.QueryAsync<ApplicationUser>("select * from ApplicationUsers where Role='Doctor' order by FirstName");
            return Ok(doctors);
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationUser>>> GetAllUsers()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<ApplicationUser> users = await connection.QueryAsync<ApplicationUser>("select * from ApplicationUsers order by FirstName");
            return Ok(users);
        }

        [HttpGet("doctor/{id}")]
        public async Task<ActionResult<ApplicationUser>> GetDoctor(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var doctor = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Id=@id",
                new { id = id });
            return Ok(doctor);
        }

        [HttpGet("doctor/username/{doctor}")]
        public async Task<ActionResult<ApplicationUser>> GetDoctorByUsername(string doctor)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var doctorDetails = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Username=@Username",
                new { Username = doctor });
            return Ok(doctorDetails);
        }

        [HttpPut]
        public async Task<ActionResult<List<ApplicationUser>>> UpdateUser(ApplicationUser user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update ApplicationUsers set IsBlocked=@IsBlocked where Id = @Id", user);
            return Ok(await SelectAllUsers(connection));
        }

        [HttpPut("admin")]
        public async Task<ActionResult<List<ApplicationUser>>> UpdateAdminEmail(ApplicationUser user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update ApplicationUsers set Email=@Email, EmailPassword=@EmailPassword where Id = @Id", user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<ApplicationUser>>> DeleteUser(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from ApplicationUsers where Id=@Id", new { Id = id });
            return Ok(await SelectAllUsers(connection));
        }

        private static async Task<IEnumerable<ApplicationUser>> SelectAllUsers(SqlConnection connection)
        {
            return await connection.QueryAsync<ApplicationUser>("select * from ApplicationUsers order by FirstName");
        }

        private async Task SendEmail(string Email)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var admin = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Role = @Role", new { Role = "Admin" });

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(admin.Email));
            email.To.Add(MailboxAddress.Parse(Email));
            email.Subject = "Payment Status";
            email.Body = new TextPart(TextFormat.Plain) { Text = "Welcome to Quality Care Hospital. You have registered successfully." };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(admin.Email, admin.EmailPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
