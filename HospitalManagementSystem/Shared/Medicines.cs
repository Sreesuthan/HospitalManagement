using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Shared
{
    public class Medicines
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime MfgDate { get; set; } = DateTime.Now;
        public DateTime ExpDate { get; set; } = DateTime.Now;
        public int AvailableMedicinesCount { get; set; }
    }
}
