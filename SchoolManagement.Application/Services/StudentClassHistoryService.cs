using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Services
{
    public class StudentClassHistoryService : IStudentClassHistoryService
    {
        private readonly IStudentClassHistoryRepository _repository;

        public StudentClassHistoryService ( IStudentClassHistoryRepository repository )
        {
            _repository = repository;
        }

        public async Task<List<StudentClassHistory>> GetClassHistoryAsync ( int studentId )
        {
            return await _repository.GetClassHistoryByStudentIdAsync ( studentId );
        }
    }
}
