using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using SchoolManagement.UI.Filter;

namespace SchoolManagement.UI.Controllers
{
   [AuthorizeUser ( "Admin", "Teacher", "Student" )]
    public class StudentAttendanceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7230/api/StudentAttendanceApi";

        public StudentAttendanceController ( IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
        }


            // ============================================
            // ADMIN: View all attendance records grouped
            // ============================================
        [AuthorizeUser ( "Admin", "Teacher" )]
        public async Task<IActionResult> Index ( DateTime? startDate, DateTime? endDate )
        {
            var from = startDate ?? DateTime.Today.AddDays ( -7 );
            var to = endDate ?? DateTime.Today;

            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}?startDate={from:yyyy-MM-dd}&endDate={to:yyyy-MM-dd}" );
            var attendanceList = new List<StudentAttendanceGroupedViewModel> ();

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync ();
                attendanceList = JsonConvert.DeserializeObject<List<StudentAttendanceGroupedViewModel>> ( jsonString );
            }

            // Load class list
            var classResponse = await _httpClient.GetAsync ( "https://localhost:7230/api/class" );
            var allClasses = new List<string> ();
            if (classResponse.IsSuccessStatusCode)
            {
                var classJson = await classResponse.Content.ReadAsStringAsync ();
                var classList = JsonConvert.DeserializeObject<List<ClassDto>> ( classJson );
                allClasses = classList
                    .Where ( c => !string.IsNullOrEmpty ( c.ClassName ) )
                    .Select ( c => c.ClassName! )
                    .ToList ();
            }

            ViewBag.AllClasses = allClasses;
            ViewBag.StartDate = from;
            ViewBag.EndDate = to;

            return View ( attendanceList );
        }


        // ============================================
        // ADMIN: Update student attendance
        // ============================================
        [AuthorizeUser ( "Admin", "Teacher" )]
        [HttpPost]
        public async Task<IActionResult> UpdateAttendance ( [FromBody] StudentAttendanceDto model )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest ( ModelState );
            }

            // Convert DateTime.Today to DateOnly for valid comparison
            var today = DateOnly.FromDateTime ( DateTime.Today );
            var allowedPastDate = today.AddDays ( -7 );

            if (model.Date > today)
            {
                return BadRequest ( "Cannot update attendance for future dates." );
            }

            if (model.Date < allowedPastDate)
            {
                return BadRequest ( "Cannot update attendance older than 7 days." );
            }

            var content = new StringContent ( JsonConvert.SerializeObject ( model ), Encoding.UTF8, "application/json" );
            var response = await _httpClient.PutAsync ( _apiBaseUrl, content );

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest ( "Error updating attendance" );
            }

            return Ok ( new { Message = "Attendance updated successfully" } );
        }

        // ====================================================
        // STUDENT: View individual student attendance by email
        // ====================================================
        [AuthorizeUser ( "Student" )]
        [HttpGet]
        public async Task<IActionResult> StudentIndividualAttendance ( )
        {
            // Retrieve logged-in user's email from session.
            var userEmail = HttpContext.Session.GetString ( "UserEmail" );

            if (string.IsNullOrEmpty ( userEmail ))
            {
                ViewBag.Error = "User email not found. Please log in again.";
                return View ();
            }

            // API endpoint to get attendance by email.
            var endpoint = $"{_apiBaseUrl}/byemail?email={userEmail}";
            var response = await _httpClient.GetAsync ( endpoint );

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync ();

                try
                {
                    // Deserialize to a list, because the API returns an array.
                    var attendanceRecords = JsonConvert.DeserializeObject<List<DailyAttendance>> ( jsonString );

                    return View ( attendanceRecords );
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine ( ex.Message );
                    ViewBag.Error = "There was an error processing your attendance data. Please try again later.";
                    return View ();
                }
            }
            else
            {
                ViewBag.Error = "No attendance records found for the logged-in user.";
                return View ();
            }
        }
    }
}
