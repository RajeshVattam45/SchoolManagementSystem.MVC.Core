using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.UI.Filter;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolManagementSystem.MVC.Controllers
{
   [AuthorizeUser ( "Admin", "Teacher" )]
    public class SubjectController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiUrl = "https://localhost:7230/api/SubjectApi";

        public SubjectController ( IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
        }

        // GET: Subject
        // Retrieves and displays all subjects.
        public async Task<IActionResult> Index ( )
        {
            try
            {
                var response = await _httpClient.GetAsync ( apiUrl );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync ();
                    var subjects = System.Text.Json.JsonSerializer.Deserialize<List<Subject>> ( json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    } );

                    return View ( subjects ?? new List<Subject> () );
                }

                ViewBag.Error = "Unable to retrieve subjects. Please try again later.";
                return View ( new List<Subject> () );
            }
            catch (Exception ex)
            {
                // Optionally log error using ILogger here
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View ( new List<Subject> () );
            }
        }


        // GET: Subject/Create
        // Displays the form to create a new subject.
        public async Task<IActionResult> Create ( )
        {
            // Fetch Employees again if creation fails
            var employeeResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/EmployeeApi" );
            var employees = JsonConvert.DeserializeObject<List<Employee>> ( employeeResponse );
            ViewBag.Employees = employees;
            return View ();
        }

        // POST: Subject/Create
        // Handles form submission to create a new subject.
        [HttpPost]
        public async Task<IActionResult> Create ( Subject subject )
        {
            if (!ModelState.IsValid)
            {
                var employeeResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/EmployeeApi" );
                var employees = JsonConvert.DeserializeObject<List<Employee>> ( employeeResponse );
                ViewBag.Employees = employees;
                return View ( subject );
            }

            var content = new StringContent ( JsonConvert.SerializeObject ( subject ), Encoding.UTF8, "application/json" );
            var response = await _httpClient.PostAsync ( apiUrl, content );

            if (response.IsSuccessStatusCode)
                return RedirectToAction ( "Index" );

            ViewBag.Error = "Failed to create subject.";
            return View ( subject );
        }

        // GET: Subject/Edit/{id}
        // Displays the edit form for a specific subject.
        public async Task<IActionResult> Edit ( int id )
        {
            var response = await _httpClient.GetStringAsync ( $"{apiUrl}/{id}" );
            var subject = JsonConvert.DeserializeObject<Subject> ( response );

            var employeeResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/EmployeeApi" );
            var employees = JsonConvert.DeserializeObject<List<Employee>> ( employeeResponse );
            ViewBag.Employees = employees;

            return View ( subject );
        }

        // POST: Subject/Edit
        // Handles form submission to update an existing subject.
        [HttpPost]
        public async Task<IActionResult> Edit ( Subject subject )
        {
            if (!ModelState.IsValid)
                return View ( subject );

            var content = new StringContent ( JsonConvert.SerializeObject ( subject ), Encoding.UTF8, "application/json" );
            var response = await _httpClient.PutAsync ( $"{apiUrl}/{subject.Id}", content );

            if (response.IsSuccessStatusCode)
                return RedirectToAction ( "Index" );

            ViewBag.Error = "Failed to update subject.";
            return View ( subject );
        }

        // GET: Subject/Delete/5
        // Displays the confirmation view for deleting a subject.
        public async Task<IActionResult> Delete ( int id )
        {
            var response = await _httpClient.GetStringAsync ( $"{apiUrl}/{id}" );
            var subject = JsonConvert.DeserializeObject<Subject> ( response );
            if (subject == null)
            {
                return NotFound ();
            }

            return View ( subject );
        }

        // POST: Subject/Delete/5
        // Handles the actual deletion of a subject after confirmation.
        [HttpPost, ActionName ( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            await _httpClient.DeleteAsync ( $"{apiUrl}/{id}" );
            return RedirectToAction ( nameof ( Index ) );
        }
    }
}
