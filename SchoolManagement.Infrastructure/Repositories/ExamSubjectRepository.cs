using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;
using System;
using System.Linq;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class ExamSubjectRepository : IExamSubjectRepository
    {
        private readonly SchoolDbContext _context;

        public ExamSubjectRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task<List<ExamSubject>> GetAllAsync ( )
        {
            return await _context.ExamSubjects
                .Include ( es => es.Exam )
                .Include ( es => es.Subject )
                .ToListAsync ();
        }

        public async Task<ExamSubject> GetByIdAsync ( int id )
        {
            return await _context.ExamSubjects
                .Include ( es => es.Exam )
                .Include ( es => es.Subject )
                .FirstOrDefaultAsync ( es => es.Id == id );
        }

        public async Task AddAsync ( ExamSubject examSubject )
        {
            _context.ExamSubjects.Add ( examSubject );
            await _context.SaveChangesAsync ();
        }

        public async Task UpdateAsync ( ExamSubject examSubject )
        {
            _context.Entry ( examSubject ).State = EntityState.Modified;
            await _context.SaveChangesAsync ();
        }

        public async Task DeleteAsync ( int id )
        {
            var entity = await _context.ExamSubjects.FindAsync ( id );
            if (entity != null)
            {
                _context.ExamSubjects.Remove ( entity );
                await _context.SaveChangesAsync ();
            }
        }

        public async Task<List<ExamSubject>> GetByExamIdAsync ( int examId )
        {
            return await _context.ExamSubjects
                .Where ( es => es.ExamId == examId )
                .Include ( es => es.Subject )
                .ToListAsync ();
        }
    }
}
