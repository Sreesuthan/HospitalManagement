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
    public class SuccessOrders
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int DocterId { get; set; }
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }
        public int PrescribedMedicinesId { get; set; }
        public int SuggestedTestsId { get; set; }
        public decimal ConsultationFee { get; set; }
        public int GstId { get; set; }
        public decimal Gst { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        [NotMapped]
        public string Username { get; set; } = string.Empty; 
        [NotMapped]
        public string ConsultingTime { get; set; } = string.Empty;
        [NotMapped]
        public DateTime Date { get; set; }
    }
}
