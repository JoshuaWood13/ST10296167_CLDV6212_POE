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
        //private readonly HttpClient _httpClient;

        private readonly FileService _fileService;
        private readonly BlobService _blobService;
        //------------------------------------------------------------------------------------------------------------------------------------------//
        //public StoreController(HttpClient httpClient)
        //{
        //    _httpClient = httpClient;
        //}

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

        //This method checks if the correct text/document file type is selected for upload
        private bool IsValidDocument(IFormFile file)
        {
            var validDocumentTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };
            return validDocumentTypes.Contains(file.ContentType);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

























        //        // This method handles the image upload process by sending the image file to an Azure Function
        //        [HttpPost]
        //        public async Task<IActionResult> UploadImage(IFormFile file)
        //        {
        //            if (file == null || file.Length == 0 || !IsValidImage(file))
        //            {
        //                TempData["UploadError"] = "Please select a valid image file (JPEG, PNG, GIF).";
        //                return RedirectToAction("Store");
        //            }

        //            using var content = new MultipartFormDataContent();
        //            content.Add(new StreamContent(file.OpenReadStream())
        //            {
        //                Headers =
        //                {
        //                    ContentType = new MediaTypeHeaderValue(file.ContentType)
        //                }
        //            }, "file", file.FileName);

        //            _httpClient.DefaultRequestHeaders.Add("x-file-name", file.FileName);

        //            var response = await _httpClient.PostAsync("https://cldv-poe-functionapp.azurewebsites.net/api/UploadImageToBlob?code=w3Xx5RWVhtt_alMqLJ3r8g3o-lmoyomh0mOOyFxTTcoPAzFuqGbsLw%3D%3D", content);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                TempData["UploadSuccess"] = "Image uploaded successfully.";
        //                return RedirectToAction("Index", "Home");
        //            }
        //            else
        //            {
        //                TempData["UploadError"] = "Error uploading image. Please try again.";
        //                return RedirectToAction("Store");
        //            }
        //        }

        //        // This method handles the file upload process by sending the file to an Azure Function
        //        [HttpPost]
        //        public async Task<IActionResult> UploadFiles(IFormFile file)
        //        {
        //            if (file != null && IsValidDocument(file))
        //            {
        //                using var content = new MultipartFormDataContent();
        //                content.Add(new StreamContent(file.OpenReadStream())
        //                {
        //                    Headers =
        //                    {
        //                        ContentType = new MediaTypeHeaderValue(file.ContentType)
        //                    }
        //                }, "file", file.FileName);

        //                content.Headers.Add("x-file-name", file.FileName);

        //                var response = await _httpClient.PostAsync("https://cldv-poe-functionapp.azurewebsites.net/api/UploadFileToAzureFiles?code=5pa53CCmLPYbQlgTWQ0P3QaGzJP42ojzekvMWupeZ6vZAzFutdGlZQ%3D%3D", content);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    TempData["UploadSuccess"] = "File uploaded successfully.";
        //                    return RedirectToAction("Index", "Home");
        //                }
        //                else
        //                {
        //                    TempData["UploadError"] = "Error uploading file.";
        //                }
        //            }
        //            else
        //            {
        //                TempData["UploadError"] = "Please select a valid document file (PDF, DOC, DOCX, TXT).";
        //            }

        //            return RedirectToAction("Store");
        //        }
        ////------------------------------------------------------------------------------------------------------------------------------------------//
        //        // This method checks if a valid file is uploaded
        //        private bool IsValidDocument(IFormFile file)
        //        {
        //            var validDocumentTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };
        //            return validDocumentTypes.Contains(file.ContentType);
        //        }

        //        // This method checks if a valid image file is uploaded
        //        private bool IsValidImage(IFormFile file)
        //        {
        //            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        //            return validImageTypes.Contains(file.ContentType);
        //        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//