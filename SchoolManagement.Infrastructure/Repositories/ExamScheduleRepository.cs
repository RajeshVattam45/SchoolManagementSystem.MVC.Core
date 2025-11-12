using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ViewModels;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class ExamScheduleRepository : IExamScheduleRepository
    {
        private readonly SchoolDbContext _context;

        public ExamScheduleRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamSchedule>> GetAllAsync ( )
        {
            return await _context.ExamSchedules
                .Include ( es => es.Exam ).ThenInclude(e => e.ExamType)
                .Include ( es => es.Class )
                .Include ( es => es.Subject )
                .ToListAsync ();
        }

        public async Task<ExamSchedule> GetByIdAsync ( int id )
        {
            return await _context.ExamSchedules
                .Include ( es => es.Exam ).ThenInclude ( e => e.ExamType )
                .Include ( es => es.Class )
                .Include ( es => es.Subject )
                .FirstOrDefaultAsync ( e => e.ScheduleId == id );
        }

        public async Task AddAsync ( ExamSchedule schedule )
        {
            await _context.ExamSchedules.AddAsync ( schedule );
        }

        public void Update ( ExamSchedule schedule )
        {
            _context.ExamSchedules.Update ( schedule );
        }

        public void Delete ( ExamSchedule schedule )
        {
            _context.ExamSchedules.Remove ( schedule );
        }

        public async Task SaveChangesAsync ( )
        {
            await _context.SaveChangesAsync ();
        }

        public async Task AddRangeAsync ( List<ExamSchedule> schedules )
        {
            await _context.ExamSchedules.AddRangeAsync ( schedules );
        }

        public async Task<Exam> GetExamWithTypeAsync ( int examId )
        {
            return await _context.Exams
                .Include ( e => e.ExamType )
                .FirstOrDefaultAsync ( e => e.ExamId == examId );
        }

        public async Task<IEnumerable<ExamSchedule>> GetByClassAndDateAsync ( int classId, DateTime examDate )
        {
            return await _context.ExamSchedules
                .Include ( e => e.Exam )
                .Where ( e => e.ClassId == classId && e.ExamDate == examDate )
                .ToListAsync ();
        }

    }
}
