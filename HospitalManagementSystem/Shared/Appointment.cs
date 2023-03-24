using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Shared
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DocterId { get; set; }
        public string PatientUserName { get; set; } = string.Empty;
        public string ConsultingTime { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today.AddDays(-1);
        public bool IsConfirmed { get; set; }
        public string RejectReason { get; set; } = string.Empty;
        public string VisitingFor { get; set; } = string.Empty;
        public bool IsNew { get; set; }
        [NotMapped]
        public string FirstName { get; set; } = string.Empty;
        [NotMapped]
        public string LastName { get; set; } = string.Empty;
        [NotMapped]
        public string Gender { get; set; } = string.Empty;
        [NotMapped]
        public int Age { get; set; }
        [NotMapped]
        public string PhoneNo { get; set; } = string.Empty;
        [NotMapped]
        public string Email { get; set; } = string.Empty;
        [NotMapped]
        public string Specialist { get; set; } = string.Empty;
    }
}
