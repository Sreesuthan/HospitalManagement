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
    public class PatientHistory
    {
        public int Id { get; set; }
        public int DocterId { get; set; }
        public string PatientUserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PhoneNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ConsultingTime { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string VisitedFor { get; set; } = string.Empty;
        public string Height { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;
        public string TemperatureF { get; set; } = string.Empty;
        public string BPLevel { get; set; } = string.Empty;
        public string SugarBeforeFood { get; set; } = string.Empty;
        public string SugarAfterFood { get; set; } = string.Empty;
        public bool IsMedicineAdded { get; set; }
        public bool IsPaymentCompleted { get; set; }
        public string Comments { get; set; } = string.Empty;
        [NotMapped]
        public string UserName { get; set; } = string.Empty;
        [NotMapped]
        public string Specialist { get; set; } = string.Empty;
    }
}
