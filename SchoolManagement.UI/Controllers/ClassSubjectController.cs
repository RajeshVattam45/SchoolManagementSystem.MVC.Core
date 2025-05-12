using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.UI.Filter;
using System.Net.Http;
using System.Net.Http.Json;

namespace SchoolManagement.UI.Controllers
{
    public class ClassSubjectController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://localhost:7230/api/ClassSubject/";
        private const string ClassApiUrl = "https://localhost:7230/api/class";

        public ClassSubjectController ( IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
        }

        [HttpGet]
        public async Task<IActionResult> Assign ( int? classId = null )
        {
            var data = await _httpClient.GetFromJsonAsync<BulkAssignViewModel> ( $"{ApiBaseUrl}bulk-data" );

            ViewBag.SelectedClassId = classId;

            if (classId.HasValue)
            {
                var assignedSubjects = await _httpClient.GetFromJsonAsync<List<int>> (
                    $"{ApiBaseUrl}assigned-subject-ids/{classId}"
                );

                if (assignedSubjects != null)
                {
                    if (!data.AssignedSubjects.ContainsKey ( classId.Value ))
                        data.AssignedSubjects[classId.Value] = new List<int> ();

                    data.AssignedSubjects[classId.Value] = assignedSubjects;
                }
            }

            return View ( data );
        }

        [HttpPost]
        public async Task<IActionResult> Assign ( int classId, List<int> selectedSubjects )
        {
            Console.WriteLine ( $"Payload: ClassId = {classId}, SelectedSubjects = {string.Join ( ", ", selectedSubjects )}" );

            var payload = new
            {
                classId,
                selectedSubjectIds = selectedSubjects
            };

            var response = await _httpClient.PostAsJsonAsync ( $"{ApiBaseUrl}assign", payload );

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Subjects assigned successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to assign subjects.";
                string errorContent = await response.Content.ReadAsStringAsync ();
                Console.WriteLine ( $"Error: {errorContent}" );
            }

            return RedirectToAction ( "Assign", new { classId } );
        }

        [HttpGet]
        public async Task<IActionResult> AssignedList ( int? classId )
        {
            var classList = await _httpClient.GetFromJsonAsync<List<ClassViewModel>> ( ClassApiUrl );
            ViewBag.ClassList = classList;
            ViewBag.SelectedClassId = classId;

            List<AssignedSubjectsViewModel> result = new ();

            if (classId.HasValue)
            {
                try
                {
                    result = await _httpClient.GetFromJsonAsync<List<AssignedSubjectsViewModel>> (
                   $"{ApiBaseUrl}class/{classId}"
                    );
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    result = new List<AssignedSubjectsViewModel> ();
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "An error occurred while fetching assigned subjects.";
                }


                if (result == null || result.Count == 0)
                {
                    TempData["Error"] = "No subjects assigned to this class.";
                }
            }

            return View ( result );
        }

        [AuthorizeUser ( "Student" )]
        // Stuudent class
        [HttpGet]
        public async Task<IActionResult> StudentSubjects ( )
        {
            // Get ClassId from Session
            var classIdString = HttpContext.Session.GetString ( "ClassId" );

            if (string.IsNullOrEmpty ( classIdString ) || !int.TryParse ( classIdString, out int classId ))
            {
                TempData["Error"] = "Class information not found in session.";
                return View ( new List<string> () );
            }

            // Fetch assigned subject IDs for the student's class
            var assignedSubjectIds = await _httpClient.GetFromJsonAsync<List<int>> (
                $"{ApiBaseUrl}assigned-subject-ids/{classId}" );

            var allSubjects = await _httpClient.GetFromJsonAsync<BulkAssignViewModel> (
                $"{ApiBaseUrl}bulk-data" );

            // Filter only assigned subjects
            var assignedSubjects = allSubjects.Subjects
                .Where ( s => assignedSubjectIds.Contains ( s.Id ) )
                .Select ( s => s.SubjectName )
                .ToList ();

            ViewBag.ClassName = allSubjects.Classes
                .FirstOrDefault ( c => c.ClassId == classId )?.ClassName ?? "Unknown";

            return View ( assignedSubjects );
        }

        public class ClassViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
