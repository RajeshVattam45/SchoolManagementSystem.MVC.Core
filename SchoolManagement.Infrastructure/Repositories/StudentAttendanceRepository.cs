using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class StudentAttendanceRepository : IStudentAttendanceRepository
    {
        public readonly SchoolDbContext _context;

        public StudentAttendanceRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task AddStudentAttandanceAsync ( StudentAttendance studentAttendance )
        {
            _context.StudentAttendances.Add ( studentAttendance );
            await _context.SaveChangesAsync ();
        }

        public async Task DeleteStudentAttendanceAsync ( int id )
        {
            var studentAttandance = await _context.StudentAttendances.FindAsync ( id );
            if (studentAttandance != null)
            {
                _context.StudentAttendances.Remove ( studentAttandance );
                await _context.SaveChangesAsync ();
            }
        }

        public async Task<List<StudentAttendance>> GetAllAttendancesAsync ( )
        {
            return await _context.StudentAttendances
                .Include ( sa => sa.Student )
                .Include ( sa => sa.Class )
                .ToListAsync ();
        }

        public async Task<StudentAttendance> GetAllAttendancesByIdAsync ( int id )
        {
            return await _context.StudentAttendances
                .Include ( sa => sa.Student )
                .Include ( sa => sa.Class )
                .FirstOrDefaultAsync ( sa => sa.Id == id );
        }

        public async Task UpdateStudentAttandanceAsynce ( StudentAttendance studentAttendance )
        {
            _context.StudentAttendances.Update ( studentAttendance );
            await _context.SaveChangesAsync ();
        }

        public async Task<List<StudentAttendance>> GetAttendancesByStudentIdAsync ( int studentId )
        {
            return await _context.StudentAttendances
                .Where ( sa => sa.StudentId == studentId )
                .Include ( sa => sa.Class )
                .ToListAsync ();
        }

        public async Task<StudentAttendance> GetAttendanceByStudentIdAndDateAsync ( int studentId, DateOnly date )
        {
            return await _context.StudentAttendances
                .FirstOrDefaultAsync ( a => a.StudentId == studentId && a.Date == date );
        }

        public async Task<IEnumerable<StudentAttendance>> GetStudentAttendanceByClassId ( int classId )
        {
            return await _context.StudentAttendances
                .Where ( a => a.Student.ClassId == classId )
                .Include ( a => a.Class )
                .ToListAsync ();
        }

        public async Task<List<StudentAttendance>> GetAttendancesByStudentEmailAsync ( string email )
        {
            return await _context.StudentAttendances
                                 .Where ( sa => sa.Student.Email == email )
                                 .ToListAsync ();
        }
    }
}
