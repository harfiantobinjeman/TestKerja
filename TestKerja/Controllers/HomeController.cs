using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using TestKerja.Models;

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
            /*string emails = "eve.holt@reqres.in";
            string passwords = "cityslicka";
            string url = "https://reqres.in/api/login";
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Credentials = new NetworkCredential(emails, passwords);
            var response = req.GetResponse();

            if (response != null)
            {
                return RedirectToAction(nameof(GetAll));
            }
            else
            {
                return View(Index);
            }*/
                return View();
        }

            public async Task<ActionResult> GetAll(string Sorting_Order, string Search_Data, string Filter_Value, int? Page_No)
        {

            ViewBag.CurrentSortOrder = Sorting_Order;
            ViewBag.SortingName = String.IsNullOrEmpty(Sorting_Order) ? "Name_Description" : "";
            ViewBag.SortingDate = Sorting_Order == "Date_Enroll" ? "Date_Description" : "Date";

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            ViewBag.FilterValue = Search_Data;



            var UsersInfo = new Users();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage Res = await client.GetAsync("api/users");

                if (Res.IsSuccessStatusCode)
                {
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    UsersInfo = JsonConvert.DeserializeObject<Users>(EmpResponse);

                }

                var user = from stu in UsersInfo.data select stu;

                if (!String.IsNullOrEmpty(Search_Data))
                {
                    user = user.Where(stu => stu.first_name.ToUpper().Contains(Search_Data.ToUpper())
                        || stu.last_name.ToUpper().Contains(Search_Data.ToUpper()));
                }
                switch (Sorting_Order)
                {
                    case "Name_Description":
                        user = user.OrderByDescending(stu => stu.last_name);
                        break;
                    case "Date_Enroll":
                        user = user.OrderBy(stu => stu.email);
                        break;
                    case "Date_Description":
                        user = user.OrderByDescending(stu => stu.email);
                        break;
                    default:
                        user = user.OrderBy(stu => stu.first_name);
                        break;
                }

                int Size_Of_Page = 4;
                int No_Of_Page = (Page_No ?? 1);
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