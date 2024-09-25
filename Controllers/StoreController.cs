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

        private readonly HttpClient _httpClient;

        public StoreController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Store()
        {
            return View();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------//
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            // Validate the image file
            if (file == null || file.Length == 0 || !IsValidImage(file))
            {
                TempData["UploadError"] = "Please select a valid image file (JPEG, PNG, GIF).";
                return RedirectToAction("Store");
            }

            // Prepare the request to the Azure Function
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file.OpenReadStream())
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(file.ContentType)
                }
            }, "file", file.FileName);

            // Add the x-file-name header to the request
            _httpClient.DefaultRequestHeaders.Add("x-file-name", file.FileName);

            // Call the Azure Function
            var response = await _httpClient.PostAsync("https://cldv-poe-functionapp.azurewebsites.net/api/UploadImageToBlob?code=w3Xx5RWVhtt_alMqLJ3r8g3o-lmoyomh0mOOyFxTTcoPAzFuqGbsLw%3D%3D", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["UploadSuccess"] = "Image uploaded successfully.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["UploadError"] = "Error uploading image. Please try again.";
                return RedirectToAction("Store");
            }
        }

        private bool IsValidImage(IFormFile file)
        {
            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            return validImageTypes.Contains(file.ContentType);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile file)
        {
            if (file != null && IsValidDocument(file))
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StreamContent(file.OpenReadStream())
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue(file.ContentType)
                    }
                }, "file", file.FileName);

                // Add 'x-file-name' header
                content.Headers.Add("x-file-name", file.FileName);

                // Call the Azure Function to upload file to Azure Files
                var response = await _httpClient.PostAsync("https://cldv-poe-functionapp.azurewebsites.net/api/UploadFileToAzureFiles?code=5pa53CCmLPYbQlgTWQ0P3QaGzJP42ojzekvMWupeZ6vZAzFutdGlZQ%3D%3D", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["UploadSuccess"] = "File uploaded successfully.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["UploadError"] = "Error uploading file.";
                }
            }
            else
            {
                TempData["UploadError"] = "Please select a valid document file (PDF, DOC, DOCX, TXT).";
            }

            return RedirectToAction("Store");
        }

        private bool IsValidDocument(IFormFile file)
        {
            var validDocumentTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };
            return validDocumentTypes.Contains(file.ContentType);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
