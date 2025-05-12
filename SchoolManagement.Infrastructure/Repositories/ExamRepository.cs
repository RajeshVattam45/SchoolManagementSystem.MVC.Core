using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly SchoolDbContext _context;

        /// <summary>
        /// Constructor that receives the database context through dependency injection.
        /// </summary>
        /// <param name="context">Database context.</param>
        public ExamRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all exams from the database, including their associated exam types.
        /// </summary>
        /// <returns>List of all exams with their types.</returns>
        public async Task<IEnumerable<Exam>> GetAllExamsAsync ( )
        {
            return await _context.Exams
                .Include ( e => e.ExamType )
                .ToListAsync ();
        }

        /// <summary>
        /// Retrieves a specific exam by ID, including its exam type.
        /// </summary>
        /// <param name="id">ID of the exam.</param>
        /// <returns>The matching exam, or null if not found.</returns>
        public async Task<Exam> GetExamByIdAsync ( int id )
        {
            return await _context.Exams
                .Include ( e => e.ExamType )
                .FirstOrDefaultAsync ( e => e.ExamId == id );
        }

        /// <summary>
        /// Adds a new exam to the database.
        /// </summary>
        /// <param name="exam">Exam entity to be added.</param>
        public async Task AddExamAsync ( Exam exam )
        {
            await _context.Exams.AddAsync ( exam );
            await _context.SaveChangesAsync ();
        }

        /// <summary>
        /// Updates an existing exam record.
        /// </summary>
        /// <param name="exam">Exam entity with updated data.</param>
        public async Task UpdateExamAsync ( Exam exam )
        {
            _context.Exams.Update ( exam );
            await _context.SaveChangesAsync ();
        }

        /// <summary>
        /// Deletes an exam by ID along with its related schedules and subjects.
        /// </summary>
        /// <param name="id">ID of the exam to be deleted.</param>
        public async Task DeleteExamAsync ( int id )
        {
            var exam = await _context.Exams
                .Include ( e => e.ExamSchedules )
                .Include ( e => e.ExamSubjects )
                .FirstOrDefaultAsync ( e => e.ExamId == id );

            if (exam != null)
            {
                if (exam.ExamSchedules != null && exam.ExamSchedules.Any ())
                {
                    _context.ExamSchedules.RemoveRange ( exam.ExamSchedules );
                }

                if (exam.ExamSubjects != null && exam.ExamSubjects.Any ())
                {
                    _context.ExamSubjects.RemoveRange ( exam.ExamSubjects );
                }

                _context.Exams.Remove ( exam );
                await _context.SaveChangesAsync ();
            }
        }

        /// <summary>
        /// Retrieves all subjects assigned to a specific exam.
        /// </summary>
        /// <param name="examId">ID of the exam.</param>
        /// <returns>List of subjects associated with the exam.</returns>
        public async Task<IEnumerable<Subject>> GetSubjectsByExamIdAsync ( int examId )
        {
            var exam = await _context.Exams
                .Include ( e => e.ExamSubjects! )
                    .ThenInclude ( es => es.Subject )
                .FirstOrDefaultAsync ( e => e.ExamId == examId );

            return exam?.ExamSubjects?
                        .Select ( es => es.Subject! )
                        .Where ( subject => subject != null )
                   ?? new List<Subject> ();
        }
    }
}
