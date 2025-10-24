using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IRepositoryContract;
namespace Services
{
    public class CountriesUploadService : ICountriesUploadService
    {
        private readonly ICountryRepository _countriesRepository;
        public CountriesUploadService(ICountryRepository db)
        {
            _countriesRepository = db;
        
        }

      
        public async Task<int> UploadCountriesFromExcel(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countryInserted = 0;
            using (ExcelPackage excelPackage=new ExcelPackage(memoryStream))
            {
                ExcelWorksheet ?workSheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (workSheet==null)
                {
                    return 0;
                }
                int rowContent = workSheet.Dimension.Rows;
                for (int row= 2; row < rowContent; row++) {
                    string? cell = Convert.ToString(workSheet.Cells[row, 1].Value);
                    if(!string.IsNullOrEmpty(cell))
                    {
                        string? countryName = cell;
                        if(_countriesRepository.GetCountryByCountryName(cell)==null)
                        {
                            Country country = new Country() { CountryName = countryName };
                          await  _countriesRepository.AddCountry(country);
                            countryInserted++;
                        }
                    }
                }
            }
            return countryInserted;
        }
    }
}

