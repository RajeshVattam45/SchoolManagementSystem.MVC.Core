using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using SchoolManagement.Core.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Services
{
    public class ClassSubjectService : IClassSubjectService
    {
        private readonly IClassService _classService;
        private readonly ISubjectService _subjectService;
        private readonly IClassSubjectRepository _classSubjectRepo;

        public ClassSubjectService (
            IClassService classService,
            ISubjectService subjectService,
            IClassSubjectRepository classSubjectRepo )
        {
            _classService = classService;
            _subjectService = subjectService;
            _classSubjectRepo = classSubjectRepo;
        }

        // Assign subjects to multiple classes
        public async Task BulkAssignSubjectsAsync ( Dictionary<int, List<int>> classSubjectMap )
        {
            foreach (var entry in classSubjectMap)
            {
                int classId = entry.Key;
                var subjectIds = entry.Value;

                var existing = await _classSubjectRepo.GetAssignedSubjectsByClassIdAsync ( classId );
                var existingSubjectIds = existing.Select ( x => x.SubjectId ?? 0 ).ToList ();

                await _classSubjectRepo.RemoveSubjectsAsync ( classId, existingSubjectIds );

                var newAssignments = subjectIds.Select ( sid => new ClassSubject
                {
                    ClassId = classId,
                    SubjectId = sid
                } ).ToList ();

                await _classSubjectRepo.BulkAssignSubjectsAsync ( newAssignments );
            }
        }

        // Assign subjects to a single class
        public async Task BulkAssignSubjectsAsync ( int classId, List<int> selectedSubjects )
        {
            var existing = await _classSubjectRepo.GetAssignedSubjectsByClassIdAsync ( classId );
            var existingIds = existing.Select ( x => x.SubjectId ?? 0 ).ToList ();

            await _classSubjectRepo.RemoveSubjectsAsync ( classId, existingIds );

            var newLinks = selectedSubjects.Select ( subId => new ClassSubject
            {
                ClassId = classId,
                SubjectId = subId
            } ).ToList ();

            await _classSubjectRepo.BulkAssignSubjectsAsync ( newLinks );
        }

        // Get all class-subject assignments
        public async Task<List<ClassSubject>> GetAssignedSubjectsAsync ( )
        {
            return await _classSubjectRepo.GetAssignedSubjectsAsync ();
        }

        // Remove selected subjects from a class
        public async Task RemoveSubjectsAsync ( int classId, List<int> subjectIds )
        {
            await _classSubjectRepo.RemoveSubjectsAsync ( classId, subjectIds );
        }

        // Return all classes, all subjects, and current assignments
        public async Task<BulkAssignViewModel> GetBulkAssignDataAsync ( )
        {
            var classes = _classService.GetAllClasses ().ToList ();
            var subjects = _subjectService.GetAllSubjects ().ToList ();
            var assignments = await _classSubjectRepo.GetAssignedSubjectsAsync ();

            var groupedAssignments = assignments
                .GroupBy ( a => a.ClassId ?? 0 )
                .ToDictionary (
                    g => g.Key,
                    g => g.Select ( a => a.SubjectId ?? 0 ).ToList ()
                );

            return new BulkAssignViewModel
            {
                Classes = classes,
                Subjects = subjects,
                AssignedSubjects = groupedAssignments
            };
        }

        // Return class → subjects (names only)
        public async Task<List<AssignedSubjectsViewModel>> AssignedSubjectsAsync ( )
        {
            var classList = _classService.GetAllClasses ().ToList ();
            var subjectList = _subjectService.GetAllSubjects ().ToList ();
            var assignments = await _classSubjectRepo.GetAssignedSubjectsAsync ();

            var result = assignments
                .GroupBy ( a => a.ClassId ?? 0 )
                .Select ( g =>
                {
                    var className = classList.FirstOrDefault ( c => c.ClassId == g.Key )?.ClassName ?? "Unknown";

                    var subjectNames = g
                        .Select ( a => subjectList.FirstOrDefault ( s => s.SubjectId == a.SubjectId )?.SubjectName ?? "Unknown" )
                        .ToList ();

                    return new AssignedSubjectsViewModel
                    {
                        ClassName = className,
                        Subjects = subjectNames
                    };
                } ).ToList ();

            return result;
        }

        public async Task<List<ClassSubject>> GetAssignedSubjectsByClassIdAsync ( int classId )
        {
            return await _classSubjectRepo.GetAssignedSubjectsByClassIdAsync ( classId );
        }

    }
}