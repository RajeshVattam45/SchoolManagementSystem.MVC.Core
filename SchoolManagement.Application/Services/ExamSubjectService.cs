using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.Application.Services
{
    public class ExamSubjectService : IExamSubjectService
    {
        private readonly IExamSubjectRepository _examSubjectRepository;

        public ExamSubjectService ( IExamSubjectRepository examSubjectRepository )
        {
            _examSubjectRepository = examSubjectRepository;
        }

        public Task<List<ExamSubject>> GetAllAsync ( ) => _examSubjectRepository.GetAllAsync ();

        public Task<ExamSubject> GetByIdAsync ( int id ) => _examSubjectRepository.GetByIdAsync ( id );

        public Task AddAsync ( ExamSubject examSubject ) => _examSubjectRepository.AddAsync ( examSubject );

        public Task UpdateAsync ( ExamSubject examSubject ) => _examSubjectRepository.UpdateAsync ( examSubject );

        public Task DeleteAsync ( int id ) => _examSubjectRepository.DeleteAsync ( id );

        public Task<List<ExamSubject>> GetByExamIdAsync ( int examId ) => _examSubjectRepository.GetByExamIdAsync ( examId );
    }
}
