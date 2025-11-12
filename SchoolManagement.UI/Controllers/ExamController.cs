using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.UI.Filter;
using System.ComponentModel.DataAnnotations;
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

        private readonly string _examTypeApiUrl = "https://localhost:7230/api/ExamTypeApi";
        private readonly string _subjectApiUrl = "https://localhost:7230/api/SubjectApi";
        private readonly string _examSubjectApiUrl = "https://localhost:7230/api/ExamSubjectApi";

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
            var examTypes = JsonConvert.DeserializeObject<List<ExamType>> (
                await _httpClient.GetStringAsync ( _examTypeApiUrl )
            );

            var subjects = JsonConvert.DeserializeObject<List<Subject>> (
                await _httpClient.GetStringAsync ( _subjectApiUrl )
            );

            var model = new ExamCreateViewModel
            {
                Subjects = subjects.Select ( s => new SelectListItem
                {
                    Value = s.Id.ToString (),
                    Text = s.SubjectName
                } ).ToList ()
            };

            ViewBag.ExamTypes = new SelectList ( examTypes, "Id", "ExamTypeName" );
            return View ( model );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( ExamCreateViewModel model )
        {
            if (!ModelState.IsValid)
            {
                var examTypes = JsonConvert.DeserializeObject<List<ExamType>> (
                    await _httpClient.GetStringAsync ( _examTypeApiUrl )
                );

                var subjects = JsonConvert.DeserializeObject<List<Subject>> (
                    await _httpClient.GetStringAsync ( _subjectApiUrl )
                );

                ViewBag.ExamTypes = new SelectList ( examTypes, "Id", "ExamTypeName" );
                model.Subjects = subjects.Select ( s => new SelectListItem
                {
                    Value = s.Id.ToString (),
                    Text = s.SubjectName
                } ).ToList ();

                return View ( model );
            }

            // Create Exam
            var examJson = JsonConvert.SerializeObject ( model.Exam );
            var examContent = new StringContent ( examJson, Encoding.UTF8, "application/json" );
            var examResponse = await _httpClient.PostAsync ( apiBaseUrl, examContent );

            if (!examResponse.IsSuccessStatusCode)
                return View ( model );

            var createdExam = JsonConvert.DeserializeObject<Exam> (
                await examResponse.Content.ReadAsStringAsync ()
            );

            // Assign subjects to exam
            foreach (var subjectId in model.SelectedSubjectIds)
            {
                var examSubject = new ExamSubject
                {
                    ExamId = createdExam.ExamId,
                    SubjectId = subjectId
                };

                var examSubjectJson = JsonConvert.SerializeObject ( examSubject );
                var examSubjectContent = new StringContent ( examSubjectJson, Encoding.UTF8, "application/json" );

                await _httpClient.PostAsync ( _examSubjectApiUrl, examSubjectContent );
            }

            return RedirectToAction ( nameof ( Index ) );
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
        public async Task<IActionResult> DeleteConfirmed ( int examId )
        {
            try
            {
                var response = await _httpClient.DeleteAsync ( $"https://localhost:7230/api/ExamApi/{examId}" );

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction ( nameof ( Index ) );
                }

                var errorDetails = await response.Content.ReadAsStringAsync ();

                // Check for foreign key violation
                if (errorDetails.Contains ( "REFERENCE constraint" ))
                {
                    ViewBag.ErrorMessage = "Unable to delete this exam because it is linked to student marks. Please delete the related marks first.";
                }
                else
                {
                    ViewBag.ErrorMessage = "Unable to delete the exam. An unexpected error occurred.";
                }

                // Redisplay the delete view with error
                return View ( "Delete", new Exam { ExamId = examId } );
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "A server error occurred while trying to delete the exam. Please try again later.";
                return View ( "Delete", new Exam { ExamId = examId } );
            }
        }
    }

    public class ExamCreateViewModel
    {
        public Exam Exam { get; set; } = new Exam ();

        [Display ( Name = "Subjects" )]
        public List<int> SelectedSubjectIds { get; set; } = new List<int> ();

        public List<SelectListItem> Subjects { get; set; } = new List<SelectListItem> ();
    }

}
