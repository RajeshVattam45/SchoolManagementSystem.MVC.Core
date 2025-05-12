using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ExamSubjectApiController : ControllerBase
    {
        private readonly IExamSubjectService _service;

        public ExamSubjectApiController ( IExamSubjectService service )
        {
            _service = service;
        }

        // GET: api/ExamSubjects
        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var examSubjects = await _service.GetAllAsync ();
            return Ok ( examSubjects );
        }

        // GET: api/ExamSubjects/5
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> Get ( int id )
        {
            var examSubject = await _service.GetByIdAsync ( id );
            if (examSubject == null)
                return NotFound ();

            return Ok ( examSubject );
        }

        // POST: api/ExamSubjects
        [HttpPost]
        public async Task<IActionResult> Post ( [FromBody] ExamSubject model )
        {
            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            await _service.AddAsync ( model );
            return Ok ( model );
        }

        // PUT: api/ExamSubjects/5
        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Put ( int id, [FromBody] ExamSubject model )
        {
            if (id != model.Id)
                return BadRequest ();

            await _service.UpdateAsync ( model );
            return Ok ();
        }

        // DELETE: api/ExamSubjects/5
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            await _service.DeleteAsync ( id );
            return Ok ();
        }

        // GET: api/ExamSubjects/exam/5
        [HttpGet ( "exam/{examId}" )]
        public async Task<IActionResult> GetByExam ( int examId )
        {
            var examSubjects = await _service.GetByExamIdAsync ( examId );
            var subjects = examSubjects.Select ( es => es.Subject );
            return Ok ( subjects );
        }
    }
}
