using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ST10296167_CLDV6212_POE.Services;

namespace ST10296167_CLDV6212_POE.Controllers
{
    public class StoreController : Controller
    {
        private readonly FileService _fileService;
        private readonly BlobService _blobService;

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public StoreController(FileService fileService, BlobService blobService)
        {
            _fileService = fileService;
            _blobService = blobService;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Store()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles uploading a valid image file to the product-images Azure Blob storage
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file != null && IsValidImage(file))
            {
                using var stream = file.OpenReadStream();
                // Upload image to Azure blob storage
                await _blobService.UploadBlobAsync("product-images", file.FileName, stream);

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var imageData = memoryStream.ToArray();
                    string fileName = file.FileName;

                    // Insert image data into SQL BlobTable
                    await _blobService.InsertBlobDbAsync(imageData, fileName);
                }
            }
            else
            {
                TempData["UploadError"] = "Please select a valid image file (JPEG, PNG, GIF).";
            }
            return RedirectToAction("Index", "Home");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
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
            return RedirectToAction("Index", "Home");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        //This method checks if the correct image file type is selected for upload
        private bool IsValidImage(IFormFile file)
        {
            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            return validImageTypes.Contains(file.ContentType);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        //This method checks if the correct text/document file type is selected for upload
        private bool IsValidDocument(IFormFile file)
        {
            var validDocumentTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };
            return validDocumentTypes.Contains(file.ContentType);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//