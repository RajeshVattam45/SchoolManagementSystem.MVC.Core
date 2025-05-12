using SchoolManagement.Core.Entites.Models;

namespace SchoolManagement.Core.ServiceInterfaces
{
    public interface IExamTypeService
    {
        Task<IEnumerable<ExamType>> GetAllExamTypesAsync ( );
        Task<ExamType> GetExamTypeByIdAsync ( int id );
        Task AddExamTypeAsync ( ExamType examType );
        Task UpdateExamTypeAsync ( ExamType examType );
        Task DeleteExamTypeAsync ( int id );
    }
}
