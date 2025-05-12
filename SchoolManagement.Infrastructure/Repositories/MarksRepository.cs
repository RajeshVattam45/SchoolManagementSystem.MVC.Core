using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class MarksRepository : IMarksRepository
    {
        private readonly SchoolDbContext _context;
        public MarksRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        // new metho
        public async Task<Marks> AddMarksAsync ( Marks marks )
        {
            _context.Marks.Add ( marks );
            await _context.SaveChangesAsync ();
            return marks;
        }

        public async Task<IEnumerable<Marks>> GetMarksByStudentIdAsync ( int studentId )
        {
            return await _context.Marks
                .Where ( m => m.StudentId == studentId )
                .Include ( m => m.Student )
                .Include ( m => m.Class )
                .Include ( m => m.Exam )
                .ThenInclude ( e => e.ExamType )
                .Include ( m => m.Subject )
                .ToListAsync ();
        }

        public async Task<IEnumerable<Marks>> GetAllMarksWithDetailsAsync ( )
        {
            return await _context.Marks
                .Include ( m => m.Student )
                .Include ( m => m.Subject )
                .Include ( m => m.Class )
                .Include ( m => m.Exam )
                    .ThenInclude ( e => e.ExamType )
                .ToListAsync ();
        }

        public async Task<bool> CheckIfMarksExistAsync ( int studentId, int examId, int subjectId, int classId )
        {
            return await _context.Marks.AnyAsync ( m =>
                m.StudentId == studentId &&
                m.ExamId == examId &&
                m.SubjectId == subjectId &&
                m.ClassId == classId );
        }
    }
}
