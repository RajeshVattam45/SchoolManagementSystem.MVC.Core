using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ExamTypeApiController : ControllerBase
    {
        private readonly IExamTypeService _examTypeService;

        public ExamTypeApiController ( IExamTypeService examTypeService )
        {
            _examTypeService = examTypeService;
        }

        // GET: api/ExamTypeApi
        [HttpGet]
        public async Task<IActionResult> GetAll ( )
        {
            var examTypes = await _examTypeService.GetAllExamTypesAsync ();
            return Ok ( examTypes );
        }

        // GET: api/ExamTypeApi/5
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetById ( int id )
        {
            var examType = await _examTypeService.GetExamTypeByIdAsync ( id );
            if (examType == null)
                return NotFound ();
            return Ok ( examType );
        }

        // POST: api/ExamTypeApi
        [HttpPost]
        public async Task<IActionResult> Create ( [FromBody] ExamType examType )
        {
            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            await _examTypeService.AddExamTypeAsync ( examType );
            // Return a 201 Created response with the location header
            return CreatedAtAction ( nameof ( GetById ), new { id = examType.Id }, examType );
        }

        // PUT: api/ExamTypeApi/5
        [HttpPut ( "{id}" )]
        public async Task<IActionResult> Update ( int id, [FromBody] ExamType examType )
        {
            if (id != examType.Id)
                return BadRequest ( "ID mismatch" );

            if (!ModelState.IsValid)
                return BadRequest ( ModelState );

            await _examTypeService.UpdateExamTypeAsync ( examType );
            return NoContent ();
        }

        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> Delete ( int id )
        {
            try
            {
                await _examTypeService.DeleteExamTypeAsync ( id );
                return NoContent ();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest ( new { message = ex.Message } );
            }
            catch (Exception ex)
            {
                return StatusCode ( 500, new { message = "Something went wrong while deleting." } );
            }
        }
    }
}
