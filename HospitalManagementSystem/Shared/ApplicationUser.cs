using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Shared
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string ConfirmationToken { get; set; } = string.Empty;
        public DateTime? ConfirmedAt { get; set; }
        public bool IsConfirmed { get; set; }
        public string PasswordResetToken { get; set; } = string.Empty;
        public DateTime? ResetTokenExpires { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Specialist { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string ConsultingTime { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public string EmailPassword { get; set; } = string.Empty;
    }
}
