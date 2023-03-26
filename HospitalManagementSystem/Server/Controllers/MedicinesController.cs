using Dapper;
using HospitalManagementSystem.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using Stripe;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public MedicinesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("medicines")]
        public async Task<ActionResult<List<Medicines>>> GetAllMedicines()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Medicines> medicines = await connection.QueryAsync<Medicines>("select * from Medicines");
            return Ok(medicines);
        }

        [HttpGet("template")]
        public async Task<ActionResult<List<Medicines>>> GetMedicineTemplate()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Medicines> medicines = await connection.QueryAsync<Medicines>("select Name, Price, MfgDate, ExpDate, AvailableMedicinesCount from Medicines where 2=1");
            return Ok(medicines);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Medicines>>> GetMedicine(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var medicine = await connection.QueryFirstAsync<Medicines>("select * from Medicines Where Id=@Id", new { Id = id });
            return Ok(medicine);
        }

        [HttpPost]
        public async Task<ActionResult<List<Medicines>>> AddMedicine(Medicines medicine)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("Insert into Medicines values(@Name, @Price, @MfgDate, @ExpDate, @AvailableMedicinesCount)", medicine);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult<List<Medicines>>> UpdateMedicine(Medicines medicine)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Medicines set Name=@Name, Price=@Price, MfgDate=@MfgDate, ExpDate=@ExpDate, AvailableMedicinesCount=@AvailableMedicinesCount where Id = @Id", medicine);
            return Ok();
        }

        [HttpPut("name")]
        public async Task<ActionResult<List<Medicines>>> UpdateMedicineWithName(Medicines medicine)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update Medicines set Name=@Name, Price=@Price, MfgDate=@MfgDate, ExpDate=@ExpDate, AvailableMedicinesCount=@AvailableMedicinesCount where Name = @Name", medicine);
            return Ok();
        }

        [HttpPost("importfromexcel")]
        public async Task<ActionResult<List<Medicines>>> ImportFromExcel(List<IFormFile> files)
        {
            var medicines = new List<Medicines>();
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            foreach (var file in files)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;
                        if (worksheet.Cells[1, 1].Value.ToString().Trim().Equals("Name") && worksheet.Cells[1, 2].Value.ToString().Trim().Equals("Price") &&
                            worksheet.Cells[1, 3].Value.ToString().Trim().Equals("MfgDate") && worksheet.Cells[1, 4].Value.ToString().Trim().Equals("ExpDate")
                            && worksheet.Cells[1, 5].Value.ToString().Trim().Equals("AvailableMedicinesCount"))
                        {
                            for (int row = 2; row <= rowCount; row++)
                            {
                                medicines.Add(
                                    new Medicines
                                    {
                                        Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        Price = Convert.ToDecimal(worksheet.Cells[row, 2].Value.ToString().Trim()),
                                        MfgDate = Convert.ToDateTime(worksheet.Cells[row, 3].Value.ToString().Trim()),
                                        ExpDate = Convert.ToDateTime(worksheet.Cells[row, 4].Value.ToString().Trim()),
                                        AvailableMedicinesCount = Convert.ToInt32(worksheet.Cells[row, 5].Value.ToString().Trim())
                                    });
                            }
                        }
                        else
                        {
                            return BadRequest("Column headers didn't match. Please download the excel template and try again...");
                        }
                    }
                }
            }
            return Ok(medicines);
        }

        [HttpPost("exporttoexcel")]
        public IActionResult ExportToExcel(List<Export_Medicines> medicines)
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection<Export_Medicines>(medicines, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"Medicines_{DateTime.Now.ToString("ddMMyyyy")}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpPost("exporttoexcelmeds")]
        public IActionResult ExportToExcelMeds(List<Export_Medicines> medicines)
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");

                workSheet.Cells["A1"].Value = "Name";
                workSheet.Cells["B1"].Value = "Price";
                workSheet.Cells["C1"].Value = "MfgDate";
                workSheet.Cells["D1"].Value = "ExpDate";
                workSheet.Cells["E1"].Value = "AvailableMedicinesCount";
                int rowStart = 2;

                foreach (var medicine in medicines)
                {
                    workSheet.Cells[string.Format("A{0}", rowStart)].Value = medicine.Name;
                    workSheet.Cells[string.Format("B{0}", rowStart)].Value = medicine.Price;
                    workSheet.Cells[string.Format("C{0}", rowStart)].Value = string.Format("{0:dd-MM-yyyy}", medicine.MfgDate);
                    workSheet.Cells[string.Format("D{0}", rowStart)].Value = string.Format("{0:dd-MM-yyyy}", medicine.ExpDate);
                    workSheet.Cells[string.Format("E{0}", rowStart)].Value = medicine.AvailableMedicinesCount;
                    rowStart++; 
                }
                workSheet.Cells["A:AZ"].AutoFitColumns();
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"Medicines_{DateTime.Now.ToString("ddMMyyyy")}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
