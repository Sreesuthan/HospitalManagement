using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<PatientHistory> PatientHistories { get; set; }
        public DbSet<PrescripedMedicine> PrescripedMedicines { get; set; }
        public DbSet<Medicines> Medicines { get; set; }
        public DbSet<SuggestedTests> SuggestedTests { get; set; }
        public DbSet<GST> Gst { get; set; }
        public DbSet<Tests> Tests { get; set; }
        public DbSet<SuccessOrders> SuccessOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medicines>().HasData(
                new Medicines { Id = 1, Name = "Med1", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 4.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 2, Name = "Med2", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 5.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 3, Name = "Med3", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 6.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 4, Name = "Med4", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 4.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 5, Name = "Med5", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 5.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 6, Name = "Med6", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 4.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 7, Name = "Med7", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 6.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 8, Name = "Med8", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 4.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 9, Name = "Med9", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 5.99m, AvailableMedicinesCount = 1000 },
                new Medicines { Id = 10, Name = "Med10", MfgDate = new DateTime(2023, 1, 1), ExpDate = new DateTime(2024, 12, 31), Price = 6.99m, AvailableMedicinesCount = 1000 }
                );

            modelBuilder.Entity<Tests>().HasData(
                new Tests { Id = 1, Name = "Blood Test", Price = 500.00m },
                new Tests { Id = 2, Name = "Urine Test", Price = 400.00m },
                new Tests { Id = 3, Name = "X-ray", Price = 300.00m },
                new Tests { Id = 4, Name = "Stool Test", Price = 450.00m },
                new Tests { Id = 5, Name = "Sputum Test", Price = 350.00m }
                );
        }
    }
}
