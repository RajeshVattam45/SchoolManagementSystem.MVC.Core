using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.UI.Filter;
using System.Text;

namespace SchoolManagement.UI.Controllers
{
    [AuthorizeUser ( "Admin" )]
    public class EmployeeController : Controller
    {
        private readonly HttpClient _httpClient;

        // Constructor initializes HttpClient and sets the base API URL.
        public EmployeeController ( IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
            _httpClient.BaseAddress = new Uri ( "https://localhost:7230/api/EmployeeApi/" );
        }
      
        // GET: /Employee
        // Fetches and displays the list of all employees.
        public async Task<IActionResult> Index ( )
        {
            var response = await _httpClient.GetAsync ( "" );
            if (!response.IsSuccessStatusCode)
                return View ( new List<Employee> () );

            var jsonData = await response.Content.ReadAsStringAsync ();
            var employees = JsonConvert.DeserializeObject<List<Employee>> ( jsonData );
            return View ( employees );
        }

        // GET: /Employee/Details/{id}
        // Shows the details of a specific employee.
        public async Task<IActionResult> Details ( int id )
        {
            var response = await _httpClient.GetAsync ( id.ToString () );
            if (!response.IsSuccessStatusCode)
                return NotFound ();

            var jsonData = await response.Content.ReadAsStringAsync ();
            var employee = JsonConvert.DeserializeObject<Employee> ( jsonData );
            return View ( employee );
        }

        // GET: /Employee/Create
        // Returns the create employee form.
        public IActionResult Create ( )
        {
            return View ();
        }

        // POST: /Employee/Create
        // Submits new employee data to the API.
        [HttpPost]
        public async Task<IActionResult> Create ( Employee employee )
        {
            var jsonData = JsonConvert.SerializeObject ( employee );
            var content = new StringContent ( jsonData, Encoding.UTF8, "application/json" );

            var response = await _httpClient.PostAsync ( "", content );
            if (!response.IsSuccessStatusCode)
                return View ( employee );

            return RedirectToAction ( nameof ( Index ) );
        }

        // GET: /Employee/Edit/{id}
        // Loads the edit form with existing employee data.
        public async Task<IActionResult> Edit ( int id )
        {
            var response = await _httpClient.GetAsync ( id.ToString () );
            if (!response.IsSuccessStatusCode)
                return NotFound ();

            var jsonData = await response.Content.ReadAsStringAsync ();
            var employee = JsonConvert.DeserializeObject<Employee> ( jsonData );
            return View ( employee );
        }

        // POST: /Employee/Edit/{id}
        // Submits updated employee data to the API.
        [HttpPost]
        public async Task<IActionResult> Edit ( int id, Employee employee )
        {
            if (id != employee.Id)
                return BadRequest ();

            var jsonData = JsonConvert.SerializeObject ( employee );
            var content = new StringContent ( jsonData, Encoding.UTF8, "application/json" );

            var response = await _httpClient.PutAsync ( id.ToString (), content );
            if (!response.IsSuccessStatusCode)
                return View ( employee );

            return RedirectToAction ( nameof ( Index ) );
        }

        // GET: /Employee/Delete/{id}
        // Displays confirmation page before deleting an employee.
        public async Task<IActionResult> Delete ( int id )
        {
            var response = await _httpClient.GetAsync ( id.ToString () );
            if (!response.IsSuccessStatusCode)
                return NotFound ();

            var jsonData = await response.Content.ReadAsStringAsync ();
            var employee = JsonConvert.DeserializeObject<Employee> ( jsonData );
            return View ( employee );
        }

        // POST: /Employee/Delete/{id}
        // Deletes the employee after confirmation.
        [HttpPost, ActionName ( "Delete" )]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            var response = await _httpClient.DeleteAsync ( id.ToString () );
            if (!response.IsSuccessStatusCode)
                return NotFound ();

            return RedirectToAction ( nameof ( Index ) );
        }
    }
}
