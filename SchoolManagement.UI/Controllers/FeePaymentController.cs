using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels.FeeViewModel;
using SchoolManagement.UI.Filter;
using System.Net.Http;
using static SchoolManagement.UI.Controllers.ClassSubjectController;

namespace SchoolManagement.UI.Controllers
{
    [AuthorizeUser ( "Admin", "Student")]
    public class FeePaymentController : Controller
    {
        private readonly HttpClient _http;

        public FeePaymentController ( IHttpClientFactory httpClientFactory )
        {
            _http = httpClientFactory.CreateClient ( "AuthorizedClient" );
            _http.BaseAddress = new Uri ( "https://localhost:7230/" );
        }

        public async Task<IActionResult> PaidStudents ( int? classId = null, int? studentId = null )
        {
            var classes = await _http.GetFromJsonAsync<List<ClassViewModel>> ( "api/class" );
            ViewBag.Classes = classes;
            ViewBag.SelectedClassId = classId;
            ViewBag.ClassList = new SelectList ( classes, "ClassId", "ClassName", classId );
            ViewBag.SelectedStudentId = studentId; // <-- pass to view

            var summaryList = new List<StudentFeeSummaryViewModel> ();

            if (classId.HasValue)
            {
                var payments = await _http.GetFromJsonAsync<List<FeePaymentViewModel>> ( $"api/FeePaymentsApi/class/{classId}" );
                var allPayments = await _http.GetFromJsonAsync<List<FeePaymentViewModel>> ( "/api/FeePaymentsApi" );

                var latestClassMap = allPayments
                    .GroupBy ( p => p.StudentId )
                    .Select ( g => new
                    {
                        StudentId = g.Key,
                        LatestClassId = g.OrderByDescending ( p => p.PaymentDate ).First ().ClassId
                    } )
                    .ToDictionary ( x => x.StudentId, x => x.LatestClassId );

                var grouped = payments
                    .Where ( p => latestClassMap.ContainsKey ( p.StudentId ) && latestClassMap[p.StudentId] == p.ClassId )
                    .GroupBy ( p => new { p.StudentId, p.ClassId } )
                    .ToList ();

                foreach (var group in grouped)
                {
                    var paymentList = group.OrderBy ( p => p.PaymentDate ).ToList ();
                    var first = paymentList.First ();
                    decimal totalPaid = paymentList.Sum ( p => p.AmountPaid );
                    var classInfo = classes.FirstOrDefault ( c => c.ClassId == group.Key.ClassId );
                    decimal totalFee = classInfo?.TotalFee ?? 0;
                    decimal pendingAmount = totalFee - totalPaid;

                    var summary = new StudentFeeSummaryViewModel
                    {
                        StudentId = first.StudentId,
                        StudentName = first.StudentName,
                        ClassId = first.ClassId,
                        ClassName = first.ClassName,
                        Payments = paymentList,
                        TotalFee = totalFee,
                        PendingAmount = pendingAmount,
                        IsComplete = pendingAmount <= 0
                    };

                    // Filter here
                    if (!studentId.HasValue || studentId == summary.StudentId)
                        summaryList.Add ( summary );
                }
            }

            return View ( summaryList );
        }

        public async Task<IActionResult> UnpaidStudents ( int classId = 0, int? studentId = null )
        {
            // Fetch all class options for the dropdown
            var classes = await _http.GetFromJsonAsync<List<ClassViewModel>> ( "api/class" );
            ViewBag.Classes = classes;

            ViewBag.SelectedClassId = classId;

            // If no class selected, return empty view
            if (classId == 0)
                return View ( new List<StudentFeeSummaryDto> () );

            // Get all students who were unpaid at some point
            var allUnpaid = await _http.GetFromJsonAsync<List<UnpaidStudentViewModel>> (
                $"api/FeePaymentsApi/unpaid/class/{classId}" );

            // Optional filtering for a specific student
            if (studentId.HasValue)
                allUnpaid = allUnpaid.Where ( u => u.StudentId == studentId.Value ).ToList ();

            // Prepare list for valid summaries with pending amount
            var summaries = new List<StudentFeeSummaryDto> ();

            foreach (var student in allUnpaid)
            {
                // Fetch full summary of fee info per student
                var summary = await _http.GetFromJsonAsync<StudentFeeSummaryDto> (
                    $"api/FeePaymentsApi/summary/student/{student.StudentId}" );

                // Only add students who still have pending amount
                if (summary != null && summary.CurrentStatus.PendingAmount > 0)
                {
                    summaries.Add ( summary );
                }
            }

            return View ( summaries );
        }

        [HttpPost]
        public async Task<IActionResult> PayFee ( UnpaidStudentViewModel model )
        {
            var payment = new
            {
                StudentId = model.StudentId,
                ClassId = model.ClassId,
                FeeType = model.FeeType,
                AmountPaid = model.AmountPaid,
                PaymentDate = DateTime.Now
            };

            var response = await _http.PostAsJsonAsync ( "api/FeePaymentsApi", payment );

            if (response.IsSuccessStatusCode)
            {
                // Redirect to PaidStudents view, filtered by classId and studentId
                return RedirectToAction ( "PaidStudents", new { classId = model.ClassId, studentId = model.StudentId } );
            }

            ModelState.AddModelError ( "", "Failed to process payment." );
            return View ( "UnpaidStudents" );
        }


