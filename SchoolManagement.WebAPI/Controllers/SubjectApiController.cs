using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class SubjectApiController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectApiController ( ISubjectService subjectService )
        {
            _subjectService = subjectService;
        }

        // GET: api/SubjectApi
        [HttpGet]
        public IActionResult GetAllSubjects ( )
        {
            var subjects = _subjectService.GetAllSubjects ();
            return Ok ( subjects );
        }

        // GET: api/SubjectApi/5
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetSubjectById ( int id )
        {
            var subject =  _subjectService.GetSubjectById ( id );
            if (subject == null)
            {
                return NotFound ();
            }
            return Ok ( subject );
        }

        // POST: api/SubjectApi
        [HttpPost]
        public async Task<IActionResult> CreateSubject ( [FromBody] Subject subject )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest ( ModelState );
            }
            // Log the EmployeeId to check if it's coming from the request
            Console.WriteLine ( $"Received EmployeeId: {subject.EmployeeId}" );
            _subjectService.AddSubject ( subject );
            return CreatedAtAction ( nameof ( GetSubjectById ), new { id = subject.Id }, subject );
        }

        // PUT: api/SubjectApi/5
        [HttpPut ( "{id}" )]
        public async Task<IActionResult> UpdateSubject ( int id, [FromBody] Subject subject )
        {
            if (id != subject.Id)
            {
                return BadRequest ( "Subject ID mismatch" );
            }

            var existingSubject =  _subjectService.GetSubjectById ( id );
            if (existingSubject == null)
            {
                return NotFound ();
            }

             _subjectService.UpdateSubject ( subject );
            return NoContent ();
        }

        // DELETE: api/SubjectApi/5
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> DeleteSubject ( int id )
        {
            var subject =  _subjectService.GetSubjectById ( id );
            if (subject == null)
            {
                return NotFound ();
            }

             _subjectService.DeleteSubject ( id );
            return NoContent ();
        }
    }
}
