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

        //------------------------------------------------------------------------------------------------------------------------------------------//
        public HomeController()
        {

        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Index()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//