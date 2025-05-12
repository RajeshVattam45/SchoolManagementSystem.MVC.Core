using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class ClassSubjectTeacherRepository : IClassSubjectTeacherRepository
    {
        public readonly SchoolDbContext _context;

        public ClassSubjectTeacherRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task<List<ClassSubjectTeacher>> GetAllAsync ( )
        {
            return await _context.ClassSubjectTeachers
               .Include ( cst => cst.Class )
               .Include ( cst => cst.Subject )
               .Include ( cst => cst.Employee )
               .ToListAsync ();
        }
    }
}
