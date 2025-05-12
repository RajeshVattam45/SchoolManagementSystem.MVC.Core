using SchoolManagement.Core.Entites.Models;
using System.Collections.Generic;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IClassSubjectTeacherRepository
    {
        Task<List<ClassSubjectTeacher>> GetAllAsync ( );
    }
}
