using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private decimal Stotal { get; set; }
        public BillController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> GenaratePdf(List<SuccessOrders> successOrders)
        {
            string fileName = "";
            HtmlToPdfConverter converter = new HtmlToPdfConverter(); 
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();

            converter.ConverterSettings = blinkConverterSettings;
            string HtmlString = await CreateBody(successOrders);
            string BaseUrl = "";

            PdfDocument document = converter.Convert(HtmlString, BaseUrl);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
                document.Close(true);

                ms.Position = 0;
            }
            var successOrder = successOrders.FirstOrDefault();
            if (successOrder != null)
            {
                fileName = $"INV00{successOrder.InvoiceId}_Receipt.pdf";
            }

            return File(response, "application/pdf", fileName);
        }

        private async Task<string> CreateBody(List<SuccessOrders> successOrders)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var successOrder = successOrders.FirstOrDefault(s => s.PrescribedMedicinesId.Equals(0) && s.SuggestedTestsId.Equals(0));
            var doctor = new ApplicationUser();
            var patient = new ApplicationUser();
            var appointment = new Appointment();
            var gst = new GST();
            var suggestedTests = new List<SuggestedTests>();
            var prescripedMedicines = new List<PrescripedMedicine>();

            if (successOrder != null)
            {
                doctor = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Id=@id",
                    new { id = successOrder.DocterId });

                patient = await connection.QueryFirstAsync<ApplicationUser>("select * from ApplicationUsers where Id=@id",
                    new { id = successOrder.PatientId });

                appointment = await connection.QueryFirstAsync<Appointment>("select * from Appointments where Id=@id",
                    new { id = successOrder.AppointmentId });

                gst = await connection.QueryFirstAsync<GST>("select * from Gst where Id=@id",
                    new { id = successOrder.GstId });
            }

            foreach (var order in successOrders)
            {
                if (order.SuggestedTestsId != 0)
                {
                    var test = await connection.QueryFirstAsync<SuggestedTests>("select * from SuggestedTests where Id=@id", new { id = order.SuggestedTestsId });
                    suggestedTests.Add(test);
                }
                if (order.PrescribedMedicinesId != 0)
                {
                    var medicine = await connection.QueryFirstAsync<PrescripedMedicine>("select * from PrescripedMedicines where Id=@id", new { id = order.PrescribedMedicinesId });
                    prescripedMedicines.Add(medicine);
                }
            }

            var tableBody = "";
            var Body1 = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "BillTemplate", "BillPage1.html"));
            foreach (var medicine in prescripedMedicines)
            {
                var Body2 = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "BillTemplate", "BillPage2.html"));
                Body2 = Body2.Replace("{name}", medicine.Name);
                Body2 = Body2.Replace("{price}", medicine.Price.ToString());
                Body2 = Body2.Replace("{quantity}", medicine.Quantity.ToString());
                var total = medicine.Price * medicine.Quantity;
                total = decimal.Round(total, 2, MidpointRounding.AwayFromZero);
                Body2 = Body2.Replace("{total}", total.ToString());
                Stotal = Stotal + total;
                tableBody = tableBody + Body2;
            }

            foreach (var test in suggestedTests)
            {
                var Body2 = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "BillTemplate", "BillPage2.html"));
                Body2 = Body2.Replace("{name}", test.Name);
                Body2 = Body2.Replace("{price}", test.Price.ToString());
                Body2 = Body2.Replace("{quantity}", test.Quantity.ToString());
                var total = test.Price * test.Quantity;
                total = decimal.Round(total, 2, MidpointRounding.AwayFromZero);
                Body2 = Body2.Replace("{total}", total.ToString());
                Stotal = Stotal + total;
                tableBody = tableBody + Body2;
            }
            var Body3 = System.IO.File.ReadAllText(Path.Combine(_env.ContentRootPath, "BillTemplate", "BillPage3.html"));

            var newBody = Body1 + tableBody + Body3;
            newBody = newBody.Replace("{imgurl}", "data:image/webp;base64,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");
            newBody = newBody.Replace("{inv}", successOrder.InvoiceId.ToString());
            newBody = newBody.Replace("{firstname}", patient.FirstName);
            newBody = newBody.Replace("{lastname}", patient.LastName);
            newBody = newBody.Replace("{address}", patient.Address);
            newBody = newBody.Replace("{city}", patient.City);
            newBody = newBody.Replace("{zip}", patient.Zip);
            newBody = newBody.Replace("{state}", patient.State);
            newBody = newBody.Replace("{phone}", patient.PhoneNo);
            newBody = newBody.Replace("{city}", patient.City);
            newBody = newBody.Replace("{date}", successOrder.OrderDate.ToShortDateString());
            newBody = newBody.Replace("{dfirstname}", doctor.FirstName);
            newBody = newBody.Replace("{dlastname}", doctor.LastName);
            newBody = newBody.Replace("{specialist}", doctor.Specialist);

            newBody = newBody.Replace("{stotal}", Stotal.ToString());
            newBody = newBody.Replace("{consfee}", successOrder.ConsultationFee.ToString());
            newBody = newBody.Replace("{gstpers}", gst.Value.ToString());
            newBody = newBody.Replace("{gst}", successOrder.Gst.ToString());
            newBody = newBody.Replace("{totalbill}", successOrder.TotalPrice.ToString());

            return newBody;
        }
    }
}
