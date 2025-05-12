using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.UI.Filter;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SchoolManagement.UI.Controllers
{

    public class StudentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly string _classApi;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor injecting dependencies: HttpClient, configuration for API base URLs, and HttpContextAccessor.
        public StudentController ( IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
            _apiBaseUrl = configuration["ApiSettings:StudentApiBaseUrl"];
            _classApi = configuration["ApiSettings:ClassApiBaseUrl"];
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// ADMIN: Show register form
        /// </summary>
        /// <returns></returns>
        [AuthorizeUser ( "Admin" )]
        public async Task<IActionResult> Register ( )
        {
            // Fetch the list of classes from the API
            var classList = await _httpClient.GetStringAsync ( "https://localhost:7230/api/class" );
            var classLists = JsonConvert.DeserializeObject<List<Class>> ( classList );

            // Pass the class list to the ViewBag for use in the dropdown
            ViewBag.ClassList = new SelectList ( classLists, "Id", "ClassName" );

            // Ensure Guardians list has at least one Guardian for binding
            var viewModel = new StudentGuardianViewModel
            {
                Student = new Student (),
                Guardians = new List<Guardian> { new Guardian () }
            };

            return View ( viewModel );
        }


        /// <summary>
        /// POST: Handle registration of student with guardians
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="ProfileImageFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register ( StudentGuardianViewModel viewModel, IFormFile? ProfileImageFile )
        {
            // Check model binding
            if (!ModelState.IsValid)
            {
                await PopulateClassListAsync ();
                var errorMessages = string.Join ( "<br/>", ModelState.Values
                    .SelectMany ( x => x.Errors )
                    .Select ( e => e.ErrorMessage ) );
                TempData["ModelErrors"] = errorMessages;
                return View ( viewModel );
            }

            // Set student role
            viewModel.Student.Role = "Student";

            // Handle profile image
            if (ProfileImageFile != null && ProfileImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream ())
                {
                    await ProfileImageFile.CopyToAsync ( memoryStream );
                    viewModel.Student.ProfileImage = memoryStream.ToArray ();
                }
            }

            // Post to Web API
            var response = await _httpClient.PostAsJsonAsync ( $"{_apiBaseUrl}/register", viewModel );

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Student registered successfully!";
                return RedirectToAction ( "StudentList" );
            }

            // Read error response from API
            var content = await response.Content.ReadAsStringAsync ();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>> ( content );

            if (errorObj != null && errorObj.ContainsKey ( "message" ))
            {
                ModelState.Clear ();
                // Add a general model error to show in ValidationSummary
                ModelState.AddModelError ( string.Empty, errorObj["message"] );
            }

            TempData["ErrorMessage"] = "Registration failed.";
            await PopulateClassListAsync ();
            return View ( viewModel );
        }

        private async Task PopulateClassListAsync ( )
        {
            var classListJson = await _httpClient.GetStringAsync ( "https://localhost:7230/api/class" );
            var classList = JsonConvert.DeserializeObject<List<Class>> ( classListJson );
            ViewBag.ClassList = new SelectList ( classList, "Id", "ClassName" );
        }

        [HttpGet]
        public async Task<IActionResult> CheckStudentIdExists ( int studentId )
        {
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/check-studentid-exists/{studentId}" );

            if (!response.IsSuccessStatusCode)
                return Json ( new { exists = false } );

            var content = await response.Content.ReadAsStringAsync ();
            var result = JsonConvert.DeserializeObject<Dictionary<string, bool>> ( content );

            return Json ( result );
        }

        /// <summary>
        /// ADMIN: Fetch & display list of students.
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [AuthorizeUser ( "Admin" )]
        public async Task<IActionResult> StudentList ( int? classId, string search )
        {
            // Fetch students
            var students = await _httpClient.GetFromJsonAsync<List<Student>> ( _apiBaseUrl );

            // Fetch class list for dropdown
            var classList = await _httpClient.GetFromJsonAsync<List<Class>> ( "https://localhost:7230/api/class" );
            ViewBag.ClassList = new SelectList ( classList, "Id", "ClassName" );

            // Filter students by class
            if (classId.HasValue)
            {
                students = students.Where ( s => s.ClassId == classId.Value ).ToList ();
            }

            // Filter by search (name or student ID)
            if (!string.IsNullOrWhiteSpace ( search ))
            {
                students = students.Where ( s =>
                    (!string.IsNullOrEmpty ( s.FirstName ) && s.FirstName.Contains ( search, StringComparison.OrdinalIgnoreCase )) ||
                    (!string.IsNullOrEmpty ( s.LastName ) && s.LastName.Contains ( search, StringComparison.OrdinalIgnoreCase )) ||
                    s.StudentId.ToString ().Contains ( search, StringComparison.OrdinalIgnoreCase )
                ).ToList ();
            }

            ViewBag.SelectedClassId = classId;
            ViewBag.SearchTerm = search;

            return View ( students );
        }

      
        /// <summary>
        /// ADMIN: Show edit form.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeUser ( "Admin" )]
        public async Task<IActionResult> Edit ( int id )
        {
            var student = await _httpClient.GetFromJsonAsync<Student> ( $"{_apiBaseUrl}/{id}" );
            if (student == null)
                return NotFound ();

            // Fetch guardians from API
            var guardians = await _httpClient.GetFromJsonAsync<List<Guardian>> ( $"{_apiBaseUrl}/{id}/guardians" );

            // Construct the ViewModel
            var viewModel = new StudentGuardianViewModel
            {
                Student = student,
                Guardians = guardians ?? new List<Guardian> ()
            };

            return View ( viewModel );
        }

        /// <summary>
        /// POST: Save student edits.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="student"></param>
        /// <param name="ProfileImageFile"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit ( int id, StudentGuardianViewModel viewModel, IFormFile? ProfileImageFile )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join ( "<br/>", ModelState.Values
                        .SelectMany ( v => v.Errors )
                        .Select ( e => e.ErrorMessage ) );
                    TempData["ErrorMessage"] = errors;
                    return View ( viewModel );
                }

                // Fetch existing student to preserve data like ProfileImage if not re-uploaded
                var existingStudent = await _httpClient.GetFromJsonAsync<Student> ( $"{_apiBaseUrl}/{id}" );
                if (existingStudent == null)
                {
                    TempData["ErrorMessage"] = "Student not found.";
                    return NotFound ();
                }

                if (ProfileImageFile != null && ProfileImageFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream ();
                    await ProfileImageFile.CopyToAsync ( memoryStream );
                    viewModel.Student.ProfileImage = memoryStream.ToArray ();
                }
                else
                {
                    viewModel.Student.ProfileImage = existingStudent.ProfileImage;
                }

                var response = await _httpClient.PutAsJsonAsync ( $"{_apiBaseUrl}/{id}", viewModel );
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Student updated successfully!";
                    return RedirectToAction ( "StudentList" );
                }

                var errorDetails = await response.Content.ReadAsStringAsync ();
                TempData["ErrorMessage"] = $"Update failed: {errorDetails}";
                return View ( viewModel );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unexpected error: {ex.Message}";
                return View ( viewModel );
            }
        }


        /// <summary>
        /// ADMIN: Delete a student.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeUser ( "Admin" )]
        public async Task<IActionResult> Delete ( int id )
        {
            var response = await _httpClient.DeleteAsync ( $"{_apiBaseUrl}/{id}" );

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Student deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete student.";
            }

            return RedirectToAction ( "StudentList" );
        }

        /// <summary>
        /// ADMIN/STUDENT: View student details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeUser ( "Admin", "Student" )]
        public async Task<IActionResult> Details ( int? id )
        {
            var userEmail = HttpContext.Session.GetString ( "UserEmail" );
            var userRole = HttpContext.Session.GetString ( "UserRole" );
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue ( ClaimTypes.NameIdentifier );

            HttpResponseMessage response;

            if (userRole == "Student")
            {
                // Student can only view their own details
                response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/byemail?email={userEmail}" );
            }
            else if (userRole == "Admin")
            {
                if (id == null)
                {
                    return BadRequest ( "Student ID is required." );
                }
                // Admin can view details of any student by ID
                response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}" );
            }
            else
            {
                return Forbid ();
            }

            if (!response.IsSuccessStatusCode)
            {
                return NotFound ();
            }

            var studentJson = await response.Content.ReadAsStringAsync ();
            var student = JsonConvert.DeserializeObject<Student> ( studentJson );

            // Get Guardians
            HttpResponseMessage guardianResponse = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{student.Id}/guardians" );
            List<Guardian> guardians = new List<Guardian> ();

            if (guardianResponse.IsSuccessStatusCode)
            {
                var guardianJson = await guardianResponse.Content.ReadAsStringAsync ();
                guardians = JsonConvert.DeserializeObject<List<Guardian>> ( guardianJson );
            }

            var viewModel = new StudentGuardianViewModel
            {
                Student = student,
                Guardians = guardians
            };

            return View ( viewModel );
        }

        /// <summary>
        /// ADMIN: Show promote form.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeUser ( "Admin" )]
        [HttpGet]
        public async Task<IActionResult> PromoteStudent ( int id )
        {
            // Get the student by ID from the student API
            var studentResponse = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}" );
            if (!studentResponse.IsSuccessStatusCode)
                return NotFound ();

            var studentJson = await studentResponse.Content.ReadAsStringAsync ();
            var student = JsonConvert.DeserializeObject<Student> ( studentJson );

            // Get the list of classes from the class API
            var classResponse = await _httpClient.GetAsync ( _classApi );
            if (!classResponse.IsSuccessStatusCode)
                return BadRequest ( "Failed to load class list" );

            var classJson = await classResponse.Content.ReadAsStringAsync ();
            var classList = JsonConvert.DeserializeObject<List<Class>> ( classJson );

            // Find the index of the current class and get the next class only
            var currentClassIndex = classList.FindIndex ( c => c.Id == student.ClassId );
            if (currentClassIndex >= 0 && currentClassIndex + 1 < classList.Count)
            {
                // Only get the next class
                classList = classList.Skip ( currentClassIndex + 1 ).Take ( 1 ).ToList ();
            }
            else
            {
                // If no next class, handle the case accordingly (e.g., no classes available for promotion)
                classList.Clear ();  // Or you could display an error or message
            }

            ViewBag.ClassList = new SelectList ( classList, "Id", "ClassName" );
            ViewBag.StudentCurrentClass = student.ClassId;

            return View ( student );
        }

        /// <summary>
        /// POST: Promote a student to a new class.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="newClassId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PromoteStudent ( int studentId, int newClassId )
        {
            var response = await _httpClient.PutAsync ( $"{_apiBaseUrl}/promote?studentId={studentId}&newClassId={newClassId}", null );

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to promote student.";
                return RedirectToAction ( "PromoteStudent", new { id = studentId } );
            }

            TempData["SuccessMessage"] = "Student promoted successfully!";
            return RedirectToAction ( "StudentList" );
        }

        private int GetLoggedInStudentId ( )
        {
            // Retrieve the StudentId from the session
            var studentId = HttpContext.Session.GetInt32 ( "StudentId" );

            // Check if the session is empty or the StudentId is null
            if (!studentId.HasValue)
            {
                throw new Exception ( "User is not logged in or session has expired." );
            }

            // Return the StudentId as a non-nullable int
            return studentId.Value;
        }


        [HttpGet]
        public IActionResult ChangePassword ( )
        {
            var studentId = GetLoggedInStudentId ();
            var model = new ChangeStudentPasswordDto { StudentId = studentId };
            return View ( model );
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword ( ChangeStudentPasswordDto model )
        {
            if (string.IsNullOrWhiteSpace ( model.OldPassword ) || string.IsNullOrWhiteSpace ( model.NewPassword ))
            {
                ModelState.AddModelError ( string.Empty, "All fields are required." );
                return View ( model );
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync ( $"{_apiBaseUrl}/change-password", model );

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Password changed successfully!";
                    return View ();
                }

                var error = await response.Content.ReadAsStringAsync ();
                ModelState.AddModelError ( string.Empty, $"Error: {error}" );
                return View ( model );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError ( string.Empty, "An error occurred: " + ex.Message );
                return View ( model );
            }
        }

        public async Task<IActionResult> DownloadStudentPdf ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}/pdf" );

            if (response.IsSuccessStatusCode)
            {
                var pdf = await response.Content.ReadAsByteArrayAsync ();
                return File ( pdf, "application/pdf", $"Student_{id}_Details.pdf" );
            }

            return View ( "Error" );
        }
    }
}
