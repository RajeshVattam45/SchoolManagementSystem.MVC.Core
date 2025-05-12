using SchoolManagement.Core.Entites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IStudentClassHistoryService
    {
        Task<List<StudentClassHistory>> GetClassHistoryAsync ( int studentId );
    }
}
