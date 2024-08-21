using Microsoft.AspNetCore.Mvc;
using ST10296167_CLDV6212_POE.Models;
using ST10296167_CLDV6212_POE.Services;
using System.Diagnostics;

namespace ST10296167_CLDV6212_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;

        public HomeController(BlobService blobService, TableService tableService, QueueService queueService, FileService fileService)
        {
            _blobService = blobService;
            _tableService = tableService;
            _queueService = queueService;
            _fileService = fileService;
        }

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

        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddEntityAsync(profile);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Products product)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddProductAsync(product);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string orderID)
        {
            await _queueService.SendMessageAsync("order-processing", $"Processing order {orderID}");
            return RedirectToAction("Index");
        }

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

        private bool IsValidImage(IFormFile file)
        {
            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            return validImageTypes.Contains(file.ContentType);
        }

        private bool IsValidDocument(IFormFile file)
        {
            var validDocumentTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };
            return validDocumentTypes.Contains(file.ContentType);
        }
    }
}
