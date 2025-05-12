using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IExamService
    {
        Task<IEnumerable<Exam>> GetAllExamsAsync ( );
        Task<Exam> GetExamByIdAsync ( int id );
        Task AddExamAsync ( Exam exam );
        Task UpdateExamAsync ( Exam exam );
        Task DeleteExamAsync ( int id );
        Task<IEnumerable<Subject>> GetSubjectsByExamIdAsync ( int examId );
    }
}
