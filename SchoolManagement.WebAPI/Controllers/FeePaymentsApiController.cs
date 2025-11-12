using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Services;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels.FeeViewModel;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route ( "api/[controller]" )]
    public class FeePaymentsApiController : ControllerBase
    {
        private readonly IFeePaymentService _service;

        public FeePaymentsApiController ( IFeePaymentService service )
        {
            _service = service;
        }

        private FeePaymentDto MapToDto ( FeePayment fp )
        {
            return new FeePaymentDto
            {
                PaymentId = fp.PaymentId,
                StudentId = fp.StudentId,
                StudentName = $"{fp.Student?.FirstName} {fp.Student?.LastName}",
                ClassId = fp.ClassId,
                ClassName = fp.Class?.ClassName,
                FeeType = fp.FeeType,
                AmountPaid = fp.AmountPaid,
                PaymentDate = fp.PaymentDate
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var payments = await _service.GetAllPaymentsAsync ();
            return Ok ( payments );
        }

        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var payment = await _service.GetPaymentByIdAsync ( id );
            if (payment == null) return NotFound ();
            return Ok ( MapToDto ( payment ) );
        }

        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] FeePayment payment )
        {
            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            await _service.AddPaymentAsync ( payment );
            return CreatedAtAction ( nameof ( GetById ), new { id = payment.PaymentId }, null );
        }

        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] FeePayment payment )
        {
            if (id != payment.PaymentId)
                return BadRequest ();

            await _service.UpdatePaymentAsync ( payment );
            return NoContent ();
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            await _service.DeletePaymentAsync ( id );
            return NoContent ();
        }

        [HttpGet ( "student/{studentId}" )]
        public async Task<IActionResult> GetByStudent ( int studentId )
        {
            var payments = await _service.GetPaymentsByStudentIdAsync ( studentId );
            var dtoList = payments.Select ( MapToDto ).ToList ();
            return Ok ( dtoList );
        }

        [HttpGet ( "class/{classId}" )]
        public async Task<IActionResult> GetByClass ( int classId )
        {
            var payments = await _service.GetPaymentsByClassIdAsync ( classId );
            var dtoList = payments.Select ( MapToDto ).ToList ();
            return Ok ( dtoList );
        }

        [HttpGet ( "unpaid/class/{classId}" )]
        public async Task<IActionResult> GetUnpaidByClass ( int classId )
        {
            var unpaidStudents = await _service.GetUnpaidStudentsByClassAsync ( classId );
            return Ok ( unpaidStudents );
        }

        [HttpGet ( "status/student/{studentId}" )]
        public async Task<ActionResult<StudentFeeStatusDto>> GetStudentFeeStatus ( int studentId )
        {
            var status = await _service.GetStudentFeeStatusAsync ( studentId );
            if (status == null)
                return NotFound ();

            return Ok ( status );
        }

        [HttpGet ( "summary/student/{studentId}" )]
        public async Task<ActionResult<StudentFeeSummaryDto>> GetStudentFeeSummary ( int studentId )
        {
            var summary = await _service.GetStudentFeeSummaryAsync ( studentId );
            if (summary == null)
                return NotFound ();

            return Ok ( summary );
        }

        [HttpGet ( "pdf/student/{studentId}" )]
        public async Task<IActionResult> DownloadStudentPaymentPdf ( int studentId, [FromQuery] int? classId = null )
        {
            var payments = await _service.GetPaymentsByStudentIdAsync ( studentId );

            if (classId.HasValue)
            {
                payments = payments
                    .Where ( p => p.ClassId == classId.Value )
                    .ToList ();
            }

            //var status = await _service.GetStudentFeeStatusAsync ( studentId );
            var status = await _service.GetStudentFeeStatusAsync ( studentId, classId );

            if (status == null)
                return NotFound ( "Student not found or has no payments." );

            if (classId.HasValue && payments.Any ())
            {
                var latestClass = payments
                    .OrderByDescending ( p => p.PaymentDate )
                    .FirstOrDefault ();

                status.ClassId = latestClass?.ClassId ?? status.ClassId;
                status.ClassName = latestClass?.ClassId.ToString () ?? status.ClassName;

                status.TotalPaid = payments.Sum ( p => p.AmountPaid );
                var pending = status.TotalFee - status.TotalPaid;
                status.IsOverdue = DateTime.Today > status.DueDate && pending > 0;
                status.PenaltyAmount = status.IsOverdue ? pending * 0.05m : 0;
                status.PendingAmount = pending + status.PenaltyAmount;
            }

            var model = new StudentPaymentDetailsViewModel
            {
                Payments = payments.Select ( p => new FeePaymentDto
                {
                    PaymentId = p.PaymentId,
                    StudentId = p.StudentId,
                    StudentName = $"{p.Student?.FirstName} {p.Student?.LastName}",
                    ClassId = p.ClassId,
                    ClassName = p.Class?.ClassName ?? "",
                    FeeType = p.FeeType,
                    AmountPaid = p.AmountPaid,
                    PaymentDate = p.PaymentDate
                } ).ToList (),
                Status = status
            };

            var pdfBytes = _service.GenerateStudentPaymentPdf ( model );
            return File ( pdfBytes, "application/pdf", $"FeePayments_{studentId}_{DateTime.Now:yyyyMMdd}.pdf" );
        }
    }
}
