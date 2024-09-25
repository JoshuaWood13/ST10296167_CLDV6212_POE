using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ST10296167_CLDV6212_POE.Models;

namespace ST10296167_CLDV6212_POE.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _httpClient;

        public CustomerController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult CustomerProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                // Convert the customer profile object to JSON
                var jsonContent = JsonConvert.SerializeObject(profile);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Function URL (replace with your actual function URL)
                string functionUrl = "https://cldv-poe-functionapp.azurewebsites.net/api/StoreCustomerProfile?code=bMxs1F2DoKHbRfk2xBn2uLy0ZX53-D6iM6Y5UGcB9tjXAzFuBZNrzQ%3D%3D";

                // Send POST request to the Azure Function
                HttpResponseMessage response = await _httpClient.PostAsync(functionUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    // Function executed successfully, redirect to success page or index
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Log error or display error message
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error: {errorMessage}");
                }
            }

            // If validation fails or function fails, stay on the same page
            return View(profile);
        }
    }
}
