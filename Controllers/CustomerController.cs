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

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
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
        // This method adds customer data to azure table and the sql database
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
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//