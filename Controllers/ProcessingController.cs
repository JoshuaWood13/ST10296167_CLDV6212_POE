using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using ST10296167_CLDV6212_POE.Services;

namespace ST10296167_CLDV6212_POE.Controllers
{
    public class ProcessingController : Controller
    {
        private readonly QueueService _queueService;

        // Controlller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public ProcessingController(QueueService queueService)
        {
            _queueService = queueService;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Processing()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles constructing an order processing queue based on user input 
        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string orderID, string customerID)
        {
            string queueMessage = $"Processing order {orderID} for Customer {customerID}";

            // Add to Azure Queue
            await _queueService.SendMessageAsync("order-processing", queueMessage);

            // Add order information to SQL Database
            await _queueService.InsertOrderInfoDbAsync(orderID, customerID);

            return RedirectToAction("Index", "Home");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles constructing and uploading a inventory management queue based on user input
        [HttpPost]
        public async Task<IActionResult> UpdateInventory(string productName, int quantity, string inventoryState)
        {
            string queueMessage = $"{quantity} units of {productName} are {inventoryState}";
            await _queueService.SendMessageAsync("inventory-management", queueMessage);
            return RedirectToAction("Index", "Home");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//