using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;
using System.Text;

namespace SchoolManagement.UI.Controllers
{
    public class MarksController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7230/api/marks";

        public MarksController ( IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
        }

        [HttpGet]
        public async Task<IActionResult> DisplayAllMarks ( )
        {
            var apiUrl = "https://localhost:7230/api/Marks/details";

            try
            {
                var allMarks = await _httpClient.GetFromJsonAsync<List<MarksDetailsDto>> ( apiUrl );

                if (allMarks != null)
                    return View ( allMarks );
                else
                    return View ( new List<MarksDetailsDto> () );
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Failed to load marks data: " + ex.Message;
                return View ( new List<MarksDetailsDto> () );
            }
        }

        [HttpGet]
        public async Task<IActionResult> RegisterMarks ( )
        {
            //// Get all Students
            //var studentsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/students" );
            //var students = JsonConvert.DeserializeObject<List<Student>> ( studentsResponse );
            //ViewBag.Students = students;

            // Get available classes
            var classResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/class" );
            var classes = JsonConvert.DeserializeObject<List<Class>> ( classResponse );
            ViewBag.Classes = new SelectList ( classes, "Id", "ClassName" );

            // Get All Exams
            var examsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamApi" );
            var exams = JsonConvert.DeserializeObject<List<Exam>> ( examsResponse );
            ViewBag.Exams = exams;

            // Get exam types
            var examTypesResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamTypeApi" );
            var examTypes = JsonConvert.DeserializeObject<List<ExamType>> ( examTypesResponse );
            ViewBag.ExamType = new SelectList ( examTypes, "ExamTypeId", "ExamTypeName" );

            // Get Subjects
            var subjectsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/SubjectApi" );
            var subjects = JsonConvert.DeserializeObject<List<Subject>> ( subjectsResponse );

            // Filter out nulls and incomplete records
            var validSubjects = subjects
                ?.Where ( s => s != null && s.Id != 0 && !string.IsNullOrWhiteSpace ( s.SubjectName ) )
                .ToList () ?? new List<Subject> ();
            //ViewBag.Subject = new SelectList ( validSubjects, "SubjectId", "SubjectName" );
            ViewBag.Subject = new SelectList ( validSubjects, "Id", "SubjectName" );

            return View ();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterMarks ( Marks marks )
        {
            if (!ModelState.IsValid)
            {
                // Log all the errors to the console or in a log file
                foreach (var key in ModelState.Keys)
                {
                    var value = ModelState[key];
                    if (value.Errors.Count > 0)
                    {
                        // Log error for each key (field) and its error message(s)
                        Console.WriteLine ( $"{key}: {string.Join ( ", ", value.Errors.Select ( e => e.ErrorMessage ) )}" );
                    }
                }

                TempData["Error"] = "Please fill all required fields.";
                return View ( marks );
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync ( "https://localhost:7230/api/Marks/register-marks", marks );

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Marks registered successfully!";
                    return RedirectToAction ( nameof ( DisplayAllMarks ) );
                }
                else
                {
                    TempData["Error"] = "Failed to register marks.";
                    return View ( marks );
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return View ( marks );
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsByClass ( int classId )
        {
            try
            {
                var studentsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/students" );
                var students = JsonConvert.DeserializeObject<List<Student>> ( studentsResponse );

                var filtered = students
                    .Where ( s => s.ClassId == classId )
                    .Select ( s => new {
                        s.Id,
                        s.FirstName,
                        s.LastName
                    } );

                return Json ( filtered );
            }
            catch (Exception ex)
            {
                return BadRequest ( "Error fetching students: " + ex.Message );
            }
        }


        // new get method for register marks
        [HttpGet]
        public async Task<IActionResult> Register ( int? classId = null, int? examId = null, int? subjectId = null )
        {
            var classResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/class" );
            var classes = JsonConvert.DeserializeObject<List<Class>> ( classResponse );
            ViewBag.Classes = new SelectList ( classes, "Id", "ClassName" );

            var examsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamApi" );
            var exams = JsonConvert.DeserializeObject<List<Exam>> ( examsResponse );
            ViewBag.Exams = new SelectList ( exams, "ExamId", "ExamName" );

            var subjectsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/SubjectApi" );
            var subjects = JsonConvert.DeserializeObject<List<Subject>> ( subjectsResponse );
            ViewBag.Subjects = new SelectList ( subjects, "Id", "SubjectName" );

            if (classId != null && examId != null && subjectId != null)
            {
                var scheduleResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamScheduleApi" );
                var schedules = JsonConvert.DeserializeObject<List<ExamSchedule>> ( scheduleResponse );
                var validSchedule = schedules.FirstOrDefault ( s =>
                    s.ClassId == classId && s.ExamId == examId && s.SubjectId == subjectId );

                if (validSchedule == null)
                {
                    ViewBag.Error = "No scheduled exam found for the selected class, subject, and exam.";
                    return View ( new List<MarksEntryViewModel> () );
                }

                var studentsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/students" );
                var students = JsonConvert.DeserializeObject<List<Student>> ( studentsResponse )?
                    .Where ( s => s.ClassId == classId ).ToList ();

                var model = new List<MarksEntryViewModel> ();

                foreach (var student in students)
                {
                    var checkUrl = $"https://localhost:7230/api/Marks/exists?studentId={student.Id}&examId={examId}&subjectId={subjectId}&classId={classId}";
                    var existsResponse = await _httpClient.GetStringAsync ( checkUrl );
                    var exists = JsonConvert.DeserializeObject<bool> ( existsResponse );

                    model.Add ( new MarksEntryViewModel
                    {
                        StudentId = student.Id,
                        StudentName = student.FirstName + " " + student.LastName,
                        ExamId = examId.Value,
                        SubjectId = subjectId.Value,
                        ClassId = classId.Value,
                        IsExistingMark = exists
                    } );
                }

                if (model.Any ( m => m.IsExistingMark ))
                {
                    ViewBag.Info = "Some or all marks have already been registered. Existing entries are read-only.";
                }

                return View ( model );
            }

            return View ( new List<MarksEntryViewModel> () );
        }

        [HttpPost]
        public async Task<IActionResult> Register ( List<MarksEntryViewModel> marksList )
        {
            if (marksList == null || !marksList.Any ())
                return BadRequest ( "No data submitted." );

            foreach (var entry in marksList)
            {
                var checkUrl = $"https://localhost:7230/api/Marks/exists?studentId={entry.StudentId}&examId={entry.ExamId}&subjectId={entry.SubjectId}&classId={entry.ClassId}";
                var existsResponse = await _httpClient.GetStringAsync ( checkUrl );
                var alreadyExists = JsonConvert.DeserializeObject<bool> ( existsResponse );

                if (alreadyExists)
                    continue;

                var mark = new Marks
                {
                    StudentId = entry.StudentId,
                    ExamId = entry.ExamId,
                    SubjectId = entry.SubjectId,
                    ClassId = entry.ClassId,
                    MarksObtained = entry.MarksObtained.Value,
                    MaxMarks = entry.MaxMarks.Value
                };

                var content = new StringContent ( JsonConvert.SerializeObject ( mark ), Encoding.UTF8, "application/json" );
                var response = await _httpClient.PostAsync ( "https://localhost:7230/api/Marks/register-marks", content );

                if (!response.IsSuccessStatusCode)
                {
                    // Log or handle error if needed
                }
            }

            return RedirectToAction ( "DisplayAllMarks" );
        }
    }
    public class ClassSubject
    {
        public string ClassName { get; set; }
        public List<string> Subjects { get; set; }
    }

    public class MarksEntryViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int ExamId { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }
        public int? MarksObtained { get; set; }
        public int? MaxMarks { get; set; }

        public bool IsExistingMark { get; set; } // NEW
    }
}
