using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IClassSubjectTeachersService
    {
        Task<List<ClassSubjectTeacher>> GetAllClassSubjectTeachersAsync ( );
    }
}
