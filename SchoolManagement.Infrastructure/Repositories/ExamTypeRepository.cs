using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class ExamTypeRepository : IExamTypeRepository
    {
        private readonly SchoolDbContext _context;

        public ExamTypeRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamType>> GetAllExamTypesAsync ( )
        {
            return await _context.ExamTypes.ToListAsync ();
        }

        public async Task<ExamType> GetExamTypeByIdAsync ( int id )
        {
            return await _context.ExamTypes.FindAsync ( id );
        }

        public async Task AddExamTypeAsync ( ExamType examType )
        {
            await _context.ExamTypes.AddAsync ( examType );
            await _context.SaveChangesAsync ();
        }

        public async Task UpdateExamTypeAsync ( ExamType examType )
        {
            _context.ExamTypes.Update ( examType );
            await _context.SaveChangesAsync ();
        }

        public async Task DeleteExamTypeAsync ( int id )
        {
            var examType = await _context.ExamTypes
                .Include ( et => et.Exams )
                    .ThenInclude ( e => e.ExamSchedules )
                .Include ( et => et.Exams )
                    .ThenInclude ( e => e.ExamSubjects )
                .FirstOrDefaultAsync ( et => et.Id == id );

            if (examType == null)
                return;

            // Loop through each exam and delete related ExamSubjects and ExamSchedules
            foreach (var exam in examType.Exams ?? Enumerable.Empty<Exam> ())
            {
                if (exam.ExamSubjects != null)
                    _context.ExamSubjects.RemoveRange ( exam.ExamSubjects );

                if (exam.ExamSchedules != null)
                    _context.ExamSchedules.RemoveRange ( exam.ExamSchedules );
            }

            // Delete all Exams
            if (examType.Exams != null)
                _context.Exams.RemoveRange ( examType.Exams );

            // Finally, delete the ExamType
            _context.ExamTypes.Remove ( examType );

            await _context.SaveChangesAsync ();
        }
    }
}
