using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;
using Newtonsoft.Json;
using SchoolManagement.UI.Filter;

namespace SchoolManagementSystem.Controllers
{
    [AuthorizeUser ( "Admin", "Teacher" )]
    public class ExamSubjectController : Controller
    {
        // URLs for different API endpoints, read from appsettings.json.
        private readonly HttpClient _httpClient;
        private readonly string _examApiUrl;
        private readonly string _subjectApiUrl;
        private readonly string _examSubjectApiUrl;

        // Constructor injection of HttpClient and IConfiguration.
        public ExamSubjectController ( IHttpClientFactory httpClientFactory, IConfiguration configuration )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );

            // Get API URLs directly from appsettings.json using IConfiguration.
            _examApiUrl = configuration["ApiSettings:ExamApiBaseUrl"];
            _subjectApiUrl = configuration["ApiSettings:SubjectApiBaseUrl"];
            _examSubjectApiUrl = configuration["ApiSettings:ExamSubjectApiBaseUrl"];
        }


        // GET: Display list of all exam-subject assignments.
        public async Task<IActionResult> Index ( )
        {
            var response = await _httpClient.GetAsync ( _examSubjectApiUrl );
            if (!response.IsSuccessStatusCode)
                return View ( "Error" );

            var data = await response.Content.ReadAsStringAsync ();
            var examSubjects = JsonConvert.DeserializeObject<List<ExamSubject>> ( data );
            return View ( examSubjects );
        }

        // GET: Render the form to assign subjects to an exam.
        public async Task<IActionResult> Create ( )
        {
            var model = new AssignSubjectToExamViewModel
            {
                // Populate subjects & exams for checkbox list.
                Subjects = await GetSubjectsAsync (),
                ExamList = await GetExamsDropdownAsync ()
            };
            return View ( model );
        }

        // POST: Handle the submission of assigned subjects to an exam.
        [HttpPost]
        public async Task<IActionResult> Create ( AssignSubjectToExamViewModel model )
        {
            // Validate form: must select an exam and at least one subject.
            if (!ModelState.IsValid || model.SelectedExamId == 0 || model.SelectedSubjectIds.Count == 0)
            {
                // Re-populate dropdowns before returning to view.
                model.Subjects = await GetSubjectsAsync ();
                model.ExamList = await GetExamsDropdownAsync ();
                ModelState.AddModelError ( "", "Please select an exam and at least one subject." );
                return View ( model );
            }

            // Loop through each selected subject and assign to the exam.
            foreach (var subjectId in model.SelectedSubjectIds)
            {
                var examSubject = new ExamSubject
                {
                    ExamId = model.SelectedExamId,
                    SubjectId = subjectId
                };

                var content = new StringContent ( JsonConvert.SerializeObject ( examSubject ), Encoding.UTF8, "application/json" );
                await _httpClient.PostAsync ( _examSubjectApiUrl, content );
            }

            return RedirectToAction ( "Index" );
        }

        // Helper method to retrieve all subjects from Subject API.
        private async Task<List<Subject>> GetSubjectsAsync ( )
        {
            var response = await _httpClient.GetAsync ( _subjectApiUrl );
            if (!response.IsSuccessStatusCode) return new List<Subject> ();

            var json = await response.Content.ReadAsStringAsync ();
            return JsonConvert.DeserializeObject<List<Subject>> ( json );
        }

        // Helper method to retrieve and prepare exams as dropdown options.
        private async Task<List<SelectListItem>> GetExamsDropdownAsync ( )
        {
            var response = await _httpClient.GetAsync ( _examApiUrl );
            if (!response.IsSuccessStatusCode) return new List<SelectListItem> ();

            var json = await response.Content.ReadAsStringAsync ();
            var exams = JsonConvert.DeserializeObject<List<Exam>> ( json );

            return exams.Select ( e => new SelectListItem
            {
                Value = e.ExamId.ToString (),
                Text = e.ExamName
            } ).ToList ();
        }

        // DELETE: Delete all subjects assigned to a specific exam.
        public async Task<IActionResult> Delete ( int examId )
        {
            var response = await _httpClient.DeleteAsync ( $"https://localhost:7230/api/ExamSubjectApi/{examId}" );
            if (!response.IsSuccessStatusCode)
                return View ( "Error" );

            return RedirectToAction ( "Index" );
        }
    }
}
