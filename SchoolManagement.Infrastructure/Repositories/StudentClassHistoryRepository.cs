using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagementSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class StudentClassHistoryRepository : IStudentClassHistoryRepository
    {
        private readonly SchoolDbContext _context;

        public StudentClassHistoryRepository ( SchoolDbContext context )
        {
            _context = context;
        }

        public async Task<List<StudentClassHistory>> GetClassHistoryByStudentIdAsync ( int studentId )
        {
            return await _context.StudentClassHistories
                                 .Include ( x => x.Class )
                                 .Where ( x => x.StudentId == studentId )
                                 .ToListAsync ();
        }
    }
}
