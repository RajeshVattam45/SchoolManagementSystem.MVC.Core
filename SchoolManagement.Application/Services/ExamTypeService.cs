using SchoolManagement.Core.Entites.Models;
using SchoolManagement.Core.RepositoryInterfaces;
using SchoolManagement.Core.ServiceInterfaces;

namespace SchoolManagement.Application.Services
{
    public class ExamTypeService : IExamTypeService
    {
        private readonly IExamTypeRepository _examTypeRepository;

        public ExamTypeService ( IExamTypeRepository examTypeRepository )
        {
            _examTypeRepository = examTypeRepository;
        }

        public async Task<IEnumerable<ExamType>> GetAllExamTypesAsync ( )
        {
            return await _examTypeRepository.GetAllExamTypesAsync ();
        }

        public async Task<ExamType> GetExamTypeByIdAsync ( int id )
        {
            return await _examTypeRepository.GetExamTypeByIdAsync ( id );
        }

        public async Task AddExamTypeAsync ( ExamType examType )
        {
            await _examTypeRepository.AddExamTypeAsync ( examType );
        }

        public async Task UpdateExamTypeAsync ( ExamType examType )
        {
            await _examTypeRepository.UpdateExamTypeAsync ( examType );
        }

        public async Task DeleteExamTypeAsync ( int id )
        {
            await _examTypeRepository.DeleteExamTypeAsync ( id );
        }
    }
}
