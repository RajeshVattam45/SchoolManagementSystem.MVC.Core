using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.UI.Filter;
using System.Net;

namespace SchoolManagement.UI.Controllers
{
    [AuthorizeUser ( "Admin", "Teacher", "Student" )]
    public class ExamScheduleMvcController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _apiBaseUrl;

        // Constructor: injects IHttpClientFactory and IConfiguration.
        public ExamScheduleMvcController ( IHttpClientFactory httpClientFactory, IConfiguration configuration )
        {
            // Create a named HttpClient instance configured in Program.cs
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );

            // Set serializer options to ignore object reference cycles and case-insensitive matching.
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true
            };

            // Retrieve API base URL from configuration (fallback to default).
            _apiBaseUrl = configuration["ApiSettings:ExamScheduleBaseApiUrl"];
        }

        public async Task<IActionResult> Index ( int? classId, int? year )
        {
            var userRole = HttpContext.Session.GetString ( "UserRole" );
            var userId = HttpContext.Session.GetInt32 ( "StudentId" );

            var response = await _httpClient.GetAsync ( _apiBaseUrl );
            if (!response.IsSuccessStatusCode)
                return View ( new List<ExamSchedule> () );

            var schedules = await response.Content.ReadFromJsonAsync<IEnumerable<ExamSchedule>> ( _jsonOptions );

            // Extract academic years from schedules
            var academicYears = schedules
                .Select ( e => e.ExamDate.Year )
                .Distinct ()
                .OrderByDescending ( y => y )
                .ToList ();

            ViewBag.AcademicYears = new SelectList ( academicYears, year );

            if (userRole == "Student" && userId.HasValue)
            {
                var studentResponse = await _httpClient.GetAsync ( $"https://localhost:7230/api/Students/{userId}" );
                if (!studentResponse.IsSuccessStatusCode)
                    return View ( new List<ExamSchedule> () );

                var student = await studentResponse.Content.ReadFromJsonAsync<Student> ( _jsonOptions );
                if (student == null)
                    return View ( new List<ExamSchedule> () );

                schedules = schedules.Where ( s => s.ClassId == student.ClassId );

                if (year.HasValue)
                    schedules = schedules.Where ( s => s.ExamDate.Year == year.Value );

                ViewBag.ClassName = student.Class?.ClassName ?? "Your Class";
                ViewBag.StudentClassId = student.ClassId;
                ViewBag.IsStudent = true;

                return View ( schedules );
            }
            else
            {
                var classResponse = await _httpClient.GetAsync ( "https://localhost:7230/api/Class" );
                var classList = classResponse.IsSuccessStatusCode
                    ? await classResponse.Content.ReadFromJsonAsync<IEnumerable<Class>> ( _jsonOptions )
                    : new List<Class> ();

                ViewBag.ClassList = new SelectList ( classList, "Id", "ClassName", classId );
                ViewBag.IsStudent = false;

                if (classId.HasValue)
                {
                    schedules = schedules.Where ( s => s.ClassId == classId.Value );

                    // 🆕 Set selected class name for display in UI
                    var selectedClass = classList.FirstOrDefault ( c => c.Id == classId.Value );
                    ViewBag.ClassName = selectedClass?.ClassName ?? $"Class ID: {classId}";
                }

                if (year.HasValue)
                    schedules = schedules.Where ( s => s.ExamDate.Year == year.Value );

                return View ( schedules );
            }
        }

        // GET: ExamScheduleMvc/Details/5
        // View detailed information about a specific schedule.
        public async Task<IActionResult> Details ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}" );
            if (response.IsSuccessStatusCode)
            {
                var schedule = await response.Content.ReadFromJsonAsync<ExamSchedule> ( _jsonOptions );
                return View ( schedule );
            }
            return NotFound ();
        }

        // GET: ExamScheduleMvc/Create
        // Show form to create a new exam schedule.
        public async Task<IActionResult> Create ( )
        {
            var exams = await _httpClient.GetFromJsonAsync<IEnumerable<Exam>> ( "https://localhost:7230/api/ExamApi" );
            var classes = await _httpClient.GetFromJsonAsync<IEnumerable<Class>> ( "https://localhost:7230/api/Class" );

            ViewBag.ExamList = new SelectList ( exams, "ExamId", "ExamName" );
            //var enrichedExams = exams.Select ( e => new {
            //    e.ExamId,
            //    DisplayName = $"{e.ExamType?.ExamTypeName ?? "Unknown"} - {e.ExamName}"
            //} );
            //ViewBag.ExamList = new SelectList ( enrichedExams, "ExamId", "DisplayName" );

            ViewBag.ClassList = new SelectList ( classes, "Id", "ClassName" );
            ViewBag.SubjectList = new SelectList ( new List<SelectListItem> () );

            return View ();
        }


        // POST: ExamScheduleMvc/Create
        // Submit form to create a new exam schedule.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create ( ExamSchedule schedule )
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var response = await _httpClient.PostAsJsonAsync ( _apiBaseUrl, schedule, _jsonOptions );
        //        //if (response.IsSuccessStatusCode)
        //        //    return RedirectToAction ( nameof ( Index ) );
        //        if (response.IsSuccessStatusCode)
        //            return RedirectToAction ( nameof ( Index ), new { classId = schedule.ClassId } );

        //    }

        //    // Repopulate dropdowns for redisplay
        //    var exams = await _httpClient.GetFromJsonAsync<IEnumerable<Exam>> ( "https://localhost:7230/api/ExamApi" );
        //    var classes = await _httpClient.GetFromJsonAsync<IEnumerable<Class>> ( "https://localhost:7230/api/Class" );

        //    // ViewBag.ExamList = new SelectList ( exams, "ExamId", "ExamName" );
        //    var enrichedExams = exams.Select ( e => new
        //    {
        //        e.ExamId,
        //        DisplayName = $"{e.ExamType?.ExamTypeName ?? "Unknown"} - {e.ExamName}"
        //    } );
        //    ViewBag.ExamList = new SelectList ( enrichedExams, "ExamId", "DisplayName" );

        //    ViewBag.ClassList = new SelectList ( classes, "Id", "ClassName" );
        //    ViewBag.SubjectList = new SelectList ( new List<SelectListItem> () );

        //    return View ( schedule );
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( ExamSchedule schedule )
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync ( _apiBaseUrl, schedule, _jsonOptions );

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction ( nameof ( Index ), new { classId = schedule.ClassId } );
                    }

                    // Read full error response
                    var errorContent = await response.Content.ReadAsStringAsync ();

                    // Extract meaningful part (first line or specific pattern)
                    var firstLine = errorContent.Split ( '\n', StringSplitOptions.RemoveEmptyEntries ).FirstOrDefault ();

                    // You could use regex if needed, but simple string trimming also works
                    var meaningfulError = firstLine;

                    // Optional: strip out exception type if present
                    if (meaningfulError?.Contains ( ":" ) == true)
                        meaningfulError = meaningfulError.Substring ( meaningfulError.IndexOf ( ':' ) + 1 ).Trim ();

                    ModelState.AddModelError ( string.Empty, meaningfulError ?? "An unexpected error occurred." );
                }
                catch (Exception ex)
                {
                    // Extract only the message, not stack trace
                    ModelState.AddModelError ( string.Empty, ex.Message );
                }
            }

            // Re-populate dropdowns for redisplay
            var exams = await _httpClient.GetFromJsonAsync<IEnumerable<Exam>> ( "https://localhost:7230/api/ExamApi" );
            var classes = await _httpClient.GetFromJsonAsync<IEnumerable<Class>> ( "https://localhost:7230/api/Class" );

            var enrichedExams = exams.Select ( e => new
            {
                e.ExamId,
                DisplayName = $"{e.ExamType?.ExamTypeName ?? "Unknown"} - {e.ExamName}"
            } );
            ViewBag.ExamList = new SelectList ( enrichedExams, "ExamId", "DisplayName" );
            ViewBag.ClassList = new SelectList ( classes, "Id", "ClassName" );

            return View ( schedule );
        }

        public class ValidationErrorResponse
        {
            public Dictionary<string, string[]> Errors { get; set; } = new ();
        }

        // GET: ExamScheduleMvc/Edit/5
        // Load the exam schedule to edit.
        public async Task<IActionResult> Edit ( int id )
        {
            // Repopulate dropdowns for redisplay
            var exams = await _httpClient.GetFromJsonAsync<IEnumerable<Exam>> ( "https://localhost:7230/api/ExamApi" );
            var classes = await _httpClient.GetFromJsonAsync<IEnumerable<Class>> ( "https://localhost:7230/api/Class" );

            ViewBag.ExamList = new SelectList ( exams, "ExamId", "ExamName" );
            ViewBag.ClassList = new SelectList ( classes, "Id", "ClassName" );
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}" );
            if (response.IsSuccessStatusCode)
            {
                var schedule = await response.Content.ReadFromJsonAsync<ExamSchedule> ( _jsonOptions );
                return View ( schedule );
            }
            return NotFound ();
        }

        // POST: ExamScheduleMvc/Edit/5
        // Submit form to update existing exam schedule.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit ( int id, ExamSchedule schedule )
        {
            if (id != schedule.ScheduleId)
                return BadRequest ();

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync ( $"{_apiBaseUrl}/{id}", schedule, _jsonOptions );
                if (response.IsSuccessStatusCode)
                    return RedirectToAction ( nameof ( Index ) );
            }
            return View ( schedule );
        }

        // GET: ExamScheduleMvc/Delete/5
        // Show confirmation page for deleting a schedule.
        // GET or POST: ExamScheduleMvc/Delete/5
        // GET: ExamScheduleMvc/Delete/5
        public async Task<IActionResult> Delete ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}" );
            if (response.IsSuccessStatusCode)
            {
                var schedule = await response.Content.ReadFromJsonAsync<ExamSchedule> ( _jsonOptions );
                return View ( schedule ); // Show confirmation view
            }
            return NotFound ();
        }

        // POST: ExamScheduleMvc/Delete/5
        // Confirm and delete the selected schedule.
        // POST: ExamScheduleMvc/Delete/5
        [HttpPost, ActionName ( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            var response = await _httpClient.DeleteAsync ( $"{_apiBaseUrl}/{id}" );
            return RedirectToAction ( nameof ( Index ) );
        }

        // GET: ExamScheduleMvc/GetSubjectsByExamId
        // Fetch subjects dynamically based on selected exam (AJAX).
        [HttpGet]
        public async Task<IActionResult> GetSubjectsByExamId ( int examId )
        {
            var response = await _httpClient.GetAsync ( $"https://localhost:7230/api/ExamApi/GetSubjectsByExamId/{examId}" );

            if (!response.IsSuccessStatusCode)
                return NotFound ( "Could not fetch subjects." );

            var subjects = await response.Content.ReadFromJsonAsync<IEnumerable<Subject>> ( _jsonOptions );
            return Json ( subjects );
        }
    }
}
