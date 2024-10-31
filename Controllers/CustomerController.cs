using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ST10296167_CLDV6212_POE.Models;
using ST10296167_CLDV6212_POE.Services;

namespace ST10296167_CLDV6212_POE.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableService _tableService;
        //private readonly HttpClient _httpClient;
//------------------------------------------------------------------------------------------------------------------------------------------//
        //public CustomerController(HttpClient httpClient)
        //{
        //    _httpClient = httpClient;
        //}

        public CustomerController(TableService tableService)
        {
            _tableService = tableService;
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        [HttpGet]
        public IActionResult CustomerProfile()
        {
            return View();
        }
//------------------------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                // Add to Azure Storage
                await _tableService.AddCustomerAsync(profile);

                // Add to SQL Database
                await _tableService.InsertCustomerDb(profile);
            }
            return RedirectToAction("Index", "Home");
        }














        // Function App functionality
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles formatting and sending customer profile data to an Azure Function
        //[HttpPost]
        //public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var jsonContent = JsonConvert.SerializeObject(profile);
        //        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //        string functionUrl = "https://cldv-poe-functionapp.azurewebsites.net/api/StoreCustomerProfile?code=bMxs1F2DoKHbRfk2xBn2uLy0ZX53-D6iM6Y5UGcB9tjXAzFuBZNrzQ%3D%3D";

        //        HttpResponseMessage response = await _httpClient.PostAsync(functionUrl, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            var errorMessage = await response.Content.ReadAsStringAsync();
        //            ModelState.AddModelError("", $"Error: {errorMessage}");
        //        }
        //    }
        //    return View(profile);
        //}
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//