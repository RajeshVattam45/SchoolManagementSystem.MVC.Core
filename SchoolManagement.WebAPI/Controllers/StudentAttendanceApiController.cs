using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class StudentAttendanceApiController : ControllerBase
    {
        private readonly IStudentAttendanceService _studentAttendanceService;

        public StudentAttendanceApiController ( IStudentAttendanceService studentAttendanceService )
        {
            _studentAttendanceService = studentAttendanceService;
        }

        // GET: api/StudentAttendanceApi
        [HttpGet]
        public async Task<IActionResult> GetStudentAttendance ( )
        {
            var attendance = await _studentAttendanceService.GetGroupedStudentAttendanceAsync ();
            return Ok ( attendance );
        }

        // POST: api/StudentAttendanceApi
        [HttpPost]
        public async Task<IActionResult> RegisterAttendance ( [FromBody] StudentAttendanceDto model )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest ( ModelState );
            }

            await _studentAttendanceService.RegisterAttendanceAsync ( model.StudentId, model.Status, model.Date );
            return Ok ( new { Message = "Attendance recorded successfully" } );
        }

        // PUT: api/StudentAttendanceApi
        [HttpPut]
        public async Task<IActionResult> UpdateAttendance ( [FromBody] StudentAttendanceDto model )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest ( ModelState );
            }

            var result = await _studentAttendanceService.UpdateAttendanceStatusAsync ( model.StudentId, model.Status, model.Date );
            if (!result)
            {
                return NotFound ( "Attendance record not found." );
            }

            return Ok ( new { Message = "Attendance updated successfully" } );
        }

        [HttpGet ( "byemail" )]
        public async Task<IActionResult> GetAttendanceByEmail ( [FromQuery] string email )
        {
            var attendanceRecords = await _studentAttendanceService.GetAttendancesByStudentEmailAsync ( email );
            if (attendanceRecords == null || !attendanceRecords.Any ())
            {
                return NotFound ( "No attendance records found for the provided email." );
            }
            return Ok ( attendanceRecords );
        }
    }
}
