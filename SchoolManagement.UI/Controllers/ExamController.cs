using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.UI.Filter;
using System.Net.Http;
using System.Text;

namespace SchoolManagement.UI.Controllers
{
    // Restrict access to only users with "Admin" role.
    [AuthorizeUser ( "Admin" )]

    public class ExamController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiBaseUrl = "https://localhost:7230/api/ExamApi";

        // Constructor to initialize HttpClient through dependency injection.
        public ExamController ( IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );

        }

        // GET: Exam/Index
        // Displays a list of all exams.
        public async Task<IActionResult> Index ( )
        {
            var response = await _httpClient.GetAsync ( apiBaseUrl );
            if (!response.IsSuccessStatusCode) return View ( new List<Exam> () );

            var jsonData = await response.Content.ReadAsStringAsync ();
            var exams = JsonConvert.DeserializeObject<List<Exam>> ( jsonData );
            return View ( exams );
        }

        // GET: Exam/Create
        // Displays the exam creation form with exam types dropdown.
        public async Task<IActionResult> Create ( )
        {
            // Fetch available exam types from API
            var examTypeResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamTypeApi" );
            var examTypes = JsonConvert.DeserializeObject<List<ExamType>> ( examTypeResponse );

            // Populate ViewBag with exam types for dropdown selection.
            ViewBag.ExamTypes = new SelectList ( examTypes, "Id", "ExamTypeName" );
            return View ();
        }

        // POST: Exam/Create
        // Submits the form data to create a new exam.
        [HttpPost]
        public async Task<IActionResult> Create ( Exam exam )
        {
            if (!ModelState.IsValid) return View ( exam );

            var jsonData = JsonConvert.SerializeObject ( exam );
            var content = new StringContent ( jsonData, Encoding.UTF8, "application/json" );

            var response = await _httpClient.PostAsync ( apiBaseUrl, content );
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction ( nameof ( Index ) );
            }

            return View ( exam );
        }

        // GET: Exam/Edit/{id}
        // Displays the exam edit form with current data and available exam types.
        public async Task<IActionResult> Edit ( int id )
        {
            // Fetch available exam types from API
            var examTypeResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamTypeApi" );
            var examTypes = JsonConvert.DeserializeObject<List<ExamType>> ( examTypeResponse );

            // Convert List<ExamType> to SelectList
            ViewBag.ExamTypes = new SelectList ( examTypes, "Id", "ExamTypeName" );

            var response = await _httpClient.GetAsync ( $"{apiBaseUrl}/{id}" );
            if (!response.IsSuccessStatusCode) return NotFound ();

            var jsonData = await response.Content.ReadAsStringAsync ();
            var exam = JsonConvert.DeserializeObject<Exam> ( jsonData );
            return View ( exam );
        }

        // POST: Exam/Edit
        // Submits the edited exam data.
        [HttpPost]
        public async Task<IActionResult> Edit ( Exam exam )
        {
            if (!ModelState.IsValid) return View ( exam );

            var jsonData = JsonConvert.SerializeObject ( exam );
            var content = new StringContent ( jsonData, Encoding.UTF8, "application/json" );

            var response = await _httpClient.PutAsync ( $"{apiBaseUrl}/{exam.ExamId}", content );
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction ( nameof ( Index ) );
            }

            return View ( exam );
        }

        // GET: Exam/Delete/{id}
        // Displays the delete confirmation page.
        public async Task<IActionResult> Delete ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{apiBaseUrl}/{id}" );
            if (!response.IsSuccessStatusCode) return NotFound ();

            var jsonData = await response.Content.ReadAsStringAsync ();
            var exam = JsonConvert.DeserializeObject<Exam> ( jsonData );
            return View ( exam );
        }

        // POST: Exam/DeleteConfirmed/{id}
        // Deletes the specified exam.
        [HttpPost, ActionName ( "Delete" )]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            var response = await _httpClient.DeleteAsync ( $"{apiBaseUrl}/{id}" );
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction ( nameof ( Index ) );
            }

            return NotFound ();
        }
    }
}
