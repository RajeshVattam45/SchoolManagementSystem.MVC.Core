using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.RepositoryInterfaces
{
    public interface IExamTypeRepository
    {
        Task<IEnumerable<ExamType>> GetAllExamTypesAsync ( );
        Task<ExamType> GetExamTypeByIdAsync ( int id );
        Task AddExamTypeAsync ( ExamType examType );
        Task UpdateExamTypeAsync ( ExamType examType );
        Task DeleteExamTypeAsync ( int id );
    }
}
