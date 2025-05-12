using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IExamSubjectRepository
    {
        Task<List<ExamSubject>> GetAllAsync ( );
        Task<ExamSubject> GetByIdAsync ( int id );
        Task AddAsync ( ExamSubject examSubject );
        Task UpdateAsync ( ExamSubject examSubject );
        Task DeleteAsync ( int id );
        Task<List<ExamSubject>> GetByExamIdAsync ( int examId );
    }
}
