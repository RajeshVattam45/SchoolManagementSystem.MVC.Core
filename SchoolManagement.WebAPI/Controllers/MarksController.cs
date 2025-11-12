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

        [HttpPost ( "register-marks" )]
        public async Task<IActionResult> RegisterMarks ( [FromBody] Marks marks )
        {
            if (marks == null)
                return BadRequest ( "Invalid marks data." );

            if (!marks.MarksObtained.HasValue || !marks.MaxMarks.HasValue)
                return BadRequest ( "MarksObtained and MaxMarks are required." );

            if (marks.MarksObtained < 0)
                return BadRequest ( "Obtained marks cannot be negative." );

            if (marks.MarksObtained > marks.MaxMarks)
                return BadRequest ( "Obtained marks cannot exceed maximum marks." );

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

        // GET: api/marks/{id}
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetMarksById ( int id )
        {
            var mark = await _service.GetMarksEditByIdAsync ( id );
            if (mark == null)
                return NotFound ();

            return Ok ( mark );
        }

        // PUT: api/marks/edit/{id}
        [HttpPut ( "edit/{id}" )]
        public async Task<IActionResult> UpdateMarks ( int id, [FromBody] Marks updatedMarks )
        {
            if (id != updatedMarks.MarkId)
                return BadRequest ( "ID mismatch." );

            var result = await _service.UpdateMarksAsync ( updatedMarks );
            if (result == null)
                return NotFound ();

            return Ok ( result );
        }

        // DELETE: api/marks/delete/{id}
        [HttpDelete ( "delete/{id}" )]
        public async Task<IActionResult> DeleteMarks ( int id )
        {
            var deleted = await _service.DeleteMarksAsync ( id );
            if (!deleted)
                return NotFound ();

            return NoContent ();
        }

        [HttpGet ( "download-pdf" )]
        public async Task<IActionResult> DownloadMarksPdf ( int studentId, string examType )
        {
            var pdfBytes = await _service.GenerateStudentMarksPdfAsync ( studentId, examType );

            if (pdfBytes == null || !pdfBytes.Any ())
                return NotFound ( "No marks data found for the selected exam." );

            return File ( pdfBytes, "application/pdf", $"Student_{studentId}_{examType}_Marks.pdf" );
        }
    }
}
