using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ST10296167_CLDV6212_POE.Models;
using ST10296167_CLDV6212_POE.Services;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ST10296167_CLDV6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;
//------------------------------------------------------------------------------------------------------------------------------------------//
        public HomeController(BlobService blobService, TableService tableService, QueueService queueService, FileService fileService)
        {
            _blobService = blobService;
            _tableService = tableService;
            _queueService = queueService;
            _fileService = fileService;
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CustomerProfile()
        {
            return View();
        }
        public IActionResult Product()
        {
            return View();
        }

        public IActionResult Store()
        {
            return View();
        }
        public IActionResult Processing()
        {
            return View();
        }

        public IActionResult Error()
        {
            var requestId = HttpContext.TraceIdentifier;
            return View(new ErrorViewModel { RequestId = requestId });
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles uploading a valid image file to the product-images Azure Blob storage
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && IsValidImage(file))
            {
                using var stream = file.OpenReadStream();
                await _blobService.UploadBlobAsync("product-images", file.FileName, stream);
            }
            else
            {
                TempData["UploadError"] = "Please select a valid image file (JPEG, PNG, GIF).";
            }
            return RedirectToAction("Index");
        }       

        // This method adds a user-input customer profie to the CustomerProfiles Azure table
        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddEntityAsync(profile);
            }
            return RedirectToAction("Index");
        }     

        // This method adds a user-input product to the Products Azure table 
        [HttpPost]
        public async Task<IActionResult> AddProduct(Products product)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddProductAsync(product);
            }
            return RedirectToAction("Index");
        }

        // This method handles constructing an order processing queue based on user input 
        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string orderID)
        {
            string queueMessage = $"Processing order {orderID}";
            await _queueService.SendMessageAsync("order-processing", queueMessage);
            return RedirectToAction("Index");
        }

        // This method handles constructing and uploading a inventory management queue based on user input
        [HttpPost]
        public async Task<IActionResult> UpdateInventory(string productName, int quantity, string inventoryState)
        {
            string queueMessage = $"{quantity} units of {productName} are {inventoryState}";
            await _queueService.SendMessageAsync("inventory-management", queueMessage);
            return RedirectToAction("Index");
        }

        // This method handles to uploading of a valid document file to the uploaded-files Azure file storage
        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile file)
        {
            if (file != null && IsValidDocument(file))
            {
                using var stream = file.OpenReadStream();
                await _fileService.UploadFileAsync("uploaded-files", file.FileName, stream);
            }
            else
            {
                TempData["UploadError"] = "Please select a valid document file (PDF, DOC, DOCX, TXT).";
            }
            return RedirectToAction("Index");
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        //This method checks if the correct image file type is selected for upload
        private bool IsValidImage(IFormFile file)
        {
            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            return validImageTypes.Contains(file.ContentType);
        }

        //This method checks if the correct text/document file type is selected for upload
        private bool IsValidDocument(IFormFile file)
        {
            var validDocumentTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };
            return validDocumentTypes.Contains(file.ContentType);
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
