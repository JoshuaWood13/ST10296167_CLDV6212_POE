using Microsoft.AspNetCore.Mvc;
using ST10296167_CLDV6212_POE.Models;
using ST10296167_CLDV6212_POE.Services;

namespace ST10296167_CLDV6212_POE.Controllers
{
    public class ProductController : Controller
    {
        private readonly TableService _tableService;

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public ProductController(TableService tableService)
        {
            _tableService = tableService;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Product()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method adds a user-input product to the Products Azure table 
        [HttpPost]
        public async Task<IActionResult> AddProduct(Products product)
        {
            if (ModelState.IsValid)
            {
                // Add to Azure Storage
                await _tableService.AddProductAsync(product);

                // Add to SQL Database
                await _tableService.InsertProductDb(product);
            }
            return RedirectToAction("Index", "Home");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//