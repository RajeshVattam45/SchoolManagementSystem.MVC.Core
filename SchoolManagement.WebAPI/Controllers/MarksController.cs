using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.Services;

namespace SchoolManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route ( "api/[controller]" )]
    public class MarksController : ControllerBase
    {
        private readonly IMarksService _service;
        public MarksController ( IMarksService service )
        {
            _service = service;
        }

        [HttpGet ( "details" )]
        public async Task<IActionResult> GetAllMarksDetails ( )
        {
            var results = await _service.GetAllMarksDetailsAsync ();
            return Ok ( results );
        }

        [HttpPost("register-marks")]
        public async Task<IActionResult> RegisterMarks ( [FromBody] Marks marks )
        {
            if (marks == null)
                return BadRequest ( "Invalid marks data" );

            var result = await _service.RegisterMarksAsync ( marks );
            return CreatedAtAction ( nameof ( GetMarksByStudent ), new { studentId = result.StudentId }, result );
        }

        [HttpGet ( "student/by/{studentId}" )]
        public async Task<IActionResult> GetMarksByStudent ( int studentId )
        {
            var marks = await _service.GetStudentMarksAsync ( studentId );
            return Ok ( marks );
        }

        [HttpGet ( "exists" )]
        public async Task<IActionResult> CheckMarksExists (
    int studentId, int examId, int subjectId, int classId )
        {
            var exists = await _service.CheckIfMarksExistAsync ( studentId, examId, subjectId, classId );
            return Ok ( exists );
        }
    }
}
