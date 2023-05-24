using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using TestKerja.Models;
using TestKerja.Paginat;

namespace TestKerja.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient httpClient;
        string Baseurl = "https://reqres.in/";

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            this.httpClient = httpClient;
        }

        public async Task<ActionResult> Index()
        {
                return View();
        }

        public async Task<ActionResult> GetAll(int page)
        {
            var UsersInfo = new Users();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/users?page="+page);

                if (Res.IsSuccessStatusCode)
                {
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    UsersInfo = JsonConvert.DeserializeObject<Users>(EmpResponse);
                }
                return View(UsersInfo);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}