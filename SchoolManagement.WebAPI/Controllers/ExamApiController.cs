using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ExamApiController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamApiController ( IExamService examService )
        {
            _examService = examService;
        }

        // GET: api/ExamApi
        [HttpGet]
        public async Task<IActionResult> GetAllExams ( )
        {
            var exams = await _examService.GetAllExamsAsync ();
            return Ok ( exams );
        }

        // GET: api/ExamApi/5
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetExamById ( int id )
        {
            var exam = await _examService.GetExamByIdAsync ( id );
            if (exam == null) return NotFound ( "Exam not found" );

            return Ok ( exam );
        }

        // POST: api/ExamApi
        [HttpPost]
        public async Task<IActionResult> CreateExam ( [FromBody] Exam exam )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest ( ModelState );
            }

            await _examService.AddExamAsync ( exam );
            return CreatedAtAction ( nameof ( GetExamById ), new { id = exam.ExamId }, exam );
        }

        // PUT: api/ExamApi/5
        [HttpPut ( "{id}" )]
        public async Task<IActionResult> UpdateExam ( int id, [FromBody] Exam exam )
        {
            if (id != exam.ExamId)
            {
                return BadRequest ( "Exam ID mismatch" );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest ( ModelState );
            }

            await _examService.UpdateExamAsync ( exam );
            return NoContent ();
        }

        // DELETE: api/ExamApi/5
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> DeleteExam ( int id )
        {
            var exam = await _examService.GetExamByIdAsync ( id );
            if (exam == null) return NotFound ( "Exam not found" );

            await _examService.DeleteExamAsync ( id );
            return NoContent ();
        }

        [HttpGet ( "GetSubjectsByExamId/{examId}" )]
        public async Task<IActionResult> GetSubjectsByExamId ( int examId )
        {
            var subjects = await _examService.GetSubjectsByExamIdAsync ( examId );
            if (subjects == null || !subjects.Any ())
            {
                return NotFound ( "No subjects found for the given exam." );
            }
            return Ok ( subjects );
        }
    }
}
