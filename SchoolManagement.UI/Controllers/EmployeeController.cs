using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.UI.Filter;
using System.Text;

namespace SchoolManagement.UI.Controllers
{
    //[AuthorizeUser ( "Admin" )]
    public class EmployeeController : Controller
    {
        private readonly HttpClient _httpClient;

        // Constructor initializes HttpClient and sets the base API URL.
        public EmployeeController ( IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
            _httpClient.BaseAddress = new Uri ( "https://localhost:7230/api/EmployeeApi/" );
        }

        [AuthorizeUser ( "Admin", "Teacher" )]
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

        [AuthorizeUser ( "Admin", "Teacher" )]
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

        [AuthorizeUser ( "Admin" )]
        // GET: /Employee/Create
        // Returns the create employee form.
        public IActionResult Create ( )
        {
            return View ();
        }

        // POST: /Employee/Create
        // Submits new employee data to the API.
        //[HttpPost]
        //public async Task<IActionResult> Create ( Employee employee )
        //{
        //    var jsonData = JsonConvert.SerializeObject ( employee );
        //    var content = new StringContent ( jsonData, Encoding.UTF8, "application/json" );

        //    var response = await _httpClient.PostAsync ( "", content );
        //    if (!response.IsSuccessStatusCode)
        //        return View ( employee );

        //    return RedirectToAction ( nameof ( Index ) );
        //}

        [HttpPost]
        public async Task<IActionResult> Create ( IFormCollection form )
        {
            var file = form.Files.GetFile ( "ImageFile" );

            using var content = new MultipartFormDataContent ();

            // Add form fields
            content.Add ( new StringContent ( form["EmployeeId"] ), "EmployeeId" );
            content.Add ( new StringContent ( form["FirstName"] ), "FirstName" );
            content.Add ( new StringContent ( form["LastName"] ), "LastName" );
            content.Add ( new StringContent ( form["PhoneNumber"] ), "PhoneNumber" );
            content.Add ( new StringContent ( form["Role"] ), "Role" );
            content.Add ( new StringContent ( form["PermanentAddress"] ), "PermanentAddress" );
            content.Add ( new StringContent ( form["CurrentAddress"] ), "CurrentAddress" );
            content.Add ( new StringContent ( form["Pincode"] ), "Pincode" );
            content.Add ( new StringContent ( form["EmployeeSalary"] ), "EmployeeSalary" );
            content.Add ( new StringContent ( form["Email"] ), "Email" );
            content.Add ( new StringContent ( form["PasswordHash"] ), "PasswordHash" );
            content.Add ( new StringContent ( form["Gender"] ), "Gender" );
            content.Add ( new StringContent ( form["DateOfJoining"] ), "DateOfJoining" );

            // Add file if exists
            if (file != null && file.Length > 0)
            {
                var streamContent = new StreamContent ( file.OpenReadStream () );
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue ( file.ContentType );
                content.Add ( streamContent, "ProfileImage", file.FileName );
            }

            // Send request to FastEndpoints API
            var response = await _httpClient.PostAsync ( "https://localhost:7230/api/employees/create", content );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError ( string.Empty, "Error creating employee." );
                return View ();
            }

            return RedirectToAction ( nameof ( Index ) );
        }

        [AuthorizeUser ( "Admin" )]
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

        [AuthorizeUser ( "Admin" )]
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
