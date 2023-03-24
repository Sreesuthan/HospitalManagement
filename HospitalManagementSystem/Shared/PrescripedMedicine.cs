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
    public class PrescripedMedicine
    {
        public int Id { get; set; }
        public int DocterId { get; set; }
        public string PatientUserName { get; set; } = string.Empty;
        public string ConsultingTime { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime MfgDate { get; set; } = DateTime.Now;
        public DateTime ExpDate { get; set; } = DateTime.Now;
        public int Quantity { get; set; }
        public string Prescription { get; set; } = string.Empty;
        public int MedicineId { get; set; }
    }
}
