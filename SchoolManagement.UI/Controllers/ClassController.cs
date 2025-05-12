using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.UI.Filter;
using System.Net.Http;
using System.Text;

namespace SchoolManagement.UI.Controllers
{
    // Allows access to Admin and Teacher roles only.
    [AuthorizeUser ( "Admin", "Teacher", "Student" )]
    public class ClassController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        // Constructor initializes HttpClient and loads the Class API base URL from config.
        public ClassController ( IHttpClientFactory httpClientFactory, IConfiguration configuration )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
            _apiBaseUrl = configuration["ApiSettings:ClassApiBaseUrl"];
        }

        // GET: /Class
        // Fetch and display a list of all classes.
        public async Task<IActionResult> Index ( )
        {
            var response = await _httpClient.GetAsync ( _apiBaseUrl );
            if (!response.IsSuccessStatusCode)
                return View ( new List<Class> () );

            var jsonString = await response.Content.ReadAsStringAsync ();
            var classes = JsonConvert.DeserializeObject<List<Class>> ( jsonString );
            return View ( classes );
        }

        // GET: /Class/Details/{id}
        // Displays detailed info for a specific class by ID.
        public async Task<IActionResult> Details ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}" );
            if (!response.IsSuccessStatusCode)
                return NotFound ();

            var jsonString = await response.Content.ReadAsStringAsync ();
            var cls = JsonConvert.DeserializeObject<Class> ( jsonString );
            return View ( cls );
        }

        // GET: /Class/Create
        // Show the class creation form.
        [HttpGet]
        public IActionResult Create ( )
        {
            return View ();
        }

        // POST: /Class/Create
        // Send new class data to the API to create a class.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( Class cls )
        {
            var jsonContent = new StringContent ( JsonConvert.SerializeObject ( cls ), Encoding.UTF8, "application/json" );
            var response = await _httpClient.PostAsync ( _apiBaseUrl, jsonContent );
            if (!response.IsSuccessStatusCode)
                return View ( cls );

            return RedirectToAction ( nameof ( Index ) );
        }

        // GET: /Class/Edit/{id}
        // Load the edit form with existing class data.
        [HttpGet]
        public async Task<IActionResult> Edit ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}" );
            if (!response.IsSuccessStatusCode)
                return NotFound ();

            var jsonString = await response.Content.ReadAsStringAsync ();
            var cls = JsonConvert.DeserializeObject<Class> ( jsonString );

            return View ( cls );
        }

        // POST: /Class/Edit/{id}
        // Update class info in the API.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit ( int id, Class cls )
        {
            if (!ModelState.IsValid)
            {
                return View ( cls );
            }
            else
            {
                foreach (var modelError in ModelState.Values.SelectMany ( x => x.Errors ))
                {
                    Console.WriteLine ( $"Error {modelError.ErrorMessage}" );
                }
            }

            // Serialize updated class and send to API via PUT
            var jsonContent = new StringContent ( JsonConvert.SerializeObject ( cls ), Encoding.UTF8, "application/json" );
            var response = await _httpClient.PutAsync ( $"{_apiBaseUrl}/{id}", jsonContent );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError ( "", "Failed to update class. Please try again." );
                return View ( cls );
            }

            return RedirectToAction ( nameof ( Index ) );
        }

        // GET: /Class/Delete/{id}
        // Delete a class record by ID.
        public async Task<IActionResult> Delete ( int id )
        {
            var response = await _httpClient.DeleteAsync ( $"{_apiBaseUrl}/{id}" );
            if (!response.IsSuccessStatusCode)
                return NotFound ();

            return RedirectToAction ( nameof ( Index ) );
        }
        [AuthorizeUser ( "Admin", "Teacher", "Student" )]

        // GET: /Class/GetStudentClassHistory/{studentId}
        // Fetch class history for a given student ID.
        [HttpGet]
        public async Task<IActionResult> LoadStudentClassHistory ( int studentId )
        {
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/student/{studentId}/class-history" );

            if (!response.IsSuccessStatusCode)
            {
                return PartialView ( "_ClassHistoryPartial", new List<StudentClassHistory> () );
            }

            var jsonString = await response.Content.ReadAsStringAsync ();
            var classHistory = JsonConvert.DeserializeObject<List<StudentClassHistory>> ( jsonString );

            return PartialView ( "_ClassHistoryPartial", classHistory );
        }
    }
}
