using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;
using System.Linq;


namespace SchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Repository responsible for managing subject assignments to classes.
    /// Implements operations to assign, retrieve, and remove subjects from classes.
    /// </summary>
    public class ClassSubjectRepository : IClassSubjectRepository
    {
        private readonly SchoolDbContext _context;

        /// <summary>
        /// Constructor that injects the application's DbContext.
        /// </summary>
        public ClassSubjectRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        /// <summary>
        /// Bulk assigns a list of subjects to a class.
        /// </summary>
        /// <param name="classSubjects">List of ClassSubject relationships to be added.</param>
        public async Task BulkAssignSubjectsAsync ( List<ClassSubject> classSubjects )
        {
            await _context.ClassSubjects.AddRangeAsync ( classSubjects );
            await _context.SaveChangesAsync ();
        }

        /// <summary>
        /// Retrieves all assigned subjects for all classes, including related class and subject data.
        /// </summary>
        /// <returns>List of ClassSubject with navigation properties populated.</returns>
        public async Task<List<ClassSubject>> GetAssignedSubjectsAsync ( )
        {
            return await _context.ClassSubjects
                .Include ( cs => cs.Class )
                .Include ( cs => cs.Subject )
                .ToListAsync ();
        }

        /// <summary>
        /// Removes specific subjects from a class based on subject IDs.
        /// </summary>
        /// <param name="classId">The class ID to remove subjects from.</param>
        /// <param name="subjectIds">List of subject IDs to remove.</param>
        public async Task RemoveSubjectsAsync ( int classId, List<int> subjectIds )
        {
            var toRemove = await _context.ClassSubjects
                .Where ( cs => cs.ClassId == classId && subjectIds.Contains ( cs.SubjectId.Value ) )
                .ToListAsync ();

            _context.ClassSubjects.RemoveRange ( toRemove );
            await _context.SaveChangesAsync ();
        }

        /// <summary>
        /// Retrieves all subject assignments for a specific class.
        /// </summary>
        /// <param name="classId">The ID of the class to retrieve subjects for.</param>
        /// <returns>List of ClassSubject assigned to the specified class.</returns>
        public async Task<List<ClassSubject>> GetAssignedSubjectsByClassIdAsync ( int classId )
        {
            return await _context.ClassSubjects
                .Where ( cs => cs.ClassId == classId )
                .Include ( cs => cs.Subject )
                .ToListAsync ();
        }
    }
}
