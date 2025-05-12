using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Services;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagement.WebAPI.Controllers
{
    [Authorize]
    [Route ( "api/[controller]" )]
    [ApiController]
    public class ClassSubjectController : ControllerBase
    {
        private readonly IClassSubjectService _classSubjectService;
        private readonly IClassService _classService;
        private readonly ISubjectService _subjectService;

        public ClassSubjectController ( IClassSubjectService classSubjectService, IClassService classService, ISubjectService subjectService )
        {
            _classSubjectService = classSubjectService;
            _classService = classService;
            _subjectService = subjectService;
        }

        // GET: api/ClassSubject/bulk-data
        // Fetch all classes, subjects, and existing assignments
        [HttpGet ( "bulk-data" )]
        public async Task<IActionResult> GetBulkAssignData ( )
        {
            var data = await _classSubjectService.GetBulkAssignDataAsync ();
            return Ok ( data );
        }

        // POST: api/ClassSubject/assign
        // Assign subjects to a single class
        [HttpPost ( "assign" )]
        public async Task<IActionResult> AssignSubjectsToClass ( [FromBody] ClassSubjectAssignmentRequest request )
        {
            if (request == null || request.SelectedSubjectIds == null)
                return BadRequest ( "Invalid input" );

            await _classSubjectService.BulkAssignSubjectsAsync ( request.ClassId, request.SelectedSubjectIds );
            return Ok ( "Subjects assigned successfully" );
        }

        // GET: api/ClassSubject/class/{classId}
        // Get assigned subjects for a specific class

        [HttpGet ( "class/{classId}" )]
        public async Task<IActionResult> GetAssignedSubjectsByClass ( int classId )
        {
            var assignedSubjects = await _classSubjectService.GetAssignedSubjectsByClassIdAsync ( classId );

            if (assignedSubjects == null || !assignedSubjects.Any ())
                return NotFound ();

            var subjectNames = assignedSubjects
                .Select ( s => s.Subject?.SubjectName ?? "Unknown" )
                .ToList ();

            var className = (_classService.GetAllClasses ())
                .FirstOrDefault ( c => c.ClassId == classId )?.ClassName ?? "Unknown";

            return Ok ( new List<AssignedSubjectsViewModel>
        {
        new AssignedSubjectsViewModel
        {
            ClassName = className,
            Subjects = subjectNames
        }} );
        }

        // GET: api/ClassSubject/assigned-subject-ids/{classId}
        [HttpGet ( "assigned-subject-ids/{classId}" )]
        public async Task<IActionResult> GetAssignedSubjectIds ( int classId )
        {
            var result = await _classSubjectService.GetAssignedSubjectsAsync ();

            // Just return a list of subject IDs for the class
            var subjectIds = result
                .Where ( cs => cs.ClassId == classId )
                .Select ( cs => cs.SubjectId )
                .ToList ();

            return Ok ( subjectIds );
        }

    }

    // ViewModel for POST request
    public class ClassSubjectAssignmentRequest
    {
        public int ClassId { get; set; }
        public List<int> SelectedSubjectIds { get; set; } = new List<int> ();
    }
}
