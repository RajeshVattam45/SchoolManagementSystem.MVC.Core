using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolManagement.Application.Services;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly IStudentClassHistoryService _studentClassHistoryService;

        public ClassController ( IClassService classService, IStudentClassHistoryService studentClassHistoryService)
        {
            _classService = classService;
            _studentClassHistoryService = studentClassHistoryService;
        }

        // Get all classes
        [HttpGet]
        public IActionResult GetAllClasses ( )
        {
            var classes = _classService.GetAllClasses ();
            return Ok ( classes );
        }

        // Get class by ID
        [HttpGet ( "{id}" )]
        public IActionResult GetClassById ( int id )
        {
            var cls = _classService.GetClassById ( id );
            if (cls == null)
                return NotFound ( $"Class with ID {id} not found." );

            return Ok ( cls );
        }

        // Add new class
        [HttpPost]
        public IActionResult AddClass ( [FromBody] Class cls )
        {
            if (cls == null)
                return BadRequest ( "Class data is required." );

            _classService.AddClass ( cls );
            return CreatedAtAction ( nameof ( GetClassById ), new { id = cls.Id }, cls );
        }

        // Update existing class
        [HttpPut ( "{id}" )]
        public IActionResult UpdateClass ( int id, [FromBody] Class cls )
        {
            var existingClass = _classService.GetClassById ( id );
            if (existingClass == null)
                return NotFound ( $"Class with ID {id} not found." );

            _classService.UpdateClass ( cls );
            return NoContent ();
        }

        // Delete class by ID
        [HttpDelete ( "{id}" )]
        public IActionResult DeleteClass ( int id )
        {
            var existingClass = _classService.GetClassById ( id );
            if (existingClass == null)
                return NotFound ( $"Class with ID {id} not found." );

            _classService.DeleteClass ( id );
            return NoContent ();
        }

        [HttpGet ( "student/{studentId}/class-history" )]
        public async Task<IActionResult> GetStudentClassHistory ( int studentId )
        {
            var history = await _studentClassHistoryService.GetClassHistoryAsync ( studentId );

            if (history == null || !history.Any ())
                return NotFound ( "No class history found." );

            // This avoids circular reference issues
            var json = JsonConvert.SerializeObject ( history, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            } );

            return Content ( json, "application/json" );
        }
    }
}
