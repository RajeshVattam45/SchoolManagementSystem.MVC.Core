using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.UI.Filter;
using SchoolManagement.UI.Models;

namespace SchoolManagement.UI.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config )
        {
            _logger = logger;
            _config = config;
        }

        // GET: /
        // The main entry point of the application - loads dashboard/summary data.
        public async Task<IActionResult> Index ( )
        {
            // Flag used in the view.
            ViewData["IsHomePage"] = true;

            // ViewModel to hold summarized data for dashboard.
            var viewModel = new SchoolSummaryViewModel ();

            // Create and dispose HttpClient using a using statement to avoid memory leaks.
            using (HttpClient client = new HttpClient ())
            {
                // Read API base URLs from appsettings.json.
                string studentApiUrl = _config["ApiSettings:StudentApiBaseUrl"];
                string employeeApiUrl = _config["ApiSettings:EmployeeApiBaseUrl"];

                // ----------------------------
                // Fetch student data
                // ----------------------------
                HttpResponseMessage studentResponse = await client.GetAsync ( studentApiUrl );
                if (studentResponse.IsSuccessStatusCode)
                {
                    // Deserialize the JSON response into a list of Student objects.
                    var studentJson = await studentResponse.Content.ReadAsStringAsync ();
                    var students = JsonConvert.DeserializeObject<List<Student>> ( studentJson );

                    // Store student count in the ViewModel.
                    viewModel.StudentCount = students.Count;
                }

                // ----------------------------
                // Fetch employee data
                // ----------------------------
                HttpResponseMessage employeeResponse = await client.GetAsync ( employeeApiUrl );
                if (employeeResponse.IsSuccessStatusCode)
                {
                    // Deserialize the JSON response into a list of Employee objects.
                    var employeeJson = await employeeResponse.Content.ReadAsStringAsync ();
                    var employees = JsonConvert.DeserializeObject<List<Employee>> ( employeeJson );

                    // Store employee count in the ViewModel.
                    viewModel.EmployeeCount = employees.Count;
                }
            }

            viewModel.BranchCount = 3;

            ViewBag.SchoolSummaryModel = viewModel;

            return View ();
        }

        // GET: /Home/Privacy
        // Returns the privacy policy page.
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: /Home/Error
        // Returns an error view in case of an unhandled exception or 404.
        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Create an error view model with the current request ID for debugging purposes.
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
