using SchoolManagement.Core.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IStudentClassHistoryRepository
    {
        Task<List<StudentClassHistory>> GetClassHistoryByStudentIdAsync ( int studentId );
    }
}
