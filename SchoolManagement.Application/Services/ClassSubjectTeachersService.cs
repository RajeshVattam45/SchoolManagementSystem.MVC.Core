using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.Application.Services
{
    public class ClassSubjectTeachersService : IClassSubjectTeachersService
    {
        private readonly IClassSubjectTeacherRepository _repository;

        public ClassSubjectTeachersService ( IClassSubjectTeacherRepository repository )
        {
            _repository = repository;
        }

        public async Task<List<ClassSubjectTeacher>> GetAllClassSubjectTeachersAsync ( )
        {
            return await _repository.GetAllAsync ();
        }
    }
}
