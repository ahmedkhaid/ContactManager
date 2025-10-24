using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesUploadService _countriesService;
  
        public CountriesController(ICountriesUploadService countriesService)
        {
            _countriesService = countriesService;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile.Length==0||excelFile==null)
            {
                ViewBag.ErrorMessage="Please select file";
                return View();
            }
            if (!Path.GetExtension(excelFile.FileName).Equals(".Xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage ="Please select xlsx file Format";
            }
            int CountriesInserted = await _countriesService.UploadCountriesFromExcel(excelFile);
            ViewBag.Message = $"File Upload Successfully {CountriesInserted}";
            return View();
        }
    }
}
