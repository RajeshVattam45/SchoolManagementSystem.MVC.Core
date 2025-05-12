using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.UI.Filter;
using System.Net.Http;
using System.Text;

namespace SchoolManagement.UI.Controllers
{
    [AuthorizeUser ( "Admin" )]

    public class ExamTypeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string apiUrl;

        // Constructor injection for IHttpClientFactory and IConfiguration.
        public ExamTypeController ( IHttpClientFactory httpClientFactory, IConfiguration configuration )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
            // Read API URL from appsettings.json.
            apiUrl = configuration["ApiSettings:ExamTypeApiBaseUerl"];
        }

        // GET: ExamType
        // Displays a list of all exam types.
        public async Task<IActionResult> Index ( )
        {
            var response = await _httpClient.GetAsync ( apiUrl );

            // If API call is successful, deserialize and return data to the view.
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync ();
                var examTypes = JsonConvert.DeserializeObject<IEnumerable<ExamType>> ( jsonData );
                return View ( examTypes );
            }
            return View ( new List<ExamType> () );
        }

        // GET: ExamType/Create
        // Shows the form to create a new exam type.
        public IActionResult Create ( )
        {
            return View ();
        }

        // POST: ExamType/Create
        // Handles the creation of a new exam type.
        [HttpPost]
        public async Task<IActionResult> Create ( ExamType examType )
        {
            if (ModelState.IsValid)
            {
                // Serialize examType and send it to the API via POST.
                var content = new StringContent ( JsonConvert.SerializeObject ( examType ), Encoding.UTF8, "application/json" );
                var response = await _httpClient.PostAsync ( apiUrl, content );
                if (response.IsSuccessStatusCode)
                    return RedirectToAction ( nameof ( Index ) );
            }
            return View ( examType );
        }

        // GET: ExamType/Edit/5
        // Fetches data for a specific exam type and shows the edit form.
        public async Task<IActionResult> Edit ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{apiUrl}/{id}" );
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync ();
                var examType = JsonConvert.DeserializeObject<ExamType> ( jsonData );
                return View ( examType );
            }
            return NotFound ();
        }

        // POST: ExamType/Edit/5
        // Handles updating an existing exam type.
        [HttpPost]
        public async Task<IActionResult> Edit ( ExamType examType )
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent ( JsonConvert.SerializeObject ( examType ), Encoding.UTF8, "application/json" );
                var response = await _httpClient.PutAsync ( $"{apiUrl}/{examType.Id}", content );
                if (response.IsSuccessStatusCode)
                    return RedirectToAction ( nameof ( Index ) );
            }
            return View ( examType );
        }

        // GET: ExamType/Delete/5
        // Shows confirmation view for deleting an exam type.
        public async Task<IActionResult> Delete ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{apiUrl}/{id}" );
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync ();
                var examType = JsonConvert.DeserializeObject<ExamType> ( jsonData );
                return View ( examType );
            }
            return NotFound ();
        }

        // POST: ExamType/Delete/5
        // Confirms and performs deletion of an exam type.
        [HttpPost, ActionName ( "Delete" )]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            var response = await _httpClient.DeleteAsync ( $"{apiUrl}/{id}" );
            if (response.IsSuccessStatusCode)
                return RedirectToAction ( nameof ( Index ) );

            return View ();
        }
    }
}
