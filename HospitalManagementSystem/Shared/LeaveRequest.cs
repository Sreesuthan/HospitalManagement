using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Shared
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public string DocterUserName { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsConfirmed { get; set; }
        public string RejectReason { get; set; } = string.Empty;
        public string LeaveReason { get; set; } = string.Empty;
    }
}
