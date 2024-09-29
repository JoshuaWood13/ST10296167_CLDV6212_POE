using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Net.Http;

namespace ST10296167_CLDV6212_POE.Controllers
{
    public class ProcessingController : Controller
    {
        private readonly HttpClient _httpClient;
//------------------------------------------------------------------------------------------------------------------------------------------//
        public ProcessingController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Processing()
        {
            return View();
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles constructing an order processing queue based on user input
        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string orderID)
        {
            string queueMessage = $"Processing order {orderID}";

            var content = new StringContent(JsonSerializer.Serialize(new
            {
                QueueName = "order-processing",
                Message = queueMessage
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://cldv-poe-functionapp.azurewebsites.net/api/SendMessageToQueue?code=PKgbSRL_WRmo4N9tAGOujPJ83gjme1khpxBH6rbR_Gq0AzFuB7X1ew%3D%3D", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["QueueSuccess"] = "Order processing message sent successfully.";
            }
            else
            {
                TempData["QueueError"] = "Error sending order processing message. Please try again.";
            }
            return RedirectToAction("Index", "Home");
        }

        // This method handles constructing and uploading an inventory management queue based on user input
        [HttpPost]
        public async Task<IActionResult> UpdateInventory(string productName, int quantity, string inventoryState)
        {
            string queueMessage = $"{quantity} units of {productName} are {inventoryState}";

            var content = new StringContent(JsonSerializer.Serialize(new
            {
                QueueName = "inventory-management",
                Message = queueMessage
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://cldv-poe-functionapp.azurewebsites.net/api/SendMessageToQueue?code=PKgbSRL_WRmo4N9tAGOujPJ83gjme1khpxBH6rbR_Gq0AzFuB7X1ew%3D%3D", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["QueueSuccess"] = "Inventory update message sent successfully.";
            }
            else
            {
                TempData["QueueError"] = "Error sending inventory update message. Please try again.";
            }

            return RedirectToAction("Index", "Home");
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//