        [AuthorizeUser ( "Student" )]
        public async Task<IActionResult> MyPayments ( int? classId = null )
        {
            var studentId = GetLoggedInStudentId ();

            // 1. Get all payments
            var paymentsResponse = await _http.GetAsync ( $"api/FeePaymentsApi/student/{studentId}" );
            if (!paymentsResponse.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load your payment history.";
                return View ( "Error" );
            }

            var allPayments = await paymentsResponse.Content.ReadFromJsonAsync<List<FeePaymentDto>> ();
            if (allPayments == null || !allPayments.Any ())
            {
                ViewBag.NoPayments = "No fee payments made yet.";
                return View ( new StudentPaymentDetailsViewModel () );
            }

            // 2. Selected class for dropdown (can be old class)
            var selectedClassId = classId ?? allPayments
                .OrderByDescending ( p => p.PaymentDate )
                .FirstOrDefault ()?.ClassId ?? 0;

            var selectedClassPayments = allPayments
                .Where ( p => p.ClassId == selectedClassId )
                .ToList ();

            // 3. Current/latest class for summary
            var currentClassId = allPayments
                .OrderByDescending ( p => p.PaymentDate )
                .FirstOrDefault ()?.ClassId ?? 0;

            var statusResponse = await _http.GetAsync (
                $"api/FeePaymentsApi/status/student/{studentId}?classId={currentClassId}" );

            StudentFeeStatusDto? status = null;
            if (statusResponse.IsSuccessStatusCode)
            {
                status = await statusResponse.Content.ReadFromJsonAsync<StudentFeeStatusDto> ();
            }

            // 4. Dropdown list
            var classOptions = allPayments
                .Select ( p => new { p.ClassId, p.ClassName } )
                .Distinct ()
                .ToList ();

            ViewBag.ClassOptions = classOptions;
            ViewBag.SelectedClassId = selectedClassId;

            // 5. Handle if no payments exist for selected class
            if (!selectedClassPayments.Any ())
            {
                ViewBag.NoPayments = "No payments found for the selected class.";
            }

            return View ( new StudentPaymentDetailsViewModel
            {
                Payments = selectedClassPayments,
                Status = status ?? new ()
            } );
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

        [AuthorizeUser ( "Student", "Admin" )]
        public async Task<IActionResult> DownloadMyPaymentsPdf ( int? studentId = null, int? classId = null )
        {
            int effectiveStudentId;

            // Session check for student login
            var sessionStudentId = HttpContext.Session.GetInt32 ( "StudentId" );

            if (sessionStudentId.HasValue)
            {
                effectiveStudentId = sessionStudentId.Value;

                if (studentId.HasValue && studentId != effectiveStudentId)
                {
                    return Forbid ();
                }
            }
            else if (studentId.HasValue)
            {
                // Admin access
                effectiveStudentId = studentId.Value;
            }
            else
            {
                TempData["Error"] = "Student ID is required to generate PDF.";
                return RedirectToAction ( "PaidStudents" );
            }

            // Build API URL with optional classId
            var query = classId.HasValue ? $"?classId={classId}" : "";
            var response = await _http.GetAsync ( $"api/FeePaymentsApi/pdf/student/{effectiveStudentId}{query}" );

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not generate PDF.";
                return RedirectToAction ( sessionStudentId.HasValue ? "MyPayments" : "PaidStudents" );
            }

            var pdfBytes = await response.Content.ReadAsByteArrayAsync ();
            return File ( pdfBytes, "application/pdf", $"FeePayments_{effectiveStudentId}_{DateTime.Now:yyyyMMdd}.pdf" );
        }

    }

    public class UnpaidStudentViewModel
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public decimal TotalFee { get; set; }

        // NEW
        public decimal TotalPaid { get; set; }

        // For form submission
        public string FeeType { get; set; } = "Offline";
        public decimal AmountPaid { get; set; }

        // Updated to reflect actual unpaid
        public decimal PendingAmount => TotalFee - TotalPaid;
    }


    public class FeePaymentViewModel
    {
        public int PaymentId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public int ClassId { get; set; }
        public string? ClassName { get; set; }
        public string FeeType { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }

        // New properties for status
        public decimal PendingAmount { get; set; } = 0;
        public DateTime? DueDate { get; set; } = null;
        public bool IsComplete { get; set; } = false;
    }

    public class StudentFeeSummaryViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public List<FeePaymentViewModel> Payments { get; set; } = new ();
        public decimal TotalPaid => Payments.Sum ( p => p.AmountPaid );

        // New explicitly set properties from backend
        public decimal TotalFee { get; set; }
        public decimal PendingAmount { get; set; }
        public bool IsComplete { get; set; }
    }

    public class ClassViewModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public decimal TotalFee { get; set; }
    }

   

}
