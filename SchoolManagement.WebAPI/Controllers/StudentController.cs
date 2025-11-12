using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Services;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;
using SchoolManagement.Services;
using SchoolManagementSystem.Data;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/students" )]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMarksService _marksService;
        private readonly IClassService _classService;

        public StudentController ( IStudentService studentService, IMarksService marksService, IClassService classService )
        {
            _studentService = studentService;
            _marksService = marksService;
            _classService = classService;
        }

        [HttpPost ( "register" )]
        public async Task<IActionResult> RegisterStudent ( [FromBody] StudentGuardianViewModel model )
        {
            try
            {
                await _studentService.RegisterStudentWithGuardiansAsync ( model );
                return Ok ( new { message = "Student registered successfully!" } );
            }
            catch (ArgumentException argEx)
            {
                return BadRequest ( new { message = argEx.Message } );
            }
            catch (Exception ex)
            {
                return BadRequest ( new { message = "Failed to register student.", error = ex.Message } );
            }
        }


        // Get All Students
        [Produces ( "application/json" )]
        [HttpGet]
        public async Task<IActionResult> GetStudents ( )
        {
            var students = await _studentService.GetAllStudentsAsync ();
            return Ok ( students );
        }

        // Get Student By ID
        [HttpGet ( "{id}" )]
        public async Task<IActionResult> GetStudentById ( int id )
        {
            var student = await _studentService.GetStudentByIdAsync ( id );
            if (student == null) return NotFound ( new { message = "Student not found!" } );

            return Ok ( student );
        }

        [HttpGet ( "{id}/guardians" )]
        public async Task<IActionResult> GetGuardiansByStudentId ( int id )
        {
            var guardians = await _studentService.GetGuardiansByStudentIdAsync ( id );
            if (guardians == null || !guardians.Any ())
            {
                return NotFound ( "No guardians found for the specified student." );
            }

            return Ok ( guardians );
        }


        [HttpPut ( "{id}" )]
        public async Task<IActionResult> UpdateStudent ( int id, [FromBody] StudentGuardianViewModel viewModel )
        {
            if (viewModel == null || viewModel.Student == null)
                return BadRequest ( new { message = "Invalid student data." } );

            var isUpdated = await _studentService.UpdateStudentAsync ( id, viewModel );
            return isUpdated
                ? Ok ( new { message = "Student updated successfully!" } )
                : BadRequest ( new { message = "Failed to update student." } );
        }


        // Delete Student
        [HttpDelete ( "{id}" )]
        public async Task<IActionResult> DeleteStudent ( int id )
        {
            var isDeleted = await _studentService.DeleteStudentAsync ( id );
            if (isDeleted)
                return Ok ( new { message = "Student deleted successfully!" } );

            return BadRequest ( new { message = "Failed to delete student." } );
        }

        [HttpPut ( "promote" )]
        public async Task<IActionResult> PromoteStudent ( [FromQuery] int studentId, [FromQuery] int newClassId )
        {
            try
            {
                await _studentService.PromoteStudentAsync ( studentId, newClassId );
                return Ok ( new { message = "Student promoted successfully!" } );
            }
            catch (Exception ex)
            {
                return BadRequest ( new { message = "Failed to promote student.", error = ex.Message } );
            }
        }

        [HttpGet ( "byemail" )]
        public async Task<IActionResult> GetByEmail ( [FromQuery] string email )
        {
            var student = await _studentService.GetStudentByEmailAsync ( email );
            if (student == null)
                return NotFound ();
            return Ok ( student );
        }

        [HttpPost ( "change-password" )]
        public async Task<IActionResult> ChangePassword ( [FromBody] ChangeStudentPasswordDto model )
        {
            try
            {
                bool result = await _studentService.ChangePasswordAsync ( model.StudentId, model.OldPassword, model.NewPassword );
                if (!result)
                    return BadRequest ( new { message = "Failed to change password." } );

                return Ok ( new { message = "Password changed successfully!" } );
            }
            catch (ArgumentException ex)
            {
                return BadRequest ( new { message = ex.Message } );
            }
            catch (Exception ex)
            {
                return StatusCode ( 500, new { message = "Internal server error", error = ex.Message } );
            }
        }

        [HttpGet ( "{id}/pdf" )]
        public async Task<IActionResult> GetStudentPdf ( int id )
        {
            try
            {
                var pdf = await _studentService.GenerateStudentDetailsPdfAsync ( id );
                return File ( pdf, "application/pdf", $"Student_{id}_Details.pdf" );
            }
            catch (Exception ex)
            {
                return BadRequest ( ex.Message );
            }
        }

        [Authorize]
        [HttpGet ( "check-studentid-exists/{studentId}" )]
        public async Task<IActionResult> CheckStudentIdExists ( int studentId )
        {
            var student = await _studentService.GetStudentByStudentIdAsync ( studentId );
            return Ok ( new { exists = student != null } );
        }

    }
}
