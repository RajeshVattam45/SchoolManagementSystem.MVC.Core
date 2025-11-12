using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.UI.Filter;
using System.Text;

namespace SchoolManagement.UI.Controllers
{
    [AuthorizeUser ( "Admin", "Teacher", "Student")]
    public class MarksController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7230/api/Marks";

        public MarksController ( IHttpClientFactory httpClientFactory )
        {
            _httpClient = httpClientFactory.CreateClient ( "AuthorizedClient" );
        }

        //[HttpGet]
        //public async Task<IActionResult> DisplayAllMarks ( )
        //{
        //    var apiUrl = "https://localhost:7230/api/Marks/details";

        //    try
        //    {
        //        var allMarks = await _httpClient.GetFromJsonAsync<List<MarksDetailsDto>> ( apiUrl );

        //        if (allMarks != null)
        //            return View ( allMarks );
        //        else
        //            return View ( new List<MarksDetailsDto> () );
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Error = "Failed to load marks data: " + ex.Message;
        //        return View ( new List<MarksDetailsDto> () );
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> DisplayAllMarks ( int? classId = null, string? examTypeName = null, string? subjectName = null )
        {
            var apiUrl = "https://localhost:7230/api/Marks/details";

            try
            {
                var allMarks = await _httpClient.GetFromJsonAsync<List<MarksDetailsDto>> ( apiUrl );

                var filteredMarks = allMarks;

                if (classId.HasValue)
                    filteredMarks = filteredMarks.Where ( m => m.ClassId == classId.Value ).ToList ();

                if (!string.IsNullOrWhiteSpace ( examTypeName ))
                    filteredMarks = filteredMarks
                        .Where ( m => string.Equals ( m.ExamTypeName, examTypeName, StringComparison.OrdinalIgnoreCase ) )
                        .ToList ();

                if (!string.IsNullOrWhiteSpace ( subjectName ))
                    filteredMarks = filteredMarks
                        .Where ( m => string.Equals ( m.SubjectName, subjectName, StringComparison.OrdinalIgnoreCase ) )
                        .ToList ();

                return View ( filteredMarks );
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Failed to load marks data: " + ex.Message;
                return View ( new List<MarksDetailsDto> () );
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchByStudent ( string query )
        {
            if (string.IsNullOrWhiteSpace ( query ))
                return RedirectToAction ( "DisplayAllMarks" );

            try
            {
                var allMarks = await _httpClient.GetFromJsonAsync<List<MarksDetailsDto>> ( "https://localhost:7230/api/Marks/details" );

                var filtered = allMarks?.Where ( m =>
                    (!string.IsNullOrEmpty ( m.StudentFullName ) && m.StudentFullName.Contains ( query, StringComparison.OrdinalIgnoreCase )) ||
                    m.StudentFullName.Contains ( query, StringComparison.OrdinalIgnoreCase ) ||
                    int.TryParse ( query, out var id ) && m.StudentFullName.Contains ( query )
                ).ToList ();
                ViewBag.Query = query;
                return View ( "DisplayAllMarks", filtered ?? new List<MarksDetailsDto> () );
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Failed to search marks: " + ex.Message;
                return View ( "DisplayAllMarks", new List<MarksDetailsDto> () );
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register ( int? classId = null, int? examTypeId = null, int? subjectId = null )
        {
            var classResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/class" );
            var classes = JsonConvert.DeserializeObject<List<Class>> ( classResponse );
            classes.Insert ( 0, new Class { Id = 0, ClassName = "Select Class" } );
            ViewBag.Classes = new SelectList ( classes, "Id", "ClassName", classId ?? 0 );

            var examTypeResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamTypeApi" );
            var examTypes = JsonConvert.DeserializeObject<List<ExamType>> ( examTypeResponse );
            examTypes.Insert ( 0, new ExamType { Id = 0, ExamTypeName = "Select Exam" } );
            ViewBag.ExamTypes = new SelectList ( examTypes, "Id", "ExamTypeName", examTypeId ?? 0 );

            var subjectsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/SubjectApi" );
            var subjects = JsonConvert.DeserializeObject<List<Subject>> ( subjectsResponse );
            subjects.Insert ( 0, new Subject { Id = 0, SubjectName = "Select Subject" } );
            ViewBag.Subjects = new SelectList ( subjects, "Id", "SubjectName", subjectId ?? 0 );

            // Only continue if all filters are applied
            if (classId != null && examTypeId != null)
            {
                var examTypeResponseData = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamTypeApi" );
                var examTypesData = JsonConvert.DeserializeObject<List<ExamType>> ( examTypeResponse );

                var selectedExamType = examTypesData.FirstOrDefault ( et => et.Id == examTypeId );
                int maxMarksFromExamType = selectedExamType?.MaxMarks ?? 100;
                

                var examsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamApi" );
                var allExams = JsonConvert.DeserializeObject<List<Exam>> ( examsResponse );
                var filteredExams = allExams.Where ( e => e.ExamTypeId == examTypeId ).ToList ();
                var examsDict = filteredExams.ToDictionary ( e => e.ExamId, e => e.ExamName );

                var scheduleResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamScheduleApi" );
                var schedules = JsonConvert.DeserializeObject<List<ExamSchedule>> ( scheduleResponse );

                // Filter schedules based on class and optional subject
                var matchingSchedules = schedules
                    .Where ( s => s.ClassId == classId &&
                                filteredExams.Any ( e => e.ExamId == s.ExamId ) &&
                                (!subjectId.HasValue || s.SubjectId == subjectId) )
                    .ToList ();

                var studentsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/students" );
                var students = JsonConvert.DeserializeObject<List<Student>> ( studentsResponse )?
                    .Where ( s => s.ClassId == classId ).ToList ();

                var allMarksResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/Marks/details" );
                var allMarks = JsonConvert.DeserializeObject<List<Marks>> ( allMarksResponse );

                var subjectsDict = JsonConvert.DeserializeObject<List<Subject>> (
                    await _httpClient.GetStringAsync ( "https://localhost:7230/api/SubjectApi" ) )
                    .ToDictionary ( s => s.Id, s => s.SubjectName );

                var model = new List<MarksEntryViewModel> ();

                foreach (var student in students)
                {
                    foreach (var schedule in matchingSchedules)
                    {
                        var subjId = schedule.SubjectId;
                        var checkUrl = $"https://localhost:7230/api/Marks/exists?studentId={student.Id}&examId={schedule.ExamId}&subjectId={subjId}&classId={classId}";
                        var existsResponse = await _httpClient.GetStringAsync ( checkUrl );
                        var exists = JsonConvert.DeserializeObject<bool> ( existsResponse );

                        int? obtainedMarks = null;
                        int? maxMarks = maxMarksFromExamType;
                        int? markId = null;

                        if (exists)
                        {
                            var mark = allMarks.FirstOrDefault ( m =>
                                m.StudentId == student.Id &&
                                m.ExamId == schedule.ExamId &&
                                m.SubjectId == subjId &&
                                m.ClassId == classId );

                            if (mark != null)
                            {
                                obtainedMarks = mark.MarksObtained;
                                maxMarks = mark.MaxMarks;
                                markId = mark.MarkId;
                            }
                        }

                        model.Add ( new MarksEntryViewModel
                        {
                            StudentId = student.Id,
                            StudentName = $"{student.FirstName} {student.LastName}",
                            ExamId = schedule.ExamId.Value,
                            ExamName = examsDict.ContainsKey ( schedule.ExamId.Value ) ? examsDict[schedule.ExamId.Value] : "N/A",
                            SubjectId = subjId.Value,
                            SubjectName = subjectsDict.ContainsKey ( subjId.Value ) ? subjectsDict[subjId.Value] : "N/A",
                            ClassId = classId.Value,
                            IsExistingMark = exists,
                            MarksObtained = (int?)obtainedMarks,
                            MaxMarks = (int?)maxMarks,
                            MarkId = markId
                        } );
                    }
                }

                if (model.Any ( m => m.IsExistingMark ))
                    ViewBag.Info = "Some or all marks already exist. Existing entries are read-only.";

                ViewBag.SelectedClassId = classId;
                ViewBag.SelectedExamTypeId = examTypeId;
                ViewBag.SelectedSubjectId = subjectId;

                return View ( model );
            }
            ViewBag.SelectedClassId = classId;
            ViewBag.SelectedExamTypeId = examTypeId;
            ViewBag.SelectedSubjectId = subjectId;

            return View ( new List<MarksEntryViewModel> () );
        }

        //[HttpPost]
        //public async Task<IActionResult> Register ( List<MarksEntryViewModel> marksList )
        //{
        //    if (marksList == null || !marksList.Any ())
        //        return BadRequest ( "No data submitted." );

        //    var errors = new List<string> ();

        //    foreach (var entry in marksList)
        //    {
        //        // Skip rows with no mark entered
        //        if (!entry.MarksObtained.HasValue || !entry.MaxMarks.HasValue)
        //            continue;

        //        if (entry.MarksObtained > entry.MaxMarks)
        //        {
        //            errors.Add ( $"Student: {entry.StudentName} - Obtained marks cannot exceed maximum marks." );
        //            continue;
        //        }

        //        var checkUrl = $"https://localhost:7230/api/Marks/exists?studentId={entry.StudentId}&examId={entry.ExamId}&subjectId={entry.SubjectId}&classId={entry.ClassId}";
        //        var existsResponse = await _httpClient.GetStringAsync ( checkUrl );
        //        var alreadyExists = JsonConvert.DeserializeObject<bool> ( existsResponse );

        //        if (alreadyExists)
        //            continue;

        //        var mark = new Marks
        //        {
        //            StudentId = entry.StudentId,
        //            ExamId = entry.ExamId,
        //            SubjectId = entry.SubjectId,
        //            ClassId = entry.ClassId,
        //            MarksObtained = entry.MarksObtained.Value,
        //            MaxMarks = entry.MaxMarks.Value
        //        };

        //        var content = new StringContent ( JsonConvert.SerializeObject ( mark ), Encoding.UTF8, "application/json" );
        //        var response = await _httpClient.PostAsync ( "https://localhost:7230/api/Marks/register-marks", content );

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var errorMessage = await response.Content.ReadAsStringAsync ();
        //            errors.Add ( $"Student: {entry.StudentName} - Error: {errorMessage}" );
        //        }
        //    }

        //    if (errors.Any ())
        //    {
        //        TempData["MarkErrors"] = errors;
        //        return RedirectToAction ( "Register", new
        //        {
        //            classId = marksList.First ().ClassId,
        //            examTypeId = marksList.First ().ExamId,
        //            subjectId = marksList.First ().SubjectId
        //        } );
        //    }

        //    TempData["SuccessMessage"] = "Marks saved/updated successfully.";
        //    return RedirectToAction ( "Register", new
        //    {
        //        classId = marksList.First ().ClassId,
        //        examTypeId = marksList.First ().ExamId,
        //        subjectId = marksList.First ().SubjectId
        //    } );
        //}

        [HttpPost]
        public async Task<IActionResult> Register ( List<MarksEntryViewModel> marksList )
        {
            if (marksList == null || !marksList.Any ())
                return BadRequest ( "No data submitted." );

            var errors = new List<string> ();

            foreach (var entry in marksList)
            {
                if (!entry.MarksObtained.HasValue || !entry.MaxMarks.HasValue)
                    continue;

                if (entry.MarksObtained > entry.MaxMarks)
                {
                    errors.Add ( $"Student: {entry.StudentName} - Obtained marks cannot exceed maximum marks." );
                    continue;
                }

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
                    var errorMessage = await response.Content.ReadAsStringAsync ();
                    errors.Add ( $"Student: {entry.StudentName} - Error: {errorMessage}" );
                }
            }

            // Fetch exam type name and subject name dynamically for redirect
            var examTypeName = await GetExamTypeNameByExamId ( marksList.First ().ExamId );
            var subjectName = await GetSubjectNameById ( marksList.First ().SubjectId );

            if (errors.Any ())
            {
                TempData["MarkErrors"] = errors;
                return RedirectToAction ( "Register", new
                {
                    classId = marksList.First ().ClassId,
                    examTypeId = marksList.First ().ExamId,
                    subjectId = marksList.First ().SubjectId
                } );
            }

            TempData["SuccessMessage"] = "Marks saved/updated successfully.";
            return RedirectToAction ( "DisplayAllMarks", new
            {
                classId = marksList.First ().ClassId,
                examTypeName = examTypeName,
                subjectName = subjectName
            } );
        }

        private async Task<string> GetExamTypeNameByExamId ( int examId )
        {
            var examApiResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamApi" );
            var exams = JsonConvert.DeserializeObject<List<Exam>> ( examApiResponse );
            var exam = exams?.FirstOrDefault ( e => e.ExamId == examId );

            if (exam == null) return "";

            var examTypeApiResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/ExamTypeApi" );
            var examTypes = JsonConvert.DeserializeObject<List<ExamType>> ( examTypeApiResponse );
            return examTypes?.FirstOrDefault ( et => et.Id == exam.ExamTypeId )?.ExamTypeName ?? "";
        }

        private async Task<string> GetSubjectNameById ( int subjectId )
        {
            var subjectsResponse = await _httpClient.GetStringAsync ( "https://localhost:7230/api/SubjectApi" );
            var subjects = JsonConvert.DeserializeObject<List<Subject>> ( subjectsResponse );
            return subjects?.FirstOrDefault ( s => s.Id == subjectId )?.SubjectName ?? "";
        }


        [HttpGet]
        public async Task<IActionResult> Edit ( int id )
        {
            var response = await _httpClient.GetAsync ( $"{_apiBaseUrl}/{id}" );
            if (!response.IsSuccessStatusCode)
                return NotFound ();

            var json = await response.Content.ReadAsStringAsync ();
            var mark = JsonConvert.DeserializeObject<Marks> ( json );
            return View ( mark );
        }

        [HttpPost]
        public async Task<IActionResult> Edit ( int id, Marks updatedMarks )
        {
            if (id != updatedMarks.MarkId)
                return BadRequest ();

            var content = new StringContent ( JsonConvert.SerializeObject ( updatedMarks ), Encoding.UTF8, "application/json" );
            var response = await _httpClient.PutAsync ( $"{_apiBaseUrl}/edit/{id}", content );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError ( "", "Failed to update mark." );
                return View ( updatedMarks );
            }

            return RedirectToAction ( "DisplayAllMarks" );
        }

        [HttpGet]
        public async Task<IActionResult> Delete ( int id )
        {
            var response = await _httpClient.DeleteAsync ( $"{_apiBaseUrl}/delete/{id}" );

            if (!response.IsSuccessStatusCode)
                return NotFound ();

            return RedirectToAction ( "DisplayAllMarks" );
        }

        [HttpGet]
        [AuthorizeUser ( "Student" )]
        public async Task<IActionResult> MyMarks ( )
        {
            int? studentId = HttpContext.Session.GetInt32 ( "StudentId" );

            if (studentId == null)
                return RedirectToAction ( "Login", "Account" );

            var response = await _httpClient.GetAsync ( $"https://localhost:7230/api/Marks/student/by/{studentId}" );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to retrieve marks.";
                return View ( new List<MarksDetailsDto> () );
            }

            var content = await response.Content.ReadAsStringAsync ();
            var allMarks = JsonConvert.DeserializeObject<List<MarksDetailsDto>> ( content );

            if (allMarks == null || !allMarks.Any ())
            {
                ViewBag.NoMarks = "Marks are not yet registered.";
                return View ( new List<MarksDetailsDto> () );
            }

            // Determine the latest classId (assumed to be the highest)
            var latestClassId = allMarks
                .OrderByDescending ( m => m.ClassId )
                .First ()
                .ClassId;

            // Filter only current class marks
            var currentClassMarks = allMarks
                .Where ( m => m.ClassId == latestClassId )
                .ToList ();

            if (!currentClassMarks.Any ())
            {
                ViewBag.NoMarks = "Marks are not yet registered for your current class.";
                return View ( new List<MarksDetailsDto> () );
            }

            return View ( currentClassMarks );
        }

        private int GetLoggedInStudentId ( )
        {
            var studentId = HttpContext.Session.GetInt32 ( "StudentId" );

            if (!studentId.HasValue)
                throw new Exception ( "User is not logged in or session has expired." );

            return studentId.Value;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf ( string examType )
        {
            int studentId = GetLoggedInStudentId ();

            // Construct the full API URL with studentId and encoded examType
            string encodedExamType = Uri.EscapeDataString ( examType );
            string requestUrl = $"{_apiBaseUrl}/download-pdf?studentId={studentId}&examType={encodedExamType}";

            try
            {
                var response = await _httpClient.GetAsync ( requestUrl );

                if (!response.IsSuccessStatusCode)
                {
                    // You can log response.StatusCode or reason if needed
                    return Content ( "❌ Failed to download PDF. Please try again." );
                }

                var pdfBytes = await response.Content.ReadAsByteArrayAsync ();

                // Return the file with a friendly name
                return File ( pdfBytes, "application/pdf", $"marks-{examType}.pdf" );
            }
            catch (Exception ex)
            {
                // Optional: log ex.Message
                return Content ( $"❌ An error occurred: {ex.Message}" );
            }
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
        public string ExamName { get; set; }

        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

        public int ClassId { get; set; }

        //[Range ( 0, int.MaxValue, ErrorMessage = "Marks must be non-negative." )]
        public int? MarksObtained { get; set; }

        //[Range ( 1, int.MaxValue, ErrorMessage = "Max marks must be at least 1." )]
        public int? MaxMarks { get; set; }

        public bool IsExistingMark { get; set; }
        public int? MarkId { get; set; }
    }

}
